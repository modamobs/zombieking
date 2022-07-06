using BackEnd;
using Coffee.UIExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PopUpEquipLegendUpdrage : MonoBehaviour
{
    private int _tbc = 0;
    [SerializeField]
    List<GameDatabase.TableDB.Equipment> equipRating7 = new List<GameDatabase.TableDB.Equipment>();
    public int GetCount => equipRating7.Count;
    public GameDatabase.TableDB.Equipment Get(int indx) => equipRating7[indx];

    // 재료 
    [SerializeField]
    List<GameDatabase.TableDB.Equipment> equipRating7Mat = new List<GameDatabase.TableDB.Equipment>();
    public int IsCheckMatCount() => equipRating7Mat.Count;
    public bool IsCheckMat(long uid) => equipRating7Mat.FindIndex(obj => long.Equals(obj.aInUid, uid)) >= 0;

    [SerializeField] GameDatabase.TableDB.Equipment eqDB = new GameDatabase.TableDB.Equipment();

    [SerializeField] PopUpViewEquipmentInfo.InfoViewEquip.Info_icon info_icon;
    [SerializeField] PopUpViewEquipmentInfo.InfoViewEquip.Info_stat info_stat;
    [SerializeField] Sprite[] statIcon = new Sprite[10];
    [SerializeField] InitOnStartEquipLegend initOnStartEquipLegend;

    Sprite transp = null;

    [SerializeField] UIUpgrade uiUpgrade;
    [System.Serializable]
    struct UIUpgrade
    {
        public Animator ani;
        public Text txPercent;
        public Text txPriceRuby;
        public Text txPriceDia;
        public GameObject goLegendComplete;
        public GameObject goBtns;
        public Image imBtnBg;
        public List<UIEffect> uiefIconGray;
        public List<Text> uiefTextGray;
        public GameObject goResultSuccess, goResultFailled;
        public GameObject goEquipSopBox;
        public Text txFailledResultTip; // 실패 보너스 확률 팁 
    }

    [SerializeField] GameObject goBlack;
    [SerializeField] OnPop onPop;

    void Awake()
    {
        if (transp == null)
        {
            transp = SpriteAtlasMng.GetInstance().GetTransparency();
            for (int i = 0; i < statIcon.Length; i++)
            {
                statIcon[i] = SpriteAtlasMng.GetInstance().GetSpriteStatIcon(i);
            }
        }
    }

    public async void SetData(GameDatabase.TableDB.Equipment _eqdb)
    {
        Loading.Bottom(false);
        goBlack.SetActive(false);
        Init(_eqdb);
        Info(_eqdb);

        _tbc = await GameDatabase.GetInstance().tableDB.GetMyTBC();
        Loading.Bottom(true);
    }

    void Init(GameDatabase.TableDB.Equipment _eqdb)
    {
        onPop.isLockEscape = false;
        eqDB = _eqdb;
        equipRating7.Clear();
        equipRating7Mat.Clear();

        equipRating7 = GameDatabase.GetInstance().tableDB.GetAllEquipment().FindAll(obj => !long.Equals(obj.aInUid, _eqdb.aInUid) && obj.eq_ty <= 7 && int.Equals(obj.m_state, 0) && int.Equals(obj.eq_rt, 7) && int.Equals(obj.eq_legend, 0));
        equipRating7.Sort((GameDatabase.TableDB.Equipment a, GameDatabase.TableDB.Equipment b) => a.eq_ty.CompareTo(b.eq_ty));
        initOnStartEquipLegend.SetInit();
    }

    void Info(GameDatabase.TableDB.Equipment _eqdb)
    {
        uiUpgrade.goEquipSopBox.SetActive(false);
        foreach (var item in uiUpgrade.uiefIconGray)
            item.enabled = false;

        // 아이콘 정보
        info_icon.imIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(_eqdb.eq_ty, _eqdb.eq_rt, _eqdb.eq_id);
        info_icon.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(_eqdb.eq_rt, _eqdb.eq_legend);
        info_icon.txRating.text = GameDatabase.StringFormat.GetStringRating(_eqdb.eq_rt);
        info_icon.txRating.color = ResourceDatabase.GetInstance().GetItemColor(_eqdb.eq_rt);
        info_icon.goLock.SetActive(_eqdb.m_lck == 1);
        info_icon.txEnhantLevel.text = string.Format("+{0}", _eqdb.m_ehnt_lv);
        info_icon.txEnhantLevel.color = ResourceDatabase.GetInstance().GetItemColor(_eqdb.eq_rt);
        info_icon.txLevel.text = string.Format("Lv.{0}", _eqdb.m_norm_lv);
        info_icon.txEquipName.text = GameDatabase.StringFormat.GetEquipName(_eqdb.eq_ty, _eqdb.eq_rt, _eqdb.eq_id);
        //info_etc.imLockBig.color = wearEquipValue.m_lock == 1 ? info_icon.coOkLock : info_icon.coNoLock;

        // 레벨 이펙트 
        if (info_icon.goRootEnhant30.activeSelf == !(_eqdb.m_ehnt_lv >= 30))
            info_icon.goRootEnhant30.SetActive(_eqdb.m_ehnt_lv >= 30);

        if (_eqdb.m_ehnt_lv >= 30)
        {
            info_icon.imRtOutlineEnhant30.color = ResourceDatabase.GetInstance().GetItemColor(_eqdb.eq_rt);
            info_icon.ugEnhant30.color1 = ResourceDatabase.GetInstance().GetItemGradientColor(_eqdb.eq_rt);
            info_icon.ugEnhant30.color2 = ResourceDatabase.GetInstance().GetItemGradientColor(_eqdb.eq_rt);
        }

        if (info_icon.goRootNorLv100.activeSelf == !(_eqdb.m_norm_lv >= 100))
            info_icon.goRootNorLv100.SetActive(_eqdb.m_norm_lv >= 100);

        if (_eqdb.m_norm_lv >= 100)
        {
            info_icon.imRtOutlineNorLv100.color = ResourceDatabase.GetInstance().GetItemColor(_eqdb.eq_rt);
            info_icon.psNorLv100.startColor = ResourceDatabase.GetInstance().GetItemColor(_eqdb.eq_rt);
        }

        // 스탯 정보
        info_stat.imStatIcon.sprite = statIcon[_eqdb.ma_st_id];
        info_stat.txStatName.text = GameDatabase.StringFormat.GetEquipStatName(_eqdb.ma_st_id); // 매인 스탯 이름 
        info_stat.txStatCombat.text = string.Format("{0:#,0}", GameDatabase.GetInstance().tableDB.GetEquipCombatPower(_eqdb, "main"));
        object[] wer_mast_val = GameDatabase.GetInstance().chartDB.GetMainStatValue(_eqdb);
        Type val_type = wer_mast_val[0].GetType();
        if (val_type == typeof(float))
            info_stat.txStatValue.text = string.Format("{0:0.000}(+{1:0.000})", (float)wer_mast_val[0], (float)wer_mast_val[1]);
        else if (val_type == typeof(long))
            info_stat.txStatValue.text = string.Format("{0:#,0}(+{1:#,0})", (long)wer_mast_val[0], (long)wer_mast_val[1]);

        // 옵션 
        info_stat.txOpStatCombat.text = string.Format("{0:#,0}", GameDatabase.GetInstance().tableDB.GetEquipCombatPower(_eqdb, "op"));
        for (int i = 0; i < 4; i++)
        {
            int opst_id = 0, opst_rlv = 0;
            switch (i)
            {
                case 0: opst_id = _eqdb.st_op.op1.id; opst_rlv = _eqdb.st_op.op1.rlv; break;
                case 1: opst_id = _eqdb.st_op.op2.id; opst_rlv = _eqdb.st_op.op2.rlv; break;
                case 2: opst_id = _eqdb.st_op.op3.id; opst_rlv = _eqdb.st_op.op3.rlv; break;
                case 3: opst_id = _eqdb.st_op.op4.id; opst_rlv = _eqdb.st_op.op4.rlv; break;
            }

            Color cor = opst_id > 0 && int.Equals(_eqdb.eq_legend, 1) && int.Equals(opst_id, _eqdb.eq_legend_sop_id) ? Color.yellow : Color.white;
            if (opst_id > 0 && opst_rlv > 0)
            {
                long opst_val = (long)GameDatabase.GetInstance().chartDB.GetEquipOptionStatValue(opst_id, opst_rlv, _eqdb.eq_rt, _eqdb.eq_id, _eqdb.m_ehnt_lv, true, _eqdb.eq_legend, _eqdb.eq_legend_sop_id, _eqdb.eq_legend_sop_rlv);
                info_stat.txOpStatValues[i].text = string.Format("+{0:#,0}", (int)opst_val);
                info_stat.txOpStatNames[i].text = GameDatabase.StringFormat.GetEquipStatName(opst_id);
                info_stat.imOpStatIcon[i].sprite = statIcon[opst_id];

                info_stat.txOpStatValues[i].color = cor;
                info_stat.txOpStatNames[i].color = cor;
            }
            else
            {
                info_stat.txOpStatNames[i].text = "-";
                info_stat.txOpStatValues[i].text = "-";
                info_stat.imOpStatIcon[i].sprite = transp;
                info_stat.txOpStatValues[i].color = Color.white;
                info_stat.txOpStatNames[i].color = Color.white;
            }
        }

        //uiUpgrade.goEquipSopBox.SetActive(_eqdb.eq_legend >= 1);
        //info_stat.goAcceOpStatRoot.SetActive(_eqdb.eq_legend >= 1);
        if (_eqdb.eq_legend >= 1)
        {
            info_stat.txAccOpStatName.text = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("equip.sop.id_{0}", _eqdb.eq_legend_sop_id)).val_string;
            info_stat.txAcceOpStatValue.text = string.Format("+{0:0.000}%", GameDatabase.GetInstance().chartDB.GetEquipSpecialOptionValue(_eqdb.eq_legend_sop_id, _eqdb.eq_legend_sop_rlv));
        }
        else
        {
            info_stat.txAccOpStatName.text = "??? 옵션 스탯 증가";
            info_stat.txAcceOpStatValue.text = "???%";
        }

        // 총 전투력 총합 
        info_icon.txAllCombat.text = string.Format("{0:#,0}", GameDatabase.GetInstance().tableDB.GetEquipCombatPower(_eqdb, "total"));
        uiUpgrade.ani.gameObject.SetActive(false);
        UpgradeInfo();
    }

    public void CheckSelectEquipDB(GameDatabase.TableDB.Equipment selectMatEqDB)
    {
        if (equipRating7.FindIndex(obj => long.Equals(obj.aInUid, selectMatEqDB.aInUid)) >= 0)
        {
            int indx = equipRating7Mat.FindIndex(obj => long.Equals(obj.aInUid, selectMatEqDB.aInUid));
            if (indx < 0)
            {
                if(equipRating7Mat.Count < 5)
                    equipRating7Mat.Add(selectMatEqDB);
                else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("재료로 사용할 수 있는 최대 수량을 초과하였습니다. (5/5)");
            }
            else
            {
                equipRating7Mat.Remove(selectMatEqDB);
            }
        }

        UpgradeInfo();
    }

    void UpgradeInfo()
    {
        uiUpgrade.goLegendComplete.SetActive(int.Equals(eqDB.eq_legend, 1));
        if (int.Equals(eqDB.eq_legend, 0))
        {
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            var userinfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
            bool isBlueDiaLack = goods_db.m_dia < PriceDia();
            int tbc = isBlueDiaLack == true ? _tbc : 0;
            int blue_dia = goods_db.m_dia;

            if (blue_dia + tbc >= PriceDia() && goods_db.m_ruby >= PriceRuby() && equipRating7Mat.Count > 0)
            {
                uiUpgrade.imBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(7);
            }
            else uiUpgrade.imBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);

            uiUpgrade.goBtns.SetActive(true);
            uiUpgrade.txPercent.text = userinfo_db.m_eq_lgnd_upgrd_rt7_p > 0 ? string.Format("성공률 : {0:0.000}%<color=yellow>(보너스 +{1:0.0}%)</color>", Rate(), userinfo_db.m_eq_lgnd_upgrd_rt7_p) : string.Format("성공률 : {0:0.000}%", Rate());
            uiUpgrade.txPriceDia.text = string.Format("x{0}", PriceDia());
            uiUpgrade.txPriceDia.color = blue_dia + tbc >= PriceDia() ? Color.white : Color.red;
            uiUpgrade.txPriceRuby.text = string.Format("x{0}", PriceRuby());
            uiUpgrade.txPriceRuby.color = goods_db.m_ruby >= PriceRuby() ? Color.white : Color.red;
        }
        else
        {
            uiUpgrade.goBtns.SetActive(false);
        }
    }

    int PriceDia() => (int)(equipRating7Mat.Count * GameDatabase.GetInstance().chartDB.GetDicBalance("equip.legend.upgrade.select1.price.dia").val_int);
    int PriceRuby() => (int)(equipRating7Mat.Count * GameDatabase.GetInstance ().chartDB.GetDicBalance("equip.legend.upgrade.select1.price.ruby").val_int);

    float Rate()
    {
        if (equipRating7Mat.Count > 0)
        {
            var userinfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
            float rate = (equipRating7Mat.Count * equipRating7Mat.Count) * GameDatabase.GetInstance ().chartDB.GetDicBalance("equip.legend.upgrade.rate.count").val_float; // 장비 선택 1개당 기본 성공률 증가 값
            foreach (var eq in equipRating7Mat)
            {
                if (int.Equals(eq.eq_ty, eqDB.eq_ty)) // 진화 장비와 같은 부위 경우 추가 증가 값
                    rate += GameDatabase.GetInstance().chartDB.GetDicBalance("equip.legend.upgrade.rate.same").val_float;

                if (eq.m_ehnt_lv >= 1) // 강화 된 장비일 경우 추가 증가 값 
                    rate += (eq.m_ehnt_lv * eq.m_ehnt_lv) * GameDatabase.GetInstance().chartDB.GetDicBalance("equip.legend.upgrade.rate.enhant").val_float;
            }

            rate += userinfo_db.m_eq_lgnd_upgrd_rt7_p;
            return rate;
        }
        else return 0.0f;
    }

    public void Click_AskStartLegendUpgrade()
    {
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        bool isBlueDiaLack = goods_db.m_dia < PriceDia();
        int tbc = isBlueDiaLack == true ? _tbc : 0;
        int blue_dia = goods_db.m_dia;
        if (blue_dia + tbc >= PriceDia() && goods_db.m_ruby >= PriceRuby())
        {
            if(equipRating7Mat.Count > 0)
            {
                if (equipRating7Mat.FindIndex(obj => obj.m_lck > 0) >= 0)
                {
                    PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(
                        "재료로 사용될 장비중 잠금 상태인 장비가 있습니다.\n확인 버튼을 누르시면 진화를 시도합니다.\n재료로 사용된 장비는 소멸됩니다.", StartLegendUpgrade);
                }
                else StartLegendUpgrade();
            }
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("재료 장비가 선택되지 않았습니다.\n재료 장비는 1~5개까지 선택할 수 있습니다.");
        }
        else
        {
            if(blue_dia + tbc < PriceDia())
            {
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
            }   
            else if(goods_db.m_ruby < PriceRuby())
            {
                if (GameDatabase.GetInstance().convenienceFunctionDB.GetUseingConvenFunAutoSale())
                    PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("루비가 부족합니다.\n루비는 장비를 분해하여 획득할 수 있습니다");
                else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("루비가 부족합니다.\n루비는 장비를 분해하여 획득할 수 있습니다.\n장비 자동 판매/분해 구매 탭으로 이동됩니다.", PopUpMng.GetInstance().Open_DailyProductReward);
            }
        }
    }

    async void StartLegendUpgrade()
    {
        if(PriceDia() <= 0 || PriceRuby () <= 0)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("가격 정보가 잘못되었습니다");
            return;
        }

        uiUpgrade.goResultSuccess.SetActive(false);
        uiUpgrade.goResultFailled.SetActive(false);
        goBlack.SetActive(true);
        onPop.isLockEscape = true;

        float rpct = GameDatabase.GetInstance().GetRandomPercent();
        LogPrint.EditorPrint("rpct : " + rpct +", Rate : " + Rate());
        bool isSuccess = rpct < Rate();

        uiUpgrade.ani.gameObject.SetActive(true);
        uiUpgrade.ani.Play("Result_EquipIconInfoReady");
        await Task.Delay(500);

        // 비용 차감 
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        var userinfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
        goods_db.m_ruby -= PriceRuby(); // 루비 차감 
        int dedDia = goods_db.m_dia -= PriceDia(); // 내 현재 블루 다이아 차감
        int dedTbc = dedDia < 0 ? Math.Abs(dedDia) : 0;
        // 실패 보너스 
        if (isSuccess)
            userinfo_db.m_eq_lgnd_upgrd_rt7_p = 0;
        else userinfo_db.m_eq_lgnd_upgrd_rt7_p += equipRating7Mat.Count;

        Task<bool> tsk_tbc = GameDatabase.GetInstance().tableDB.DeductionTBC(dedTbc);
        Task<bool> tsk_goods = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db); // 강화 비용 값 전송 
        Task<bool> tsk_uinfo = GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userinfo_db);
        while (tsk_tbc.IsCompleted == false || tsk_goods.IsCompleted == false || tsk_uinfo.IsCompleted == false) { await Task.Delay(100); }

        // 애니 실행 
        string anName = isSuccess == true ? string.Format("Result_EquipIconInfoStart_rt{0}", eqDB.eq_rt) : "Result_EquipIconInfoStart";
        uiUpgrade.ani.Play(anName);
        while (uiUpgrade.ani.GetCurrentAnimatorStateInfo(0).IsName(anName) == false)
            await Task.Delay(100);

        if (!isSuccess)
        {
            foreach (var item in uiUpgrade.uiefIconGray)
                item.enabled = true;

            // 아이콘 정보
            info_icon.txRating.color = ResourceDatabase.GetInstance().GetItemColor(0);
            info_icon.txEnhantLevel.color = ResourceDatabase.GetInstance().GetItemColor(0);
            info_icon.txEquipName.text = GameDatabase.StringFormat.GetEquipName(eqDB.eq_ty, eqDB.eq_rt, eqDB.eq_id);

            // 레벨 이펙트 
            if (info_icon.goRootEnhant30.activeSelf == !(eqDB.m_ehnt_lv >= 30))
                info_icon.goRootEnhant30.SetActive(eqDB.m_ehnt_lv >= 30);

            info_icon.imRtOutlineEnhant30.color = ResourceDatabase.GetInstance().GetItemColor(0);
            info_icon.ugEnhant30.color1 = ResourceDatabase.GetInstance().GetItemGradientColor(0);
            info_icon.ugEnhant30.color2 = ResourceDatabase.GetInstance().GetItemGradientColor(0);

            if (info_icon.goRootNorLv100.activeSelf == !(eqDB.m_norm_lv >= 100))
                info_icon.goRootNorLv100.SetActive(eqDB.m_norm_lv >= 100);

            if (eqDB.m_norm_lv >= 100)
            {
                info_icon.imRtOutlineNorLv100.color = ResourceDatabase.GetInstance().GetItemColor(0);
                info_icon.psNorLv100.startColor = ResourceDatabase.GetInstance().GetItemColor(0);
            }
        }

        while (uiUpgrade.ani.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f)
            await Task.Delay(100);

        uiUpgrade.ani.speed = 1;

        uiUpgrade.goResultSuccess.SetActive(isSuccess);
        uiUpgrade.goResultFailled.SetActive(!isSuccess);

        if (isSuccess)
        {
            float eq_sop_rpct = GameDatabase.GetInstance().GetRandomPercent();
            float adPct = 0.0f;
            for (int i = 0; i < 10; i++)
            {
                int pct = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("equip.sop.pct_id_{0}", i)).val_int;
                if(pct > 0)
                {
                    adPct += pct;
                    if(eq_sop_rpct < adPct)
                    {
                        eqDB.eq_legend = 1;
                        var sop = GameDatabase.GetInstance().tableDB.GetEquipAcceRandomSpecialOption(false);
                        eqDB.eq_legend_sop_id = sop.id;
                        eqDB.eq_legend_sop_rlv = sop.rlv;
                        break;
                    }
                }
            }
            
            info_icon.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(eqDB.eq_rt, eqDB.eq_legend);

            Task task = GameDatabase.GetInstance().tableDB.SetUpdateChangeEquipmentData(
                eqDB, ParamT.Collection(new ParamT.P[]
                {
                    new ParamT.P() { k = "eq_legend", v = eqDB.eq_legend },
                    new ParamT.P() { k = "eq_legend_sop_id", v = eqDB.eq_legend_sop_id },
                    new ParamT.P() { k = "eq_legend_sop_rlv", v = eqDB.eq_legend_sop_rlv },
                }));

            while (task.IsCompleted == false) await Task.Delay(100);

            MainUI.GetInstance().RefreshGameStatViewInfo(eqDB.eq_ty);
        }
        else
        {
            
              uiUpgrade.txFailledResultTip.text = string.Format("실패 보상 확률 <color=#00E9FF>{0:0.0}%획득</color>, 누적 실패 보상 확률 <color=#00E9FF>{1:0.0}%</color>\n다음 진화 시도시 성공률 <color=#00E9FF>{2:0.0}%</color> 부터 시작됩니다.",
                  equipRating7Mat.Count, userinfo_db.m_eq_lgnd_upgrd_rt7_p, userinfo_db.m_eq_lgnd_upgrd_rt7_p);
        }

        List<TransactionParam> TransactionParamList = new List<TransactionParam>();
        TransactionParam tParam = new TransactionParam();
        for (int i = 0; i < equipRating7Mat.Count; i++)
        {
            if (!string.IsNullOrEmpty(equipRating7Mat[i].indate))
            {
                if (tParam.GetWriteValues().Count >= 10)
                {
                    TransactionParamList.Add(tParam);
                    tParam = new TransactionParam();
                }

                var tmp_db = equipRating7Mat[i];
                tmp_db.m_state = -1;
                List<BackEnd.WRITE> writes = new List<BackEnd.WRITE> { new WRITE { Action = TransactionAction.Update, Param = ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "m_state", v = tmp_db.m_state } }) } };
                tParam.AddUpdateList(BackendGpgsMng.tableName_Equipment, tmp_db.indate, writes);
            }
        }

        if (tParam.GetWriteValues().Count > 0)
        {
            TransactionParamList.Add(tParam);
            tParam = new TransactionParam();
        }

        if(TransactionParamList.Count > 0)
        {
            foreach (var send_param in TransactionParamList)
            {
                BackendReturnObject bro = null;
                SendQueue.Enqueue(Backend.GameInfo.TransactionWrite, send_param, callback => { bro = callback; });
                while (bro != null) await Task.Delay(100);
            }
        }

        foreach (var item in equipRating7Mat)
            GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(item, true);

        BackendGpgsMng.GetInstance().ASetInventoryBackUp(true);
        MainUI.GetInstance().inventory.initOnStartInventoryAll.SetInit(true);
        _tbc = await GameDatabase.GetInstance().tableDB.GetMyTBC();
        Init(eqDB);
    }

    public void Click_CloseResult()
    {
        if (!onPop.isLockEscape)
        {
            Info(eqDB);
            goBlack.SetActive(false);
        }
    }

    public void Click_AutoSelect()
    {
        equipRating7Mat.Clear();
        if (equipRating7.Count < 21)
        {
            
        }
        else
        {

        }

        initOnStartEquipLegend.SetInit();
    }

    public void Click_Release()
    {

    }
}
