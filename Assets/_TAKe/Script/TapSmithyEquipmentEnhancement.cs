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
using System.Text.RegularExpressions;
using CodeStage.AntiCheat.ObscuredTypes;

/// <summary>
/// 장비 강화 
/// </summary>
public class TapSmithyEquipmentEnhancement : MonoBehaviour
{
    [Header("-강화 정보-")]
    [SerializeField] EnhantInfo enhantInfo; // 강화 정보 
    [Header("-선택 장비 아이콘 정보-")]
    [SerializeField] EquipIcon selectEquip; // 선택 아이콘 정보
    [Header("-선택 강화석 아이콘 정보-")]
    [SerializeField] StonIcon selectSton; // 강화석 아이콘 정보 

    [Header("-결과 아이콘 정보-")]
    [SerializeField] EquipIcon resultEquip; // 결과 아이콘 정보 
    [Header("- 결과 스탯 정보-")]
    [SerializeField] EquipStat resultEquipStat; // 결과 스탯 정보 

    [SerializeField] private GameDatabase.TableDB.Equipment eqDB;

    public long GetSelectEquipUid => eqDB.aInUid;
    public int GetSelectEquipType => eqDB.eq_ty;

    /// <summary>
    /// 강화 정보 
    /// </summary>
    [System.Serializable]
    class EnhantInfo
    {
        public GameObject goRoot_ReadyEquiptSton; // 강화 준비 선택된 장비+강화석 
        public GameObject goRoot_ResultEquip; // 강화 결과 아이콘 
        public GameObject goRoot_UnselectReady; // 장비 선택안됬을 때
        public GameObject goRoot_SelectReady; // 장비 선택됬을때 
        public GameObject goRoot_EnhantSuccess; // 장비 강화 성공
        public GameObject goRoot_EnhantFailure; // 장비 강화 실패

        public GameObject goRoot_EquipIcon; // 장비 아이콘 부모 
        public GameObject goRoot_StonIcon; // 강화석 아이콘 부모 

        public CanvasGroup canSelectReady;
        public Animator aniResultIcon;
        public Text txTitle; // 강화 준비 or 결과 
        public ToggleGroup toGroup;
        public Text[] txBlessCount; // 강화 주문서 수량 

        public Text txRate; // 성공률 
        public Text txBefPreviewCombat; // 강화 전 전투력 
        public Text txAftPreviewCombat; // 강화 후 전투력
        public Text txGoldPrice; // 강화 골드 가격 
        public Text txRubyEtherPrice; // 강화 루비(에테르) 필요 수량 
        public Image imRubyEther; // 강화 루비(에테르) 아이콘 
        public Image txStartBtnBg;
        public BlockScreen blockScreen;

        public GameObject[] goCloseBtns;
    }

    /// <summary>
    /// 강화 결과 장비 스탯 정보 
    /// </summary>
    [System.Serializable]
    class EquipStat
    {
        public Text txAllCombat; // 전투력 총합
        public Text txMainStatCombat; // 매인 전투력 
        public Text txMainStatName;
        public Text txMainStatValue;

        public Text txOpStatCombat; // 옵션 전투력 
        public Text[] txOpStatName; // 옵션 스탯 이름
        public Text[] txOpStatValues; // 옵션 스탯 값 

        public GameObject goAcSOpStrikethrough; // 장신ㄱ ㅜ전용옵션 취소선
        public Text txAcSOpStatName; // 장신구 전용 옵션 이름
        public Text txAcSOpStatValue;// 장신구 전용 옵션 값 
    }

    /// <summary>
    /// 장비 아이콘 정보 
    /// </summary>
    [System.Serializable]
    class EquipIcon
    {
        public Image imIcon;
        public Image imIconFailure;
        public Image imRatingBg;
        public Image imRatingOutline;
        public Text txRating;
        public Text txEnhantLevel;
        public Text txName;

        public GameObject goNowWear;
        public GameObject goLock;
    }

    /// <summary>
    /// 선택된 강화석 아이콘 정보 
    /// </summary>
    [System.Serializable]
    class StonIcon
    {
        public Image imIcon;
        public Image imRatingBg;
        public Text txRating;
        public Text txName;
        public Text txCount;
    }

    string nameStrAllCombat, nameStrMnSt, nameStrOp, nameStrSpOp, str_AcceSynthesisFailPercent;
    string str_enhancement_percent = "";
    void Awake()
    {
        nameStrAllCombat = LanguageGameData.GetInstance().GetString("ui.name.combat_power"); // 전투력 
        nameStrMnSt = LanguageGameData.GetInstance().GetString("ui.name.stat"); // str 스탯 
        nameStrOp = LanguageGameData.GetInstance().GetString("ui.name.option"); // str 옵션  
        nameStrSpOp = LanguageGameData.GetInstance().GetString("text.ornamentOption"); // str 장신구 전용 옵션 
        str_enhancement_percent = LanguageGameData.GetInstance().GetString("ui.name.enhancement_percent");
        ObjectTapInit();
    }

    void Start()
    {
        foreach (Toggle toggle in enhantInfo.toGroup.ActiveToggles()) // 주문서 토글 리셋 
        {
            toggle.isOn = false;
        }
    }

    string anName = "";

    public void SetData(GameDatabase.TableDB.Equipment _eqDB)
    {
        anName = "";
        _eqDB.m_norm_lv = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(_eqDB.eq_ty).m_norm_lv;
        eqDB = _eqDB;

        enhantInfo.txStartBtnBg.gameObject.SetActive(false);
        if (continuityEnhant.isTrue == false && continuityEnhant.isPause == false)
        {
            enhantInfo.blockScreen.gameObject.SetActive(false);
        }
            
        enhantInfo.aniResultIcon.speed = 1;

        // 장비 정보 정상 
        if (_eqDB.aInUid > 0)
        {
            enhantInfo.goRoot_UnselectReady.SetActive(false);
            enhantInfo.goRoot_SelectReady.SetActive(true);
            enhantInfo.canSelectReady.alpha = 1f;
            enhantInfo.goRoot_EquipIcon.SetActive(true);
            enhantInfo.goRoot_StonIcon.SetActive(true);

            // 장비 
            selectEquip.imIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(_eqDB.eq_ty, _eqDB.eq_rt, _eqDB.eq_id);
            selectEquip.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(_eqDB.eq_rt, _eqDB.eq_legend);
            selectEquip.imRatingOutline.color = ResourceDatabase.GetInstance().GetItemColor(_eqDB.eq_rt);
            selectEquip.goNowWear.SetActive(_eqDB.m_state > 0);
            selectEquip.txRating.text = GameDatabase.StringFormat.GetRatingColorText(_eqDB.eq_rt, false);
            selectEquip.txEnhantLevel.text = string.Format("Lv.{0}", _eqDB.m_ehnt_lv);
            selectEquip.txName.text = GameDatabase.StringFormat.GetEquipName(_eqDB.eq_ty, _eqDB.eq_rt, _eqDB.eq_id);
            selectEquip.goLock.SetActive(_eqDB.m_lck == 1);

            // 강화석 
            bool isEqAc = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(_eqDB.eq_ty); // 장비 or 장신구 
            int stn_rt = GameDatabase.GetInstance().smithyDB.GetEnhantStonTypeRating(_eqDB.m_ehnt_lv); // 적용될 강화석 등급 
            int stn_cnt = GameDatabase.GetInstance().smithyDB.GetEnhantStonTypeRatingCount(_eqDB.eq_ty, stn_rt); // 내 소지 수량 
            int stn_use_cnt = GameDatabase.GetInstance().smithyDB.GetEnhantStonCount(_eqDB.m_ehnt_lv); // 필요 수량 
            string str_ston_name = isEqAc == true ?
                LanguageGameData.GetInstance().GetString(string.Format("item.name.item_27_{0}", stn_rt)) :
                LanguageGameData.GetInstance().GetString(string.Format("item.name.item_21_{0}", stn_rt));

            selectSton.imIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteItemStonType(_eqDB.eq_ty, stn_rt);
            selectSton.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(stn_rt);
            selectSton.txRating.text = GameDatabase.StringFormat.GetRatingColorText(stn_rt, false);
            selectSton.txCount.text = string.Format("{0}/{1}", stn_use_cnt, stn_cnt);
            selectSton.txCount.color = stn_use_cnt <= stn_cnt ? Color.white : Color.red;
            selectSton.txName.text = str_ston_name;

            // 강화 정보 
            // 축복 주문서 수량Re
            enhantInfo.txBlessCount[0].text = string.Format("1/{0}", GameDatabase.GetInstance().tableDB.GetItem(GameDatabase.TableDB.item_type_beless, 1).count);
            enhantInfo.txBlessCount[1].text = string.Format("1/{0}", GameDatabase.GetInstance().tableDB.GetItem(GameDatabase.TableDB.item_type_beless, 2).count);
            enhantInfo.txBlessCount[2].text = string.Format("1/{0}", GameDatabase.GetInstance().tableDB.GetItem(GameDatabase.TableDB.item_type_beless, 3).count);
            Percent(false); // 강화 성공률 

            enhantInfo.txBefPreviewCombat.text = string.Format("(Lv.{0}) {1:#,0}", _eqDB.m_ehnt_lv, GameDatabase.GetInstance().tableDB.GetEquipCombatPower(_eqDB, "total"));
            var temp2_eqDb = _eqDB;
            temp2_eqDb.m_ehnt_lv++;
            enhantInfo.txAftPreviewCombat.text = string.Format("(Lv.{0}) {1:#,0}", _eqDB.m_ehnt_lv + 1, GameDatabase.GetInstance().tableDB.GetEquipCombatPower(temp2_eqDb, "total"));

            bool isEqAcc = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(_eqDB.eq_ty);
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            long ruby_ether = isEqAcc == true ? goods_db.m_ether : goods_db.m_ruby;
            long gold_price = GameDatabase.GetInstance().questDB.GetQuestEquipEnhantGold(_eqDB.eq_rt, _eqDB.eq_id, _eqDB.m_ehnt_lv);
            int ruby_ether_price = GameDatabase.GetInstance().questDB.GetQuestEquipEnhantRubyOrEther(isEqAcc, _eqDB.eq_rt, _eqDB.eq_id);

            enhantInfo.txGoldPrice.text = goods_db.m_gold >= gold_price ? string.Format("{0:#,0}", gold_price) : string.Format("<color=red>{0:#,0}</color>", gold_price); // 강화 가격 
            enhantInfo.txRubyEtherPrice.text = ruby_ether >= ruby_ether_price ? string.Format("{0:#,0}", ruby_ether_price) : string.Format("<color=red>{0:#,0}</color>", ruby_ether_price); // 강화 필요 루비(에테르) 
            enhantInfo.txStartBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(goods_db.m_gold >= gold_price && ruby_ether >= ruby_ether_price);
            enhantInfo.txStartBtnBg.gameObject.SetActive(true);
            enhantInfo.imRubyEther.sprite = SpriteAtlasMng.GetInstance().GetSpriteGoods(isEqAcc == true ? 12 : 13);
        }
        else  // 장비 정보 없음 
        {
            ObjectTapInit();
        }
    }

    public void ObjectTapInit()
    {
        enhantInfo.goRoot_ReadyEquiptSton.SetActive(true);
        enhantInfo.goRoot_ResultEquip.SetActive(false);
        enhantInfo.goRoot_UnselectReady.SetActive(true);
        enhantInfo.goRoot_SelectReady.SetActive(false);
        enhantInfo.goRoot_EnhantSuccess.SetActive(false);
        enhantInfo.goRoot_EnhantFailure.SetActive(false);
        enhantInfo.goRoot_EquipIcon.SetActive(false);
        enhantInfo.goRoot_StonIcon.SetActive(false);
    }

    /// <summary>
    /// 강화 확률 
    /// </summary>
    public void Percent(bool isClick)
    {
        var eqDb = eqDB;
        int useRating = 0;
        foreach (Toggle toggle in enhantInfo.toGroup.ActiveToggles())
        {
            if (useRating == 0)
            {
                string strTmp = Regex.Replace(toggle.name, @"\D", "");
                useRating = int.Parse(strTmp);
                break;
            }
        }

        float probability = GameDatabase.GetInstance().smithyDB.GetEnhantRate(eqDb.m_ehnt_lv);
        int blessingCnt = GameDatabase.GetInstance().tableDB.GetItem(GameDatabase.TableDB.item_type_beless, useRating).count;
        if (blessingCnt > 0 && useRating > 0)
        {
            probability = GameDatabase.GetInstance().smithyDB.GetEnhantBlessingRate(useRating, probability);
        }
        else
        {
            foreach (Toggle toggle in enhantInfo.toGroup.ActiveToggles()) // 주문서 토글 리셋 
                toggle.isOn = false;

            if (useRating > 0 && blessingCnt <= 0)
            {
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("강화 축복 주문서의 수량이 부족합니다.\n골드 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
            }
        }

        string ratingColor = blessingCnt > 0 && useRating > 0f ? "#FFC400" : "#FFFFFF";
        float applyProbability = probability > 100.0f ? 100f : probability;
        enhantInfo.txRate.text = string.Format("{0}  <color={1}>{2:0.00}%</color>", str_enhancement_percent, ratingColor, applyProbability);
    }

    /// <summary>
    /// 선택한 장비 해제 
    /// </summary>
    public void Click_Release()
    {
        SetData(default);
        foreach (var item in GameObject.FindObjectsOfType<ScrollIndexCallbackSmithy>())
        {
            item.SelectRelease();
        }
    }

    /// <summary>
    /// 강화 시작 
    /// </summary>
    public void Click_StartEnhant()
    {
        enhantInfo.blockScreen.gameObject.SetActive(false);
        TaskEnhantStart();
        ContinuityEnhantInfo(true);
    }

    async void TaskEnhantStart()
    {
        var eqDb = eqDB;
        bool isEqAcc = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(eqDb.eq_ty);
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        long gold_price = GameDatabase.GetInstance().questDB.GetQuestEquipEnhantGold(eqDb.eq_rt, eqDb.eq_id, eqDb.m_ehnt_lv);
        int ruby_ether_price = GameDatabase.GetInstance().questDB.GetQuestEquipEnhantRubyOrEther(isEqAcc, eqDb.eq_rt, eqDb.eq_id);
        int stn_rt = GameDatabase.GetInstance().smithyDB.GetEnhantStonTypeRating(eqDb.m_ehnt_lv); // 적용될 강화석 등급 
        int stn_cnt = GameDatabase.GetInstance().smithyDB.GetEnhantStonTypeRatingCount(eqDb.eq_ty, stn_rt); // 내 소지 수량 
        int stn_use_cnt = GameDatabase.GetInstance().smithyDB.GetEnhantStonCount(eqDb.m_ehnt_lv); // 필요 수량 
        long goods_rb_et = isEqAcc == true ? goods_db.m_ether : goods_db.m_ruby; // 루비(에테르) 

        if (goods_db.m_gold < gold_price && !continuityEnhant.isTrue)
        {
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("골드가 부족합니다.\n골드 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
            return;
        }
        else if (goods_rb_et < ruby_ether_price && !continuityEnhant.isTrue)
        {
            if (isEqAcc)
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("에테르가 족합니다.\n\n에테르는 장신구(목걸이/귀걸이/반지)를 분해하여 획득할 수 있습니다");
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("루비가 부족합니다.\n\n루비는 장비(무기/방패/방어구)를 분해하여 획득할 수 있습니다");
            }
            return;
        }
        else if (stn_cnt < stn_use_cnt && !continuityEnhant.isTrue)
        {
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("강화석이 부족합니다.\n강화석 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
            return;
        }

        if (continuityEnhant.isTrue == false && continuityEnhant.isPause == false)
        {
            enhantInfo.blockScreen.gameObject.SetActive(true);
            enhantInfo.blockScreen.CenterAlphaZero();
        } 
        else
        {
            enhantInfo.blockScreen.CenterObjectEnable();
        }

        enhantInfo.goRoot_ReadyEquiptSton.SetActive(false);
        enhantInfo.goRoot_ResultEquip.SetActive(true);
        enhantInfo.canSelectReady.alpha = 0.3f;

        // #강화 축복 사용했다면 
        int blessingRating = 0;
        foreach (Toggle toggle in enhantInfo.toGroup.ActiveToggles())
        {
            if (blessingRating == 0)
            {
                try
                {
                    blessingRating = int.Parse(Regex.Replace(toggle.name, @"\D", ""));
                }
                catch (Exception e)
                { LogPrint.PrintError(e); }
                break;
            }
        }

        if (blessingRating > 0)
        {
            var item_bless = GameDatabase.GetInstance().tableDB.GetItem(GameDatabase.TableDB.item_type_beless, blessingRating);
            item_bless.count--;
            GameDatabase.GetInstance().tableDB.UpdateClientDB_Item(item_bless);
        }

        // #강화 비용 값 전송 
        //var goods = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        //goods.m_gold -= gold_price;
        //if (isEqAcc == true)
        //    goods.m_ether -= ruby_ether_price;
        //else goods.m_ruby -= ruby_ether_price;

        GameDatabase.GetInstance().tableDB.SetUpdateGoods("gold", gold_price, "-");
        if (isEqAcc)
            GameDatabase.GetInstance().tableDB.SetUpdateGoods("ether", ruby_ether_price, "-");
        else GameDatabase.GetInstance().tableDB.SetUpdateGoods("ruby", ruby_ether_price, "-");

        //Task taskA = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods);

        // #강화에 사용된 강화석 차감 값 전송 
        var item_db = GameDatabase.GetInstance().tableDB.GetItem(isEqAcc ? GameDatabase.TableDB.item_type_ac_ston : GameDatabase.TableDB.item_type_eq_ston, stn_rt);
        item_db.count -= stn_use_cnt;
        //Task taskB = GameDatabase.GetInstance().tableDB.SendDataItem(item_db);

        GameDatabase.GetInstance().tableDB.UpdateClientDB_Item(item_db);

        float rate = GameDatabase.GetInstance().GetRandomPercent();
        float probability = GameDatabase.GetInstance().smithyDB.GetEnhantRate(eqDb.m_ehnt_lv);
        if (blessingRating > 0)
        {
            int blsCnt = GameDatabase.GetInstance().tableDB.GetItem(GameDatabase.TableDB.item_type_beless, blessingRating).count;
            if (blsCnt > 0)
                probability = GameDatabase.GetInstance().smithyDB.GetEnhantBlessingRate(blessingRating, probability);
        }

        bool isSuccess = rate <= probability; // 성공 or 실패 
        if (isSuccess)
        {
            eqDb.m_ehnt_lv++;
            eqDB = eqDb;
            Task taskD = GameDatabase.GetInstance().tableDB.SetUpdateChangeEquipmentData(eqDb, ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "m_ehnt_lv", v = eqDb.m_ehnt_lv } }));
            while (taskD.IsCompleted == false) await Task.Delay(100);

            GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr2, 1); // 일일미션, nbr2 장비 강화 성공 
            if (eqDb.eq_ty == 0) // 업적, nbr4 무기 강화 레벨 10 ~ 35 달성!
                GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr4, eqDb.m_ehnt_lv, false); // 업적, nbr4 무기 강화 레벨 10 ~ 35 달성!
            else
            if (eqDb.eq_ty == 1) // 업적, nbr5 방패 강화 레벨 10 ~ 35 달성!
                GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr5, eqDb.m_ehnt_lv, false); // 업적, nbr5 방패 강화 레벨 10 ~ 35 달성!
            else
            if (eqDb.eq_ty >= 2 && eqDb.eq_ty <= 7) // 업적, nbr6 방어구 강화 레벨 10 ~ 35 달성!
                GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr6, eqDb.m_ehnt_lv, false); // 업적, nbr6 방어구 강화 레벨 10 ~ 35 달성!
            else
            if (eqDb.eq_ty >= 8 && eqDb.eq_ty <= 10) // 업적, nbr7 장신구 강화 레벨 10 ~ 35 달성!
                GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr7, eqDb.m_ehnt_lv, false); // 업적, nbr7 장신구 강화 레벨 10 ~ 35 달성!
        }

        AlreadyResultUI(isSuccess);
        foreach (var item in enhantInfo.goCloseBtns)
            item.SetActive(false);

        // 강화 애니 실행 
        if(continuityEnhant.isTrue)
            enhantInfo.aniResultIcon.speed = 5;
        else enhantInfo.aniResultIcon.speed = 1;

        anName = isSuccess == true ? string.Format("Result_EquipIconInfoStart_rt{0}", eqDb.eq_rt) : "Result_EquipIconInfoStart";
        enhantInfo.aniResultIcon.Play(anName);
        while (enhantInfo.aniResultIcon.GetCurrentAnimatorStateInfo(0).IsName(anName) == false)
            await Task.Delay(100);

        while (enhantInfo.aniResultIcon.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f)
            await Task.Delay(100);

        enhantInfo.aniResultIcon.speed = 1;
        enhantInfo.goRoot_SelectReady.SetActive(false);
        if (isSuccess)
        {
            enhantInfo.goRoot_EnhantSuccess.SetActive(true);
            GameDatabase.GetInstance().characterDB.SetPlayerStatValue();
            MainUI.GetInstance().tapCharacter.wearingEquipmentInfo.SetWearEquipView(eqDB.eq_ty);
            MainUI.GetInstance().tapCharacter.SetStatView(GameDatabase.GetInstance().characterDB.GetStat());
            GameMng.GetInstance().myPZ.igp.statValue = GameDatabase.GetInstance().characterDB.GetStat(); // 장비 관련, 스탯 
        }
        else
        {
            enhantInfo.goRoot_EnhantFailure.SetActive(true);
        }

        MainUI.GetInstance().tapSmithy.smithyScrollRefreshCellEquip?.Invoke(); // 선택 셀 새로 고침 

        // 연속강화 상태 체크 후 
        if(continuityEnhant.isTrue == false && continuityEnhant.isPause == false)
        {
            enhantInfo.blockScreen.CenterObjectDisable();
            enhantInfo.blockScreen.OnText(0.5f); // 연속 강화하기 창을 띄운다. 
        }

            await Task.Delay(250);

        if (continuityEnhant.isTrue == false && continuityEnhant.isPause == false)
        {
            foreach (var item in enhantInfo.goCloseBtns)
                item.SetActive(true);
        }

        anName = "";
    }

    public void Click_SkipResultAni()
    {
        if (continuityEnhant.isTrue)
            return;
        
        if (enhantInfo.aniResultIcon.speed < 10 && enhantInfo.aniResultIcon.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f)
            enhantInfo.aniResultIcon.speed = 10;
    }

    /// <summary>
    /// 애니 실행동안 미리 성공 결과 정보 세팅 
    /// </summary>
    void AlreadyResultUI(bool isSuccess)
    {
        var eqDb = eqDB;
        // ----- 아이콘 정보 -----
        resultEquip.imIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(eqDb.eq_ty, eqDb.eq_rt, eqDb.eq_id);
        resultEquip.imIcon.enabled = isSuccess;
        resultEquip.imIconFailure.sprite = resultEquip.imIcon.sprite;
        resultEquip.imIconFailure.enabled = !isSuccess;

        resultEquip.imRatingOutline.enabled = isSuccess;
        if (isSuccess) resultEquip.imRatingOutline.color = ResourceDatabase.GetInstance().GetItemColor(eqDb.eq_rt);

        resultEquip.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(isSuccess == true ? eqDb.eq_rt : 0, eqDb.eq_legend);
        resultEquip.txRating.text = isSuccess == true ? GameDatabase.StringFormat.GetRatingColorText(eqDb.eq_rt, false) : GameDatabase.StringFormat.GetStringRating(eqDb.eq_rt);
        if (isSuccess) resultEquip.txRating.color = Color.white;
        else resultEquip.txRating.color = Color.gray;

        resultEquip.txName.text = GameDatabase.StringFormat.GetEquipName(eqDb.eq_ty, eqDb.eq_rt, eqDb.eq_id);
        if (isSuccess) resultEquip.txName.color = Color.white;
        else resultEquip.txName.color = Color.gray;

        resultEquip.txEnhantLevel.text = string.Format("Lv.{0}", eqDb.m_ehnt_lv);
        if (isSuccess) resultEquip.txEnhantLevel.color = Color.white;
        else resultEquip.txEnhantLevel.color = Color.gray;

        if (isSuccess) // 스탯은 성공시에만 보여줌 
        {
            resultEquipStat.txAllCombat.text = string.Format("{0} <size=30>({1:#,0})</size>", nameStrAllCombat, GameDatabase.GetInstance().tableDB.GetEquipCombatPower(eqDb, "total"));

            // ----- 매인 스탯 (+강화 스탯 값) -----
            resultEquipStat.txMainStatCombat.text = string.Format("{0} <size=24>({1:#,0})</size>", nameStrMnSt, GameDatabase.GetInstance().tableDB.GetEquipCombatPower(eqDb, "main"));
            resultEquipStat.txMainStatName.text = GameDatabase.StringFormat.GetEquipStatName(eqDb.ma_st_id);// 매인 스탯 이름 
            object[] mn_stat_val = GameDatabase.GetInstance().chartDB.GetMainStatValue(eqDb);
            if (mn_stat_val[0].GetType() == typeof(float))
                resultEquipStat.txMainStatValue.text = string.Format("{0:0.000}", (float)mn_stat_val[0]);
            else if (mn_stat_val[0].GetType() == typeof(long))
                resultEquipStat.txMainStatValue.text = string.Format("{0:#,0}", (long)mn_stat_val[0]);

            // ----- 옵션 스탯 -----
            var statOp = eqDb.st_op;
            resultEquipStat.txOpStatCombat.text = string.Format("{0} <size=24>(+{1:#,0})</size>", nameStrOp, GameDatabase.GetInstance().tableDB.GetEquipCombatPower(eqDb, "op"));
            for (int i = 0; i < 4; i++)
            {
                var sod = i == 0 ? statOp.op1 : i == 1 ? statOp.op2 : i == 2 ? statOp.op3 : i == 3 ? statOp.op4 : new GameDatabase.TableDB.StatOp();
                if (sod.id > 0)
                {
                    resultEquipStat.txOpStatName[i].text = GameDatabase.StringFormat.GetEquipStatName(sod.id);
                    object wer_op_stat_val = (long)GameDatabase.GetInstance().chartDB.GetEquipOptionStatValue(sod.id, sod.rlv, eqDb.eq_rt, eqDb.eq_id, eqDb.m_ehnt_lv, true, eqDb.eq_legend, eqDb.eq_legend_sop_id, eqDb.eq_legend_sop_rlv);
                    resultEquipStat.txOpStatValues[i].text = string.Format("+{0:#,0}", (long)wer_op_stat_val);
                }
                else
                {
                    resultEquipStat.txOpStatName[i].text = "-";
                    resultEquipStat.txOpStatValues[i].text = "-";
                }
            }

            // ----- 장신구 전용 옵션 -----
            if (GameDatabase.GetInstance().chartDB.GetIsPartsTypeAcce(eqDb.eq_ty))
            {
                float[] wer_acOp_statVal = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionValue(eqDb);
                resultEquipStat.txAcSOpStatName.text = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionName(eqDb.st_sop_ac.id);
                resultEquipStat.txAcSOpStatValue.text = string.Format("+{0:0.000}%", wer_acOp_statVal[0]);
                resultEquipStat.goAcSOpStrikethrough.SetActive(false);
            }
            else
            {
                resultEquipStat.txAcSOpStatName.text = "-";
                resultEquipStat.txAcSOpStatValue.text = "-";
                resultEquipStat.goAcSOpStrikethrough.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 결과창 닫기 
    /// </summary>
    public void Click_ResultClose()
    {
        if (continuityEnhant.isTrue)
            return;

        ObjectTapInit();
        SetData(eqDB);
        MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
    }

    #region ---------- 연속 강화하기 ----------
    [Header("- 연속 강화 정보-")]
    [SerializeField] ContinuityEnhant continuityEnhant;
    [System.Serializable]
    struct ContinuityEnhant
    {
        public bool isTrue, isPause;
        public int count, min_count, max_count;
        public Text txContinuity, txContinuityMax;
        public GameObject goBtnStart, goBtnStop, goBtnMinus, goBtnPlus, goClose;
    }

    void ContinuityEnhantInfo(bool init)
    {
        ContinuityCount(init);
        ContinuityEnhantState();

        continuityEnhant.goBtnMinus.SetActive(true);
        continuityEnhant.goBtnPlus.SetActive(true);
        continuityEnhant.goBtnStart.SetActive(true);
        continuityEnhant.goBtnStop.SetActive(false);
    }
    
    void ContinuityCount(bool init)
    {
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        bool isEqAcc = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(eqDB.eq_ty);
        long ruby_ether = isEqAcc == true ? goods_db.m_ether : goods_db.m_ruby;
        long gold_price = GameDatabase.GetInstance().questDB.GetQuestEquipEnhantGold(eqDB.eq_rt, eqDB.eq_id, eqDB.m_ehnt_lv);
        int ruby_ether_price = GameDatabase.GetInstance().questDB.GetQuestEquipEnhantRubyOrEther(isEqAcc, eqDB.eq_rt, eqDB.eq_id);
        int stn_rt = GameDatabase.GetInstance().smithyDB.GetEnhantStonTypeRating(eqDB.m_ehnt_lv); // 적용될 강화석 등급 
        int stn_cnt = GameDatabase.GetInstance().smithyDB.GetEnhantStonTypeRatingCount(eqDB.eq_ty, stn_rt); // 내 소지 수량 
        int stn_use_cnt = GameDatabase.GetInstance().smithyDB.GetEnhantStonCount(eqDB.m_ehnt_lv); // 필요 수량 

        int gold_lopCnt = (int)(goods_db.m_gold / gold_price);
        int rbet_lopCnt = (int)(ruby_ether / ruby_ether_price);
        int stn_lopCnt = (int)(stn_cnt / stn_use_cnt);
        bool lack = goods_db.m_gold >= gold_price && ruby_ether >= ruby_ether_price && stn_cnt >= stn_use_cnt;

        if(gold_lopCnt >= rbet_lopCnt && stn_lopCnt >= rbet_lopCnt)
            continuityEnhant.max_count = rbet_lopCnt;
        else if (rbet_lopCnt >= stn_lopCnt && gold_lopCnt >= stn_lopCnt)
            continuityEnhant.max_count = stn_lopCnt;
        else if (stn_lopCnt >= gold_lopCnt && rbet_lopCnt >= gold_lopCnt)
            continuityEnhant.max_count = gold_lopCnt;

        if (init)
        {
            continuityEnhant.isPause = false;
            continuityEnhant.isTrue = false;
            if(continuityEnhant.max_count >= 1 && lack)
            {
                continuityEnhant.count = 1;
                continuityEnhant.min_count = 1;
            }
            else
            {
                continuityEnhant.count = 0;
                continuityEnhant.min_count = 0;
            }
        }
        else
        {
            if (continuityEnhant.isTrue && continuityEnhant.count > continuityEnhant.max_count)
            {
                continuityEnhant.count = continuityEnhant.max_count;
                continuityEnhant.min_count = continuityEnhant.max_count;
            }
        }
        
        continuityEnhant.txContinuity.text = string.Format("{0}회 연속 강화", continuityEnhant.count);
        continuityEnhant.txContinuityMax.text = string.Format("* 현재 레벨 기준 최대 {0}회 시도 가능 *", continuityEnhant.max_count);
        continuityEnhant.goClose.SetActive(false);
    }

    void ContinuityEnhantState()
    {
        continuityEnhant.txContinuity.text = string.Format("{0}회 연속 강화", continuityEnhant.count);
        continuityEnhant.txContinuityMax.text = string.Format("* 현재 레벨 기준 최대 {0}회 시도 가능 *", continuityEnhant.max_count);
    }

    public  void Click_ContinuityEnhantStart()
    {
        if (!continuityEnhant.isTrue)
        {
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            bool isEqAcc = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(eqDB.eq_ty);
            long ruby_ether = isEqAcc == true ? goods_db.m_ether : goods_db.m_ruby;
            long gold_price = GameDatabase.GetInstance().questDB.GetQuestEquipEnhantGold(eqDB.eq_rt, eqDB.eq_id, eqDB.m_ehnt_lv);
            int ruby_ether_price = GameDatabase.GetInstance().questDB.GetQuestEquipEnhantRubyOrEther(isEqAcc, eqDB.eq_rt, eqDB.eq_id);
            int stn_rt = GameDatabase.GetInstance().smithyDB.GetEnhantStonTypeRating(eqDB.m_ehnt_lv); // 적용될 강화석 등급 
            int stn_cnt = GameDatabase.GetInstance().smithyDB.GetEnhantStonTypeRatingCount(eqDB.eq_ty, stn_rt); // 내 소지 수량 
            int stn_use_cnt = GameDatabase.GetInstance().smithyDB.GetEnhantStonCount(eqDB.m_ehnt_lv); // 필요 수량 

            if (goods_db.m_gold >= gold_price && ruby_ether >= ruby_ether_price && stn_cnt >= stn_use_cnt)
            {
                if (continuityEnhant.count <= 0)
                    continuityEnhant.count = continuityEnhant.min_count;
                else if (continuityEnhant.count < continuityEnhant.min_count)
                    continuityEnhant.count = continuityEnhant.min_count;
                else if (continuityEnhant.count > continuityEnhant.min_count)
                    continuityEnhant.count = continuityEnhant.min_count;

                if (continuityEnhant.count >= 1)
                {
                    continuityEnhant.isPause = false;
                    continuityEnhant.isTrue = true;
                    foreach (var item in enhantInfo.goCloseBtns)
                        item.SetActive(false);

                    enhantInfo.goRoot_ResultEquip.SetActive(false);
                    enhantInfo.goRoot_EnhantFailure.SetActive(false);
                    enhantInfo.goRoot_ReadyEquiptSton.SetActive(true);

                    continuityEnhant.goBtnMinus.SetActive(false);
                    continuityEnhant.goBtnPlus.SetActive(false);
                    continuityEnhant.goBtnStart.SetActive(false);
                    continuityEnhant.goBtnStop.SetActive(true);
                    continuityEnhant.goClose.SetActive(false);

                    StopCoroutine("ContinuityEnhantStart");
                    StartCoroutine("ContinuityEnhantStart");
                }
            }
            else
            {
                if (goods_db.m_gold < gold_price)
                {
                    PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("골드가 부족합니다.");
                }
                else if(ruby_ether < ruby_ether_price)
                {
                    if (isEqAcc)
                        PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("에테르가 족합니다.\n에테르는 장신구(목걸이/귀걸이/반지)를 분해하여 획득할 수 있습니다");
                    else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("루비가 족합니다.\n루비는 장비(무기/방패/방어구)를 분해하여 획득할 수 있습니다");
                }
                else if(stn_cnt < stn_use_cnt)
                {
                    PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("강화석이 부족합니다.");
                }
            }
        }
    }

    public void Click_ContinuityEnhantStop()
    {
        if (continuityEnhant.isTrue && !continuityEnhant.isPause)
        {
            continuityEnhant.isPause = true;
            continuityEnhant.goBtnStop.SetActive(false);
        }
    }

    IEnumerator ContinuityEnhantStart()
    {
        WaitForSeconds sec1 = new WaitForSeconds(0.25f);
        WaitForSeconds sec2 = new WaitForSeconds(1.0f);
        yield return null;
        bool lack = true;
        while (lack)
        {
            if (continuityEnhant.count > 0 && !continuityEnhant.isPause)
            {
                var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
                bool isEqAcc = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(eqDB.eq_ty);
                long ruby_ether = isEqAcc == true ? goods_db.m_ether : goods_db.m_ruby;
                long gold_price = GameDatabase.GetInstance().questDB.GetQuestEquipEnhantGold(eqDB.eq_rt, eqDB.eq_id, eqDB.m_ehnt_lv);
                int ruby_ether_price = GameDatabase.GetInstance().questDB.GetQuestEquipEnhantRubyOrEther(isEqAcc, eqDB.eq_rt, eqDB.eq_id);
                int stn_rt = GameDatabase.GetInstance().smithyDB.GetEnhantStonTypeRating(eqDB.m_ehnt_lv); // 적용될 강화석 등급 
                int stn_cnt = GameDatabase.GetInstance().smithyDB.GetEnhantStonTypeRatingCount(eqDB.eq_ty, stn_rt); // 내 소지 수량 
                int stn_use_cnt = GameDatabase.GetInstance().smithyDB.GetEnhantStonCount(eqDB.m_ehnt_lv); // 필요 수량 

                lack = goods_db.m_gold >= gold_price && ruby_ether >= ruby_ether_price && stn_cnt >= stn_use_cnt;
                if (lack)
                {
                    int eht_lv = eqDB.m_ehnt_lv;
                    enhantInfo.aniResultIcon.speed = 5;

                    TaskEnhantStart();
                    yield return sec1;
                    while (!string.IsNullOrEmpty(anName))
                        yield return null;

                    continuityEnhant.count--;
                    continuityEnhant.min_count--;
                    continuityEnhant.max_count--;
                    ContinuityEnhantState();

                    if (eht_lv != eqDB.m_ehnt_lv) // 성공 
                    {
                        ContinuityCount(false);
                        yield return sec2;
                    }

                    ObjectTapInit();
                    SetData(eqDB);
                    MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
                    yield return sec1;
                }
                else
                {
                    continuityEnhant.isPause = true;
                    ObjectTapInit();
                    SetData(eqDB);
                    MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
                    break;
                }
            }
            else break;
        }

        if (!continuityEnhant.isPause)
        {
            continuityEnhant.count = 0;
            continuityEnhant.min_count = 0;
        }

        continuityEnhant.isTrue = false;
        continuityEnhant.isPause = false;
        
        continuityEnhant.goBtnMinus.SetActive(true);
        continuityEnhant.goBtnPlus.SetActive(true);
        continuityEnhant.goBtnStart.SetActive(true);
        continuityEnhant.goBtnStop.SetActive(false);
        continuityEnhant.goClose.SetActive(true);
        enhantInfo.blockScreen.CenterObjectDisable();

        foreach (var item in enhantInfo.goCloseBtns)
            item.SetActive(true);
    }

    bool isContinuityPlus = false;
    bool isPressOn = false;
    public void UpPress_ContinuityyMinus() => isPressOn = false;
    public void DwPress_ContinuityMinus()
    {
        if (continuityEnhant.min_count > 1)
        {
            isPressOn = true;
            isContinuityPlus = false;
            StopCoroutine("Routin_OnPress");
            StartCoroutine("Routin_OnPress");
        }
    }

    public void UpPress_ContinuityPlus() => isPressOn = false;
    public void DwPress_ContinuityPlus()
    {
        if(continuityEnhant.min_count < continuityEnhant.max_count)
        {
            isPressOn = true;
            isContinuityPlus = true;
            StopCoroutine("Routin_OnPress");
            StartCoroutine("Routin_OnPress");
        }
    }
    public void Click_ContinuityMax()
    {
        if(continuityEnhant.max_count > 0)
        {
            continuityEnhant.min_count = continuityEnhant.max_count;
            continuityEnhant.txContinuity.text = string.Format("{0}회 연속 강화", continuityEnhant.min_count);
        }
        else
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("최대 강화 가능 횟수가 0입니다.");
        }
    }

    IEnumerator Routin_OnPress()
    {
        float press_time = 0.5f;
        yield return null;
        while (isPressOn)
        {
            int bCnt = continuityEnhant.min_count;
            if (isContinuityPlus)
            {
                if (continuityEnhant.min_count < continuityEnhant.max_count)
                {
                    var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
                    bool isEqAcc = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(eqDB.eq_ty);
                    long ruby_ether = isEqAcc == true ? goods_db.m_ether : goods_db.m_ruby;
                    long gold_price = GameDatabase.GetInstance().questDB.GetQuestEquipEnhantGold(eqDB.eq_rt, eqDB.eq_id, eqDB.m_ehnt_lv);
                    int ruby_ether_price = GameDatabase.GetInstance().questDB.GetQuestEquipEnhantRubyOrEther(isEqAcc, eqDB.eq_rt, eqDB.eq_id);
                    int stn_rt = GameDatabase.GetInstance().smithyDB.GetEnhantStonTypeRating(eqDB.m_ehnt_lv); // 적용될 강화석 등급 
                    int stn_cnt = GameDatabase.GetInstance().smithyDB.GetEnhantStonTypeRatingCount(eqDB.eq_ty, stn_rt); // 내 소지 수량 
                    int stn_use_cnt = GameDatabase.GetInstance().smithyDB.GetEnhantStonCount(eqDB.m_ehnt_lv); // 필요 수량 

                    if (goods_db.m_gold >= gold_price * (bCnt + 1) && ruby_ether >= ruby_ether_price * (bCnt + 1) && stn_cnt >= stn_use_cnt * (bCnt + 1))
                    {
                        continuityEnhant.min_count++;
                    }
                    else
                    {
                        if (goods_db.m_gold < gold_price)
                        {
                            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("골드가 부족합니다.");
                            break;
                        }
                        else if (ruby_ether < ruby_ether_price)
                        {
                            if (isEqAcc)
                                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("에테르가 족합니다.\n에테르는 장신구(목걸이/귀걸이/반지)를 분해하여 획득할 수 있습니다");
                            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("루비가 족합니다.\n루비는 장비(무기/방패/방어구)를 분해하여 획득할 수 있습니다");
                            break;
                        }
                        else if (stn_cnt < stn_use_cnt)
                        {
                            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("강화석이 부족합니다.");
                            break;
                        }
                    }
                }
            }
            else
            {
                if (continuityEnhant.min_count > 1)
                    continuityEnhant.min_count--;
            }

            if (bCnt != continuityEnhant.min_count)
            {
                continuityEnhant.txContinuity.text = string.Format("{0}회 연속 강화", continuityEnhant.min_count);
                yield return new WaitForSeconds(press_time);
                if (press_time > 0.05f)
                    press_time -= 0.2f;
                else if (press_time > 0.025f)
                    press_time -= 0.05f;
                else press_time = 0.025f;
            }
            else break;
        }
    }
    #endregion
}