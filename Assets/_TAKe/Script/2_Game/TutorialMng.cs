using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialMng : MonoSingleton<TutorialMng>
{
    [SerializeField] MainType mType = MainType.TOP;
    [System.Serializable]
    enum MainType
    {
        TOP,
        BOTTOM_TOP,
        BOTTOM
    }

    private bool isFirstPlay = false;
    [SerializeField] Transform trFullRoot;
    [SerializeField] RectTransform rectCopyObjectWhiteBg, rectCopyObjectTouchLock;
    [SerializeField] TutorialUIView nowTutorialUIView;
    [SerializeField] GameObject goNowTutorialTapBox;

    [SerializeField] List<GameObject> tempMainTopInstanctObject = new List<GameObject>();
    [System.Serializable]
    [SerializeField]
    struct TutorialUIView
    {
        public GameObject goRootMsgBox;
        public GameObject goTutorialBox; // 튜토 진행중인 탭 box를 copy create 
        public VerticalLayoutGroup vlgImageText;
        public CanvasGroup canvasGroupImageText;
        public Button btnNext, btnBack, btnReward;

        public GameObject rootOnlyText;
        public Text txOnlyText;

        public GameObject rootImageText;
        public ImageLineText[] imageLineText;
    }

    [System.Serializable]
    [SerializeField]
    struct ImageLineText
    {
        public GameObject goRoot;
        public Image img;
        public Text txt;
    }

    #region ------------ 공용 ------------
    private void SetCopyObjectSetting(GameObject copyObject, bool isBtnSetActiveFalseOrEnableDisable, bool isWhiteRedBox, bool isEventTriggerDisable = false)
    {
        GameObject inst_CopyObject = (GameObject)Instantiate(copyObject, copyObject.transform.parent);// trFullRoot.transform);
        inst_CopyObject.SetActive(true);
        inst_CopyObject.transform.localScale = Vector3.one;
        inst_CopyObject.transform.SetParent(trFullRoot.transform);
        tempMainTopInstanctObject.Add(inst_CopyObject);

        if (isWhiteRedBox)
        {
            GameObject inst_WhiteBg = Instantiate(rectCopyObjectWhiteBg.gameObject, inst_CopyObject.transform);
            inst_WhiteBg.transform.localScale = Vector3.one;
            RectTransform rect_CopyWhiteBg = inst_WhiteBg.GetComponent<RectTransform>();
            rect_CopyWhiteBg.anchorMin = rectCopyObjectWhiteBg.anchorMin;
            rect_CopyWhiteBg.anchorMax = rectCopyObjectWhiteBg.anchorMax;
            rect_CopyWhiteBg.anchoredPosition = rectCopyObjectWhiteBg.anchoredPosition;
            rect_CopyWhiteBg.sizeDelta = rectCopyObjectWhiteBg.sizeDelta;
            tempMainTopInstanctObject.Add(inst_WhiteBg);
        }

        var btns = inst_CopyObject.gameObject.GetComponentsInChildren<Button>();
        if (btns.Length > 0)
        {
            if (isBtnSetActiveFalseOrEnableDisable)
            {
                foreach (Button item in btns)
                    item.gameObject.SetActive(false);
            }
            else
            {
                foreach (Button item in btns)
                    item.enabled = false;
            }
        }

        if (isEventTriggerDisable)
        {
            var eventTriggers = inst_CopyObject.gameObject.GetComponentsInChildren<EventTrigger>();
            foreach (var item in eventTriggers)
                item.enabled = false;
        }
    }

    private void SetCopyObjectNowNowTutorialBox(GameObject copyObject, bool isWhiteRedBoxOrTouchLock)
    {
        if (goNowTutorialTapBox != copyObject)
            Destroy(goNowTutorialTapBox);

        GameObject inst_TutoBoxCopyObject = (GameObject)Instantiate(copyObject, copyObject.transform.parent);// trFullRoot.transform);
        inst_TutoBoxCopyObject.SetActive(true);
        inst_TutoBoxCopyObject.transform.localScale = Vector3.one;
        inst_TutoBoxCopyObject.transform.SetParent(trFullRoot.transform);
        goNowTutorialTapBox = inst_TutoBoxCopyObject;

        RectTransform rectBox = isWhiteRedBoxOrTouchLock == true ? rectCopyObjectWhiteBg : rectCopyObjectTouchLock;
        GameObject inst_WhiteBg = Instantiate(rectBox.gameObject, inst_TutoBoxCopyObject.transform);
        inst_WhiteBg.transform.localScale = Vector3.one;
        RectTransform rect_CopyWhiteBg = inst_WhiteBg.GetComponent<RectTransform>();
        rect_CopyWhiteBg.anchorMin = rectBox.anchorMin;
        rect_CopyWhiteBg.anchorMax = rectBox.anchorMax;
        rect_CopyWhiteBg.anchoredPosition = rectBox.anchoredPosition;
        rect_CopyWhiteBg.sizeDelta = rectBox.sizeDelta;
        tempMainTopInstanctObject.Add(inst_WhiteBg);
    }

    private void SetInstantClearRemove()
    {
        foreach (var item in tempMainTopInstanctObject)
            Destroy(item);

        tempMainTopInstanctObject.Clear();
    }

    void SetMessageBoxUI()
    {
        return;
        if(mType == MainType.TOP)
        {
            nowTutorialUIView = tutorialMainTopObject;
        }
        else if(mType == MainType.BOTTOM_TOP)
        {
            nowTutorialUIView = tutorialMainBottomTopObject;
        }
        else if(mType == MainType.BOTTOM)
        {
            nowTutorialUIView = tutorialMainBottomObject;
        }
    }

    private void OnClickListenerMainToBack(UnityAction call)
    {
        nowTutorialUIView.btnBack.gameObject.SetActive(call != null);
        nowTutorialUIView.btnBack.onClick.RemoveAllListeners();
        if (call != null)
        {
            nowTutorialUIView.btnBack.onClick.AddListener(SetInstantClearRemove);
            nowTutorialUIView.btnBack.onClick.AddListener(call);
        }
    }
    private void OnClickListenerMainTopNext(UnityAction call)
    {
        nowTutorialUIView.btnNext.gameObject.SetActive(true);
        nowTutorialUIView.btnReward.gameObject.SetActive(false);
        nowTutorialUIView.btnNext.onClick.RemoveAllListeners();
        if (call != null)
        {
            nowTutorialUIView.btnNext.onClick.AddListener(SetInstantClearRemove);
            nowTutorialUIView.btnNext.onClick.AddListener(call);
        }
    }
    private void OnClickListenerMainToReward(UnityAction call)
    {
        nowTutorialUIView.btnNext.gameObject.SetActive(false);
        nowTutorialUIView.btnReward.gameObject.SetActive(true);
        nowTutorialUIView.btnReward.onClick.RemoveAllListeners();
        if (call != null)
        {
            nowTutorialUIView.btnReward.onClick.AddListener(SetInstantClearRemove);
            nowTutorialUIView.btnReward.onClick.AddListener(call);
        }
    }

    private void SetMainToOnlyText(string txtVal)
    {
        nowTutorialUIView.rootOnlyText.SetActive(true);
        nowTutorialUIView.txOnlyText.text = txtVal;

        nowTutorialUIView.rootImageText.SetActive(false);
    }

    private async void SetMainToImageText(Sprite[] imgs, string[] txts)
    {
        nowTutorialUIView.rootOnlyText.SetActive(false);
        nowTutorialUIView.rootImageText.SetActive(true);
        for (int i = 0; i < nowTutorialUIView.imageLineText.Length; i++)
        {
            if (i < imgs.Length)
            {
                nowTutorialUIView.imageLineText[i].img.sprite = imgs[i] != null ? imgs[i] : SpriteAtlasMng.GetInstance().GetTransparency();

                if (txts[i] != null)
                {
                    nowTutorialUIView.imageLineText[i].txt.text = txts[i].ToString();
                    nowTutorialUIView.imageLineText[i].goRoot.SetActive(true);
                }
            }
            else
            {
                nowTutorialUIView.imageLineText[i].goRoot.SetActive(false);
            }
        }

        nowTutorialUIView.canvasGroupImageText.alpha = 0;
        nowTutorialUIView.vlgImageText.spacing = 0;
        await Task.Delay(100);
        nowTutorialUIView.vlgImageText.spacing = 5;
        nowTutorialUIView.canvasGroupImageText.alpha = 1;
    }
    #endregion


    void Awake()
    {
        trFullRoot.gameObject.SetActive(false);
        tutorialMainTopObject.goRootMsgBox.SetActive(false);
        tutorialMainBottomTopObject.goRootMsgBox.SetActive(false);
        tutorialMainBottomObject.goRootMsgBox.SetActive(false);
    }

    [Header("1. 매인 상단 UI(챕터/챕터 반복/재화/기타 기능 버튼) 설명")]
    #region ##### 매인 화면의 Top 튜토리얼 ##### 
    [SerializeField] TutorialUIView tutorialMainTopObject;
    public MainTop mainTop;
    [System.Serializable]
    [SerializeField]
    public struct MainTop
    {
        public GameObject goGoods, goStageChater, goChapterLoop, goEtcBtn, goHpBubbleDbuff;
    }

    public void FirstPlayStart()
    {
        isFirstPlay = true;
        SetStartMainTop();
    }

    /// <summary>
    /// 처음 진행시 각 UI 기능 설명 
    /// 재화 설명 UI 생성 
    /// </summary>
    public void SetStartMainTop()
    {
        mType = MainType.TOP;
        if (nowTutorialUIView.goRootMsgBox != null)  // old 
            nowTutorialUIView.goRootMsgBox.SetActive(false);

        SetMessageBoxUI();
        // new 
        nowTutorialUIView = tutorialMainTopObject;
        trFullRoot.gameObject.SetActive(true);
        nowTutorialUIView.goRootMsgBox.SetActive(true);

        SetMainToOnlyText("쌉 좀비 나이트 키우기를 찾아주셔서 감사합니다!\n먼저 간단한 매인 화면의 기능을 알아보겠습니다.\n 오래 걸리지 않으니 한번 꼭 봐주세요!~");
        OnClickListenerMainToBack(null);
        OnClickListenerMainTopNext(SetMainTop_Goods);
    }

    /// <summary>
    /// 1
    /// </summary>
    void SetMainTop_Goods()
    {
        mType = MainType.TOP;
        SetMessageBoxUI();
        Sprite[] sprs = new Sprite[]
        {
            SpriteAtlasMng.GetInstance ().GetSprite("goods_ruby"),
            SpriteAtlasMng.GetInstance ().GetSprite("goods_ether"),
            SpriteAtlasMng.GetInstance ().GetSprite("goods_blue_dia"),
            SpriteAtlasMng.GetInstance ().GetSprite("goods_white_dia_TBC"),
            SpriteAtlasMng.GetInstance ().GetSprite("goods_gold"),
        };
        string[] txts = new string[]
        {
            "<color=#FF0080>루비</color>는 <color=#00FFFF>[인벤토리]->[판매/분해]</color>를 눌러서 분해를 하면 획득할 수 있습니다.",
            "<color=#00FF1E>에테르</color>는 <color=#00FFFF>[인벤토리]->[판매/분해]</color>를 눌러서 분해를 하면 획득할 수 있습니다.",
            "<color=#00FFFF>블루 다이아</color>는 게임내 여러 활동으로 획득할 수있는 재화입니다.",
            "<color=#FF0080>레드 다이아</color>는 <color=#00FFFF>[상점]->[다이아 상점]</color>에서 구매할 수 있는 재화입니다.",
            "<color=#FFFF00>골드</color>는 몬스터 처치, 퀘스트 완료 보상, <color=#00FFFF>[상점]->[아이템 상점]</color>에서 구매, <color=#00FFFF>[인벤토리]->[판매/분해]</color>를 눌러서 판매하면 획득할 수 있습니다.",
        };
        SetMainToImageText(sprs, txts);
        SetCopyObjectNowNowTutorialBox(nowTutorialUIView.goTutorialBox, false);
        SetCopyObjectSetting(mainTop.goGoods, true, true);
        OnClickListenerMainToBack(SetStartMainTop);
        OnClickListenerMainTopNext(SetMainTop_ChapterStage);
    }

    /// <summary>
    /// 2
    /// </summary>
    void SetMainTop_ChapterStage()
    {
        mType = MainType.TOP;
        SetMessageBoxUI();
        Sprite[] sprs = new Sprite[]
        {
            null,
            null,
            SpriteAtlasMng.GetInstance ().GetSprite("tutorial_chapter_mode_default"),
            SpriteAtlasMng.GetInstance ().GetSprite("tutorial_chapter_mode_boss"),
        };
        string[] txts = new string[]
        {
            "           <color=yellow>챕터</color>는 각 스테이지를 완료하면 <color=yellow>1씩</color> 증가합니다.",
            "           챕터가 높을 수록 장비의 <color=yellow>드랍률이 증가</color>하며, 퀘스트 완료 <color=yellow>골드</color> 획득량이 증가합니다.",
            "<color=#00FFFF>[일반 모드중]</color>은 일반몬스터를 9마리 처치하여야 보스와 상대할 수 있습니다. <color=#00FFFF>[일반 모드중]</color>상태에서 버튼을 누르면 <color=#00FFFF>[보스 모드]</color>로 변경됩니다.",
            "<color=#00FFFF>[보스 모드중]</color>은 스테이지의 보스를 상대합니다.<color=#00FFFF>[보스 모드중]</color>상태에서 버튼을 누르면 <color=#00FFFF>[일반 모드]</color>로 변경됩니다.",
        };
        SetMainToImageText(sprs, txts);
        SetCopyObjectSetting(mainTop.goStageChater, false, true);
        OnClickListenerMainToBack(SetMainTop_Goods);
        OnClickListenerMainTopNext(SetMainTop_ChapterChapterStageLoop);
    }

    /// <summary>
    /// 3
    /// </summary>
    void SetMainTop_ChapterChapterStageLoop()
    {
        mType = MainType.TOP;
        SetMessageBoxUI();
        Sprite[] sprs = new Sprite[]
        {
            SpriteAtlasMng.GetInstance ().GetSprite("tutorial_chapterloop_drop"),
            SpriteAtlasMng.GetInstance ().GetSprite("tutorial_chapterloop_combat"),
            SpriteAtlasMng.GetInstance ().GetSprite("tutorial_chapterloop_btn"),
        };
        string[] txts = new string[]
        {
            "현재 진행중인 챕터 스테이지에서의 각 장비별 <color=yellow>드랍률</color>이 표시됩니다.",
            "현재 진행중인 챕터 스테이지를 클리어하는데 필요한 <color=yellow>전투력</color>이 표시됩니다.",
            "모든 챕터 정보를 확인할 수있으며, 원하는 챕터를 <color=yellow>반복</color>으로 진행하도록하여 <color=yellow>장비를 빠르게 획득</color>할 수 있습니다.",
        };
        SetMainToImageText(sprs, txts);
        SetCopyObjectSetting(mainTop.goChapterLoop, false, true);
        OnClickListenerMainToBack(SetMainTop_ChapterStage);
        OnClickListenerMainTopNext(SetMainTop_HpBubbleDbuff);
    }

    /// <summary>
    /// 4
    /// </summary>
    void SetMainTop_HpBubbleDbuff()
    {
        mType = MainType.TOP;
        SetMessageBoxUI();
        Sprite[] sprs = new Sprite[]
        {
            SpriteAtlasMng.GetInstance ().GetSprite("tutorial_bubble"),
            SpriteAtlasMng.GetInstance ().GetSprite("tutorial_dbf1"),
            SpriteAtlasMng.GetInstance ().GetSprite("tutorial_dbf2"),
            SpriteAtlasMng.GetInstance ().GetSprite("tutorial_hp_bar"),
        };
        string[] txts = new string[]
        {
            "일반 공격을 할때마다 <color=yellow>1개씩 채워지며</color>, 대기중인 스킬의 포인트만큼 채워지면 해당 <color=yellow>스킬이 발동</color>됩니다.",
            "<color=yellow>버프/디버프 스킬 발동</color>시 표시되며, 아이콘의 <color=yellow>노란 동그라미는 자신의 공격 동안 이 스킬이 유지</color>된다는 뜻이고, 숫자는 자신의 몇회 공격 동안을 의미합니다.",
            "<color=yellow>버프/디버프 스킬 발동</color>시 표시되며, 아이콘의 <color=red>빨간 동그라미는 상대의 공격 동안 이 스킬이 유지</color>된다는 뜻이고, 숫자는 상대의 몇회 공격 동안을 의미합니다.",
            "현재 HP상태를 보여줍니다. <color=yellow>[남은 체력 / 최대 체력]</color>\n상대 체력이 0이 되면 <color=yellow>승리</color>하게 되고, 내 체력이 0이되면 <color=red>패배</color>하게 됩니다..",
        };
        SetMainToImageText(sprs, txts);
        SetCopyObjectSetting(mainTop.goHpBubbleDbuff, false, true);
        OnClickListenerMainToBack(SetMainTop_ChapterChapterStageLoop);
        OnClickListenerMainTopNext(SetMainTop_EtcBtn);
    }

    /// <summary>
    /// 5
    /// </summary>
    void SetMainTop_EtcBtn()
    {
        mType = MainType.TOP;
        SetMessageBoxUI();
        Sprite[] sprs = new Sprite[]
        {
            SpriteAtlasMng.GetInstance ().GetSprite("cog"),
            SpriteAtlasMng.GetInstance ().GetSprite("trophy-cup"),
            SpriteAtlasMng.GetInstance ().GetSprite("envelope"),
            SpriteAtlasMng.GetInstance ().GetSprite("night-sleep"),
            SpriteAtlasMng.GetInstance ().GetSprite("calendar"),
            SpriteAtlasMng.GetInstance ().GetSprite("achievement"),
            SpriteAtlasMng.GetInstance ().GetSprite("bookmarklet"),
            SpriteAtlasMng.GetInstance ().GetSprite("present"),
        };
        string[] txts = new string[]
        {
            "설정 : 게임 내 기능들을 설정 및 확인 할 수 있습니다.",
            "랭킹 : 유저들의 챕터 스테이지의 랭킹 및 유저 정보를 확인 할 수 있습니다.",
            "우편함 : 접속 보상, 이벤트 보상, 유저간 우편(추후)기능으로 보상품을 획득 할 수 있습니다.",
            "절전 모드 : 화면을 끈 상태로 게임이 진행되며 배터리를 절약을 할 수 있습니다.",
            "출석부 : 일일 출석 보상으로 다양한 아이템을 획득할 수 있습니다.",
            "미션/업적 : 일일 미션과 업적을 통하여 재화를 획득할 수 있습니다.",
            "장비 도감 강화 : 장비를 획득할 때마다 각 장비 카운트가 추가되어 장비 도감을 강화하여 강력해질 수 있습니다.",
            "데일리 : 상품을 구매시 보상품을 획득할 수 있습니다.",
        };
        SetMainToImageText(sprs, txts);
        SetCopyObjectSetting(mainTop.goEtcBtn, false, true);
        OnClickListenerMainToBack(SetMainTop_HpBubbleDbuff);
        OnClickListenerMainTopNext(SetMainTop_End);
    }

    /// <summary>
    /// 6
    /// </summary>
    void SetMainTop_End ()
    {
        mType = MainType.TOP;
        SetMessageBoxUI();
        SetMainToOnlyText("간단한 매인 화면의 기능을 알아보았습니다.");
        OnClickListenerMainToBack(SetMainTop_EtcBtn);
        OnClickListenerMainTopNext(SetStartMainBottomTop);
    }
    #endregion

    [Header("2. 매인 중간 상단 UI (스킬카드/편의기능) 설명")]
    #region ##### 매인 화면의 BottomTop 튜토리얼 #####
    [SerializeField] TutorialUIView tutorialMainBottomTopObject;
    public MainBottomTop mainBottomTop;
    [System.Serializable]
    [SerializeField]
    public struct MainBottomTop
    {
        public GameObject goSkillUserFunction, goAutoSale, goAutoPotion, goSkillUse, goGameSpeed, goMySkillCard;
    }

    /// <summary>
    /// 1
    /// </summary>
    public void SetStartMainBottomTop()
    {
        mType = MainType.BOTTOM_TOP;
        if (nowTutorialUIView.goRootMsgBox != null)  // old 
            nowTutorialUIView.goRootMsgBox.SetActive(false);

        SetMessageBoxUI();
        // new 
        nowTutorialUIView = tutorialMainBottomTopObject;
        trFullRoot.gameObject.SetActive(true);
        nowTutorialUIView.goRootMsgBox.SetActive(true);

        SetMainToOnlyText("이번엔 스킬과 편의 기능에 대해서 알아보겠습니다."); // 먼저 간단한 매인 화면의 기능을 알아보겠습니다.
        SetCopyObjectSetting(mainBottomTop.goSkillUserFunction, false, false);
        SetCopyObjectNowNowTutorialBox(nowTutorialUIView.goTutorialBox, false);

        OnClickListenerMainToBack(null);
        OnClickListenerMainTopNext(SetMainBottomTop_SkillUserFunction);
    }

    /// <summary>
    /// 2
    /// </summary>
    void SetMainBottomTop_SkillUserFunction()
    {
        mType = MainType.BOTTOM_TOP;
        SetMessageBoxUI();
        Sprite[] sprs = new Sprite[]
        {
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_card_wait"),
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_card_bubble"),
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_card_R"),
        };
        string[] txts = new string[]
        {
             "장착한 스킬 카드에 불이 들어와있는 카드는 <color=yellow>발동 대기중인 카드</color>입니다.",
             "상대방을 공격 성공할때 마다 1개씩 발동 포인트가 쌓이고, 모두 채워지면 <color=yellow>자동으로 발동</color>됩니다.",
             "<color=red>(R)</color> 표시는 랜덤으로 장착된 카드를 표시해줍니다. 스킬은 <color=#00FFFF>[스킬]</color>에서 착용할 수 있으며, 만약 스킬 슬롯을 모두 비워놓을 경우 보유중인 스킬 카드중에서 랜덤<color=red>(R)</color>으로 매번 자동 장착됩니다."
        };
        SetMainToImageText(sprs, txts);
        SetCopyObjectSetting(mainBottomTop.goMySkillCard, false, true);
        OnClickListenerMainToBack(SetStartMainBottomTop);
        OnClickListenerMainTopNext(SetMainBottomTop_SkillCard);
    }

    /// <summary>
    /// 3
    /// </summary>
    void SetMainBottomTop_SkillCard()
    {
        mType = MainType.BOTTOM_TOP;
        SetMessageBoxUI();
        Sprite[] sprs = new Sprite[]
        {
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_auto_sale_off1"),
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_auto_sale_off2"),
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_auto_sale_on"),

        };
        string[] txts = new string[]
        {
            "장비 자동 판매(분해) OFF : 사용 가능한 시간이 없을때 버튼 상태입니다.",
            "장비 자동 판매(분해) OFF : 판매(분해) 기능을 중지한 상태이며, 클릭시 ON상태로 변경할 수 있습니다.",
            "장비 자동 판매(분해) ON : <color=yellow>판매(분해) 기능을 사용중인 상태</color>이며, 클릭시 OFF상태로 변경할 수 있습니다.",
        };
        SetMainToImageText(sprs, txts);
        SetCopyObjectSetting(mainBottomTop.goAutoSale, false, true);
        OnClickListenerMainToBack(SetMainBottomTop_SkillUserFunction);
        OnClickListenerMainTopNext(SetMainBottomTop_Potion);
    }

    void SetMainBottomTop_Potion()
    {
        mType = MainType.BOTTOM_TOP;
        SetMessageBoxUI();
        Sprite[] sprs = new Sprite[]
        {
            SpriteAtlasMng.GetInstance ().GetSprite("tutorial_auto_potion_off"),
            SpriteAtlasMng.GetInstance ().GetSprite("tutorial_auto_potion_on"),
            SpriteAtlasMng.GetInstance ().GetSprite("tutorial_auto_potion_on_time"),
        };
        string[] txts = new string[]
        {
            "물약 사용 OFF : 물약 사용을 중지한 상태이며, 클릭시 ON상태로 변경할 수 있습니다.",
            "물약 사용 ON : <color=yellow>물약을 사용중인 상태</color>이며, 클릭시 OFF상태로 변경할 수 있습니다.",
            "물약 재사용 시간 : 물약을 사용중일때 <color=yellow>다음 사용될 시간</color>입니다.",
        };
        SetMainToImageText(sprs, txts);
        SetCopyObjectSetting(mainBottomTop.goAutoPotion, false, true);
        OnClickListenerMainToBack(SetMainBottomTop_SkillCard);
        OnClickListenerMainTopNext(SetMainBottomTop_SkillUse);
    }

    void SetMainBottomTop_SkillUse()
    {
        mType = MainType.BOTTOM_TOP;
        SetMessageBoxUI();
        Sprite[] sprs = new Sprite[]
        {
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_skill_off"),
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_skill_on"),

        };
        string[] txts = new string[]
        {
            "스킬 사용 OFF : 스킬 발동을 중지한 상태입니다. 일반 공격만 하게됩니다.",
            "스킬 사용 ON : <color=yellow>스킬 발동을 사용중인 상태</color>입니다.",
        };
        SetMainToImageText(sprs, txts);
        SetCopyObjectSetting(mainBottomTop.goSkillUse, false, true);
        OnClickListenerMainToBack(SetMainBottomTop_Potion);
        OnClickListenerMainTopNext(SetMainBottomTop_GameSpeed);
    }

    void SetMainBottomTop_GameSpeed()
    {
        mType = MainType.BOTTOM_TOP;
        SetMessageBoxUI();
        Sprite[] sprs = new Sprite[]
        {
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_speed_x2_off"),
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_speed_x2_on"),

        };
        string[] txts = new string[]
        {
            "게임 속도 x2 OFF : 게임의 진행 속도를 기본 속도로 진행합니다.",
            "게임 속도 x2 ON : 게임의 진행 속도를 <color=yellow>x2배</color> 속도로 진행합니다.",
        };
        SetMainToImageText(sprs, txts);
        SetCopyObjectSetting(mainBottomTop.goGameSpeed, false, true);
        OnClickListenerMainToBack(SetMainBottomTop_SkillUse);
        OnClickListenerMainTopNext(SetMainBottomTop_End);
    }

    void SetMainBottomTop_End()
    {
        mType = MainType.BOTTOM_TOP;
        SetMessageBoxUI();
        SetMainToOnlyText("스킬과 편의 기능에 대해서 알아보았습니다.");
        OnClickListenerMainToBack(SetMainBottomTop_GameSpeed);
        OnClickListenerMainTopNext(SetStartMainBottom);
    }
    #endregion

    [Header("3. 매인 하단 UI(퀘스트/채팅/하단 탭 매뉴 버튼) 설명")]
    #region ##### 매인 화면의 Bottom 튜토리얼 ##### 
    [SerializeField] TutorialUIView tutorialMainBottomObject;
    public MainBottom mainBottom;
    [System.Serializable]
    [SerializeField]
    public struct MainBottom
    {
        public GameObject goBottomTap1, goBottomTap2, goQuestCell, goBottomTapBtns, goBtnQuestChat, goBtnInventory, goBtnSmyth, goBtnSkill, goBtnContents, goBtnShop;
    }

    /// <summary>
    /// 1
    /// </summary>
    public void SetStartMainBottom()
    {
        mType = MainType.BOTTOM;
        if (nowTutorialUIView.goRootMsgBox != null)  // old 
            nowTutorialUIView.goRootMsgBox.SetActive(false);

        SetMessageBoxUI();
        // new 
        nowTutorialUIView = tutorialMainBottomObject;
        trFullRoot.gameObject.SetActive(true);
        nowTutorialUIView.goRootMsgBox.SetActive(true);
        
        SetMainToOnlyText("이번엔 퀘스트와 하단의 버튼에 대해서 알아보겠습니다."); // 먼저 간단한 매인 화면의 기능을 알아보겠습니다.
        SetCopyObjectSetting(mainBottom.goBottomTap1, false, false);
        SetCopyObjectSetting(mainBottom.goBottomTap2, false, false);
        SetCopyObjectNowNowTutorialBox(nowTutorialUIView.goTutorialBox, false);

        OnClickListenerMainToBack(null);
        OnClickListenerMainTopNext(SetMainBottom_QuestQuestCell);
    }

    /// <summary>
    /// 2
    /// </summary>
    void SetMainBottom_QuestQuestCell()
    {
        mType = MainType.BOTTOM;
        SetMessageBoxUI();
        Sprite[] sprs = new Sprite[]
        {
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_main_bottom_a1"),
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_main_bottom_a2"),
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_main_bottom_a3"),
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_main_bottom_a4"),
        };
        string[] txts = new string[]
        {
            "퀘스트의 번호와, 레벨이 표시됩니다. 현재 퀘스트 Lv.100 달성해야 다음 퀘스트의 레벨을 올릴 수 있습니다.",
            "퀘스트의 시간 게이지가 완료되면 보상으로 골드를 획득합니다.",
            "레벨업에 필요한 비용이 충족되면 버튼이 활성화 됩니다.",
            "레벨업에 필요한 비용이 부족 할 경우에는 버튼이 비활성화 됩니다.", 
        };
        SetMainToImageText(sprs, txts);
        SetCopyObjectSetting(mainBottom.goQuestCell, false, true, true);
        OnClickListenerMainToBack(SetStartMainBottom);
        OnClickListenerMainTopNext(SetMainBottom_TapButtons1);
    }

    /// <summary>
    /// 3
    /// </summary>
    void SetMainBottom_TapButtons1()
    {
        mType = MainType.BOTTOM;
        SetMessageBoxUI();
        Sprite[] sprs = new Sprite[]
        {
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_main_bottom_b1"),
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_main_bottom_b2"),
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_main_bottom_b3"),
        };
        string[] txts = new string[]
        {
            "<color=#00FFFF>[퀘스트/채팅]</color> : 퀘스트와 채팅을 할 수 있는 탭입니다.",
            "<color=#00FFFF>[인벤토리]</color> : 장비/장신구/아이템이 보관되는 탭이며, 좀비의 스탯을 올리고, 드랍된 장비를 확인 및 교체 할 수 있습니다.",
            "<color=#00FFFF>[대장간]</color> : 장비를 강화, 강화석 진화, 강화 레벨 전승, 장신구 옵션 변경, 장신구 합성을 할 수 있습니다.",
        };
        SetMainToImageText(sprs, txts);
        SetCopyObjectSetting(mainBottom.goBottomTapBtns, false, true);
        OnClickListenerMainToBack(SetMainBottom_QuestQuestCell);
        OnClickListenerMainTopNext(SetMainBottom_TapButtons2);
    }

    /// <summary>
    /// 4
    /// </summary>
    void SetMainBottom_TapButtons2()
    {
        mType = MainType.BOTTOM;
        SetMessageBoxUI();
        Sprite[] sprs = new Sprite[]
        {
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_main_bottom_b4"),
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_main_bottom_b5"),
             SpriteAtlasMng.GetInstance ().GetSprite("tutorial_main_bottom_b6"),
        };
        string[] txts = new string[]
        {
            "<color=#00FFFF>[스킬]</color> : 각종 스킬들을 레벨업 및 장착을 할 수 있습니다.",
            "<color=#00FFFF>[컨텐츠]</color> : 던전 입장과 아레나에서 유저와 대결을 할 수 있습니다.",
            "<color=#00FFFF>[상점]</color> : 여러가지 아이템을 구매할 수 있습니다.",
        };
        SetMainToImageText(sprs, txts);
        SetCopyObjectSetting(mainBottom.goBottomTapBtns, false, true);
        OnClickListenerMainToBack(SetMainBottom_TapButtons1);
        OnClickListenerMainTopNext(SetMainBottom_End);
    }

    void SetMainBottom_End()
    {
        mType = MainType.BOTTOM;
        SetMessageBoxUI();
        if (isFirstPlay)
        {
            SetMainToOnlyText("<color=#00FFFF>보상 받기</color> 버튼을 누르시면 보상으로 다이아 x1000개가 지급됩니다.");
            OnClickListenerMainToBack(SetMainBottom_TapButtons2);
            OnClickListenerMainToReward(SetMainBottom_Reward);
        }
        else
        {
            if (goNowTutorialTapBox != null)
                Destroy(goNowTutorialTapBox);

            isFirstPlay = false;
            trFullRoot.gameObject.SetActive(false);
        }
    }

    void SetMainBottom_Reward()
    {
        mType = MainType.BOTTOM;
        SetMessageBoxUI();
        if (goNowTutorialTapBox != null)
            Destroy(goNowTutorialTapBox);

        if (isFirstPlay)
        {
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            goods_db.m_dia += 1000;
            GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("보상으로 다이아 x1000개가 지급되었습니다.");
            InitGame.GetInstance().Attend(false);
        }

        isFirstPlay = false;
        trFullRoot.gameObject.SetActive(false);
    }
    #endregion



}
