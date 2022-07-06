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
using System.Net.Mail;
using CodeStage.AntiCheat.ObscuredTypes;

/// <summary>
/// 장신구 합성 
/// </summary>
public class TapSmithyOrnamentSynthesis : MonoBehaviour
{
    [SerializeField] private GameDatabase.TableDB.Equipment _nowSelectEquipMat1;
    [SerializeField] private GameDatabase.TableDB.Equipment _nowSelectEquipMat2;
    [SerializeField] private GameDatabase.TableDB.Equipment _nowResultEquip;

    public GameDatabase.TableDB.Equipment GetNowSelectEquipMat1() => _nowSelectEquipMat1;
    public GameDatabase.TableDB.Equipment GetNowSelectEquipMat2() => _nowSelectEquipMat2;

    [SerializeField] bool isSyntWait = false;
    [SerializeField] string lastAddMat = "empty";
    [SerializeField] AcSyntInfo acSyntInfo_Mat1;
    [SerializeField] AcSyntInfo acSyntInfo_Mat2;
    [SerializeField] AcSyntResultInfo acSyntInfo_Result;

    [SerializeField]
    GameObject
        go_RootMat,             // 선택한 합성 준비 재료 아이콘 탭 
        go_RootResult,          // 합성 결과 아이콘 탭 
        go_RootSyntInfo,        // 재료 선택 알림 탭 
        go_RootSyntReady,       // 합성 준비 탭
        go_RootSyntInfoSuccess,  // 합성 성공 결과 스탯 탭 
        go_RootSyntInfoFail;   // 합설 실패

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
     
        public Text txRedDiaGoldPrice;
        public Text txRedDiaPrice;
        public Text txBlueDiaGoldPrice;
        public Text txBlueDiaPrice;
        public Image imRedDiaStartBtnBg;
        public CanvasGroup cnva_RedDiaStartBtn;
        public Image imBlueDiaStartBtnBg;
        public CanvasGroup cnva_BlueDiaStartBtn;

        // 스탯 
        public Text txt_AllCombat, txt_MnStatCompat, txt_MnStatName, txt_MnStatVal;

        // 장비 현재 옵션 및 전용 옵션 
        public Text txt_NowSopCompat, txt_NowSopName, txt_NowSopStat, txt_NowOpCompat;
        public Text[] txt_NowOpName, txt_NowOpStat;

        // 성공 확률
        public Text txt_SuccessRate;
        public Animator an_ResultIcon;
        public BlockScreen blockScreen;
    }

    string nameStrAllCombat, nameStrMnSt, nameStrOp, nameStrSpOp, str_AcceSynthesisFailPercent;

    void Awake()
    {
        nameStrAllCombat = LanguageGameData.GetInstance().GetString("ui.name.combat_power"); // 전투력 
        nameStrMnSt = LanguageGameData.GetInstance().GetString("ui.name.stat"); // str 스탯 
        nameStrOp = LanguageGameData.GetInstance().GetString("ui.name.option"); // str 옵션  
        nameStrSpOp = LanguageGameData.GetInstance().GetString("text.ornamentOption"); // str 장신구 전용 옵션 
        str_AcceSynthesisFailPercent = LanguageGameData.GetInstance().GetString("text.acceSynthesisFailPercent");
    }

    void Start()
    {
        SetDatas(default, true, -1);
    }

    public async void SetDatas(GameDatabase.TableDB.Equipment _eqDB, bool init = false, int init_mat = -1)
    {
        _eqDB.m_norm_lv = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(_eqDB.eq_ty).m_norm_lv;
        acSyntInfo_Result.blockScreen.gameObject.SetActive(false);

        if (init)
        {
            lastAddMat = "empty";
            isSyntWait = false;
            go_RootMat.SetActive(true);
            go_RootResult.SetActive(false);
            go_RootSyntInfo.SetActive(true);
            go_RootSyntReady.SetActive(false);
            go_RootSyntInfoSuccess.SetActive(false);
            go_RootSyntInfoFail.SetActive(false);
            acSyntInfo_Result.txt_SuccessOrFail.text = "장신구 합성";
            acSyntInfo_Result.txt_SuccessOrFail.color = Color.white;
            acSyntInfo_Result.cnva_RedDiaStartBtn.alpha = 1.0f;
            acSyntInfo_Result.cnva_BlueDiaStartBtn.alpha = 1.0f;

            if (init_mat == -1)
            {
                lastAddMat = "empty";
                _nowSelectEquipMat1 = default;
                _nowSelectEquipMat2 = default;
                _nowResultEquip = default;
                acSyntInfo_Mat1.img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
                acSyntInfo_Mat2.img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
                
                acSyntInfo_Mat1.go_Root.SetActive(false);
                acSyntInfo_Mat2.go_Root.SetActive(false);
                acSyntInfo_Result.acSyntMatInfo.go_Root.SetActive(false);
            }
            else if (init_mat == 1)
            {
                lastAddMat = "mat1";
                _nowSelectEquipMat1 = default;
                acSyntInfo_Mat1.img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
                acSyntInfo_Mat1.go_Root.SetActive(false);
            }
            else if (init_mat == 2)
            {
                lastAddMat = "mat2";
                _nowSelectEquipMat2 = default;
                acSyntInfo_Mat2.img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
                acSyntInfo_Mat2.go_Root.SetActive(false);
            }
        }
        else
        {
            if(_nowSelectEquipMat1.aInUid > 0 && _nowSelectEquipMat2.aInUid > 0)
            {
                if (string.Equals(lastAddMat, "mat1"))
                    _nowSelectEquipMat1 = default;
                else if (string.Equals(lastAddMat, "mat2"))
                    _nowSelectEquipMat2 = default;
            }

            if (_nowSelectEquipMat1.aInUid <= 0) // 재료 1 정보 입력 
            {
                lastAddMat = "mat1";
                _nowSelectEquipMat1 = _eqDB;
                acSyntInfo_Mat1.go_Root.SetActive(true);

                // ----- 재료1 장신구의 아이콘 정보 -----
                acSyntInfo_Mat1.img_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(_eqDB.eq_ty, _eqDB.eq_rt, _eqDB.eq_id);
                acSyntInfo_Mat1.img_RatingLine.color = ResourceDatabase.GetInstance().GetItemColor(_eqDB.eq_rt);
                acSyntInfo_Mat1.img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(_eqDB.eq_rt);
                acSyntInfo_Mat1.txt_Rating.color = ResourceDatabase.GetInstance().GetItemColor(_eqDB.eq_rt);
                acSyntInfo_Mat1.txt_Rating.text = _eqDB.eq_rt > 0 ? LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", _eqDB.eq_rt)) : "";

                acSyntInfo_Mat1.txt_EhntLv.text = string.Format("Lv.{0}", _eqDB.m_ehnt_lv); // 장비 강화 레벨 
                acSyntInfo_Mat1.txt_EquipName.text = GameDatabase.StringFormat.GetEquipName(_eqDB.eq_ty, _eqDB.eq_rt, _eqDB.eq_id); // 장비 이름 
            }
            else if (_nowSelectEquipMat1.aInUid > 0 && _nowSelectEquipMat2.aInUid <= 0)  // 재료 2 정보 입력 
            {
                lastAddMat = "mat2";
                _nowSelectEquipMat2 = _eqDB;
                acSyntInfo_Mat2.go_Root.SetActive(true);

                // ----- 재료2 장신구의 아이콘 정보 -----
                acSyntInfo_Mat2.img_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(_eqDB.eq_ty, _eqDB.eq_rt, _eqDB.eq_id);
                acSyntInfo_Mat2.img_RatingLine.color = ResourceDatabase.GetInstance().GetItemColor(_eqDB.eq_rt);
                acSyntInfo_Mat2.img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(_eqDB.eq_rt);
                acSyntInfo_Mat2.txt_Rating.color = ResourceDatabase.GetInstance().GetItemColor(_eqDB.eq_rt);
                acSyntInfo_Mat2.txt_Rating.text = _eqDB.eq_rt > 0 ? LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", _eqDB.eq_rt)) : "";

                acSyntInfo_Mat2.txt_EhntLv.text = string.Format("Lv.{0}", _eqDB.m_ehnt_lv); // 장비 강화 레벨 
                acSyntInfo_Mat2.txt_EquipName.text = GameDatabase.StringFormat.GetEquipName(_eqDB.eq_ty, _eqDB.eq_rt, _eqDB.eq_id); // 장비 이름
            }

            // 합성 준비 완료 -> 정보 세팅 
            if (_nowSelectEquipMat1.aInUid > 0 && _nowSelectEquipMat2.aInUid > 0)
            {
                go_RootSyntInfo.SetActive(false);
                go_RootSyntReady.SetActive(true);
                acSyntInfo_Result.imRedDiaStartBtnBg.gameObject.SetActive(false);
                acSyntInfo_Result.imBlueDiaStartBtnBg.gameObject.SetActive(false);

                float dft_prob = GameDatabase.GetInstance().smithyDB.GetAcceSyntRate(); // 기본 성공률 
                float bns_prob = GameDatabase.GetInstance().tableDB.GetUserInfo().m_ac_synt_rt6_p; // 실패 보너스 성공률
                float probability = dft_prob + bns_prob > 100 ? 100 : dft_prob + bns_prob; // 기본 성공률  + 보너스 성공률 = 총 성공률 
                acSyntInfo_Result.txt_SuccessRate.text = string.Format(str_AcceSynthesisFailPercent, probability, bns_prob).Replace("\\n", "\n");

                var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
                long gold_price = GameDatabase.GetInstance().questDB.GetQuestEquipAcceSyntAdvcGold(_eqDB.eq_rt, _eqDB.eq_id);
                int tbc_dia_price = GameDatabase.GetInstance().questDB.GetQuestEquipAcceSyntAdvcTBCDia(_eqDB.eq_rt, _eqDB.eq_id);
                int tbc_dia = await GameDatabase.GetInstance().tableDB.GetMyTBC();
                acSyntInfo_Result.txRedDiaGoldPrice.text = goods_db.m_gold >= gold_price ? string.Format("{0:#,0}", gold_price) : string.Format("<color=red>{0:#,0}</color>", gold_price); // 강화 가격 
                acSyntInfo_Result.txRedDiaPrice.text = tbc_dia >= tbc_dia_price ? string.Format("{0:#,0}", tbc_dia_price) : string.Format("<color=red>{0:#,0}</color>", tbc_dia_price); // 강화 필요 레드 다이아 

                acSyntInfo_Result.txBlueDiaGoldPrice.text = goods_db.m_gold >= gold_price * 3 ? string.Format("{0:#,0}", gold_price * 3) : string.Format("<color=red>{0:#,0}</color>", gold_price * 3); // 강화 가격 
                acSyntInfo_Result.txBlueDiaPrice.text = tbc_dia + goods_db.m_dia >= tbc_dia_price * 3 ? string.Format("{0:#,0}", tbc_dia_price * 3) : string.Format("<color=red>{0:#,0}</color>", tbc_dia_price * 3); // 강화 필요 블루 다이아 

                acSyntInfo_Result.imRedDiaStartBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(goods_db.m_gold >= gold_price && tbc_dia >= tbc_dia_price);
                acSyntInfo_Result.imRedDiaStartBtnBg.gameObject.SetActive(true);
                acSyntInfo_Result.imBlueDiaStartBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(goods_db.m_gold >= gold_price * 3 && tbc_dia + goods_db.m_dia >= tbc_dia_price * 3);
                acSyntInfo_Result.imBlueDiaStartBtnBg.gameObject.SetActive(true);
            } 
        }
    }

    [SerializeField] private GameDatabase.TableDB.Equipment tempEnrollment_EqData = default;
    /// <summary>
    /// 등록후 해제 팝업 열면서 임시로 데이터를 보관 있음
    /// </summary>
    public void TempEnrollment(GameDatabase.TableDB.Equipment _equip) => tempEnrollment_EqData = _equip;

    /// <summary>
    /// 등록후 해제 팝업에서 등록 버튼을 눌렀을 때 
    /// </summary>
    public async void TempEnrollmentSetData()
    {
        // 착용중인 장비 해제 
        tempEnrollment_EqData.m_state = 0;
        Param update_param_wear = new Param();
        update_param_wear.Add("m_state", tempEnrollment_EqData.m_state);
        Task<bool> bTask = GameDatabase.GetInstance().tableDB.SetUpdateChangeEquipmentData(tempEnrollment_EqData, update_param_wear);
        await bTask;

        if (bTask.Result == true)
        {
            SetDatas(tempEnrollment_EqData);
            GameDatabase.GetInstance().tableDB.SetttingEquipWearingData(tempEnrollment_EqData.eq_ty);
            MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit(false);
            tempEnrollment_EqData = default;
        }
        else
        {

        }
    }

    /// <summary>
    /// 등록 후 해제 팝업에서 취소 눌렀을 때 
    /// </summary>
    public void TempEnrollmentCancel()
    {
        tempEnrollment_EqData = default;
    }

    int fail_r_ac_synt_result_cnt = 0;
    // 영웅 -> 고대로 합성 시작 (TBC) 
    public void Click_StartSyntRedDia() => StartSynt(true);
    // 영웅 -> 고대로 합성 시작 (Blue Dia) 
    public void Click_StartSyntBlueDia() => StartSynt(false);
    // 시작 
    public async void StartSynt(bool isTbc)
    {
        Loading.Full(false);
        var mat1_equip = _nowSelectEquipMat1;
        var mat2_equip = _nowSelectEquipMat2;

        // 장신구 합성은 영웅 등급부터 가능
        if (mat1_equip.eq_rt < 5 || mat2_equip.eq_rt < 5)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("장신구 합성은 {0} 등급부터 가능합니다.", GameDatabase.StringFormat.GetRatingColorText(5)));
            SetDatas(default, true, -1);
            MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
            return;
        }

        long dft_gold_price = GameDatabase.GetInstance().questDB.GetQuestEquipAcceSyntAdvcGold(mat1_equip.eq_rt, mat1_equip.eq_id);
        long gold_price = isTbc ? dft_gold_price : dft_gold_price * 3;
        int dft_dia_price = GameDatabase.GetInstance().questDB.GetQuestEquipAcceSyntAdvcTBCDia(mat1_equip.eq_rt, mat1_equip.eq_id);
        int dia_price = isTbc ? dft_dia_price : dft_dia_price * 3;
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        bool isBlueDiaLack = goods_db.m_dia < dia_price;
        int tbc = isTbc == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
        int blue_dia = goods_db.m_dia;

        bool isOkDia = isTbc == true ? tbc >= dia_price : tbc + blue_dia >= dia_price;
        if (goods_db.m_gold >= gold_price && isOkDia)
        {
            bool isError = false;
            if(isTbc == true) // TBC 
            {
                // tbc 비용 차감 
                Task<string> tbc_ded_result = GameDatabase.GetInstance().tableDB.DeductionTBC(GameDatabase.GetInstance().chartDB.GetDicBalance("equip.smithy.acce.rt5_synt_tbc_key").val_string);
                await tbc_ded_result;
                if (string.Equals(tbc_ded_result.Result, "success") == false)
                {
                    isError = true;
                    PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(tbc_ded_result.Result);
                }
            }
            else
            {
                int dedDia = goods_db.m_dia -= dia_price;
                int dedTbc = dedDia < 0 ? System.Math.Abs(dedDia) : 0;
                Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
                Task tsk2 = GameDatabase.GetInstance().tableDB.DeductionTBC(dedTbc);
                while (tsk1.IsCompleted == false || tsk2.IsCompleted == false)
                    await Task.Delay(100);
            }

            if(!isError)
            {
                acSyntInfo_Result.blockScreen.gameObject.SetActive(true);
                acSyntInfo_Result.cnva_RedDiaStartBtn.alpha = 0.3f;
                acSyntInfo_Result.cnva_BlueDiaStartBtn.alpha = 0.3f;

                var uInfo = GameDatabase.GetInstance().tableDB.GetUserInfo();
                string random_probability = GameDatabase.GetInstance().chartProbabilityDB.GetSelectedProbabilityFileId(string.Format("r_ac_synt_p{0}", uInfo.m_ac_synt_rt6_p < 15 ? (ObscuredInt)15 : uInfo.m_ac_synt_rt6_p > 100 ? (ObscuredInt)100 : uInfo.m_ac_synt_rt6_p));
                BackendReturnObject bro1 = null;
                SendQueue.Enqueue(Backend.Probability.GetProbability, random_probability, callback => { bro1 = callback; });
                while (bro1 == null) { await Task.Delay(100); }

                if (bro1.IsSuccess()) // 합성 결과 
                {
                    JsonData row = bro1.GetReturnValuetoJSON()["element"];
                    bool isSyntSuccess = string.Equals(RowPaser.StrPaser(row, "result"), "success");
                    if (isSyntSuccess == false) // 실패 보상으로 에테르 지급 
                        GameDatabase.GetInstance().tableDB.SetUpdateGoods("ether", GameDatabase.GetInstance().chartDB.GetDicBalance("synt.acce.rt5.fail.reward.ether").val_int, "+");

                    #region ##### 재료 1, 2 소모 #####
                    mat1_equip.m_state = -1;
                    mat2_equip.m_state = -1;
                    GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(mat1_equip, true);
                    GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(mat2_equip, true);
                    GameDatabase.GetInstance().tableDB.SetUpdateGoods("gold", gold_price, "-"); //  골드 차감 

                    bool isSendMat1 = false, isSendMat2 = false;
                    TransactionParam tParam = new TransactionParam();
                    if (string.IsNullOrEmpty(mat1_equip.indate) == false)
                    {
                        Param p1 = new Param();
                        p1.Add("m_state", mat1_equip.m_state);
                        tParam.AddUpdateList(BackendGpgsMng.tableName_Equipment, mat1_equip.indate, new List<BackEnd.WRITE> { new WRITE { Action = TransactionAction.Update, Param = p1 } });
                        isSendMat1 = true;
                    }

                    if (string.IsNullOrEmpty(mat2_equip.indate) == false)
                    {
                        Param p2 = new Param();
                        p2.Add("m_state", mat2_equip.m_state);
                        tParam.AddUpdateList(BackendGpgsMng.tableName_Equipment, mat2_equip.indate, new List<BackEnd.WRITE> { new WRITE { Action = TransactionAction.Update, Param = p2 } });
                        isSendMat2 = true;
                    }

                    if (isSendMat1 || isSendMat2)
                    {
                        bro1 = null;
                        SendQueue.Enqueue(Backend.GameInfo.TransactionWrite, tParam, callback => { bro1 = callback; });
                        while (bro1 == null) { await Task.Delay(100); }
                    }

                    _nowSelectEquipMat1 = default;
                    _nowSelectEquipMat2 = default;
                    #endregion

                    #region ##### 합성 성공 결과 장비 서버 전송 
                    if (isSyntSuccess)
                    {
                        _nowResultEquip = GameDatabase.GetInstance().tableDB.GetNewEquipmentData
                        (
                            RowPaser.IntPaser(row, "pt_ty"),
                            RowPaser.IntPaser(row, "rat"),
                            RowPaser.IntPaser(row, "idx")
                        );

                        long unused_uid = GameDatabase.GetInstance().tableDB.GetUnusedUID();
                        _nowResultEquip.indate = GameDatabase.GetInstance().tableDB.GetUIDSearchToInDate(unused_uid);
                        string send_type = string.IsNullOrEmpty(_nowResultEquip.indate) ? "insert" : "change";

                        Task<bool> tsk = GameDatabase.GetInstance().tableDB.SendDataEquipment(_nowResultEquip, send_type);
                        while (tsk.IsCompleted == false) { await Task.Delay(100); }

                        if (string.Equals(send_type, "change") && unused_uid > 0)
                            GameDatabase.GetInstance().tableDB.SetUnusedInDateToEmpty(unused_uid);

                        uInfo.m_ac_synt_rt6_p = 0;
                        uInfo.m_ac_sync_succ_cnt++;
                        GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr8, 1, true, true); // 업적, nbr8 장신구 합성 성공! 
                        GameDatabase.GetInstance().equipmentEncyclopediaDB.DropAcquisitionAdded(_nowResultEquip.eq_ty, _nowResultEquip.eq_rt, _nowResultEquip.eq_id); // 도감 추가
                        NotificationIcon.GetInstance().CheckNoticeAutoWear(_nowResultEquip, false);
                    }
                    else
                    {
                        uInfo.m_ac_synt_rt6_p += isTbc ? GameDatabase.GetInstance ().chartDB.GetDicBalance("ac.red.dia.rt6.synt.failled.bns").val_int : GameDatabase.GetInstance().chartDB.GetDicBalance("ac.blue.dia.rt6.synt.failled.bns").val_int;
                        uInfo.m_ac_synt_fail_cnt++;
                        _nowResultEquip = new GameDatabase.TableDB.Equipment() { eq_ty = mat1_equip.eq_ty, eq_rt = mat1_equip.eq_rt, eq_id = mat1_equip.eq_id };
                        GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr9, 1, true, true); // 업적, nbr9 장신구 합성 실패 
                    }
                    #endregion

                    Task<bool> t5 = GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(uInfo);
                    while (t5.IsCompleted == false) { await Task.Delay(100); }

                    MainUI.GetInstance().topUI.GetInfoViewTBC();
                    SyntResultUI(isSyntSuccess);
                }
                else
                {
                    await Task.Delay(1000);
                    if (fail_r_ac_synt_result_cnt < 3)
                    {
                        StartSynt(isTbc);
                        fail_r_ac_synt_result_cnt++;
                    }
                    else
                    {
                        SetDatas(default, true, -1);
                        MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
                    }
                }
            }
        }
        else
        {
            if (goods_db.m_gold < gold_price)
            {
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("골드가 부족합니다.\n골드 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
            }
            else if (tbc < dia_price)
            {
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("레드 다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
            }
        }

        Loading.Full(true);
    }
    // 합성 결과 
    async void SyntResultUI(bool isSuccess)
    {
        acSyntInfo_Result.blockScreen.CenterAlphaZero();
        acSyntInfo_Result.ani.Play("AcceSyntStart");
        go_RootSyntReady.SetActive(false);      // 합성 준비 탭 

        while (!acSyntInfo_Result.ani.GetCurrentAnimatorStateInfo(0).IsName("AcceSyntStart"))
            await Task.Delay(100);

        while (acSyntInfo_Result.ani.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            await Task.Delay(100);
            if (acSyntInfo_Result.ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f && !go_RootResult.activeSelf)
            {
                go_RootResult.SetActive(true); // 합성 결과 아이콘(애니) 탭 켬  

                // 미리 성공or실패 UI정보를 세팅 
                if (isSuccess == true)
                    SuccessData();
                else
                    FailData();
            }
        }
        
        go_RootMat.SetActive(false);            // 선택한 합성 준비 재료 아이콘 탭 
        string anName = isSuccess &&_nowResultEquip.eq_rt >= 5 ? string.Format("Result_EquipIconInfoStart_rt{0}", _nowResultEquip.eq_rt) : "Result_EquipIconInfoStart";
        acSyntInfo_Result.an_ResultIcon.Play(anName);

        while (acSyntInfo_Result.an_ResultIcon.GetCurrentAnimatorStateInfo(0).IsName(anName) == false)
            await Task.Delay(100);

        while (acSyntInfo_Result.an_ResultIcon.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f)
            await Task.Delay(100);

        // 결과 창 
        if (isSuccess == true)
        {
            go_RootSyntInfoSuccess.SetActive(true);  // 합성 결과 스탯 탭 
            acSyntInfo_Result.txt_SuccessOrFail.text = "합성 성공!!";
            acSyntInfo_Result.txt_SuccessOrFail.color = Color.white;
        }
        else
        {
            go_RootSyntInfoFail.SetActive(true);  // 합성 실패 
            acSyntInfo_Result.txt_SuccessOrFail.text = "합성 실패...";
            acSyntInfo_Result.txt_SuccessOrFail.color = Color.gray;
        }

        MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
        acSyntInfo_Result.blockScreen.CenterObjectDisable();
        acSyntInfo_Result.blockScreen.OnText(0.5f);
    }
    /// <summary> 성공 결과 </summary>
    private void SuccessData()
    {
        var success_eqdb = _nowResultEquip;
        // ----- 아이콘 정보 -----
        Color corRat = ResourceDatabase.GetInstance().GetItemColor(success_eqdb.eq_rt);
        acSyntInfo_Result.acSyntMatInfo.img_RatingLine.color = corRat;
        acSyntInfo_Result.acSyntMatInfo.img_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(success_eqdb.eq_ty, success_eqdb.eq_rt, success_eqdb.eq_id);
        acSyntInfo_Result.acSyntMatInfo.img_Icon.enabled = true;
        acSyntInfo_Result.img_IconGray.enabled = false;
        acSyntInfo_Result.acSyntMatInfo.img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(success_eqdb.eq_rt);
        acSyntInfo_Result.acSyntMatInfo.txt_Rating.color = corRat;
        acSyntInfo_Result.acSyntMatInfo.txt_Rating.text = success_eqdb.eq_rt > 0 ? LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", success_eqdb.eq_rt)) : "";
        acSyntInfo_Result.acSyntMatInfo.txt_EquipName.text = GameDatabase.StringFormat.GetEquipName(success_eqdb.eq_ty, success_eqdb.eq_rt, success_eqdb.eq_id);
        acSyntInfo_Result.txt_AllCombat.text = string.Format("{0} <size=30>({1:#,0})</size>", nameStrAllCombat, GameDatabase.GetInstance().tableDB.GetEquipCombatPower(success_eqdb, "total"));

        // ----- 매인 스탯 (+강화 스탯 값) -----
        // 매인 스탯 전투력 
        acSyntInfo_Result.txt_MnStatCompat.text = string.Format("{0} <size=24>({1:#,0})</size>", nameStrMnSt, GameDatabase.GetInstance().tableDB.GetEquipCombatPower(success_eqdb, "main"));
        acSyntInfo_Result.txt_MnStatName.text = GameDatabase.StringFormat.GetEquipStatName(success_eqdb.ma_st_id);// 매인 스탯 이름 
        object[] mn_stat_val = GameDatabase.GetInstance().chartDB.GetMainStatValue(success_eqdb);
        if (mn_stat_val[0].GetType() == typeof(float))
            acSyntInfo_Result.txt_MnStatVal.text = string.Format("{0:0.000}", (float)mn_stat_val[0]);
        else if (mn_stat_val[0].GetType() == typeof(long))
            acSyntInfo_Result.txt_MnStatVal.text = string.Format("{0:#,0}", (long)mn_stat_val[0]);


        // ----- 옵션 스탯 -----
        var statOp = success_eqdb.st_op;
        // 옵션 스탯 전투력 
        acSyntInfo_Result.txt_NowOpCompat.text = string.Format("{0} <size=24>(+{1:#,0})</size>", nameStrOp, GameDatabase.GetInstance().tableDB.GetEquipCombatPower(success_eqdb, "op"));
        for (int i = 0; i < 4; i++)
        {
            var sod = i == 0 ? statOp.op1 : i == 1 ? statOp.op2 : i == 2 ? statOp.op3 : i == 3 ? statOp.op4 : new GameDatabase.TableDB.StatOp();
            if (sod.id > 0)
            {
                acSyntInfo_Result.txt_NowOpName[i].text = GameDatabase.StringFormat.GetEquipStatName(sod.id);
                object wer_op_stat_val = (long)GameDatabase.GetInstance().chartDB.GetEquipOptionStatValue(sod.id, sod.rlv, success_eqdb.eq_rt, success_eqdb.eq_id, success_eqdb.m_ehnt_lv, true, success_eqdb.eq_legend, success_eqdb.eq_legend_sop_id, success_eqdb.eq_legend_sop_rlv);
                acSyntInfo_Result.txt_NowOpStat[i].text = string.Format("+{0:#,0}", (long)wer_op_stat_val);
            }
            else
            {
                acSyntInfo_Result.txt_NowOpName[i].text = "-";
                acSyntInfo_Result.txt_NowOpStat[i].text = "-";
            }
        }

        // ----- 장신구 전용 옵션 -----
        float[] wer_acOp_statVal = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionValue(success_eqdb);
        acSyntInfo_Result.txt_NowSopName.text = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionName(success_eqdb.st_sop_ac.id);
        acSyntInfo_Result.txt_NowSopStat.text = string.Format("+{0:0.000}%", wer_acOp_statVal[0]);
    }
    /// <summary> 실패 결과 </summary>
    private void FailData ()
    {
        var fail_eqdb = _nowResultEquip;
        acSyntInfo_Result.acSyntMatInfo.img_RatingLine.color = Color.gray;
        acSyntInfo_Result.img_IconGray.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(fail_eqdb.eq_ty, fail_eqdb.eq_rt, fail_eqdb.eq_id);
        acSyntInfo_Result.img_IconGray.enabled = true;
        acSyntInfo_Result.acSyntMatInfo.img_Icon.enabled = false;
        acSyntInfo_Result.acSyntMatInfo.img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
        acSyntInfo_Result.acSyntMatInfo.txt_Rating.color = ResourceDatabase.GetInstance().GetItemColor(0);
        acSyntInfo_Result.acSyntMatInfo.txt_Rating.text = string.Empty;
        acSyntInfo_Result.acSyntMatInfo.txt_EquipName.text = string.Empty;
    }
    // 왼쪾 재료 해제 
    public void Click_ReleaseMat1()
    {
        if(!isSyntWait && _nowSelectEquipMat1.aInUid > 0)
        {
            foreach (var item in GameObject.FindObjectsOfType<ScrollIndexCallbackSmithy>())
                item.RefreshSelectReleaseAcceSynthesis(_nowSelectEquipMat1);

            SetDatas(default, true, 1);
        }
    }

    // 오른쪽 재료 해제 
    public void Click_ReleaseMat2()
    {
        if (!isSyntWait && _nowSelectEquipMat2.aInUid > 0)
        {
            foreach (var item in GameObject.FindObjectsOfType<ScrollIndexCallbackSmithy>())
                item.RefreshSelectReleaseAcceSynthesis(_nowSelectEquipMat2);

            SetDatas(default, true, 2);
        }
    }

    public void Click_Close ()
    {
        SetDatas(default, true, -1);
        MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
    }
}
