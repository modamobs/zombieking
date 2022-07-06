using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopUpShopPieceExchange : MonoBehaviour
{
    [SerializeField] DB db;
    [System.Serializable]
    struct DB
    {
        public int equip_ty;  // 장비 타입 번호 0:무기.....7:신발, 장신구 타입 번호 8:목걸, 9:귀걸, 10:반지 
        public GameDatabase.TableDB.Item item;
        public GameDatabase.TableDB.Equipment result;
        public UnityAction confirmAction; // 교환 완료시 새로고침 (교환 리스트)
    }
    [SerializeField] List<GameDatabase.TableDB.Equipment> resultEquips = new List<GameDatabase.TableDB.Equipment>();

    [SerializeField] UI ui;
    [System.Serializable]
    struct UI
    {
        public GameObject goEquipBtnsRoot, goAcceBtnsRoot;

        public GameObject goTapInfo;

        public Text txTitle;
        //public Button btnConfirm;
        public Image imBtnConfirmBg;
        public Image imMinusBtnBg, imPlusBtnBg, imMinBtnBg, imMaxBtnBg;

        public GameObject goBtnResultSaleDecomp;
        public GameObject goBtnResultClose;

        public GameObject[] goBtnsSelectBoxGrad;    // 장비 타입 버튼 선택 그라데이션 
        public Image[] imBtnsSelectOutline;    // 장비 타입 버튼 테두리 

        public Text txSaleReward; // 판매 보상 : 골드 

        public Image imDecompRewardIcon; // 분해 보상 아이콘 : 장비 -> 루비, 장신구 -> 에테르 
        public Text txDecompReward; // 분해 보상 카운트 

        public PieceUI pieceUI;
        [System.Serializable]
        public struct PieceUI
        {
            public Image imIcon;
            public Image imRatingOutline;
            public Image imRatingBg;
            public Text txRating;
            public Text txCount;
        }

        public List<EquipUI> equipUI;
        [System.Serializable]
        public struct EquipUI
        {
            public Image imIcon;
            public Image imRatingOutline;
            public Image imRating;
            public Text txRating;
        }

        public ResultUI resultUI;
        [System.Serializable]
        public struct ResultUI
        {
            public GameObject goResult;
            public Image imIcon;
            public Image imRatingOutline;
            public Image imRating;
            public Text txRating;
            public Text txName;
        }
    }

    [SerializeField] Color coSelectBtnBox, coNoneBtnBox;
    Sprite spBtnBgWhite, spBtnBgGray;
    Sprite spEquipDeompReward, spAcceDecompReward;
    string srTitleEquip, srTitleAcce;
    
    void Awake()
    {
        spBtnBgWhite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(true);
        spBtnBgGray = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);
        srTitleEquip = LanguageGameData.GetInstance().GetString("title.equip.piece.exchange");
        srTitleAcce = LanguageGameData.GetInstance().GetString("title.acce.piece.exchange");
        spEquipDeompReward = SpriteAtlasMng.GetInstance ().GetSpriteRuby();
        spAcceDecompReward = SpriteAtlasMng.GetInstance().GetSpriteEther();
    }

    public void Init(GameDatabase.TableDB.Item iItem, UnityAction uAct)
    {
        ui.goTapInfo.SetActive(true);
        ui.resultUI.goResult.SetActive(false);

        db.item = iItem;
        db.result = new GameDatabase.TableDB.Equipment() { eq_ty = -1 };
        db.confirmAction = uAct;

        ui.goEquipBtnsRoot.SetActive(db.item.type == 28);
        ui.goAcceBtnsRoot.SetActive(db.item.type == 29);
        ui.txTitle.text = db.item.type == 28 ? srTitleEquip : srTitleAcce;

        //ui.btnConfirm.onClick.RemoveAllListeners();
        //ui.btnConfirm.onClick.AddListener(Listener_StartExchange);

        ui.imDecompRewardIcon.sprite = db.item.type == 28 ? spEquipDeompReward : spAcceDecompReward;

        Buttons();
        InfoPiece();
        InfoEquip();
    }

    public void ClickButtonType(int eq_ty_id)
    {
        db.equip_ty = eq_ty_id;
        Buttons();
        InfoPiece();
        InfoEquip();
    }

    void Buttons()
    {
        // 무기,방패~~~신발 
        if(db.item.type == 28)
        {
            if (db.equip_ty > 7)
                db.equip_ty = 0;
        }
        // 목걸이,귀걸이,반지  
        else if(db.item.type == 29)
        {
            if (db.equip_ty < 8)
                db.equip_ty = 8;
        }

        for (int i = 0; i < ui.goBtnsSelectBoxGrad.Length; i++)
        {
            ui.goBtnsSelectBoxGrad[i].SetActive(i == db.equip_ty);
            ui.imBtnsSelectOutline[i].color = i == db.equip_ty ? coSelectBtnBox : coNoneBtnBox;
        }
    }

    void InfoPiece()
    {
        int reqCnt = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("exchange.piece.equip.type.{0}", db.equip_ty)).val_int;
        exch_min = db.item.count >= reqCnt ? 1 : 0;
        exch_max = db.item.count >= reqCnt ? db.item.count / reqCnt > max ? max : db.item.count / reqCnt : 0;
        exch_cnt = exch_min;

        ui.pieceUI.imIcon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(db.item.type, db.item.rating);
        ui.pieceUI.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(db.item.rating);
        ui.pieceUI.imRatingOutline.color = ResourceDatabase.GetInstance().GetItemShinyColor(db.item.rating);
        ui.pieceUI.txRating.text = GameDatabase.StringFormat.GetRatingColorText(db.item.rating, false);
        ui.pieceUI.txCount.text = string.Format("( {0} / {1} )", db.item.count, reqCnt);
        ui.pieceUI.txCount.color = db.item.count < reqCnt ? Color.red : Color.white;
      
        MinMaxButton();
    }

    void MinMaxButton()
    {
        ui.imBtnConfirmBg.sprite = exch_cnt <= 0 ? spBtnBgGray : spBtnBgWhite;
        ui.imMinusBtnBg.sprite = exch_cnt >= 2 ? SpriteAtlasMng.GetInstance().GetSpriteButtonBox("purple") : SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);
        ui.imPlusBtnBg.sprite = exch_cnt < exch_max && exch_cnt < max ? SpriteAtlasMng.GetInstance().GetSpriteButtonBox("blue") : SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);
        ui.imMinBtnBg.sprite = exch_cnt >= 2 ? SpriteAtlasMng.GetInstance().GetSpriteButtonBox("purple") : SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);
        ui.imMaxBtnBg.sprite = exch_cnt < exch_max && exch_cnt < max ? SpriteAtlasMng.GetInstance().GetSpriteButtonBox("blue") : SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);
        txUseExchCnt.text = string.Format("{0}/{1}회 교환", exch_cnt, max);
    }

    ArrayList ids = new ArrayList();
    void InfoEquip()
    {
        ids.Clear();
        
        int ty = db.equip_ty;
        int rt = db.item.rating;
        var perIDList = GameDatabase.GetInstance().shopDB.GetLuckPieceExchangePerId(rt);
        foreach (var item in perIDList)
            ids.Add(item.equip_id);

        while(ids.Count < 4)
            ids.Add(0);

        for (int i = 0; i < 4; i++)
        {
            int id = (int)ids[i];
            ui.equipUI[i].imIcon.sprite = id >= 1 ? SpriteAtlasMng.GetInstance().GetSpriteEquip(ty, rt, id) : SpriteAtlasMng.GetInstance().GetTransparency();
            ui.equipUI[i].imRating.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(rt);
            ui.equipUI[i].imRatingOutline.color = ResourceDatabase.GetInstance().GetItemShinyColor(rt);
            ui.equipUI[i].txRating.text = id >= 1 ? GameDatabase.StringFormat.GetRatingColorText(rt, false) : "";
        }
    }

    public void Click_OneExchange() => StartExchange();
    public void Click_UseExchange()
    {
        if (exch_cnt > 0)
            StartExchange(exch_cnt);
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("교환에 필요한 조각의 수량이 부족합니다.");
    }


    /// <summary>
    /// 교환하기  
    /// </summary>
    private async void StartExchange(int exh_cnt = 1)
    {
        int reqCnt = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("exchange.piece.equip.type.{0}", db.equip_ty)).val_int * exh_cnt;
        int pieceCnt = db.item.count * exh_cnt;
        if (pieceCnt >= reqCnt)
        {
            PopUpMng.GetInstance().Open_ShopLuckEquipResultEmpty();
            soHwanResults.Clear();
            for (int i = 0; i < exh_cnt; i++)
            {
                //db.result = new GameDatabase.TableDB.Equipment() { eq_ty = -1 };
                float r_pct = GameDatabase.GetInstance().GetRandomPercent();
                var perIDList = GameDatabase.GetInstance().shopDB.GetLuckPieceExchangePerId(db.item.rating);
                float acr_pct = 0.0f;
                foreach (var item in perIDList)
                {
                    acr_pct += item.exch_per;
                    if (r_pct <= acr_pct)
                    {
                        var result_db = GameDatabase.GetInstance().tableDB.GetNewEquipmentData(db.equip_ty, db.item.rating, item.equip_id);
                        //var resultEqDb = GameDatabase.GetInstance().tableDB.GetNewEquipmentData(db.equip_ty, db.item.rating, item.equip_id);
                        //GameDatabase.GetInstance().equipmentEncyclopediaDB.DropAcquisitionAdded(db.result.eq_ty, db.result.eq_rt, db.result.eq_id);
                        //resultEquips.Add(resultEqDb);

                        soHwanResults.Add(new AcceResult()
                        {
                            isRtFixed = false,
                            ac_type = result_db.eq_ty,
                            ac_rt = result_db.eq_rt,
                            ac_id = result_db.eq_id,
                        });
                        break;
                    }
                }
            }

            db.item.count -= reqCnt;
            Task tsk1 = GameDatabase.GetInstance().tableDB.SendDataItem(db.item);
            while (Loading.Bottom(tsk1.IsCompleted) == false) await Task.Delay(100);

            bool isAcce = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(db.equip_ty);
            PopUpMng.GetInstance().Open_ShopLuckEquipResult(soHwanResults, isAcce == true ? "장신구 조각 교환" : "장비 조각 교환", false, isAcce == true ? "acce" : "equip"); // 결과 
            db.confirmAction();
            InfoPiece();

            //ui.goTapInfo.SetActive(false);
            //db.result = new GameDatabase.TableDB.Equipment() { eq_ty = -1 };
            //float r_pct = GameDatabase.GetInstance().GetRandomPercent();
            //var perIDList = GameDatabase.GetInstance().shopDB.GetLuckPieceExchangePerId(db.item.rating);
            //float acr_pct = 0.0f;
            //foreach (var item in perIDList)
            //{
            //    acr_pct += item.exch_per;
            //    if (r_pct <= acr_pct)
            //    {
            //        db.result = GameDatabase.GetInstance().tableDB.GetNewEquipmentData(db.equip_ty, db.item.rating, item.equip_id);
            //        break;
            //    }
            //}

            //// 교환 장비 id per(확률) 오류 
            //if (db.result.eq_ty == -1)
            //{
            //    PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("교환될 장비의 데이터의 타입이 잘못되었습니다.");
            //    gameObject.SetActive(false);
            //}
            //// 정상 
            //else
            //{
            //    int rTy = db.result.eq_ty, rRt = db.result.eq_rt, rId = db.result.eq_id;
            //    db.item.count -= reqCnt;
            //    Task tsk1 = GameDatabase.GetInstance().tableDB.SendDataItem(db.item);
            //    while (Loading.Bottom(tsk1.IsCompleted) == false) await Task.Delay(100);

            //    GameDatabase.GetInstance().equipmentEncyclopediaDB.DropAcquisitionAdded(db.result.eq_ty, db.result.eq_rt, db.result.eq_id);

            //    // 판매 or 분해 보상 
            //    int rwd_bns = GameDatabase.GetInstance().tableDB.GetDicBalance("sale.decomposition.bonus").val_int;
            //    ui.txSaleReward.text = string.Format("{0:#,0}", GameDatabase.GetInstance().questDB.GetQuestEquipSaleGold(rRt, rId) * rwd_bns);

            //    int decompRwd = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(rTy) == true ?
            //        GameDatabase.GetInstance().questDB.GetQuestEquipDecompEther(rRt, rId) * rwd_bns :
            //        GameDatabase.GetInstance().questDB.GetQuestEquipDecompRuby(rRt, rId) * rwd_bns;

            //    ui.txDecompReward.text = string.Format("{0:#,0}", decompRwd);

            //    ui.resultUI.imIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(rTy, rRt, rId);
            //    ui.resultUI.imRating.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(rRt);
            //    ui.resultUI.imRatingOutline.color = ResourceDatabase.GetInstance().GetItemColor(rRt);
            //    ui.resultUI.txRating.text = GameDatabase.StringFormat.GetStringRating(rRt);
            //    ui.resultUI.txRating.color = ResourceDatabase.GetInstance().GetItemColor(rRt);
            //    ui.resultUI.txName.text = GameDatabase.StringFormat.GetEquipName(rTy, rRt, rId);
            //    db.confirmAction();

            //    ui.goBtnResultSaleDecomp.SetActive(false);
            //    ui.goBtnResultClose.SetActive(false);
            //    ui.resultUI.goResult.SetActive(true);

            //    await Task.Delay(2000);
            //    ui.goBtnResultSaleDecomp.SetActive(true);
            //    ui.goBtnResultClose.SetActive(true);
            //    InfoPiece();
            //}
        }
        else
        {
            string str_rt = GameDatabase.StringFormat.GetRatingColorText(db.item.rating, false);
            if(GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(db.item.rating))
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("{0} 등급 장신구 조각의 수량이 부족합니다.", str_rt));
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("{0} 등급 장비 조각의 수량이 부족합니다.", str_rt));
        }
    }

    [SerializeField]
    List<AcceResult> soHwanResults = new List<AcceResult>();

    bool isPressOn = false, isPlus = false;
    [SerializeField] int exch_min, exch_max, exch_cnt = 0, max = 12;
    [SerializeField] Text txUseExchCnt;
    public void UpPress_Minus() => isPressOn = false;
    public void DwPress_Minus()
    {
        if (exch_cnt > exch_min)
        {
            isPressOn = true;
            isPlus = false;
            StopCoroutine("Routin_OnPress");
            StartCoroutine("Routin_OnPress");
        }
    }

    public void UpPress_Plus() => isPressOn = false;
    public void DwPress_Plus()
    {
        if (exch_cnt >= exch_min && exch_cnt < exch_max)
        {
            isPressOn = true;
            isPlus = true;
            StopCoroutine("Routin_OnPress");
            StartCoroutine("Routin_OnPress");
        }
    }
    public void Click_Min()
    {
        if(exch_min > 0)
        {
            exch_cnt = exch_min;
            txUseExchCnt.text = string.Format("{0}/{1}회 교환", exch_cnt, max);
            MinMaxButton();
        }
    }
    public void Click_Max()
    {
        if(exch_cnt < exch_max)
        {
            exch_cnt = exch_max;
            txUseExchCnt.text = string.Format("{0}/{1}회 교환", exch_cnt, max);
            MinMaxButton();
        }
    }

    IEnumerator Routin_OnPress()
    {
        float press_time = 0.1f;
        yield return null;
        while (isPressOn)
        {
            int bCnt = exch_cnt;
            if (isPlus)
            {
                if (exch_cnt < exch_max && exch_cnt < max)
                {
                    exch_cnt++;
                    MinMaxButton();
                }
            }
            else
            {
                if (exch_cnt > exch_min)
                {
                    exch_cnt--;
                    MinMaxButton();
                }
            }

            if (bCnt != exch_cnt)
            {
                MinMaxButton();
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


    public void Click_StartExchange()
    {
        //soHwanResults.Clear();
        //foreach (JsonData brw1 in broRow1)
        //{
        //    soHwanResults.Add(new AcceResult()
        //    {
        //        isRtFixed = false,
        //        ac_type = RowPaser.IntPaser(brw1, "ac_type"),
        //        ac_rt = RowPaser.IntPaser(brw1, "ac_rt"),
        //        ac_id = RowPaser.IntPaser(brw1, "ac_id"),
        //    });
        //}

        //PopUpMng.GetInstance().Open_ShopLuckEquipResult(soHwanResults); // 결과 
    }

    /// <summary>
    /// 버튼 : 결과 닫기 
    /// </summary>
    public void Click_CloseResult ()
    {
        //if(db.result.eq_ty != -1)
        //{
        //    GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(db.result);
        //    NotificationIcon.GetInstance().CheckNoticeAutoWear(db.result, false);
        //    GameDatabase.GetInstance().chat.ChatSendItemMessage("equip", db.result.eq_ty, db.result.eq_rt, db.result.eq_id);
        //    //MainUI.GetInstance().NewEquipItemInventortRefresh();
        //}

        //InfoPiece();
        //InfoEquip();

        //ui.goTapInfo.SetActive(true);
        //ui.resultUI.goResult.SetActive(false);
    }

    /// <summary>
    /// 버튼 : 결과 판매 
    /// </summary>
    public async void Click_ResultSale()
    {
        //int rwd_bns = GameDatabase.GetInstance().tableDB.GetDicBalance("sale.decomposition.bonus").val_int;
        //int rwdCnt = GameDatabase.GetInstance().questDB.GetQuestEquipSaleGold(db.result.eq_rt, db.result.eq_id) * rwd_bns;
        //string rtStr = GameDatabase.StringFormat.GetRatingColorText(db.result.eq_rt, false);
        //string txt = string.Format("선택한 {0}등급 장비를 판매하시겠습니까?\n보상으로 <color=#FFE800>골드</color> {1:#,0}(+{2:#,0}) 획득합니다.", rtStr, rwdCnt, (int)(rwdCnt / rwd_bns));
        //PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, AskResultSaleRewardGold);
    }

    /// <summary>
    /// 버튼 : 결과 분해 
    /// </summary>
    public void Click_Decomp()
    {
        //bool isAcce = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(db.result.eq_ty) == true;
        //string rtStr = GameDatabase.StringFormat.GetRatingColorText(db.result.eq_rt, false);
        //int rwd_bns = GameDatabase.GetInstance().tableDB.GetDicBalance("sale.decomposition.bonus").val_int;
        //// 장신구 분해 보상 
        //if (isAcce)
        //{
        //    int rwd_cnt = GameDatabase.GetInstance().questDB.GetQuestEquipDecompEther(db.result.eq_rt, db.result.eq_id);
        //    int rwdCnt = rwd_cnt * rwd_bns;
        //    string txt = string.Format("선택한 {0}등급 장신구를 분해하시겠습니까?\n보상으로 <color=#00FF1E>에테르</color> x{1:#,0}(+{2:#,0}) 획득합니다.", rtStr, rwdCnt, (int)(rwdCnt / rwd_bns));
        //    PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, AskResultDecompRewardEther);
        //}
        //else // 장비 분해 보상 
        //{
        //    int rwd_cnt = GameDatabase.GetInstance().questDB.GetQuestEquipDecompRuby(db.result.eq_rt, db.result.eq_id);
        //    int rwdCnt = rwd_cnt * rwd_bns;
        //    string txt = string.Format("선택한 {0}등급 장비를 분해하시겠습니까?\n보상으로 <color=#FF0080>루비</color> x{1:#,0}(+{2:#,0}) 획득합니다.", rtStr, rwdCnt, (int)(rwdCnt / rwd_bns));
        //    PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, AskResultDecompRewardRuby);
        //}
    }

    void AskResultDecompRewardRuby()
    {
        //string rtStr = GameDatabase.StringFormat.GetRatingColorText(db.result.eq_rt, false);
        //int rwd_cnt = GameDatabase.GetInstance().questDB.GetQuestEquipDecompRuby(db.result.eq_rt, db.result.eq_id);
        //int rwd_bns = GameDatabase.GetInstance().tableDB.GetDicBalance("sale.decomposition.bonus").val_int;
        //int rwdCnt = rwd_cnt * rwd_bns;
        //GameDatabase.GetInstance().tableDB.SetUpdateGoods("ruby", rwdCnt, "+");
        //PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("{0}등급 장비를 분해 완료하였습니다.\n보상으로 <color=#FF0080>루비</color> x{1:#,0}(+{2:#,0}) 획득하였습니다..", rtStr, rwdCnt, (int)(rwdCnt / rwd_bns)));
        //CloseResult();
    }

    void AskResultDecompRewardEther()
    {
        //string rtStr = GameDatabase.StringFormat.GetRatingColorText(db.result.eq_rt, false);
        //int rwd_cnt = GameDatabase.GetInstance().questDB.GetQuestEquipDecompEther(db.result.eq_rt, db.result.eq_id);
        //int rwd_bns = GameDatabase.GetInstance().tableDB.GetDicBalance("sale.decomposition.bonus").val_int;
        //int rwdCnt = rwd_cnt * rwd_bns;
        //GameDatabase.GetInstance().tableDB.SetUpdateGoods("ether", rwdCnt, "+");
        //PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("{0}등급 장신구를 분해 완료하였습니다.\n보상으로 <color=#00FF1E>에테르</color> x{1:#,0}(+{2:#,0}) 획득하였습니다.", rtStr, rwdCnt, (int)(rwdCnt / rwd_bns)));
        //CloseResult();
    }

    void AskResultSaleRewardGold()
    {
        //string rtStr = GameDatabase.StringFormat.GetRatingColorText(db.result.eq_rt, false);
        //int rwd_cnt = GameDatabase.GetInstance().questDB.GetQuestEquipDecompEther(db.result.eq_rt, db.result.eq_id);
        //int rwd_bns = GameDatabase.GetInstance().tableDB.GetDicBalance("sale.decomposition.bonus").val_int;
        //long rwdCnt = rwd_cnt * rwd_bns;
        //GameDatabase.GetInstance().tableDB.SetUpdateGoods("gold", rwdCnt, "+");
        //PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("{0}등급 장비를 판매 완료하였습니다.\n보상으로 <color=#FF0080>골드</color> {1:#,0}(+{2:#,0}) 획득하였습니다.", rtStr, rwdCnt, (long)(rwdCnt / rwd_bns)));
        //CloseResult();
    }

    void CloseResult()
    {
        //db.result.eq_ty = -1;
        //ui.goBtnResultSaleDecomp.SetActive(false);
        //ui.goTapInfo.SetActive(true);
        //ui.resultUI.goResult.SetActive(false);
    }
}