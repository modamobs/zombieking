using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BackEnd;
using static BackEnd.BackendAsyncClass;
using System.Threading.Tasks;
using System;
using UnityEngine.Events;

public enum popupnotice_enum
{
    e0Default = 0,
    /// <summary> 텝-대장간 : 장신구 합성 (착용중인 장신구를 합성하려고 선택했을때 해제하고 등독하겠냐는 팝업) </summary> 
    e1Tap_Smithy__AcceSyntRelsToEnrol,
    /// <summary> 탭-인벤토리 : 인벤토리 확장 (인벤토리의 확장 버튼을 눌렀을때 인벤토리를 확장시키겠냐는 팝업) </summary>
    e2Tap_Inventory__IvenEpns,
}

public class PopUpNotice : MonoBehaviour
{
    [SerializeField] popupnotice_enum n_enum = popupnotice_enum.e0Default;
    [SerializeField] GameObject go_Box, go_Line;
    public Button button_No, button_Ok, button_BkgClose;
    [SerializeField] Text text_BtnNo, text_BtnOk, text_BoxSubject, text_BoxContents, text_LineContents;
    [SerializeField] Animation ani_PopUpLine, ani_DropBox;
    [SerializeField] Image imLineBox;

    /// <summary>
    /// str_BtnCancel:취소, str_BtnClose:닫기, str_BtnConfirm:확인, str_BtnEnrollment:등록
    /// </summary>
    string str_BtnCancel, str_BtnClose, str_BtnConfirm, str_BtnEnrollment;

    // drop 
    Queue<GameDatabase.TableDB.Equipment> dropEquipQ = new Queue<GameDatabase.TableDB.Equipment>();
    Queue<EquipSale> dropEquipSaleQ = new Queue<EquipSale>();
    struct EquipSale
    {
        public bool isSaleOrDecomp;
        public bool isEqAcce;
        public int eqTy, eqRt, eqId, rwdCnt;
        public string txt;
    }
    Queue<GameDatabase.TableDB.Item> dropItemQ = new Queue<GameDatabase.TableDB.Item>();
    Queue<GameDatabase.TableDB.Skill> dropSkillQ = new Queue<GameDatabase.TableDB.Skill>();
    Queue<int[]> dropGoldQ = new Queue<int[]>();
    [SerializeField] GameObject go_ItemDropBox;
    [SerializeField] Image image_DropIcon;
    [SerializeField] Image image_DropRatingBg;
    [SerializeField] Image image_DropRatingLine;
    [SerializeField] Text text_DropContents;

    string str_equip, str_acce, str_item, str_skill, str_gold;

    void Awake ()
    {
        str_BtnCancel = LanguageGameData.GetInstance().GetString("notice.btn.name.cancel"); // 취소 
        str_BtnConfirm = LanguageGameData.GetInstance().GetString("notice.btn.name.confirm");// 확인
        str_BtnClose = LanguageGameData.GetInstance().GetString("notice.btn.name.close"); // 닫기 
        str_BtnEnrollment = LanguageGameData.GetInstance().GetString("notice.btn.name.enrollment"); // 등록 
        str_equip = LanguageGameData.GetInstance().GetString("text.equip");
        str_acce = LanguageGameData.GetInstance().GetString("text.acce");
        str_item = LanguageGameData.GetInstance().GetString("text.item");
        str_skill = LanguageGameData.GetInstance().GetString("text.skill");
        str_gold = LanguageGameData.GetInstance().GetString("text.coin");
        go_ItemDropBox.gameObject.SetActive(false);
    }

    void OnDisabale ()
    {
        n_enum = popupnotice_enum.e0Default;
    }

    // ##################################################
    // ##################################################
    #region ##### 드롭 알림 #####
    /// <summary>
    /// 장비 드롭 알림 
    /// </summary>
    public void DropViewEquipQueue(GameDatabase.TableDB.Equipment dropEquip)
    {
        dropEquipQ.Enqueue(dropEquip);
        if(!go_ItemDropBox.activeSelf)
        {
            DropViewEquip();
        }
    }

    /// <summary>
    /// 아이템 드롭 알림 
    /// </summary>
    public void DropViewItemQueue(GameDatabase.TableDB.Item dropItem)
    {
        dropItemQ.Enqueue(dropItem);
        if (!go_ItemDropBox.activeSelf)
        {
            DropViewItem();
        }
    }

    /// <summary>
    /// 스킬 드롭 알림 
    /// </summary>
    public void DropViewSkillQueue(GameDatabase.TableDB.Skill dropSkill)
    {
        dropSkillQ.Enqueue(dropSkill);
        if (!go_ItemDropBox.activeSelf)
        {
            DropViewSkill();
        }
    }

    /// <summary>
    /// 골드 드롭 알림 
    /// </summary>
    public void SetDropGoldQueue(int drop_gold, int bns_gold)
    {
        dropGoldQ.Enqueue(new int[] { drop_gold + bns_gold, bns_gold });
        if (!go_ItemDropBox.activeSelf)
        {
            DropViewGold();
        }
    }

    public void DropSaleDecompositionQueue(bool isSaleOrDecomp, bool isAcOrEq, int eqTy, int eqRt, int eqId, int rwdCnt)
    {
        string rtName = GameDatabase.StringFormat.GetStringRating(eqRt);
        string corHex = ResourceDatabase.GetInstance().GetItemHexColor(eqRt);
        string txtStr = "";
        if (isSaleOrDecomp) // 판매 
        {
            if (isAcOrEq)
                txtStr = string.Format("<color={0}>[{1} 장신구 판매]</color> 골드 +{2:#,0} 획득", corHex, rtName, rwdCnt);
            else txtStr = string.Format("<color={0}>[{1} 장비 판매]</color> 골드 +{2:#,0} 획득", corHex, rtName, rwdCnt);
        }
        else // 분해 
        {
            if (isAcOrEq)
                txtStr = string.Format("<color={0}>[{1} 장신구 분해]</color> <color=#00FF1E>에테르 +{2}</color>획득", corHex, rtName, rwdCnt);
            else txtStr = string.Format("<color={0}>[{1} 장비 분해]</color> <color=#FF0000>루비 +{2}</color>획득", corHex, rtName, rwdCnt);
        }

        if(!string.Equals(txtStr, ""))
        {
            dropEquipSaleQ.Enqueue(new EquipSale() { isSaleOrDecomp = isSaleOrDecomp, isEqAcce = isAcOrEq, eqTy = eqTy, eqRt = eqRt, eqId = eqId, rwdCnt = rwdCnt, txt = txtStr });
            if (!go_ItemDropBox.activeSelf)
            {
                DropViewEquipSaleDecomposition();
            }
        }
    }

    private void DropViewEquipSaleDecomposition()
    {
        if(dropEquipSaleQ.Count > 0)
        {
            var eqSale = dropEquipSaleQ.Dequeue();
            image_DropIcon.sprite = eqSale.isSaleOrDecomp == true ? SpriteAtlasMng.GetInstance ().GetSpriteGold() : eqSale.isEqAcce == true ? SpriteAtlasMng.GetInstance().GetSpriteEther() : SpriteAtlasMng.GetInstance().GetSpriteRuby();
            image_DropRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
            image_DropRatingLine.color = ResourceDatabase.GetInstance().GetItemColor(eqSale.eqRt);
            text_DropContents.text = eqSale.txt;
            SetOpenDropViewBox();
        }
    }

    /// <summary>
    /// 아이템 획득시 사용 
    /// </summary>
    private void DropViewEquip()
    {
        if(dropEquipQ.Count > 0)
        {
            var eq_data = dropEquipQ.Dequeue();
            string eqName = GameDatabase.StringFormat.GetEquipName(eq_data.eq_ty, eq_data.eq_rt, eq_data.eq_id);
            string rtName = GameDatabase.StringFormat.GetStringRating(eq_data.eq_rt);
            string corHex = ResourceDatabase.GetInstance().GetItemHexColor(eq_data.eq_rt);
            image_DropIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(eq_data.eq_ty, eq_data.eq_rt, eq_data.eq_id);
            image_DropRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(eq_data.eq_rt);
            image_DropRatingLine.color = ResourceDatabase.GetInstance().GetItemColor(eq_data.eq_rt);
            if (GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(eq_data.eq_rt))
                text_DropContents.text = string.Format("<color={0}>[{1} 장신구 획득] {2}</color>", corHex, rtName, eqName);
            else
                text_DropContents.text = string.Format("<color={0}>[{1} 장비 획득] {2}</color>", corHex, rtName, eqName);
            SetOpenDropViewBox();
        }
    }
    private void DropViewItem()
    {
        if(dropItemQ.Count > 0)
        {
            var it_data = dropItemQ.Dequeue();
            string itName = LanguageGameData.GetInstance().GetString(string.Format("item.name.item_{0}_{1}", it_data.type, it_data.rating));
            string corHex = ResourceDatabase.GetInstance().GetItemHexColor(it_data.rating);
            image_DropIcon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(it_data.type, it_data.rating);
            image_DropRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(it_data.rating);
            image_DropRatingLine.color = ResourceDatabase.GetInstance().GetItemColor(it_data.rating);
            text_DropContents.text = string.Format("<color={0}>[아이템 획득] {1}</color>", corHex, itName);
            SetOpenDropViewBox();
        }
    }

    private void DropViewSkill ()
    {
        if (dropSkillQ.Count > 0)
        {
            var sk_data = dropSkillQ.Dequeue();
            string skName = LanguageGameData.GetInstance().GetString(string.Format("skill.name_{0}", sk_data.idx));
            string corHex = ResourceDatabase.GetInstance().GetItemHexColor(0);
            image_DropIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteSkill(sk_data.idx);
            image_DropRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(sk_data.cliend_rating);
            image_DropRatingLine.color = ResourceDatabase.GetInstance().GetItemColor(sk_data.cliend_rating);
            text_DropContents.text = string.Format("<color={0}>[스킬 획득] {1}</color>", corHex, skName);
            SetOpenDropViewBox();
        }
    }

    private void DropViewGold ()
    {
        if(dropGoldQ.Count > 0)
        {
            var gd_data = dropGoldQ.Dequeue();
            int drop_total_gold = gd_data[0];
            int bns_gold = gd_data[1];
            string corHex = ResourceDatabase.GetInstance().GetItemHexColor(0);
            image_DropIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteGold();
            image_DropRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
            image_DropRatingLine.color = ResourceDatabase.GetInstance().GetItemColor(0);
            if(bns_gold > 0)
                text_DropContents.text = string.Format("<color={0}>[골드 획득] {1:#,0}</color> <color=yellow>(+{2:#,0})</color>", corHex, drop_total_gold, bns_gold);
            else
                text_DropContents.text = string.Format("<color={0}>[골드 획득] {1:#,0}</color>", corHex, drop_total_gold);
            SetOpenDropViewBox();
        }
    }

    private void SetOpenDropViewBox ()
    {
        go_ItemDropBox.SetActive(true);
        ani_DropBox.Stop();
        ani_DropBox.Play("UIDropNotice");
        StopCoroutine
            ("DropBoxClose");
        StartCoroutine
            ("DropBoxClose");
    }

    private IEnumerator DropBoxClose ()
    {
        yield return null;
        while (ani_DropBox.IsPlaying("UIDropNotice"))
            yield return null;

        yield return null;
        if (dropEquipQ.Count > 0)
        {
            DropViewEquip();
        }
        else if(dropEquipSaleQ.Count > 0)
        {
            DropViewEquipSaleDecomposition();
        }
        else if (dropItemQ.Count > 0)
        {
            DropViewItem();
        }
        else if (dropSkillQ.Count > 0)
        {
            DropViewSkill();
        }
        else if (dropGoldQ.Count > 0)
        {
            DropViewGold();
        }
        else
        {
            go_ItemDropBox.SetActive(false);
        }
    }
    #endregion

    // ##################################################
    // ##################################################
    #region ##### 1줄짜리 알림 및 경고 메시지 #####
    Queue<OneLineMsgType> msg = new Queue<OneLineMsgType>();
    struct OneLineMsgType
    {
        public string msg;
        public Color cor;
    }

    /// <summary>
    /// 1줄짜리 알림 : 주로 경고 메시지 
    /// </summary>
    public void SetEnqueueOneMsg(string str_id, Color col)
    {
        msg.Enqueue(new OneLineMsgType() { msg = LanguageSystemData.GetInstance().GetString(str_id), cor = col });
        if(go_Line.activeSelf == false)
        {
            NotifMsgDequeue();
        }
    }

    public void SetOneMsg(string str_id, Color col) => NotifMsgDequeue(str_id, col);

    void NotifMsgDequeue()
    {
        var msgType = msg.Dequeue();
        text_LineContents.text = msgType.msg;
        text_LineContents.color = msgType.cor;
        imLineBox.color = msgType.cor;

        go_Line.SetActive(true);
        ani_PopUpLine.Stop();
        ani_PopUpLine.Play("UINoticePopUpLine");

        StopCoroutine("LineClose");
        StartCoroutine("LineClose");
    }

    void NotifMsgDequeue(string msg, Color co)
    {
        text_LineContents.text = msg;
        text_LineContents.color = co;
        imLineBox.color = co;

        go_Line.SetActive(true);
        ani_PopUpLine.Stop();
        ani_PopUpLine.Play("UINoticePopUpLine");

        StopCoroutine("LineClose");
        StartCoroutine("LineClose");
    }

    private IEnumerator LineClose ()
    {
        yield return null;
        while (ani_PopUpLine.IsPlaying("UINoticePopUpLine"))
            yield return null;

        PopUpMng.GetInstance().LastMsgReset();
        go_Line.SetActive(false);

        yield return null;
        if(msg.Count > 0)
        {
            NotifMsgDequeue();
        }
    }

    public void CloseNotifMsg()
    {
        StopCoroutine("LineClose");
        PopUpMng.GetInstance().LastMsgReset();
        go_Line.SetActive(false);
    }
    #endregion


    // ##################################################
    // ##################################################
    #region ##### 팝업 알림 및 메시지, 확인창 #####
    /// <summary>
    /// 팝업 박스 알림 : 선택해야 할 액션이 필요한 경우 
    /// </summary>
    public void OpenPopUpBox(popupnotice_enum _enum, string _text = "")
    {
        n_enum = _enum;

        go_Box.SetActive(true);
        switch (n_enum)
        {
            case popupnotice_enum.e1Tap_Smithy__AcceSyntRelsToEnrol:
                text_BtnNo.text = str_BtnCancel;
                text_BtnOk.text = str_BtnEnrollment;
                break;
            default:
                text_BtnNo.text = str_BtnCancel;
                text_BtnOk.text = str_BtnConfirm;
                break;
        }

        SetBoxText(n_enum, _text);
    }

    /// <summary>
    /// 알림 확인 닫기 박스 팝업 
    /// </summary>
    public void OpenCloseConmfirmNoticeBox(string _txt, UnityAction btnCloseAction = null)
    {
        text_BoxContents.text = _txt; // 내용 
        button_Ok.onClick.RemoveAllListeners();
        button_BkgClose.onClick.RemoveAllListeners();

        if (btnCloseAction != null)
        {
            button_Ok.onClick.AddListener(btnCloseAction);
            if(btnCloseAction == Application.Quit)
            {
                button_BkgClose.onClick.AddListener(btnCloseAction);
            }
        }

        button_No.gameObject.SetActive(false);
        button_Ok.gameObject.SetActive(true);
        go_Box.SetActive(true);
    }

    /// <summary>
    /// 일반 알림 팝업 
    /// </summary>
    public void OpenAskNoticeBoxListener(string _txt, UnityAction btnConfirmAction, UnityAction btnCancelAction = null, bool isOnCancel = true, bool isConfirm = true)
    {
        button_Ok.onClick.RemoveAllListeners();
        button_Ok.onClick.AddListener(btnConfirmAction);

        LogPrint.Print("btnCancel : " + btnCancelAction + ", is null ? : " + (btnCancelAction == null));
        if(btnCancelAction != null)
        {
            button_No.onClick.RemoveAllListeners();
            button_No.onClick.AddListener(btnCancelAction);
        }

        text_BoxContents.text = _txt; // 내용 
        text_BtnNo.text = str_BtnCancel;
        text_BtnOk.text = str_BtnConfirm;
        button_No.gameObject.SetActive(isOnCancel);
        button_Ok.gameObject.SetActive(isConfirm);
        go_Box.SetActive(true);
    }

    public void ClosePopUpBox()
    {
        if (go_Box.activeSelf)
        {
            button_Ok.onClick.RemoveAllListeners();
            go_Box.SetActive(false);
        }
    }

    /// <summary>알림 텍스트 </summary>
    private void SetBoxText(popupnotice_enum _enum, string _text)
    {
        switch (_enum)
        {
            case popupnotice_enum.e0Default:

                break;
            case popupnotice_enum.e1Tap_Smithy__AcceSyntRelsToEnrol:
                text_BoxSubject.text = "알림";
                _text = "해당 아이템은 현재 장착중입니다.\n장착을 해제하고 등록하시겠습니까?";
                break;
        }

        text_BoxContents.text = _text; // 내용 
    }

    /// <summary> 취소, 닫기 </summary>
    public void ClickClose ()
    {
        go_Box.SetActive(false);
    }

    /// <summary> 확인 </summary>
    public void ClickConfirm ()
    {
        go_Box.SetActive(false);
    }

    public void ClickBackground()
    {
        ClickClose();
    }
    #endregion
}
