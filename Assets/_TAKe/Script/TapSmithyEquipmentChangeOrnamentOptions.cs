using BackEnd;
using static BackEnd.BackendAsyncClass;
using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using LitJson;

/// <summary>
/// 장신구 옵션 변경 
/// </summary>
public class TapSmithyEquipmentChangeOrnamentOptions : MonoBehaviour
{
    [SerializeField] private GameDatabase.TableDB.Equipment _nowSelectEquip;
    [SerializeField] private GameDatabase.TableDB.Equipment _resultEquip;
    public GameDatabase.TableDB.Equipment GetSelectEquipAcce() => _nowSelectEquip;

    [SerializeField] AcOpChangeInfo acOpChngInfo;
    [System.Serializable]
    public struct AcOpChangeInfo
    {
        // 옵션 및 전용 옵션 리스트 
        public GameObject go_SpOpList, go_OpList;
        public GameObject go_TapEmpty, go_TapSelect;
        public CanvasGroup[] cnva_SpOp, cnva_Op;
        public Text[] text_SpOpStatName, text_OpStatName;
        public Text[] text_SpOpStat, text_OpStat;
        public Image img_SpOpBtn, img_OpBtn;
        public CanvasGroup cnva_NowSpop, cnva_NowOp;
        public GameObject[] go_LineBoxSpOp, go_LineBoxOp;

        public Text txt_GoldPrice; // 변경 골드 소모량  
        public Text txt_DiaPrice; // 변경 다이아 소모량 

        // 장비 아이콘 정보 
        public Text txt_Rating, txt_EhntLv, txt_EquipName;
        public Image img_Icon, img_RatingBg, imRatingOutline;
        public GameObject go_WearLabel, go_Lock, go_CardLine;

        // 장비 현재 옵션 및 전용 옵션 
        public Text txt_NowOpCompat, txt_NowSopTitleName, txt_NowSpOpCompat, txt_NowSpOpName, txt_NowSpOpStat;
        public Text[] txt_NowOpName, txt_NowOpStat;

        public GameObject go_opRootBtn, go_comRevBtn;
      
        public Text txGoldPrice;
        public Text txDiaPrice;
        public Image imStartBtnBg;
        public CanvasGroup cg_StartBtn;

        public ChangeType chngOpType;
        public enum ChangeType
        {
            SpOp,
            Op
        }

        public BlockScreen blockScreen;
    }

    string nameStrOp, nameStrSpOp;

    void Awake ()
    {
        nameStrOp = LanguageGameData.GetInstance().GetString("ui.name.option"); // str : 옵션  
        nameStrSpOp = LanguageGameData.GetInstance().GetString("text.ornamentOption"); // str : 장신구 전용 옵션 
    }
    
    void Start ()
    {
        //SetDatas(default, true);
    }

    public async void SetDatas (GameDatabase.TableDB.Equipment _eqDB, bool select = true)
    {
        _eqDB.m_norm_lv = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(_eqDB.eq_ty).m_norm_lv;
        bool isNotSelect = _eqDB.aInUid <= 0;
        _nowSelectEquip = _eqDB;
        acOpChngInfo.blockScreen.gameObject.SetActive(false);
        waitChange = !select;

        if (select)
        {
            foreach (var item in acOpChngInfo.cnva_SpOp)
            {
                item.alpha = 1.0f;
                item.gameObject.SetActive(true);
            }
            foreach (var item in acOpChngInfo.cnva_Op)
            {
                item.alpha = 1.0f;
                item.gameObject.SetActive(true);
            }
            //foreach (var item in opChngInfo.go_LineBoxSpOp) item.SetActive(false);
            //foreach (var item in opChngInfo.go_LineBoxOp) item.SetActive(false);

            acOpChngInfo.cg_StartBtn.alpha = 1.0f;
            acOpChngInfo.go_opRootBtn.SetActive(true);
            acOpChngInfo.go_comRevBtn.SetActive(false);
            acOpChngInfo.go_CardLine.SetActive(false);
        }
        else
        {
            acOpChngInfo.cg_StartBtn.alpha = 0.3f;
            acOpChngInfo.go_opRootBtn.SetActive(false);
            acOpChngInfo.go_comRevBtn.SetActive(true);
        }
        
        acOpChngInfo.go_SpOpList.SetActive(acOpChngInfo.chngOpType == AcOpChangeInfo.ChangeType.SpOp);
        acOpChngInfo.go_OpList.SetActive(acOpChngInfo.chngOpType == AcOpChangeInfo.ChangeType.Op);
        acOpChngInfo.img_SpOpBtn.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(acOpChngInfo.chngOpType == AcOpChangeInfo.ChangeType.SpOp);
        acOpChngInfo.img_OpBtn.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(acOpChngInfo.chngOpType == AcOpChangeInfo.ChangeType.Op);
        acOpChngInfo.go_TapEmpty.SetActive(isNotSelect);
        acOpChngInfo.go_TapSelect.SetActive(!isNotSelect);
        acOpChngInfo.cnva_NowSpop.alpha = acOpChngInfo.chngOpType == AcOpChangeInfo.ChangeType.SpOp ? 1.0f : 0.3f;
        acOpChngInfo.cnva_NowOp.alpha = acOpChngInfo.chngOpType == AcOpChangeInfo.ChangeType.Op ? 1.0f : 0.3f;

        bool isAcce = GameDatabase.GetInstance ().tableDB.GetIsPartsTypeAcce(_eqDB.eq_ty);

        // #########
        // 전용 옵션 리스트 및 예상 값 
        // #########
        if (acOpChngInfo.chngOpType == AcOpChangeInfo.ChangeType.SpOp)
        {
            if(isAcce) // 장신구 
            {
                // ----- 장신구 전용 옵션 리스트 [이름, 값] -----
                for (int i = 0; i < GameDatabase.TableDB.acce_SpecialOpStatCount; i++)
                {
                    acOpChngInfo.cnva_SpOp[i].gameObject.SetActive(true);
                    acOpChngInfo.text_SpOpStatName[i].text = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionName(i + 1);
                    if (!isNotSelect)
                    {
                        float[] spOpValue = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionMinMax(_eqDB.eq_rt, _eqDB.eq_id, i + 1);
                        acOpChngInfo.text_SpOpStat[i].text = string.Format("{0:0.000}%~{1:0.000}%", spOpValue[0], spOpValue[1]);
                    }
                    else acOpChngInfo.text_SpOpStat[i].text = "??? ~ ???";
                }
            }
            else
            {
                // ----- 장비 전용 옵션 리스트 [이름, 값] -----
                for (int i = 0; i < 10; i++)
                {
                    string eqSopName = GameDatabase.GetInstance().chartDB.GetEquipSpecialOptionName(i + 1);
                    acOpChngInfo.cnva_SpOp[i].gameObject.SetActive(!string.IsNullOrEmpty(eqSopName));
                    if (!string.IsNullOrEmpty(eqSopName))
                    {
                        acOpChngInfo.text_SpOpStatName[i].text = GameDatabase.GetInstance().chartDB.GetEquipSpecialOptionName(i + 1);
                        if (!isNotSelect)
                        {
                            float[] spOpValue = GameDatabase.GetInstance().chartDB.GetEquipSpecialOptionMinMax(_eqDB.eq_rt, _eqDB.eq_id, i + 1);
                            acOpChngInfo.text_SpOpStat[i].text = string.Format("{0:0.000}%~{1:0.000}%", spOpValue[0], spOpValue[1]);
                        }
                        else acOpChngInfo.text_SpOpStat[i].text = "??? ~ ???";
                    }
                }
            }
        }
        else
        {
            // #########
            // 일반 옵션 리스트 및 예상 값 (장비, 장신구 통합) 
            // #########
            // ----- 일반 옵션 리스트 [이름, 값] ----- 
            int arrId = 0;
            for (int opst_id = 1; opst_id <= GameDatabase.TableDB.equip_OpStatCount; opst_id++)
            {
                if (opst_id == 1 || opst_id == 2 || opst_id == 4 || opst_id == 5 || opst_id == 8)
                {
                    acOpChngInfo.text_OpStatName[arrId].text = GameDatabase.StringFormat.GetEquipStatName(opst_id);
                    if (!isNotSelect)
                    {
                        object opValueMin = (long)GameDatabase.GetInstance().chartDB.GetEquipOptionStatValue(opst_id, 1, _eqDB.eq_rt, _eqDB.eq_id, _eqDB.m_ehnt_lv, true, _eqDB.eq_legend, _eqDB.eq_legend_sop_id, _eqDB.eq_legend_sop_rlv);
                        object opValueMax = (long)GameDatabase.GetInstance().chartDB.GetEquipOptionStatValue(opst_id, GameDatabase.GetInstance().chartDB.GetDicBalanceStatMaxLevel(), _eqDB.eq_rt, _eqDB.eq_id, _eqDB.m_ehnt_lv, true, _eqDB.eq_legend, _eqDB.eq_legend_sop_id, _eqDB.eq_legend_sop_rlv);
                        acOpChngInfo.text_OpStat[arrId].text = string.Format("{0:#,0} ~ {1:#,0}", (long)opValueMin, (long)opValueMax);
                    }
                    else acOpChngInfo.text_OpStat[arrId].text = "??? ~ ???";

                    arrId++;
                }
            }
        }

        LogPrint.Print("<color=red>-----------isNotSelect-------------:" + isNotSelect + "</color>");
        // 선택 되었을 때 
        if (isNotSelect == false)
        {
            acOpChngInfo.imStartBtnBg.gameObject.SetActive(false);
            // ----- 현재 전용 옵션 이름, 전투력, 옵션 값 -----
            if (isAcce)
            {
                float[] wer_acOp_statVal = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionValue(_eqDB);
                acOpChngInfo.txt_NowSopTitleName.text = "<color=#FFAD00>장신구 전용 옵션</color>";
                acOpChngInfo.txt_NowSpOpName.text = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionName(_eqDB.st_sop_ac.id);
                acOpChngInfo.txt_NowSpOpStat.text = string.Format("{0:0.000}%", wer_acOp_statVal[0]);
            }
            else
            {
                float wer_eqOp_statVal = GameDatabase.GetInstance().chartDB.GetEquipSpecialOptionValue(_eqDB.eq_legend_sop_id, _eqDB.eq_legend_sop_rlv);
                acOpChngInfo.txt_NowSopTitleName.text = "<color=#FFE800>진화된 전설 전용 옵션</color>";
                acOpChngInfo.txt_NowSpOpName.text = GameDatabase.GetInstance().chartDB.GetEquipSpecialOptionName(_eqDB.eq_legend_sop_id);
                acOpChngInfo.txt_NowSpOpStat.text = string.Format("{0:0.000}%", wer_eqOp_statVal);
            }

            // ----- 현재 옵션 이름, 전투력, 옵션 값 -----
            var statOp = _eqDB.st_op;
            for (int i = 0; i < 4; i++)
            {
                var sod = i == 0 ? statOp.op1 : i == 1 ? statOp.op2 : i == 2 ? statOp.op3 : i == 3 ? statOp.op4 : new GameDatabase.TableDB.StatOp();
                if (sod.id > 0)
                {
                    acOpChngInfo.txt_NowOpName[i].text = GameDatabase.StringFormat.GetEquipStatName(sod.id);
                    object wer_op_stat_val = (long)GameDatabase.GetInstance().chartDB.GetEquipOptionStatValue(sod.id, sod.rlv, _eqDB.eq_rt, _eqDB.eq_id, _eqDB.m_ehnt_lv, true, _eqDB.eq_legend, _eqDB.eq_legend_sop_id, _eqDB.eq_legend_sop_rlv);
                    acOpChngInfo.txt_NowOpStat[i].text = string.Format("+{0:#,0}", (long)wer_op_stat_val);
                }
                else
                {
                    acOpChngInfo.txt_NowOpName[i].text = "-";
                    acOpChngInfo.txt_NowOpStat[i].text = "-";
                }
            }
            acOpChngInfo.txt_NowOpCompat.text = string.Format("{0} <size=24>(+{1:#,0})</size>", nameStrOp, GameDatabase.GetInstance().tableDB.GetEquipCombatPower(_eqDB, "op"));

            // ----- 현재 장신구의 아이콘 정보 -----
            acOpChngInfo.go_WearLabel.SetActive(GameDatabase.GetInstance().tableDB.IsNowUseSlotWearEquip(_eqDB));
            acOpChngInfo.img_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(_eqDB.eq_ty, _eqDB.eq_rt, _eqDB.eq_id);
            acOpChngInfo.img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(_eqDB.eq_rt, _eqDB.eq_legend);
            acOpChngInfo.imRatingOutline.color = ResourceDatabase.GetInstance().GetItemColor(_eqDB.eq_rt);
            acOpChngInfo.txt_Rating.color = ResourceDatabase.GetInstance().GetItemColor(_eqDB.eq_rt);
            acOpChngInfo.go_Lock.SetActive(_eqDB.m_lck == 1);
            if (_eqDB.eq_rt > 0)
            {
                acOpChngInfo.txt_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", _eqDB.eq_rt));
            }
            else
            {
                acOpChngInfo.txt_Rating.text = string.Empty;
            }

            acOpChngInfo.txt_EhntLv.text = string.Format("Lv.{0}", _eqDB.m_ehnt_lv); // 장비 강화 레벨 
            acOpChngInfo.txt_EquipName.text = GameDatabase.StringFormat.GetEquipName(_eqDB.eq_ty, _eqDB.eq_rt, _eqDB.eq_id);

            LogPrint.Print("<color=red>-----------------------------------</color>");
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            long gold_price = isAcce ? GameDatabase.GetInstance().questDB.GetQuestAcceOptionChangeGold(_eqDB.eq_rt, _eqDB.eq_id) : GameDatabase.GetInstance().questDB.GetQuestEquipOptionChangeGold(_eqDB.eq_rt, _eqDB.eq_id);
            int diaPrice = isAcce ? GameDatabase.GetInstance().questDB.GetQuestAcceOptionChangeDia(_eqDB.eq_rt, _eqDB.eq_id) : GameDatabase.GetInstance().questDB.GetQuestEquipOptionChangeDia(_eqDB.eq_rt, _eqDB.eq_id, acOpChngInfo.chngOpType == AcOpChangeInfo.ChangeType.SpOp);

            bool isBlueDiaLack = goods_db.m_dia < diaPrice;
            int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
            int blue_dia = goods_db.m_dia;
            acOpChngInfo.txGoldPrice.text = goods_db.m_gold >= gold_price ? string.Format("{0:#,0}", gold_price) : string.Format("<color=red>{0:#,0}</color>", gold_price); // 강화 가격 
            acOpChngInfo.txDiaPrice.text = blue_dia + tbc >= diaPrice ? string.Format("{0:#,0}", diaPrice) : string.Format("<color=red>{0:#,0}</color>", diaPrice); // 강화 필요 루비(에테르) 
            acOpChngInfo.imStartBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(goods_db.m_gold >= gold_price && blue_dia + tbc >= diaPrice);
            acOpChngInfo.imStartBtnBg.gameObject.SetActive(true);
        }
    }

    /// 옵션 타입 변경 (전용 옵션 or 일반 옵션)
    public void Click_ChangeOpTyie(bool isSpOp)
    {
        var now_chty = acOpChngInfo.chngOpType;
        var chng_chty = isSpOp ? AcOpChangeInfo.ChangeType.SpOp : AcOpChangeInfo.ChangeType.Op;
        if (!object.Equals(now_chty, chng_chty))
        {
            acOpChngInfo.chngOpType = chng_chty;
            SetDatas(_nowSelectEquip);
        }
    }

    public void Click_Release ()
    {
        if (acOpChngInfo.blockScreen.gameObject.activeSelf)
        {
            return;
        }

        if (_nowSelectEquip.aInUid > 0)
        {
            SetDatas(default);
            foreach (var item in GameObject.FindObjectsOfType<ScrollIndexCallbackSmithy>())
                item.RefreshSelectReleaseAcceChangeOption();
        }
    }

    private bool waitChange =false;
    public async void Click_StartOpChange ()
    {
        if(!waitChange)
        {
            var temp_eq = _nowSelectEquip;
            bool isAcce = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(temp_eq.eq_ty);
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            long gold_price = isAcce ? GameDatabase.GetInstance().questDB.GetQuestAcceOptionChangeGold(temp_eq.eq_rt, temp_eq.eq_id) : GameDatabase.GetInstance().questDB.GetQuestEquipOptionChangeGold(temp_eq.eq_rt, temp_eq.eq_id);
            int diaPrice = isAcce ? GameDatabase.GetInstance().questDB.GetQuestAcceOptionChangeDia(temp_eq.eq_rt, temp_eq.eq_id) : GameDatabase.GetInstance().questDB.GetQuestEquipOptionChangeDia(temp_eq.eq_rt, temp_eq.eq_id, acOpChngInfo.chngOpType == AcOpChangeInfo.ChangeType.SpOp);

            bool isBlueDiaLack = goods_db.m_dia < diaPrice;
            int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
            int blue_dia = goods_db.m_dia;
            if (goods_db.m_gold >= gold_price && blue_dia + tbc >= diaPrice)
            {
                waitChange = true;
                acOpChngInfo.blockScreen.gameObject.SetActive(true);

                // 비용 차감 
                goods_db.m_gold -= gold_price;
                int dedDia = goods_db.m_dia -= diaPrice; // 내 현재 블루 다이아 차감
                int dedTbc = dedDia < 0 ? Math.Abs(dedDia) : 0;
                Task<bool> tsk_tbc = GameDatabase.GetInstance().tableDB.DeductionTBC(dedTbc);
                Task<bool> tsk_goods = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db); // 강화 비용 값 전송 
                while (tsk_tbc.IsCompleted == false || tsk_goods.IsCompleted == false) { await Task.Delay(100); }

                acOpChngInfo.go_opRootBtn.SetActive(false);
                if (acOpChngInfo.chngOpType == AcOpChangeInfo.ChangeType.SpOp)
                {
                    if(isAcce)
                        temp_eq.st_sop_ac = GameDatabase.GetInstance().tableDB.GetEquipAcceRandomSpecialOption(true);
                    else
                    {
                        var eq_sop = GameDatabase.GetInstance().tableDB.GetEquipAcceRandomSpecialOption(false);
                        temp_eq.eq_legend_sop_id = eq_sop.id;
                        temp_eq.eq_legend_sop_rlv = eq_sop.rlv;
                    }
                }
                else temp_eq.st_op = GameDatabase.GetInstance().tableDB.GetEquipRandomOption(temp_eq.eq_ty, temp_eq.eq_rt);

                _resultEquip = temp_eq;
                acOpChngInfo.cg_StartBtn.alpha = 0.3f;

                StopCoroutine("ChangeViewRoutin");
                StartCoroutine("ChangeViewRoutin");

                GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr19, 1, true, true); // 업적, nbr19 장신구 옵션 변경하기!
            }
            else
            {
                if (goods_db.m_gold < gold_price)
                {
                    PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("골드가 부족합니다.\n골드 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
                }
                else if (blue_dia + tbc < diaPrice)
                {
                    PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
                }
            }
        }
    }

    private IEnumerator ChangeViewRoutin ()
    {
        acOpChngInfo.blockScreen.CenterAlphaZero();
        if (acOpChngInfo.chngOpType == AcOpChangeInfo.ChangeType.SpOp) // 전용 옵션 
        {
            var spop_result_eq = _resultEquip;
            bool isAcce = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(spop_result_eq.eq_ty);
            int spop_zero_id = isAcce ? spop_result_eq.st_sop_ac.id - 1 : spop_result_eq.eq_legend_sop_id - 1;
            Text spop_txt_stat = acOpChngInfo.text_SpOpStat[spop_zero_id];
            CanvasGroup spop_cnva_grop = acOpChngInfo.cnva_SpOp[spop_zero_id];
            List<CanvasGroup> spop_shake = new List<CanvasGroup>();
            foreach (var item in acOpChngInfo.cnva_SpOp)
                spop_shake.Add(item);

            while (spop_shake.Count > 1)
            {
                yield return null;
                var spop_cg = spop_shake[UnityEngine.Random.Range(0, spop_shake.Count)];
                if (!object.Equals(spop_cg, spop_cnva_grop))
                {
                    if (spop_cg.gameObject.activeSelf)
                    {
                        while (spop_cg.alpha > 0)
                        {
                            yield return null;
                            spop_cg.alpha -= Time.deltaTime * 25f;
                            if (spop_cg.alpha <= 0)
                                spop_cg.gameObject.SetActive(false);
                        }
                    }
                    
                    spop_shake.Remove(spop_cg);
                }
            }

            yield return null;
            if(GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(spop_result_eq.eq_ty))
            {
                float[] wer_acOp_statVal = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionValue(spop_result_eq);
                spop_txt_stat.text = string.Format("{0:0.000}%", wer_acOp_statVal[0]);
            }
            else
            {
                float wer_eqOp_statVal = GameDatabase.GetInstance().chartDB.GetEquipSpecialOptionValue(spop_result_eq.eq_legend_sop_id, spop_result_eq.eq_legend_sop_rlv);
                spop_txt_stat.text = string.Format("{0:0.000}%", wer_eqOp_statVal);
            }
        }
        // 옵션 
        else
        {
            var op_result_eq = _resultEquip;
            var op_result_so = op_result_eq.st_op;
            foreach (var item in acOpChngInfo.cnva_Op)
            {
                item.alpha = 0;
                item.gameObject.SetActive(false);
            }
            yield return null;

            // ----- 결과 옵션 값 -----
            int forId = 0;
            for (int i = 0; i < 4; i++)
            {
                var sod = i == 0 ? op_result_so.op1 : i == 1 ? op_result_so.op2 : i == 2 ? op_result_so.op3 : i == 3 ? op_result_so.op4 : new GameDatabase.TableDB.StatOp();
                if (sod.id > 0)
                {
                    acOpChngInfo.text_OpStatName[forId].text = GameDatabase.StringFormat.GetEquipStatName(sod.id);
                    object wer_op_stat_val = (long)GameDatabase.GetInstance().chartDB.GetEquipOptionStatValue(sod.id, sod.rlv, op_result_eq.eq_rt, op_result_eq.eq_id, op_result_eq.m_ehnt_lv, true, op_result_eq.eq_legend, op_result_eq.eq_legend_sop_id, op_result_eq.eq_legend_sop_rlv);
                    acOpChngInfo.text_OpStat[forId].text = string.Format("+{0:#,0}", (long)wer_op_stat_val);

                    acOpChngInfo.cnva_Op[forId].gameObject.SetActive(true);
                    while (acOpChngInfo.cnva_Op[forId].alpha < 1)
                    {
                        yield return null;
                        acOpChngInfo.cnva_Op[forId].alpha += Time.deltaTime * 3f;
                    }

                    forId++;
                }
            }
        }

        acOpChngInfo.go_CardLine.SetActive(true);
        acOpChngInfo.blockScreen.CenterObjectDisable();
        acOpChngInfo.blockScreen.OnText(0.5f);
        acOpChngInfo.go_comRevBtn.SetActive(true);
    }

    /// <summary> 결과 서버 전송 </summary>
    private async Task<string> SendComplet ()
    {
        var send_equip = _resultEquip;
        Param upd_pam = new Param();
        if (acOpChngInfo.chngOpType == AcOpChangeInfo.ChangeType.SpOp)
        {
            if(GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(send_equip.eq_ty))
            {
                upd_pam.Add("st_sop_ac", JsonUtility.ToJson(send_equip.st_sop_ac));
            }
            else
            {
                upd_pam.Add("eq_legend_sop_id", send_equip.eq_legend_sop_id);
                upd_pam.Add("eq_legend_sop_rlv", send_equip.eq_legend_sop_rlv);
            }
        }
        else
        {
            upd_pam.Add("st_op", JsonUtility.ToJson(send_equip.st_op));
        }

        Task<bool> t2 = GameDatabase.GetInstance().tableDB.SendDataEquipment(send_equip, "update", upd_pam);
        await t2;
        if (t2.Result)
            return "success";
        else return "fail";
    }

    // 변경 확정 
    public async void Click_Complete ()
    {
        Task<string> t = SendComplet();
        await t;
        if(t.Result == "success")
        {
            SetDatas(_resultEquip);
            MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit(true);
            GameDatabase.GetInstance().characterDB.SetPlayerStatValue(); // 스탯 
            MainUI.GetInstance().tapCharacter.SetStatView(GameDatabase.GetInstance().characterDB.GetStat());
            GameMng.GetInstance().myPZ.Setting(); // 스탯 
        }
        else
        {
            if (t.Result == "fail")
            {

            }
        }

        waitChange = false;
    }

    // 되돌리기 
    public void Click_Revert()
    {
        SetDatas(_nowSelectEquip);
        waitChange = false;
    }
}
