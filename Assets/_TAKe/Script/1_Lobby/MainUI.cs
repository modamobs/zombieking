using Assets.Pixelation.Scripts;
using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoSingleton<MainUI>
{
    public TopUI topUI;
    [SerializeField] MobilePostProcessing mpp;
    [SerializeField] CanvasGroup gameTopScreenBlackWhite;
    [SerializeField] CanvasGroup gameFullScreenBlackWhite;
    [SerializeField] GameObject[] Taps = new GameObject[6];

    [SerializeField] MobilePostProcessing mps;

    [SerializeField] Canvas canvasHome;
    public bool isOnSkillEftObj() => canvasHome.enabled == true && isChatQuestSizeUp == false;

    public bool isChatQuestSizeUp = false;

    [SerializeField] HomeOptionButtons homeOptionButtons;
    [SerializeField] StageTypeUI stageTypeUI;

    public AndroidShareUsing androidShareUsing;
    public TapGameBattleInfo tapGameBattleInfo;
    public TapCharacter tapCharacter;
    public Inventory inventory;
    public TapSmithy tapSmithy;
    public TapSkill tapSkill;
    public TapPet tapPet;
    public TapDungeon tapDungeon;
    public TapIAP tapIAP;
    public TapStageDropInfo tapStageDropInfo;
    public TapQuest tapQuest;
    public TapNoticeTipMessage tapNoticeTipMessage;

    //#if UNITY_EDITOR
    //    void OnDrawGizmos()
    //    {
    //        if (Application.isPlaying)
    //        {
    //            float fScaleWidth = ((float)Screen.width / (float)Screen.height) / ((float)9 / (float)16);
    //            float top = (720 * (720 / (720 * fScaleWidth)));
    //            RectBottom.offsetMax = new Vector2(RectBottom.offsetMax.x, -top);
    //        }
    //    }
    //#endif

    public float[] sx = new float[] { 0,1,1,1,1,1,1,1,1,1 };

    void Awake()
    {
        gameTopScreenBlackWhite.alpha = 0;
        gameTopScreenBlackWhite.gameObject.SetActive(false);
        gameFullScreenBlackWhite.alpha = 0;
        gameFullScreenBlackWhite.gameObject.SetActive(false);
    }

    public void Init ()
    {
        OnTap(0);

        inventory.tapOpenSP = GameDatabase.GetInstance().GetUniqueIDX();
        System.DateTime ntDate = BackendGpgsMng.GetInstance().GetNowTime();
        
        string rank_dateStr = PlayerPrefs.GetString(PrefsKeys.key_PvPArenaRankNextLoadTime);
        if (string.IsNullOrEmpty(rank_dateStr))
        {
            GameDatabase.GetInstance().pvpBattleRecord.structData.rank.nextLoadTIme = ntDate;
            PlayerPrefs.SetString(PrefsKeys.key_PvPArenaRankNextLoadTime, ntDate.ToString());
        }
        else GameDatabase.GetInstance().pvpBattleRecord.structData.rank.nextLoadTIme = System.DateTime.Parse(rank_dateStr);

        string math_dateStr = PlayerPrefs.GetString(PrefsKeys.key_PvPArenaMatchNextLoadTime);
        LogPrint.Print(" 1 math_dateStr : " + math_dateStr);
        LogPrint.Print(" 2 math_dateStr : " + GameDatabase.GetInstance().pvpBattleRecord.structData.match.nextLoadTime);
        if (string.IsNullOrEmpty(math_dateStr))
        {
            GameDatabase.GetInstance().pvpBattleRecord.structData.match.nextLoadTime = ntDate;
            PlayerPrefs.SetString(PrefsKeys.key_PvPArenaMatchNextLoadTime, ntDate.ToString());
        }
        else GameDatabase.GetInstance().pvpBattleRecord.structData.match.nextLoadTime = System.DateTime.Parse(math_dateStr);

        string record_dateStr = PlayerPrefs.GetString(PrefsKeys.key_PvPArenaRecordNextLoadTime);
        if (string.IsNullOrEmpty(record_dateStr))
        {
            GameDatabase.GetInstance().pvpBattleRecord.structData.record.nextLoadTIme = ntDate;
            PlayerPrefs.SetString(PrefsKeys.key_PvPArenaRecordNextLoadTime, ntDate.ToString());
        }
        else GameDatabase.GetInstance().pvpBattleRecord.structData.record.nextLoadTIme = System.DateTime.Parse(record_dateStr);

        tapGameBattleInfo.Init_BuffDebuff();
        inventory.CharacterEquipStatUIRefresh(-1);
        tapQuest.Init();
    }

    public void OnTap(int onTapID)
    {
        LogPrint.Print("<color=magenta> ---------------- on tap : " + onTapID + "</color>");
        for (int i = 0; i < Taps.Length; i++)
        {
            Taps[i].SetActive(i == onTapID);
            if(onTapID == 0)
            {
                canvasHome.enabled = true;
                if (isChatQuestSizeUp == false)
                {
                    foreach (ParticleSystemRenderer item in tapGameBattleInfo.psCardEfts)
                        item.enabled = true;
                }
            }
            else
            {
                canvasHome.enabled = false;
                foreach (var item in tapGameBattleInfo.psCardEfts)
                    item.enabled = false;
            }
        }
    }

    public void QuestChatSizeUpDwHomeSkillEft(bool isEftOnOrOff)
    {
        foreach (var item in tapGameBattleInfo.psCardEfts)
        {
            item.enabled = isEftOnOrOff;
        }
    }

    /// <summary>
    /// 매인 탭 변경 
    /// </summary>
    public void ClickMenuButton(int idx)
    {
        var md = GameMng.GetInstance().mode_type;
        if (md == IG.ModeType.CHAPTER_CONTINUE || md == IG.ModeType.CHAPTER_LOOP)
        {
            OnTap(idx);
            if (idx == 1) // 인벤토리 
            {
                MainUI.GetInstance().inventory.inventoryType = Inventory.InventoryType.ALL;
                PopUpMng.GetInstance().popUpInventorySort.enum_SortInvnHighLow = (SortInventorytHighLow)PlayerPrefs.GetInt(PrefsKeys.prky_SortInventorytHighLow_etc);
                PopUpMng.GetInstance().popUpInventorySort.enum_SortInventory = (SortInventory)PlayerPrefs.GetInt(PrefsKeys.prky_SortInventory_etc);

                MainUI.GetInstance().inventory.initOnStartInventoryAll.SetInit(true);
                inventory.InitRightTapBtn();
            }
            else if (idx == 2) // 대장간 
            {
                PopUpMng.GetInstance().popUpInventorySort.enum_SortInvnHighLow = SortInventorytHighLow.HIGH_TO_LOW;
                PopUpMng.GetInstance().popUpInventorySort.enum_SortInventory = SortInventory.RATING;
                tapSmithy.smithyListType = SmithyListType.EQUIP_WEAPON_SHIELD;

                tapSmithy.TapOpen(SmithyTapType.Enhancement, default, default);
                MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
            }
            else if (idx == 3)
            {
                int useMainSlot = GameDatabase.GetInstance().tableDB.GetUseMainSlot();
                tapSkill.Init(useMainSlot);
            }
            else if(idx == 4) // 펫 
            {
                tapPet.SetData();
            }
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("던전 및 PvP 진행 중에는 사용할 수 없는 기능입니다.");
    }

    /// <summary>
    /// 새로운 장비, 아이템 획득시 인벤열려있을때에만 바로 새로 고침 
    /// </summary>
    public void NewEquipItemInventortRefresh ()
    {
        if(inventory.inventoryType != Inventory.InventoryType.Disable)
        {
            inventory.initOnStartInventoryAll.SetInit();
        }
    }

    /// <summary>
    /// 새로운 스킬 획득시 스킬리스트 탭이 열려있을때에만 바로 새로 고침 
    /// </summary>
    public void NewSkillScrollRefresh ()
    {
        if(Taps[3].activeInHierarchy)
        {
            int useMainSlot = GameDatabase.GetInstance().tableDB.GetUseMainSlot();
            tapSkill.Init(useMainSlot);
        }
    }

    public void OnClickRightSlider(GameObject go)
    {
        string aName = go.gameObject.activeSelf ? "UIRightListClose" : "UIRightListOpen";
        go.transform.parent.GetComponent<Animator>().Play(aName);
    }

    /// <summary>
    /// 카메라 화면 전환 -> 어둡게 
    /// </summary>
    public async Task PlayScreenPixelSwitchingToBlack(bool isFullBlack, float mpt = 0.001f)
    {
        float val = 0f;
        if (isFullBlack == false)
        {
            gameTopScreenBlackWhite.alpha = 0f;
            gameTopScreenBlackWhite.gameObject.SetActive(true);
            
            while (gameTopScreenBlackWhite.alpha < 1)
            {
                val += mpt;
                gameTopScreenBlackWhite.alpha += val;
                await Task.Delay(1);
            }
        }
        else
        {
            gameFullScreenBlackWhite.alpha = 0f;
            gameFullScreenBlackWhite.gameObject.SetActive(true);
            while (gameFullScreenBlackWhite.alpha < 1)
            {
                val += mpt;
                gameFullScreenBlackWhite.alpha += val;
                await Task.Delay(1);
            }
        }

        System.GC.Collect();
    }

    /// <summary>
    /// 카메라 화면 전환 -> 밝게  
    /// </summary>
    public async Task PlayScreenPixelSwitchingToWhite(bool isFullBlack, float mpt = 0.01f)
    {
        float val = 0f;
        if (isFullBlack == false)
        {
            gameTopScreenBlackWhite.alpha = 1;
            gameTopScreenBlackWhite.gameObject.SetActive(true);

            while (gameTopScreenBlackWhite.alpha > 0)
            {
                val += mpt;
                gameTopScreenBlackWhite.alpha -= val;
                await Task.Delay(1);
            }

            gameTopScreenBlackWhite.gameObject.SetActive(false);
        }
        else
        {
            gameFullScreenBlackWhite.alpha = 1;
            gameFullScreenBlackWhite.gameObject.SetActive(true);

            while (gameFullScreenBlackWhite.alpha > 0)
            {
                val += mpt;
                gameFullScreenBlackWhite.alpha -= val;
                await Task.Delay(1);
            }

            gameFullScreenBlackWhite.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 장비를 교체하거나 착용했을 때 UI, 게임정보를 새로 고침, eq_type : -1이면 전체 새로 고침 
    /// </summary>
    public void RefreshGameStatViewInfo(int eq_type)
    {
        GameDatabase.GetInstance().characterDB.SetPlayerStatValue(); // 스탯 
        MainUI.GetInstance().inventory.CharacterEquipStatUIRefresh(eq_type);

        GameDatabase.GetInstance().tableDB.SetttingEquipWearingData(eq_type);
        MainUI.GetInstance().inventory.initOnStartInventoryAll.SetInit();

        GameMng.GetInstance().myPZ.Setting(); // 스탯 
        GameMng.GetInstance().myPZ.SettingParts(eq_type); // 필드 좀비 
    }

    /// <summary>
    /// 스탯 DB, 스탯 View(인벤토리의 상단 오른쪽), 인게임 스탯 
    /// </summary>
    public void RefreshGameStat()
    {
        GameDatabase.GetInstance().characterDB.SetPlayerStatValue(); // 스탯 
        MainUI.GetInstance().tapCharacter.SetStatView(GameDatabase.GetInstance().characterDB.GetStat());
        GameMng.GetInstance().myPZ.Setting(); // 스탯 
    }

    #region # 인벤토리 정보 새로 고침 #
    /// <summary> 인벤토리 확장버튼의 현재수량/최대수량 표시 </summary>
    public void InventoryItemTotalCountRefresh()
    {
        if(inventory.gameObject.activeSelf)
            inventory.InventoryLevelCount();
    }
    #endregion 

    public void ClickHomeOptionButton (GameObject root)
    {
        bool isOn = root.activeSelf;
        if (isOn)
            homeOptionButtons.ani.Play("HomeOptionButtonsOff");
        else
            homeOptionButtons.ani.Play("HomeOptionButtonsOn");

        homeOptionButtons.imIcon.sprite = SpriteAtlasMng.GetInstance().GetHomeOptionButton(isOn);
        homeOptionButtons.txOnOff.text = isOn == true ? "열기" : "닫기";
    }

    /// <summary>
    /// 챕터 스테이지 진행 모드 변경 버튼 
    /// </summary>
    public void Click_StageTypeChange ()
    {
        if(GameMng.GetInstance ().mode_type == IG.ModeType.CHAPTER_LOOP)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("스테이지 반복 모드<color=yellow>(∞)</color>에서는 일반, 보스 모드를 변경할 수 없습니다.");
        }
        else
        {
            int nowSType = PlayerPrefs.GetInt(PrefsKeys.prky_StageType);
            if (nowSType == 0)
            {
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("현재 일반 진행 모드입니다.\n 확인 버튼을 누르시면 <color=yellow><보스 진행 모드></color>로 변경됩니다.\n<color=red>(보스 몬스터만 상대)</color>", Listener_StageTypeChange);
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("현재 보스 진행 모드입니다.\n 확인 버튼을 누르시면 <color=yellow><일반 진행 모드></color>로 변경됩니다.\n<color=red>(일반 + 보스 몬스터 상대)</color>", Listener_StageTypeChange);
            }
        }
    }

    public void Listener_StageTypeChange()
    {
        int nowSType = PlayerPrefs.GetInt(PrefsKeys.prky_StageType);
        PlayerPrefs.SetInt(PrefsKeys.prky_StageType, nowSType == 0 ? 1 : 0);
        GameMng.GetInstance().ChapterStageType(true);
    }

    public void UIChapterStageTypeButton()
    {
        var st = GameMng.GetInstance().stage_type;
        if(st == IG.StageType.NORMAL_MONSTER)
        {
            stageTypeUI.txt.text = "<color=#FFFFFF>일반 모드중</color>";
            stageTypeUI.imBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSprite("rating_bg_3");
        }
        else if(st == IG.StageType.BOSS_MONSTER)
        {
            stageTypeUI.txt.text = "<color=#FF0084>보스 모드중</color>";
            stageTypeUI.imBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSprite("rating_bg_5");
        }
        //stageTypeUI
    }

    public void Click_MoveGoldShop()
    {
        if (GameMng.GetInstance().mode_type == IG.ModeType.CHAPTER_LOOP || GameMng.GetInstance().mode_type == IG.ModeType.CHAPTER_CONTINUE)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("골드 구매 탭으로 이동됩니다.", Listener_MoveItemShop);
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("던전 및 PvP 진행 중에는 사용할 수 없는 기능입니다.");
    }

    public void Click_MoveTbcShop()
    {
        if (GameMng.GetInstance().mode_type == IG.ModeType.CHAPTER_LOOP || GameMng.GetInstance().mode_type == IG.ModeType.CHAPTER_CONTINUE)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아 구매 탭으로 이동됩니다.", Listener_MoveTbcShop);
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("던전 및 PvP 진행 중에는 사용할 수 없는 기능입니다.");
    }

    public void Listener_MoveTbcShop()
    {
        PopUpMng.GetInstance().RmvAllClosePop();
        tapIAP.TopTapChange(tapIAP.tapShopDiamond.gameObject);
        OnTap(6);
    }

    public void Listener_MoveItemShop()
    {
        PopUpMng.GetInstance().RmvAllClosePop();
        tapIAP.TopTapChange(tapIAP.tapShopItem.gameObject);
        OnTap(6);
    }

    public void Listener_MoveLuckShop()
    {
        PopUpMng.GetInstance().RmvAllClosePop();
        tapIAP.TopTapChange(tapIAP.tapShopLuck.gameObject);
        OnTap(6);
    }

    public void Listener_MoveExchangeShop()
    {
        PopUpMng.GetInstance().RmvAllClosePop();
        tapIAP.TopTapChange(tapIAP.tapShopExchange.gameObject);
        OnTap(6);
    }

    public void Listener_MoveDungeonTop()
    {
        PopUpMng.GetInstance().RmvAllClosePop();
        //tapDungeon.Click_TapOpen(1);
        //OnTap(5);

        MainUI.GetInstance().ClickMenuButton(4);
        MainUI.GetInstance().tapDungeon.Click_TapOpen(1);
    }
}