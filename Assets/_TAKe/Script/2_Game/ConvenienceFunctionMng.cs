using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class ConvenienceFunctionMng : MonoSingleton<ConvenienceFunctionMng>
{
    public GameMng gMgr = null;

    string strAutoSkill, strAutoPosion, strAutoGameSpeedX2, strAutoGameSpeedX3, strAutoSale, strAutoDecomp;
    public IG.ConvenienceFunction convenFun = new IG.ConvenienceFunction(); // 유저 편의 기능 

    void Start()
    {
        strAutoSkill = LanguageGameData.GetInstance().GetString("text.cnvc.fun.skill");
        strAutoPosion = LanguageGameData.GetInstance().GetString("text.cnvc.fun.posion");
        strAutoGameSpeedX2 = LanguageGameData.GetInstance().GetString("text.cnvc.fun.speed.x2");
        strAutoGameSpeedX3 = LanguageGameData.GetInstance().GetString("text.cnvc.fun.speed.x3");
        strAutoSale = LanguageGameData.GetInstance().GetString("text.cnvc.fun.sale");
        strAutoDecomp = LanguageGameData.GetInstance().GetString("text.cnvc.fun.decomp");
    }

    /// <summary> 편의 기능 초기화 </summary>
    public void InitConvenience()
    {
        // 자동 스킬 
        PlayerPrefs.DeleteKey(PrefsKeys.key_Convenience_OnfAutoSkill);
        string sAutoSkill_Onf = PlayerPrefs.GetString(PrefsKeys.key_Convenience_OnfAutoSkill);
        LogPrint.Print("<color=yellow> sAutoSkill_Onf : " + sAutoSkill_Onf + "</color>");
        if (string.IsNullOrEmpty(sAutoSkill_Onf))
            sAutoSkill_Onf = "ON";

        convenFun.cfAutoSkill.onOff = sAutoSkill_Onf == "OFF" ? IG.ConvenienceFunction.OnOff.OFF : IG.ConvenienceFunction.OnOff.ON;
        UIConvenienceAutoSkill();
         
        // 자동 물약 
        string sAutoPotion_Onf = PlayerPrefs.GetString(PrefsKeys.key_Convenience_OnfAutoPotion);
        LogPrint.Print("<color=green>자동 물약 sAutoPotion_Onf : " + sAutoPotion_Onf + "</color>");
        int use_rt = PlayerPrefs.GetInt(PrefsKeys.key_AutoPosionRating);
        if (use_rt == 0)
        {
            use_rt = 1;
            PlayerPrefs.SetInt(PrefsKeys.key_AutoPosionRating, use_rt);
        }

        int loadPcr = PlayerPrefs.GetInt(PrefsKeys.key_AutoPotionUsePercent);
        if (loadPcr == 0)
            loadPcr = 30;

        convenFun.cfAutoPosion.onOff = string.IsNullOrEmpty(sAutoPotion_Onf) || sAutoPotion_Onf == "OFF" ? IG.ConvenienceFunction.OnOff.OFF : IG.ConvenienceFunction.OnOff.ON;
        convenFun.cfAutoPosion.iUseRating = use_rt;
        convenFun.cfAutoPosion.recoPcr = GameDatabase.GetInstance().convenienceFunctionDB.AutoPotion_RecoveryPercent(use_rt);
        convenFun.cfAutoPosion.usePcr = loadPcr * 0.01f;
        UIConvenienceAutoPosion();

        // 게임 진행 속도 
        string sGameSpeed_Onf = PlayerPrefs.GetString(PrefsKeys.key_Convenience_OnfGameSpeed);
        convenFun.cfGameSpeed.onOffSpeed = string.IsNullOrEmpty(sGameSpeed_Onf) || sGameSpeed_Onf == "OFF" ? 
            IG.ConvenienceFunction.ConvenienceGameSpeed.OnOffSpeed.OFF : sGameSpeed_Onf == "ONx2" ? IG.ConvenienceFunction.ConvenienceGameSpeed.OnOffSpeed.ONx2 : IG.ConvenienceFunction.ConvenienceGameSpeed.OnOffSpeed.ONx3;
        UIConvenienceGameSpeed();

        // 장비 자동 판매 
        InitConvenienceAutoSale();
    }

    // 장비 자동 판매 초기화 
    public void InitConvenienceAutoSale()
    {
        string strSetting = PlayerPrefs.GetString(PrefsKeys.key_AutoSaleSetting);
        try
        {
            convenFun.cfAutoSale.saleSetting = JsonUtility.FromJson<IG.ConvenienceFunction.ConvenienceAutoSale.SaleSetting>(strSetting);
            if (convenFun.cfAutoSale.saleSetting.saleEquipType.Length == 0)
                convenFun.cfAutoSale.saleSetting.saleEquipType = new bool[3];

            if (convenFun.cfAutoSale.saleSetting.saleEquipRating.Length == 0)
                convenFun.cfAutoSale.saleSetting.saleEquipRating = new bool[7];

            if (convenFun.cfAutoSale.saleSetting.saleAcceRating.Length == 0)
                convenFun.cfAutoSale.saleSetting.saleAcceRating = new bool[7];

            if (convenFun.cfAutoSale.saleSetting.saleOrDecomp.Length == 0)
                convenFun.cfAutoSale.saleSetting.saleOrDecomp = new bool[2];
        }
        catch (System.Exception e) 
        {
            LogPrint.Print("e : " + e);
            convenFun.cfAutoSale.saleSetting.saleEquipType = new bool[3];
            convenFun.cfAutoSale.saleSetting.saleEquipRating = new bool[7];
            convenFun.cfAutoSale.saleSetting.saleAcceRating = new bool[7];
            convenFun.cfAutoSale.saleSetting.saleOrDecomp = new bool[2];
        }

        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        DateTime endDate = GameDatabase.GetInstance().convenienceFunctionDB.GetDate_ConvenFunAutoSale();
        if((endDate - nDate).TotalSeconds <= 0)
            endDate = GameDatabase.GetInstance().convenienceFunctionDB.GetDate_ConvenFunAutoSaleVideo();

        //bool isPremanent = GameDatabase.GetInstance().tableDB.GetUserInfo().m_auto_eq_sale_permanent == 1;
        if ((endDate - nDate).TotalSeconds > 0) // 남은 시간
        {
            convenFun.cfAutoSale.isPremanent = false;
            convenFun.cfAutoSale.endDate = endDate;

            string sAutoSale_Onf = PlayerPrefs.GetString(PrefsKeys.key_Convenience_OnfAutoSale);
            convenFun.cfAutoSale.onOff = string.IsNullOrEmpty(sAutoSale_Onf) || sAutoSale_Onf == "OFF" ? IG.ConvenienceFunction.OnOff.OFF : IG.ConvenienceFunction.OnOff.ON;
        }
        else
        {
            convenFun.cfAutoSale.onOff = IG.ConvenienceFunction.OnOff.OFF;
            PlayerPrefs.SetString(PrefsKeys.key_Convenience_OnfAutoSale, "OFF");
        }

        UIConvenienceAutoSale();
    }

    /// <summary> 자동 스킬 버튼 Click </summary>
    public void ChangeConvenienceAutoSkill()
    {
        string sAutoSkill_Onf = PlayerPrefs.GetString(PrefsKeys.key_Convenience_OnfAutoSkill);
        if (string.IsNullOrEmpty(sAutoSkill_Onf))
            sAutoSkill_Onf = "ON";

        convenFun.cfAutoSkill.onOff = string.Equals(sAutoSkill_Onf, "OFF") ? IG.ConvenienceFunction.OnOff.ON : IG.ConvenienceFunction.OnOff.OFF;
        PlayerPrefs.SetString(PrefsKeys.key_Convenience_OnfAutoSkill, convenFun.cfAutoSkill.onOff.ToString());
        UIConvenienceAutoSkill();
    }

    /// <summary> 자동 물약 버튼 Click </summary>
    public void ChangeConvenienceAutoPosion(bool isClick = false)
    {
        if (convenFun.cfAutoPosion.onOff == IG.ConvenienceFunction.OnOff.OFF)
        {
            PopUpMng.GetInstance().Open_AutoPotion(); // 팝업 
        }
        else
        {
            if (isClick == true)
            {
                convenFun.cfAutoPosion.onOff = IG.ConvenienceFunction.OnOff.OFF;
            }

            PlayerPrefs.SetString(PrefsKeys.key_Convenience_OnfAutoPotion, convenFun.cfAutoPosion.onOff.ToString());
            UIConvenienceAutoPosion();
        }
    }

    /// <summary>
    /// 팝업에서 물약 선택 완료 
    /// </summary>
    public void ConvenienceAutoPotion(int rt, int usePcr) // usePcr 물약 사용 기준 
    {
        convenFun.cfAutoPosion.iUseRating = rt;
        convenFun.cfAutoPosion.recoPcr = GameDatabase.GetInstance().convenienceFunctionDB.AutoPotion_RecoveryPercent(rt);
        convenFun.cfAutoPosion.usePcr = usePcr * 0.01f;
        convenFun.cfAutoPosion.onOff = IG.ConvenienceFunction.OnOff.ON;

        // Save 
        PlayerPrefs.SetInt(PrefsKeys.key_AutoPosionRating, convenFun.cfAutoPosion.iUseRating);
        PlayerPrefs.SetInt(PrefsKeys.key_AutoPotionUsePercent, usePcr);
        
        ChangeConvenienceAutoPosion();
    }

    /// <summary>
    /// 수량 부족으로 OFF 시킴
    /// </summary>
    public void ConvenienceAutoPotionlack()
    {
        convenFun.cfAutoPosion.onOff = IG.ConvenienceFunction.OnOff.OFF;
        PlayerPrefs.SetString(PrefsKeys.key_Convenience_OnfAutoPotion, convenFun.cfAutoPosion.onOff.ToString());
        UIConvenienceAutoPosion();
    }

    /// <summary> 게임 속도 버튼 Click </summary>
    public void ChangeConvenienceGameSpeed()
    {
        string val = PlayerPrefs.GetString(PrefsKeys.key_Convenience_OnfGameSpeed);
        convenFun.cfGameSpeed.onOffSpeed = string.IsNullOrEmpty(val) || string.Equals(val, "OFF") ? IG.ConvenienceFunction.ConvenienceGameSpeed.OnOffSpeed.ONx2 :
            string.Equals(val, "ONx2") ? IG.ConvenienceFunction.ConvenienceGameSpeed.OnOffSpeed.ONx3 : IG.ConvenienceFunction.ConvenienceGameSpeed.OnOffSpeed.OFF;

        PlayerPrefs.SetString(PrefsKeys.key_Convenience_OnfGameSpeed, convenFun.cfGameSpeed.onOffSpeed.ToString());
        UIConvenienceGameSpeed();
    }

    /// <summary> 자동 판매 버튼 Click </summary>
    public void ChangeConvenienceAutoSale()
    {
        if (gMgr.mode_type == IG.ModeType.CHAPTER_CONTINUE || gMgr.mode_type == IG.ModeType.CHAPTER_LOOP)
        {
            LogPrint.Print("<color=white> ChangeConvenienceAutoSale </color>");
            DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
            DateTime endDate = GameDatabase.GetInstance().convenienceFunctionDB.GetDate_ConvenFunAutoSale();
            if ((endDate - nDate).TotalSeconds <= 0)
                endDate = GameDatabase.GetInstance().convenienceFunctionDB.GetDate_ConvenFunAutoSaleVideo();

            bool isPremanent = GameDatabase.GetInstance().tableDB.GetUserInfo().m_auto_eq_sale_permanent == 1;
            if (isPremanent || (endDate - nDate).TotalSeconds > 0) // 영구 구매 or 남은 시간
            {
                if (convenFun.cfAutoSale.onOff == IG.ConvenienceFunction.OnOff.OFF)
                {
                    LogPrint.Print("OFF");
                    PopUpMng.GetInstance().Open_AutoSale();
                }
                else
                {
                    LogPrint.Print("ON");
                    convenFun.cfAutoSale.isPremanent = isPremanent;
                    convenFun.cfAutoSale.endDate = endDate;

                    string val = PlayerPrefs.GetString(PrefsKeys.key_Convenience_OnfAutoSale);
                    convenFun.cfAutoSale.onOff = string.IsNullOrEmpty(val) || string.Equals(val, "OFF") ? IG.ConvenienceFunction.OnOff.ON : IG.ConvenienceFunction.OnOff.OFF;
                    PlayerPrefs.SetString(PrefsKeys.key_Convenience_OnfAutoSale, convenFun.cfAutoSale.onOff.ToString());
                }

                UIConvenienceAutoSale();
            }
            else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("자동 판매/분해 사용 가능 시간이 없습니다.\n확인 버튼을 누르시면 구매 창으로 이동됩니다.", AutoSalePopUpAction);
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("챕터 진행(반복) 중인경우에만 변경할 수 있습니다.");
    }

    void AutoSalePopUpAction()
    {
        PopUpMng.GetInstance().Open_DailyProductReward();
    }

    /// <summary>
    /// 팝업에서 자동 판매 세팅 완료
    /// </summary>
    public void ConvenienceAutoSale(bool[] saleType, bool[] saleEquipRt, bool[] saleAcceRt, bool[] saleOrDecomp)
    {
        convenFun.cfAutoSale.saleSetting.saleEquipType = saleType;
        convenFun.cfAutoSale.saleSetting.saleEquipRating = saleEquipRt;
        convenFun.cfAutoSale.saleSetting.saleAcceRating = saleAcceRt;
        convenFun.cfAutoSale.saleSetting.saleOrDecomp = saleOrDecomp;
        PlayerPrefs.SetString(PrefsKeys.key_AutoSaleSetting, JsonUtility.ToJson(convenFun.cfAutoSale.saleSetting));// Save 

        convenFun.cfAutoSale.onOff = IG.ConvenienceFunction.OnOff.ON;
        ChangeConvenienceAutoSale();
    }

    /// <summary> 자동 스킬 UI </summary>
    void UIConvenienceAutoSkill()
    {
        bool isOn = convenFun.cfAutoSkill.onOff == IG.ConvenienceFunction.OnOff.ON;
        convenFun.cfAutoSkill.imBtnBg.sprite = isOn ? SpriteAtlasMng.GetInstance().GetSpriteRatingBg(6) : SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);
        convenFun.cfAutoSkill.txBtn.text = string.Format("{0} {1}", strAutoSkill, isOn == true ? "ON" : "OFF");
    }

    /// <summary> 자동 물약 UI </summary>
    public void UIConvenienceAutoPosion()
    {
        StopCoroutine("Routin_AutoPotion");
        bool isNotUse = GameMng.GetInstance().mode_type == IG.ModeType.PVP_BATTLE_ARENA; // 사용 금지 -> PvP 배틀 아레나 => 물약 사용 금지 
        convenFun.cfAutoPosion.goNotUse.SetActive(isNotUse);
        if (!isNotUse)
        {
            bool isOn = convenFun.cfAutoPosion.onOff == IG.ConvenienceFunction.OnOff.ON;
            convenFun.cfAutoPosion.imBtnBg.sprite = isOn ? SpriteAtlasMng.GetInstance().GetSpriteRatingBg(6) : SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);
            convenFun.cfAutoPosion.onUIShiny.gameObject.SetActive(isOn);
            if (isOn)
            {
                int useRt = convenFun.cfAutoPosion.iUseRating;
                int useRtPotionCnt = GameDatabase.GetInstance().tableDB.GetItem(20, useRt).count;
                string strCoRt = GameDatabase.StringFormat.GetRatingColorText(useRt, true);
                convenFun.cfAutoPosion.txBtn.text = string.Format("{0}\n{1}", strAutoPosion, isOn == true ? "ON" : "OFF");
                convenFun.cfAutoPosion.txOnCnt.text = string.Format("{0} x{1}", strCoRt, useRtPotionCnt);
                convenFun.cfAutoPosion.goOnObject.SetActive(true);
                StartCoroutine("Routin_AutoPotion");
            }
            else
            {
                convenFun.cfAutoPosion.txBtn.text = string.Format("{0}\n{1}", strAutoPosion, "OFF");
                convenFun.cfAutoPosion.goOnObject.SetActive(false);
            }
        }
    }

    /// <summary> 물약 사용 루프 </summary>
    IEnumerator Routin_AutoPotion()
    {
        int useRt = convenFun.cfAutoPosion.iUseRating;
        int useRtPotionCnt = GameDatabase.GetInstance().tableDB.GetItem(20, useRt).count;
        string strCoRt = GameDatabase.StringFormat.GetRatingColorText(useRt, true);

        yield return null;
        while (useRtPotionCnt > 0)
        {
            if (convenFun.cfAutoPosion.onOff == IG.ConvenienceFunction.OnOff.OFF)
                yield break;
            else
            {
                DateTime nt = BackendGpgsMng.GetInstance().GetNowTime();
                DateTime dt = convenFun.cfAutoPosion.reUseDate;
                int sec = (int)(dt - nt).TotalSeconds;
                if (sec > 0)
                {
                    convenFun.cfAutoPosion.txReUseTime.text = sec.ToString();
                }
                else
                {
                    if (gMgr.myPZ != null && GameMng.GetInstance().mode_type != IG.ModeType.PVP_BATTLE_ARENA)
                    {
                        long maxMyHp = gMgr.myPZ.igp.statValue.stat5_valHealth;
                        long nowHp = gMgr.myPZ.GetHp();
                        long pcrHp = (long)(maxMyHp * convenFun.cfAutoPosion.usePcr);
                        convenFun.cfAutoPosion.txReUseTime.text = "~";

                        while (nowHp >= pcrHp)
                        {
                            if (gMgr.myPZ != null)
                            {
                                nowHp = gMgr.myPZ.GetHp();
                                pcrHp = (long)(maxMyHp * convenFun.cfAutoPosion.usePcr);
                            }

                            yield return new WaitForSeconds(0.1f);
                        }

                        if (gMgr.myPZ != null && GameMng.GetInstance().mode_type != IG.ModeType.PVP_BATTLE_ARENA)
                        {
                            gMgr.myPZ.RecoveryPotionHp(convenFun.cfAutoPosion.recoPcr);  // 일정량 체력 회복 
                            var item_potion = GameDatabase.GetInstance().tableDB.GetItem(20, useRt);
                            item_potion.count--;
                            useRtPotionCnt = item_potion.count;
                            GameDatabase.GetInstance().tableDB.UpdateClientDB_Item(item_potion);
                            if (useRtPotionCnt <= 0)
                            {
                                ConvenienceAutoPotionlack();
                            }
                            else
                            {
                                convenFun.cfAutoPosion.txOnCnt.text = string.Format("x{0} {1}", useRtPotionCnt.ToString(), strCoRt);
                                convenFun.cfAutoPosion.reUseDate = BackendGpgsMng.GetInstance().GetNowTime().AddSeconds(10);
                            }
                        }
                    }
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary> 게임 속도 UI </summary>
    async void UIConvenienceGameSpeed()
    {
        while(gMgr.myPZ == null || gMgr.orPZ == null)
            await Task.Delay(100);

        bool isOn = convenFun.cfGameSpeed.onOffSpeed == IG.ConvenienceFunction.ConvenienceGameSpeed.OnOffSpeed.ONx2 || convenFun.cfGameSpeed.onOffSpeed == IG.ConvenienceFunction.ConvenienceGameSpeed.OnOffSpeed.ONx3;
        convenFun.cfGameSpeed.imBtnBg.sprite = isOn ? SpriteAtlasMng.GetInstance().GetSpriteRatingBg(6) : SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);
        if (!isOn)
        {
            convenFun.cfGameSpeed.txBtn.text = "게임 속도 증가\nOFF";
        }
        else
        {
            convenFun.cfGameSpeed.txBtn.text = string.Format("{0}\n{1}", convenFun.cfGameSpeed.onOffSpeed == IG.ConvenienceFunction.ConvenienceGameSpeed.OnOffSpeed.ONx2 ? strAutoGameSpeedX2 : strAutoGameSpeedX3, isOn == true ? "ON" : "OFF");
        }

        gMgr.myPZ.anim.speed = GameMng.GetInstance().GameSpeed;
        gMgr.orPZ.anim.speed = GameMng.GetInstance().GameSpeed;
    }

    /// <summary> 자동 판매 UI </summary>
    void UIConvenienceAutoSale()
    {
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        DateTime endDate = GameDatabase.GetInstance().convenienceFunctionDB.GetDate_ConvenFunAutoSale();
        if ((endDate - nDate).TotalSeconds <= 0)
            endDate = GameDatabase.GetInstance().convenienceFunctionDB.GetDate_ConvenFunAutoSaleVideo();

        if ((endDate - nDate).TotalSeconds > 0)
        {
            convenFun.cfAutoSale.endDate = endDate;
            StopCoroutine("Routin_AutoSale");
            StartCoroutine("Routin_AutoSale");
        }
        else
        {
            convenFun.cfAutoSale.endDate = nDate.AddMinutes(-1);
            convenFun.cfAutoSale.txBtn.text = "장비 자동\n판매(분해) OFF";
        }

        bool isOn = convenFun.cfAutoSale.onOff == IG.ConvenienceFunction.OnOff.ON;
        convenFun.cfAutoSale.imBtnBg.sprite = isOn ? SpriteAtlasMng.GetInstance ().GetSpriteRatingBg(6) : SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);
        convenFun.cfAutoSale.onUIShiny.SetActive(isOn);
    }

    /// <summary>
    /// 장비 ty, rt 를 가지고 자동판매 ON되어있다면 체크 및 판매or분해 
    /// </summary>
    public bool AutoSaleOnEquipType(int eqTy, int eqRt, int eqId)
    {
        if (convenFun.cfAutoSale.onOff == IG.ConvenienceFunction.OnOff.OFF)
            return false;

        bool dropIsAcce = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(eqTy);
        bool isSaleEquipRt = false; // 설정해 놓은 장비 자동 판매or분해 등급 찾기 
        int chkRt = 1;
        var seleRating = dropIsAcce ? convenFun.cfAutoSale.saleSetting.saleAcceRating : convenFun.cfAutoSale.saleSetting.saleEquipRating;
        foreach (bool b in seleRating)
        {
            if (b == true)
            {
                if (chkRt == eqRt)
                {
                    isSaleEquipRt = true;
                    break;
                }
            }

            chkRt++;
        }

        bool isSale = false;
        if (isSaleEquipRt)
        {
            int chk_eqTy = 0;
            foreach (bool b in convenFun.cfAutoSale.saleSetting.saleEquipType)
            {
                if (chk_eqTy == 0) // 무/방 
                {
                    if (b == true && (eqTy >= 0 && eqTy <= 1))
                    {
                        isSale = true;
                        break;
                    }
                }
                else if (chk_eqTy == 1) // 방어구 
                {
                    if (b == true && (eqTy >= 2 && eqTy <= 7))
                    {
                        isSale = true;
                        break;
                    }
                }
                else if (chk_eqTy == 2) // 장신구 
                {
                    if (b == true && (eqTy >= 8 && eqTy <= 10))
                    {
                        isSale = true;
                        break;
                    }
                }

                chk_eqTy++;
            }

            if (isSale)
            {
                bool isAcce = GameDatabase.GetInstance().chartDB.GetIsPartsTypeAcce(eqTy);
                if (convenFun.cfAutoSale.saleSetting.saleOrDecomp[0] == true) // 판매 
                {
                    float pet_sop1_value = GameMng.GetInstance().myPZ.igp.statValue.petSpOpTotalFigures.sop1_value * 0.01f;
                    int rwdGoldCnt = GameDatabase.GetInstance().questDB.GetQuestEquipSaleGold(eqRt, eqId);
                    int result_rwdGoldCnt = rwdGoldCnt + (int)(rwdGoldCnt * pet_sop1_value);
                    GameDatabase.GetInstance().tableDB.SetUpdateGoods("gold", result_rwdGoldCnt, "+");
                    PopUpMng.GetInstance().OpenDropInfoViewAutoSaleDecomposition(true, isAcce, eqTy, eqRt, eqId, result_rwdGoldCnt);
                }
                else if (convenFun.cfAutoSale.saleSetting.saleOrDecomp[1] == true) // 분해 
                {
                    // 장신구(목걸이/귀걸이/반지) 분해 
                    if (isAcce)
                    {
                        int rwdEtherCnt = GameDatabase.GetInstance().questDB.GetQuestEquipDecompEther(eqRt, eqId);
                        GameDatabase.GetInstance().tableDB.SetUpdateGoods("ether", rwdEtherCnt, "+");
                        PopUpMng.GetInstance().OpenDropInfoViewAutoSaleDecomposition(false, isAcce, eqTy, eqRt, eqId, rwdEtherCnt);
                    }
                    // 장비(무기/방패/방어구) 분해 
                    else
                    {
                        int rwdRubyCnt = GameDatabase.GetInstance().questDB.GetQuestEquipDecompRuby(eqRt, eqId);
                        GameDatabase.GetInstance().tableDB.SetUpdateGoods("ruby", rwdRubyCnt, "+");
                        PopUpMng.GetInstance().OpenDropInfoViewAutoSaleDecomposition(false, isAcce, eqTy, eqRt, eqId, rwdRubyCnt);
                    }
                }

                GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr3, 1); // 업적, nbr3 장비 판매/분해!
                GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr12, 1); // 일일미션, nbr12 장비 판매/분해 하기! 
            }
        }

        return isSale;
    }

    /// <summary> 자동 판매 남은 시간 </summary>
    IEnumerator Routin_AutoSale()
    {
        bool isOn = convenFun.cfAutoSale.onOff == IG.ConvenienceFunction.OnOff.ON;
        bool isSale = convenFun.cfAutoSale.saleSetting.saleOrDecomp[0];
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        DateTime endDate = convenFun.cfAutoSale.endDate;
        int totalSec = (int)(endDate - nDate).TotalSeconds;
        yield return null;
        while (endDate > nDate)
        {
            nDate = BackendGpgsMng.GetInstance().GetNowTime();
            totalSec = (int)(endDate - nDate).TotalSeconds;
            if (totalSec >= 0 && convenFun.cfAutoSale.isPremanent == false)
            {
                int day, hours, minute, second;
                day = totalSec / (24 * 3600);

                totalSec = totalSec % (24 * 3600);
                hours = totalSec / 3600;

                totalSec %= 3600;
                minute = totalSec / 60;

                totalSec %= 60;
                second = totalSec;

                convenFun.cfAutoSale.txBtn.text = string.Format("<color=#FFE800>{0} {1}\n{2}</color>", isSale == true ? strAutoSale : strAutoDecomp, isOn == true ? "ON" : "OFF",
                    string.Format("{0:00}d {1:00}:{2:00}:{3:00}", day, hours, minute, second));
            }

            yield return new WaitForSeconds(0.5f);
        }

        InitConvenienceAutoSale();
    }
}
