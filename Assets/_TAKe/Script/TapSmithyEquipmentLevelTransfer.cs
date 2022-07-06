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
using CodeStage.AntiCheat.ObscuredTypes;

/// <summary>
/// 장비 레벨 이전  
/// </summary>
public class TapSmithyEquipmentLevelTransfer : MonoBehaviour
{
    [SerializeField] private GameDatabase.TableDB.Equipment temp_man_equip;
    [SerializeField] private GameDatabase.TableDB.Equipment temp_mat_equip;

    public GameDatabase.TableDB.Equipment GetSelectMain => temp_man_equip;
    public GameDatabase.TableDB.Equipment GetSelectMat => temp_mat_equip;

    [SerializeField] private long equipHighLevelUID;

    /// <summary> 강화 레벨 전승의 메인 장비를 선택되었다면 장비타입 번호를 리턴, 그렇지 않다면 -1 </summary>
    public int GetSelectMainEquipType() => temp_man_equip.aInUid > 0 ? temp_man_equip.eq_ty : -1;
    public int GetSelectMainEnhantLevel() => temp_man_equip.m_ehnt_lv;
    public long GetSelectMainUID() => temp_man_equip.aInUid;

    [SerializeField] GameObject  go_tapResult, go_tapIconResult, go_tapInfoResult, go_tapMatReady, go_tapOkSelect, go_tapNoSelect, go_noSelectNoti1, go_noSelectNoti2;
    [SerializeField] GameObject go_Mat, go_Man; // 재료 장비, 이전 받을 장비 
    // 레벨 이전 장비 탭 
    [SerializeField] Image image_MatIcon, image_ManIcon; // 이전 장비 아이콘, 이전 받을 장비 매인 아이콘 
    [SerializeField] Image image_MatRatingBg, image_ManRatingBg; // 이전 장비 등급 배경, 이전 받을 장비 등급 배경 
    [SerializeField] Image image_MatRatingOutline, image_ManRatinOutline; // 이전 장비 등급 라인, 이전 받을 장비 등급 라인 
    [SerializeField] Text text_MatRating, text_ManRating;
    [SerializeField] Text text_MatEnhantLevel, text_ManEnhantLevel; // 재료 장비 레벨, 매인 장비 레벨 
    [SerializeField] Text text_MatName, text_ManName; // 재료 장비 이름, 매인 장비 이름 

    // 레벨 이전 시작 탭 
    [SerializeField] Text text_sMatEnhantLevel, text_sManEnhantLevel; // 강화 레벨 전 -> 후 
    [SerializeField] Text text_sMainStatName, text_sBefMainStat, text_sAftMainStat; // 이전받을 장비의 현재 스탯, 이전 받은 후 스탯  
    
    [SerializeField] Text text_TransferGoldPrice; // 전승 골드 비용
    [SerializeField] Text text_TransferDiaPrice; // 전승 다이아 비용 
    [SerializeField] Image imStartBtnBg;
    void Awake ()
    {
        nameStrAllCombat = LanguageGameData.GetInstance().GetString("ui.name.combat_power"); // 전투력 
        nameStrMnSt = LanguageGameData.GetInstance().GetString("ui.name.stat"); // str 스탯 
        nameStrOp = LanguageGameData.GetInstance().GetString("ui.name.option"); // str 옵션  
        nameStrSpOp = LanguageGameData.GetInstance().GetString("text.ornamentOption"); // str 장신구 전용 옵션 

        Init();
    }

    public void Init ()
    {
        equipHighLevelUID = 0;
        go_Man.SetActive(false);
        go_Mat.SetActive(false);
        temp_man_equip = default;
        temp_mat_equip = default;
        image_ManRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
        image_MatRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
    }

    public void SetInitData(GameDatabase.TableDB.Equipment _eqDB)
    {
        LogPrint.Print("-------------- 111 SetInitData---------------- _equip.uid : " + _eqDB.aInUid);

        _eqDB.m_norm_lv = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(_eqDB.eq_ty).m_norm_lv;
        acSyntInfo_Result.blockScreen.gameObject.SetActive(false);
        go_tapIconResult.SetActive(false);

        if (_eqDB.aInUid <= 0)
        {
            SetData(default, true, true);
            SetData(default, false, true);

            acSyntInfo_Result.txt_SuccessOrFail.text = "강화 레벨 전승 준비";
            
            go_tapMatReady.SetActive(true);
            go_tapIconResult.SetActive(false);
            go_tapResult.SetActive(false);
            go_tapOkSelect.SetActive(false);
            go_tapNoSelect.SetActive(true);
            //go_noSelectNoti1.SetActive(true);
            //go_noSelectNoti2.SetActive(false);
        }
        else if (temp_man_equip.aInUid <= 0 && temp_mat_equip.aInUid <= 0)
        {
            // 레벨 이전 받을 장비를 선택 
            SetData(_eqDB, true, false);

            //go_noSelectNoti1.SetActive(false);
            //go_noSelectNoti2.SetActive(true);
        }
        else if (temp_man_equip.aInUid > 0)
        {
            if(temp_man_equip.eq_ty == _eqDB.eq_ty)
            {
                // 이전 재료 장비를 선택 
                SetData(_eqDB, false, false);
            }
            else
            {
                LogPrint.Print("같은 부위의 장비를 선택하세요");
            }
        }
        else
        {
            LogPrint.Print("nu-=");
        }

        go_noSelectNoti1.SetActive(temp_man_equip.aInUid == 0 && temp_mat_equip.aInUid == 0);
        go_noSelectNoti2.SetActive(!(temp_man_equip.aInUid == 0 && temp_mat_equip.aInUid == 0));
    }

    public async void SetData(GameDatabase.TableDB.Equipment _equip, bool _isMainEquip, bool _release)
    {
        LogPrint.Print("-------------- 222 SetData---------------- _equip.uid : " + _equip.aInUid);

        // 해제 
        if (_equip.aInUid <= 0)
        {
            // 매인 해제 
            if (_isMainEquip == true && _release)
            {
                temp_man_equip.aInUid = 0;
                equipHighLevelUID = 0;
                go_Man.SetActive(false);
            }

            // 재료 해제 
            if(_isMainEquip == false && _release)
            {
                temp_mat_equip.aInUid = 0;
                go_Mat.SetActive(false);
            }
        }
        else // 선택 
        {
            int parts_type = _equip.eq_ty;
            int main_stat_id = _equip.ma_st_id;
            int parts_rating = _equip.eq_rt;
            int parts_idx = _equip.eq_id;
            int enhan_level = _equip.m_ehnt_lv;
            int state = _equip.m_state;
            int state_lock = _equip.m_lck;

            if (_isMainEquip == true) // 이전받을 장비 데이터 
            {
                temp_man_equip = _equip;
                go_Man.SetActive(true);

                image_ManIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(parts_type, parts_rating, parts_idx);
                equipHighLevelUID = GameDatabase.GetInstance().smithyDB.GetEquipTypeToHighEnhantLevelUID(parts_type, enhan_level);
                image_ManRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(parts_rating, _equip.eq_legend);
                image_ManRatinOutline.color = ResourceDatabase.GetInstance().GetItemColor(parts_rating);
                text_ManRating.color = ResourceDatabase.GetInstance().GetItemColor(parts_rating);
                text_ManRating.text = parts_rating > 0 ? LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", parts_rating)) : "";
                text_ManEnhantLevel.text = string.Format("Lv.{0}", enhan_level); // 장비 레벨 
                text_ManName.text = GameDatabase.StringFormat.GetEquipName(parts_type, parts_rating, parts_idx); // 장비 이름 
            }
            else // 이전 재료 장비 데이터 
            {
                temp_mat_equip = _equip;
                go_Mat.SetActive(true);
                imStartBtnBg.gameObject.SetActive(false);

                image_MatIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(parts_type, parts_rating, parts_idx);
                image_MatRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(parts_rating, _equip.eq_legend);
                image_MatRatingOutline.color = ResourceDatabase.GetInstance().GetItemColor(parts_rating);
                text_MatRating.color = ResourceDatabase.GetInstance().GetItemColor(parts_rating);
                text_MatRating.text = parts_rating > 0 ? LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", parts_rating)) : "";
                text_MatEnhantLevel.text = string.Format("Lv.{0}", enhan_level); // 장비 레벨 
                text_MatName.text = GameDatabase.StringFormat.GetEquipName(parts_type, parts_rating, parts_idx); // 장비 이름 

                var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
                long gold_price = GameDatabase.GetInstance().questDB.GetQuestEquipTransferGold(_equip.eq_rt, _equip.eq_id, _equip.m_ehnt_lv); // 레벨 이전 골드 비용 
                int dia_price = GameDatabase.GetInstance().questDB.GetQuestEquipTransferDia(_equip.eq_rt, _equip.eq_id); // 레벨 이전 다이아 비용 

                bool isBlueDiaLack = goods_db.m_dia < dia_price;
                int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
                text_TransferGoldPrice.text = goods_db.m_gold >= gold_price ? string.Format("{0:#,0}", gold_price) : string.Format("<color=red>{0:#,0}</color>", gold_price); // 강화 가격 
                text_TransferDiaPrice.text = goods_db.m_dia + tbc >= dia_price ? string.Format("{0:#,0}", dia_price) : string.Format("<color=red>{0:#,0}</color>", dia_price); // 강화 필요 루비(에테르) 
                imStartBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(goods_db.m_gold >= gold_price && goods_db.m_dia + tbc >= dia_price);
                imStartBtnBg.gameObject.SetActive(true);
            }

            LogPrint.Print("temp_man_equip.aInUid:" + temp_man_equip.aInUid + ", temp_mat_equip.aInUid : " + temp_mat_equip.aInUid);
            // 재료 + 매인 선택 완료 
            if(temp_man_equip.aInUid > 0 && temp_mat_equip.aInUid > 0)
            {
                go_tapNoSelect.SetActive(false);
                go_tapOkSelect.SetActive(true);
            }
           
            go_tapInfoResult.SetActive(false);  // 합성 결과 스탯 탭 
        }
    }

    /// <summary>
    /// 선택 해제 : 재료만 해제 
    /// </summary>
    public void ClickMatItemRelease()
    {
        if (temp_mat_equip.aInUid <= 0)
            return;

        foreach (var item in GameObject.FindObjectsOfType<ScrollIndexCallbackSmithy>())
        {
            item.ReleaseEnhantTransferMat(temp_man_equip);
        }

        SetData(default, false, true);

        go_tapIconResult.SetActive(false);
        go_tapResult.SetActive(false);
        go_tapOkSelect.SetActive(false);
        go_tapNoSelect.SetActive(true);
        //go_noSelectNoti1.SetActive(false);
        //go_noSelectNoti2.SetActive(true);
    }

    /// <summary>
    /// 선택 해제 : 재료 + 매인 해제 
    /// </summary>
    public void ClickMainItemRelease()
    {
        if (temp_man_equip.aInUid <= 0)
            return;

        foreach (var item in GameObject.FindObjectsOfType<ScrollIndexCallbackSmithy>())
        {
            item.SelectReleaseEnhantTransfer();
        }

        SetInitData(default);
    }

    public async void ClickStart()
    {
        var eq_mat_data = temp_mat_equip;
        var eq_man_data = temp_man_equip;
        long gold_price = GameDatabase.GetInstance().questDB.GetQuestEquipTransferGold(eq_mat_data.eq_rt, eq_mat_data.eq_id, eq_mat_data.m_ehnt_lv); // 레벨 이전 골드 비용 
        int dia_price = GameDatabase.GetInstance().questDB.GetQuestEquipTransferDia(eq_mat_data.eq_rt, eq_mat_data.eq_id); // 레벨 이전 다이아 비용 
        if (eq_mat_data.aInUid > 0 && eq_man_data.aInUid > 0)
        {
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            bool isBlueDiaLack = goods_db.m_dia < dia_price;
            int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
            if (goods_db.m_gold >= gold_price && goods_db.m_dia + tbc >= dia_price)
            {
                acSyntInfo_Result.blockScreen.gameObject.SetActive(true);

                // 비용 차감 
                goods_db.m_gold -= gold_price;
                int dedDia = goods_db.m_dia -= dia_price; // 내 현재 블루 다이아 차감
                int dedTbc = dedDia < 0 ? Math.Abs(dedDia) : 0;
                Task<bool> tsk_tbc = GameDatabase.GetInstance().tableDB.DeductionTBC(dedTbc);
                Task<bool> tsk_goods = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db); // 강화 비용 값 전송 
                while (tsk_tbc.IsCompleted == false || tsk_goods.IsCompleted == false) { await Task.Delay(100); }

                int chng_eh_lv = eq_mat_data.m_ehnt_lv;
                eq_man_data.m_ehnt_lv = chng_eh_lv;
                eq_mat_data.m_ehnt_lv = 0;

                Param mat_update_param = new Param();
                Param man_update_param = new Param();
                mat_update_param.Add("m_ehnt_lv", 0);
                if (eq_man_data.m_ehnt_lv <= 15)
                    man_update_param.Add("m_ehnt_lv", eq_man_data.m_ehnt_lv);
                else // 강화 레벨 하락 랜덤 
                {
                    float rate_acc = 0.0f;
                    float r = GameDatabase.GetInstance().GetRandomPercent();
                    for (int i = 0; i <= 3; i++)
                    {
                        rate_acc += GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("equip.enhant.transfer.lv.{0}", i)).val_float;
                        if (r < rate_acc)
                        {
                            man_update_param.Add("m_ehnt_lv", eq_man_data.m_ehnt_lv -= i);
                            break;
                        }
                    }
                }
 
                Task<bool> task_mat = GameDatabase.GetInstance().tableDB.SetUpdateChangeEquipmentData(eq_mat_data, mat_update_param); // 재료 레벨 초기화 전송 
                Task<bool> task_man = GameDatabase.GetInstance().tableDB.SetUpdateChangeEquipmentData(eq_man_data, man_update_param); // 이전된 장비 레벨 전송 
                while (task_mat.IsCompleted == false || task_man.IsCompleted == false) { await Task.Delay(100); }

                foreach (var item in GameObject.FindObjectsOfType<ScrollIndexCallbackSmithy>())
                    item.EquipCellRefresh();

                if (eq_mat_data.m_state > 0 || eq_man_data.m_state > 0)
                {
                    GameDatabase.GetInstance().characterDB.SetPlayerStatValue();
                    MainUI.GetInstance().tapCharacter.SetStatView(GameDatabase.GetInstance().characterDB.GetStat());
                    GameMng.GetInstance().myPZ.igp.statValue = GameDatabase.GetInstance().characterDB.GetStat(); // 장비 관련, 스탯
                }

                PlayTransferAnimation(eq_man_data);
            }
            else
            {
                if (goods_db.m_gold < gold_price)
                {
                    PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("골드가 부족합니다.\n골드 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
                }
                else if (goods_db.m_dia + tbc < dia_price)
                {
                    PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
                }
            }
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("선택된 장비의 데이터 식별ID값 오류입니다. 문제가 지속된다면 게임을 재실행 해주시기 바랍니다.");
    }

    [SerializeField] AcSyntResultInfo acSyntInfo_Result;
    [System.Serializable]
    public struct AcSyntInfo
    {
        // 아이콘 정보 
        public Text txt_Rating, txt_EhntLv, txt_EquipName;
        public Image img_Icon, img_RatingBg, img_RatingLine;
        public GameObject go_Root;
    }
    [System.Serializable]
    public struct AcSyntResultInfo
    {
        // 아이콘 정보 
        public AcSyntInfo acSyntMatInfo;
        public Animator ani;
        public Text txt_SuccessOrFail; // 성공 결과 라벨 
        public Image img_IconGray;
        public CanvasGroup cnva_StartBtn;

        // 스탯 
        public Text txt_AllCombat, txt_MnStatCompat, txt_MnStatName, txt_MnStatVal;

        // 장비 현재 옵션 및 전용 옵션 
        public Text txt_NowSopCompat, txt_NowSopName, txt_NowSopStat, txt_NowOpCompat;
        public Text[] txt_NowOpName, txt_NowOpStat;

        public GameObject go_sopStrikethrough;
        public Animator an_ResultIcon;
        public BlockScreen blockScreen;
    }

    string nameStrAllCombat, nameStrMnSt, nameStrOp, nameStrSpOp;

    async void PlayTransferAnimation(GameDatabase.TableDB.Equipment result_eqDb)
    {
        acSyntInfo_Result.blockScreen.CenterAlphaZero();
        acSyntInfo_Result.ani.Play("AcceSyntStart");
        go_tapOkSelect.SetActive(false);      // 합성 준비 탭 
        while (!acSyntInfo_Result.ani.GetCurrentAnimatorStateInfo(0).IsName("AcceSyntStart"))
            await Task.Delay(100);

        while (acSyntInfo_Result.ani.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            await Task.Delay(100);
            if (acSyntInfo_Result.ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f && !go_tapIconResult.activeSelf)
            {
                go_tapIconResult.SetActive(true); // 합성 결과 아이콘(애니) 탭 켬  

                // 미리 성공or실패 UI정보를 세팅 
                SuccessData(result_eqDb);
            }
        }

        go_tapMatReady.SetActive(false);            // 선택한 합성 준비 재료 아이콘 탭 
        string anName = string.Format("Result_EquipIconInfoStart_rt{0}", result_eqDb.eq_rt);
        acSyntInfo_Result.an_ResultIcon.Play(anName);

        while (acSyntInfo_Result.an_ResultIcon.GetCurrentAnimatorStateInfo(0).IsName(anName) == false)
            await Task.Delay(100);

        while (acSyntInfo_Result.an_ResultIcon.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f)
            await Task.Delay(100);

        // 결과 창 
        go_tapInfoResult.SetActive(true);  // 합성 결과 스탯 탭 
        acSyntInfo_Result.txt_SuccessOrFail.text = "강화 레벨 전승 완료!!";
        MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
        acSyntInfo_Result.blockScreen.CenterObjectDisable();
        acSyntInfo_Result.blockScreen.OnText(0.5f);
    }

    /// <summary> 전승 결과 </summary>
    private void SuccessData(GameDatabase.TableDB.Equipment result_eqDb)
    {
        // ----- 아이콘 정보 -----
        Color corRat = ResourceDatabase.GetInstance().GetItemColor(result_eqDb.eq_rt);
        acSyntInfo_Result.acSyntMatInfo.img_RatingLine.color = corRat;
        acSyntInfo_Result.acSyntMatInfo.img_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(result_eqDb.eq_ty, result_eqDb.eq_rt, result_eqDb.eq_id);
        acSyntInfo_Result.acSyntMatInfo.img_Icon.enabled = true;
        acSyntInfo_Result.acSyntMatInfo.img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(result_eqDb.eq_rt, result_eqDb.eq_legend);
        acSyntInfo_Result.acSyntMatInfo.txt_Rating.color = corRat;
        acSyntInfo_Result.acSyntMatInfo.txt_Rating.text = result_eqDb.eq_rt > 0 ? LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", result_eqDb.eq_rt)) : "";
        acSyntInfo_Result.acSyntMatInfo.txt_EquipName.text = GameDatabase.StringFormat.GetEquipName(result_eqDb.eq_ty, result_eqDb.eq_rt, result_eqDb.eq_id); // 장비 이름 
        acSyntInfo_Result.acSyntMatInfo.txt_EhntLv.text = string.Format("Lv.{0}", result_eqDb.m_ehnt_lv);
        acSyntInfo_Result.txt_AllCombat.text = string.Format("{0} <size=30>({1:#,0})</size>", nameStrAllCombat, GameDatabase.GetInstance().tableDB.GetEquipCombatPower(result_eqDb, "total"));

        // ----- 매인 스탯 (+강화 스탯 값) -----
        // 매인 스탯 전투력 
        acSyntInfo_Result.txt_MnStatCompat.text = string.Format("{0} <size=24>({1:#,0})</size>", nameStrMnSt, GameDatabase.GetInstance().tableDB.GetEquipCombatPower(result_eqDb, "main"));
        acSyntInfo_Result.txt_MnStatName.text = GameDatabase.StringFormat.GetEquipStatName(result_eqDb.ma_st_id);// 매인 스탯 이름 
        object[] mn_stat_val = GameDatabase.GetInstance().chartDB.GetMainStatValue(result_eqDb);
        if (mn_stat_val[0].GetType() == typeof(float))
            acSyntInfo_Result.txt_MnStatVal.text = string.Format("{0:0.000}", (float)mn_stat_val[0]);
        else if (mn_stat_val[0].GetType() == typeof(long))
            acSyntInfo_Result.txt_MnStatVal.text = string.Format("{0:#,0}", (long)mn_stat_val[0]);

        // ----- 옵션 스탯 -----
        var statOp = result_eqDb.st_op;
        // 옵션 스탯 전투력 
        acSyntInfo_Result.txt_NowOpCompat.text = string.Format("{0} <size=24>(+{1:#,0})</size>", nameStrOp, GameDatabase.GetInstance().tableDB.GetEquipCombatPower(result_eqDb, "op"));
        for (int i = 0; i < 4; i++)
        {
            var sod = i == 0 ? statOp.op1 : i == 1 ? statOp.op2 : i == 2 ? statOp.op3 : i == 3 ? statOp.op4 : new GameDatabase.TableDB.StatOp();
            if (sod.id > 0)
            {
                acSyntInfo_Result.txt_NowOpName[i].text = GameDatabase.StringFormat.GetEquipStatName(sod.id);
                object wer_op_stat_val = (long)GameDatabase.GetInstance().chartDB.GetEquipOptionStatValue(sod.id, sod.rlv, result_eqDb.eq_rt, result_eqDb.eq_id, result_eqDb.m_ehnt_lv, true, result_eqDb.eq_legend, result_eqDb.eq_legend_sop_id, result_eqDb.eq_legend_sop_rlv);
                acSyntInfo_Result.txt_NowOpStat[i].text = string.Format("+{0:#,0}", (long)wer_op_stat_val);
            }
            else
            {
                acSyntInfo_Result.txt_NowOpName[i].text = "-";
                acSyntInfo_Result.txt_NowOpStat[i].text = "-";
            }
        }

        // ----- 장신구 전용 옵션 -----
        if(GameDatabase.GetInstance ().chartDB.GetIsPartsTypeAcce(result_eqDb.eq_ty))
        {
            float[] wer_acOp_statVal = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionValue(result_eqDb);
            acSyntInfo_Result.txt_NowSopName.text = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionName(result_eqDb.st_sop_ac.id);
            acSyntInfo_Result.txt_NowSopStat.text = string.Format("+{0:0.000}%", wer_acOp_statVal[0]);
            acSyntInfo_Result.go_sopStrikethrough.SetActive(false);
        }
        else
        {
            acSyntInfo_Result.go_sopStrikethrough.SetActive(true);
            acSyntInfo_Result.txt_NowSopName.text = "-";
            acSyntInfo_Result.txt_NowSopStat.text = "-";
        }
    }

    public void Click_ResultClose()
    {
        //ClickMainItemRelease();
        SetInitData(default);
        MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
    }
}
