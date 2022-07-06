using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameDatabase;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BackEnd;

public class PopUpShopLuckEquipResult : MonoBehaviour
{
    [SerializeField] string gc_type;
    [SerializeField] bool openShopLuck = false;
    [SerializeField] GameObject goBtnOpenAll, goBtnClose;
    [SerializeField] bool isOnParticle = false; // 결과물에 영웅등급 이상이 있다면 True로 변경됨 
    bool isOnCardParticle = false; // isOnParticle == True 일때에만 isOnCardParticle On/Off ㅎ ㅏㄹ수 있음
    public Text txTitle, txSubTitle;
    [SerializeField] List<UIResult> uiResults = new List<UIResult>();
    [System.Serializable]
    class UIResult
    {
     
        public GameObject goRoot;
        public Animator ani;
       
        public Image imIcon;
        public Image imRatingBg;
        public Image imRatingOutline;
        public Text txRating;
        public Text txName;

        public bool isDbNull;
        public bool isOpen;
        public AcceResult acResult;

        public GameObject goCardOnParticle;
        public ParticleSystemRenderer cardParticlesRdr1 = new ParticleSystemRenderer();
        public ParticleSystemRenderer cardParticlesRdr2 = new ParticleSystemRenderer();
    }

    [SerializeField] List<GameDatabase.TableDB.Equipment> resultEquips = new List<TableDB.Equipment>();
    [SerializeField] List<GameDatabase.TableDB.Pet> resultPets = new List<TableDB.Pet>();

    public void Init()
    {
        goBtnOpenAll.SetActive(false);
        goBtnClose.SetActive(false);

        resultEquips.Clear();
        resultPets.Clear();
        for (int i = 0; i < uiResults.Count; i++)
        {
            uiResults[i].isDbNull = true;
            uiResults[i].isOpen = false;
            uiResults[i].acResult = default;
            uiResults[i].goRoot.SetActive(false);
            uiResults[i].ani.gameObject.SetActive(false);
        }

        isOnParticle = false;
        isOnCardParticle = false;
        StopCoroutine("ResultParticle");
        StartCoroutine("ResultParticle");
    }

    public void SetData(List<AcceResult> list, string title, bool isTapShopLuck, string gcType)
    {
        gc_type = gcType;
        openShopLuck = isTapShopLuck;
        txTitle.text = string.IsNullOrEmpty(title) ? "소환" : title;
        txSubTitle.text = string.Equals(gcType, "equip") || string.Equals(gcType, "acce") ? "<color=yellow>[행운상점/교환상점]</color>에서 장비 획득 즉시 판매/분해시 <color=magenta>100%</color>의 보상을 추가로 획득합니다."
            : "";
        goSaleDecomp = null;
        for (int i = 0; i < list.Count; i++)
        {
            var rDb = list[i];
            uiResults[i].isDbNull = false;
            uiResults[i].isOpen = false;
            uiResults[i].acResult = rDb;

            if(string.Equals(gcType, "equip") || string.Equals(gcType, "acce"))
            {
                uiResults[i].imIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(rDb.ac_type, rDb.ac_rt, rDb.ac_id);
                uiResults[i].imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(rDb.ac_rt);
                uiResults[i].imRatingOutline.color = ResourceDatabase.GetInstance().GetItemShinyColor(rDb.ac_rt);
                uiResults[i].txRating.text = GameDatabase.StringFormat.GetRatingColorText(rDb.ac_rt, false);
                uiResults[i].txName.text = GameDatabase.StringFormat.GetEquipName(rDb.ac_type, rDb.ac_rt, rDb.ac_id);
                uiResults[i].goRoot.SetActive(true);
            }
            else if(string.Equals(gcType, "pet"))
            {
                uiResults[i].imIcon.sprite = SpriteAtlasMng.GetInstance().GetPetIcon(rDb.ac_rt, rDb.ac_id);
                uiResults[i].imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(rDb.ac_rt);
                uiResults[i].imRatingOutline.color = ResourceDatabase.GetInstance().GetItemShinyColor(rDb.ac_rt);
                uiResults[i].txRating.text = GameDatabase.StringFormat.GetRatingColorText(rDb.ac_rt, false);
                uiResults[i].txName.text = GameDatabase.GetInstance().chartDB.GetCdbPet(rDb.ac_rt, rDb.ac_id).name;
                uiResults[i].goRoot.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
                return;
            }
        }

        foreach (var item in uiResults)
        {
            if (item.goRoot.activeSelf)
            {
                item.ani.gameObject.SetActive(true);
                item.ani.Play("Result_ShopLuckGachaReady");
            }
        }

        goBtnOpenAll.SetActive(true);
    }

    
    public void OnCardParicle ()
    {
        foreach (var item in uiResults)
        {
            if(item.goCardOnParticle.activeSelf)
            {
                item.cardParticlesRdr1.enabled = true;
                item.cardParticlesRdr2.enabled = true;
                isOnCardParticle = true;
            }
        }
    }

    void OffCardParticle() 
    {
        foreach (var item in uiResults)
        {
            if (item.goCardOnParticle.activeSelf)
            {
                item.cardParticlesRdr1.enabled = false;
                item.cardParticlesRdr2.enabled = false;
                isOnCardParticle = false;
            }
        }
    }

    IEnumerator ResultParticle()
    {
        while(gameObject.activeSelf)
        {
            if (isOnParticle)
            {
                if (PopUpMng.GetInstance().GetOnPopUpCount() > 1)
                {
                    if(isOnCardParticle)
                        OffCardParticle();
                }
                else
                {
                    if(!isOnCardParticle)
                        OnCardParicle();
                }
            }

            yield return null;
        }
    }

    GameObject goSaleDecomp;
    AcceResult acSaleDecomp;
    /// <summary>
    /// 버튼 : 자세히 보기 또는 개별 오픈 
    /// </summary>
    public void ClickDetail(GameObject btn)
    {
        string strTmp = Regex.Replace(btn.gameObject.name, @"\D", ""); 
        int nTmp = int.Parse(strTmp);
        LogPrint.Print("nTmp : " + nTmp);
        
        int ty = uiResults[nTmp].acResult.ac_type;
        int rt = uiResults[nTmp].acResult.ac_rt;
        int id = uiResults[nTmp].acResult.ac_id;

        // 미오픈 상태 -> 오픈 
        if (uiResults[nTmp].isOpen == false)
        {
            uiResults[nTmp].isOpen = true;
            if (rt < 5 || !openShopLuck)
            {
                uiResults[nTmp].ani.Play("Result_ShopLuckGachaStart");
            }
            else
            {
                uiResults[nTmp].ani.Play(string.Format("Result_ShopLuckGachaStart_rt{0}", rt));
                isOnParticle = true;
            }

            if(string.Equals(gc_type, "equip") || string.Equals(gc_type, "acce"))
            {
                var resultEqDb = GameDatabase.GetInstance().tableDB.GetNewEquipmentData(ty, rt, id);
                GameDatabase.GetInstance().equipmentEncyclopediaDB.DropAcquisitionAdded(resultEqDb.eq_ty, resultEqDb.eq_rt, resultEqDb.eq_id);
                uiResults[nTmp].acResult.tempUID = resultEqDb.aInUid;
                resultEquips.Add(resultEqDb);
                if (openShopLuck)
                    GetInstance().chat.ChatSendItemMessage("equip", ty, rt, id);
            }
            else if (string.Equals(gc_type, "pet"))
            {
                var new_rnd_sop = GameDatabase.GetInstance().tableDB.GetPetRandomSpecialOption(rt);
                resultPets.Add
                (
                     new GameDatabase.TableDB.Pet()
                     {
                         aInUid = GameDatabase.GetInstance().GetUniqueIDX(),
                         p_rt = rt,
                         p_id = id,
                         m_state = 0,
                         m_lck = 0,
                         p_lv = 0,
                         p_lv_residual = 0.0f,
                         sOp1 = new_rnd_sop[0],
                         sOp2 = new_rnd_sop[1],
                         sOp3 = new_rnd_sop[2],
                         statOp = GameDatabase.GetInstance().tableDB.GetPetRandomOption(rt)
                     }
                );
            }

            ChecOpenAllkButton();
        }
        // 정보 보기
        else
        {
            if (string.Equals(gc_type, "equip") || string.Equals(gc_type, "acce"))
            {
                int index = resultEquips.FindIndex(x => x.eq_ty == ty && x.eq_rt == rt && id == x.eq_id);
                if (index >= 0)
                {
                    goSaleDecomp = btn;
                    acSaleDecomp = uiResults[nTmp].acResult;
                    PopUpMng.GetInstance().Open_ViewItemInfo(resultEquips[index], true, null, true, true);
                }
            }
            else if (string.Equals(gc_type, "pet"))
            {

            }
        }
    }

    /// <summary>
    /// 장비 정보 팝업애서 판매/분해 눌렀을때 버튼 리스너 
    /// </summary>
    public void CellSaleOrDecomp()
    {
        if(goSaleDecomp != null)
        {
            int rmvIdx = resultEquips.FindIndex(x => x.aInUid == acSaleDecomp.tempUID);
            if (rmvIdx >= 0)
            {
                resultEquips.RemoveAt(rmvIdx);
                goSaleDecomp.SetActive(false);
            }
        }
    }

    async void ChecOpenAllkButton()
    {
        bool isNoOpenAll = false;
        for (int i = 0; i < uiResults.Count; i++)
        {
            if (uiResults[i].isDbNull == false && uiResults[i].isOpen == false)
            {
                isNoOpenAll = true;
                break;
            }
        }

        if (isNoOpenAll == true)
        {
            goBtnOpenAll.SetActive(true);
            goBtnClose.SetActive(false);
        }
        else
        {
            goBtnOpenAll.SetActive(false);
            await Task.Delay(2000);
            goBtnClose.SetActive(true);
        }
    }

    /// <summary>
    /// 버튼 : 모두 오픈 
    /// </summary>
    public void Click_OpenAll()
    {
        for (int i = 0; i < uiResults.Count; i++)
        {
            if(uiResults[i].isDbNull == false && uiResults[i].isOpen == false)
            {
                uiResults[i].isOpen = true;
                int ty = uiResults[i].acResult.ac_type;
                int rt = uiResults[i].acResult.ac_rt;
                int id = uiResults[i].acResult.ac_id;
                if (rt < 5 || !openShopLuck)
                {
                    uiResults[i].ani.Play("Result_ShopLuckGachaStart");
                }
                else
                {
                    uiResults[i].ani.Play(string.Format("Result_ShopLuckGachaStart_rt{0}", rt));
                    isOnParticle = true;
                }

                if (string.Equals(gc_type, "equip") || string.Equals(gc_type, "acce"))
                {
                    var resultEqDb = GameDatabase.GetInstance().tableDB.GetNewEquipmentData(ty, rt, id);
                    GameDatabase.GetInstance().equipmentEncyclopediaDB.DropAcquisitionAdded(resultEqDb.eq_ty, resultEqDb.eq_rt, resultEqDb.eq_id);
                    uiResults[i].acResult.tempUID = resultEqDb.aInUid;
                    resultEquips.Add(resultEqDb);
                    if (openShopLuck)
                        GetInstance().chat.ChatSendItemMessage("equip", ty, rt, id);
                }
                else if (string.Equals(gc_type, "pet"))
                {
                    var new_rnd_sop = GameDatabase.GetInstance().tableDB.GetPetRandomSpecialOption(rt);
                    resultPets.Add
                    (
                        new GameDatabase.TableDB.Pet()
                        {
                            aInUid = GameDatabase.GetInstance().GetUniqueIDX(),
                            p_rt = rt,
                            p_id = id,
                            m_state = 0,
                            m_lck = 0,
                            p_lv = 0,
                            p_lv_residual = 0.0f,
                            sOp1 = new_rnd_sop[0],
                            sOp2 = new_rnd_sop[1],
                            sOp3 = new_rnd_sop[2],
                            statOp = GameDatabase.GetInstance().tableDB.GetPetRandomOption(rt)
                        }
                    );
                }
            }
        }

        goBtnOpenAll.SetActive(false);
        goBtnClose.SetActive(false);
        ChecOpenAllkButton();
    }

    /// <summary>
    /// 버튼 : 닫기 버튼 -> 오픈되지 않은 카드 체크 후 창 닫기 
    /// </summary>
    public async void Click_OpenCheckToClose()
    {
        Loading.Full(false);
        bool isNoOpenAll = false;
        for (int i = 0; i < uiResults.Count; i++)
        {
            if(uiResults[i].isDbNull == false && uiResults[i].isOpen == false)
            {
                isNoOpenAll = true;
                break;
            }
        }

        if(isNoOpenAll == true)
        {
            PopUpMng.GetInstance().Open_MessageNotif("아직 오픈하지 않는 카드가 있습니다.");
        }
        else
        {
            if (string.Equals(gc_type, "equip") || string.Equals(gc_type, "acce"))
            {
                foreach (GameDatabase.TableDB.Equipment item in resultEquips)
                {
                    var item_db = item;
                    if (item_db.eq_rt >= 5 && openShopLuck)
                    {
                        BackendReturnObject bro1 = null;
                        Param p = GetInstance().tableDB.EquipParamCollection(item_db);
                        long unUID = GetInstance().tableDB.GetUnusedUID();
                        string unInDate = GetInstance().tableDB.GetUIDSearchToInDate(unUID);
                        if (unUID == 0 || string.IsNullOrEmpty(unInDate))
                            SendQueue.Enqueue(Backend.GameInfo.Insert, BackendGpgsMng.tableName_Equipment, p, callback => { bro1 = callback; });
                        else
                            SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_Equipment, unInDate, p, callback => { bro1 = callback; });

                        while (Loading.Full(bro1) == false) { await Task.Delay(100); }

                        if (unUID == 0 || string.IsNullOrEmpty(unInDate)) // Insert 
                        {
                            item_db.indate = bro1.GetReturnValuetoJSON()["inDate"].ToString();
                        }
                        else // Update 
                        {
                            item_db.indate = unInDate;
                            GetInstance().tableDB.SetUnusedInDateToEmpty(unUID);
                        }
                    }

                    GetInstance().tableDB.UpdateClientDB_Equip(item_db);
                    NotificationIcon.GetInstance().CheckNoticeAutoWear(item_db, false);
                }
            }
            else if(string.Equals(gc_type, "pet"))
            {
                foreach (GameDatabase.TableDB.Pet item in resultPets)
                {
                    await GameDatabase.GetInstance().tableDB.SendDataPet(item, "insert");
                }
            }

            GameDatabase.GetInstance().tableDB.TempInventoryList();
            Loading.Full(true);
            gameObject.SetActive(false);
        }
    }
}
