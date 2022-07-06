using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using BackEnd;

[System.Serializable]
public struct TapChangeButton
{
    public int id;
    public GameObject go_button;
    public Image ui_bg;
    public UIGradient ui_outline;
    public GameObject go_icon;
    public Text ui_tap_name;
    public Animation ui_ani;
}

public class Inventory : MonoBehaviour
{
    [SerializeField] TapObject tapObject;

    public long tapOpenSP = 0;
    public InventoryType inventoryType = InventoryType.ALL;
    public InitOnStartInventoryAll initOnStartInventoryAll;
    //public WearingEquipmentInfo wearingEquipmentInfo;
    [SerializeField] TapChangeButton[] rightButtons = new TapChangeButton[6];
    [SerializeField] Text textInventorySlotCount;
    [SerializeField] GameObject goInvnUp;
    [SerializeField] GameObject goAutoWearingLoading, goAutoNormalLevelUpLoading;

    

    [System.Serializable]
    public enum InventoryType
    {
        Disable,
        ALL,
        EQUIP_WEAPON_SHIELD,
        EQUIP_COSTUME,
        EQUIP_ACCE,
        ITEM,
        ETC
    }

    void Awake()
    {
        string strITapOpenSp = PlayerPrefs.GetString(PrefsKeys.prky_InventoryTapOpenSp);
        //tapOpenSP = string.IsNullOrEmpty(strITapOpenSp) ? GameDatabase.GetInstance().GetUniqueIDX() : long.Parse(strITapOpenSp);
    }

    void OnEnable()
    {
        tapObject.aniIcon.Play("MainButtonActiveOnScale");
        tapObject.txName.fontStyle = FontStyle.Bold;
        tapObject.txName.color = tapObject.onCorSelect;
        tapObject.goOutline.SetActive(true);

        MainUI.GetInstance().tapCharacter.wearingEquipmentInfo.SetWearEquipView(-1);

        if(PopUpMng.GetInstance().popUpProficiency.gameObject.activeSelf)
            PopUpMng.GetInstance().popUpProficiency.SetData();

        //bool isCheckInvenComplete = GameDatabase.GetInstance().GetUniqueIDX() > NotificationIcon.GetInstance().tapLastInventoryOpenSP;
        //NotificationIcon.GetInstance().CheckNoticeNewEquipRating(-1, -1, isCheckInvenComplete);
    }

    void OnDisable()
    {
        tapObject.txName.fontStyle = FontStyle.Normal;
        tapObject.txName.color = tapObject.noCorSelect;
        tapObject.goOutline.SetActive(false);
        inventoryType = InventoryType.Disable;

        NotificationIcon.GetInstance().CheckNoticeNewEquipType(-1, -1, true);
        tapOpenSP = GameDatabase.GetInstance().GetUniqueIDX();
    }

    void Start ()
    {
        InventoryLevelCount();
    }

    /// <summary>
    /// 캐릭터 착용장비, 스탯 정보 텍스트 새로 고침 
    /// </summary>
    public void CharacterEquipStatUIRefresh (int eq_type)
    {
        LogPrint.PrintError("CharacterEquipStatUIRefresh");
        MainUI.GetInstance().tapCharacter.wearingEquipmentInfo.SetWearEquipView(eq_type);
        MainUI.GetInstance().tapCharacter.SetStatView(GameDatabase.GetInstance().characterDB.GetStat());
    }

    public void InventoryLevelCount ()
    {
        int invnCnt = GameDatabase.GetInstance().inventoryDB.GetInItemsCount();
        int invnLvCnt = GameDatabase.GetInstance().inventoryDB.GetLevelInvenCount();
        textInventorySlotCount.text = string.Format("{0} / {1}",
            invnCnt, /*인벤에 들어있는 수량*/
            invnLvCnt); /*인벤 칸수*/

        int max_invn = GameDatabase.GetInstance().chartDB.GetDicBalance("inventory.max.level").val_int;
        var uInfo = GameDatabase.GetInstance().tableDB.GetUserInfo();

        goInvnUp.SetActive(!(uInfo.m_invn_lv >= max_invn));
    }

    public void ResetRightButton ()
    {
        for (int i = 0; i < rightButtons.Length; i++)
            rightButtons[i].ui_ani.Play(i == 0 ? "UIInventoryRightButtonOn" : "UIInventoryRightButtonOff");
    }

    public void InitRightTapBtn()
    {
        foreach (var item in rightButtons)
        {
            if (item.id == 1)
                item.ui_ani.Play("UIInventoryRightButtonOn");
            else
                item.ui_ani.Play("UIInventoryRightButtonOff");
        }
    }

    public void ClickRightTapChange(int num)
    {
        foreach (var item in rightButtons)
        {
            if(item.id == num)
            {
                // ON 
                if(item.go_icon.activeSelf && !item.ui_ani.isPlaying)
                {
                    var beforInvenType = MainUI.GetInstance().inventory.inventoryType;
                    if (num == 1 && beforInvenType != InventoryType.ALL) // ALL 
                    {
                        MainUI.GetInstance().inventory.inventoryType = InventoryType.ALL;
                    }
                    else if (num == 2 && beforInvenType != InventoryType.EQUIP_WEAPON_SHIELD) // ALL 
                    {
                        MainUI.GetInstance().inventory.inventoryType = InventoryType.EQUIP_WEAPON_SHIELD;
                        if(beforInvenType == InventoryType.EQUIP_COSTUME)
                            NotificationIcon.GetInstance().CheckNoticeNewEquipType(2, -1, true);
                        else if(beforInvenType == InventoryType.EQUIP_ACCE)
                            NotificationIcon.GetInstance().CheckNoticeNewEquipType(8, -1, true);
                        
                        NotificationIcon.GetInstance().CheckNoticeNewEquipType(0, -1, true);
                    }
                    else if (num == 3 && beforInvenType != InventoryType.EQUIP_COSTUME) // ALL 
                    {
                        MainUI.GetInstance().inventory.inventoryType = InventoryType.EQUIP_COSTUME;
                        if (beforInvenType == InventoryType.EQUIP_WEAPON_SHIELD)
                            NotificationIcon.GetInstance().CheckNoticeNewEquipType(0, -1, true);
                        else if (beforInvenType == InventoryType.EQUIP_ACCE)
                            NotificationIcon.GetInstance().CheckNoticeNewEquipType(8, -1, true);
                        
                        NotificationIcon.GetInstance().CheckNoticeNewEquipType(2, -1, true);
                    }
                    else if (num == 4 && beforInvenType != InventoryType.EQUIP_ACCE) // ALL 
                    {
                        MainUI.GetInstance().inventory.inventoryType = InventoryType.EQUIP_ACCE;
                        if (beforInvenType == InventoryType.EQUIP_WEAPON_SHIELD)
                            NotificationIcon.GetInstance().CheckNoticeNewEquipType(0, -1, true);
                        else if (beforInvenType == InventoryType.EQUIP_COSTUME)
                            NotificationIcon.GetInstance().CheckNoticeNewEquipType(2, -1, true);
                        
                        NotificationIcon.GetInstance().CheckNoticeNewEquipType(8, -1, true);
                    }
                    else if (num == 5 && beforInvenType != InventoryType.ITEM) // ALL 
                    {
                        MainUI.GetInstance().inventory.inventoryType = InventoryType.ITEM;
                    }
                    else if (num == 6 && beforInvenType != InventoryType.ETC) // ALL 
                    {
                        MainUI.GetInstance().inventory.inventoryType = InventoryType.ETC;
                    }

                    if (beforInvenType != MainUI.GetInstance().inventory.inventoryType)
                    {
                        item.ui_ani.Play("UIInventoryRightButtonOn");

                        PopUpMng.GetInstance().popUpInventorySort.enum_SortInvnHighLow = (SortInventorytHighLow)PlayerPrefs.GetInt(PrefsKeys.prky_SortInventorytHighLow_etc);
                        
                        if(MainUI.GetInstance().inventory.inventoryType == InventoryType.ALL)
                            PopUpMng.GetInstance().popUpInventorySort.enum_SortInventory = (SortInventory)PlayerPrefs.GetInt(PrefsKeys.prky_SortInventory_etc);
                        else PopUpMng.GetInstance().popUpInventorySort.enum_SortInventory = (SortInventory)PlayerPrefs.GetInt(PrefsKeys.prky_SortInventory_equip);

                        MainUI.GetInstance().inventory.initOnStartInventoryAll.SetInit(true);
                    }
                }
            }
            else // OFF  
            {
                item.ui_ani.Play("UIInventoryRightButtonOff");
            }
        }
    }

    private List<GameDatabase.TableDB.Equipment> changeEquipDBs = new List<GameDatabase.TableDB.Equipment>();
    /// <summary> 장비 자동 장착 클릭 -> 전투력 높은 순 </summary>
    public async void Click_AutoWearing()
    {
        try
        {
            changeEquipDBs.Clear();
            goAutoWearingLoading.SetActive(true);
            bool isChange = false;
            for (int f_ty = 0; f_ty <= 10; f_ty++)
            {
                var wearEquipDB = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(f_ty);
                var eq_ty_finds = GameDatabase.GetInstance().tableDB.GetAllEquipment().FindAll(e => int.Equals(e.m_state, 0) && int.Equals(e.eq_ty, f_ty) && e.eq_rt >= wearEquipDB.eq_rt);
                if (eq_ty_finds.Count > 0)
                {
                    eq_ty_finds.Sort((GameDatabase.TableDB.Equipment x, GameDatabase.TableDB.Equipment y) => 
                                GameDatabase.GetInstance().tableDB.GetEquipCombatPower(y, "total", wearEquipDB.m_norm_lv).CompareTo(GameDatabase.GetInstance().tableDB.GetEquipCombatPower(x, "total", wearEquipDB.m_norm_lv)));

                    long nowWear_combat = GameDatabase.GetInstance().tableDB.GetEquipCombatPower(wearEquipDB, "total");
                    long high_combat = GameDatabase.GetInstance().tableDB.GetEquipCombatPower(eq_ty_finds[0], "total", wearEquipDB.m_norm_lv);
                    if (nowWear_combat < high_combat)
                    {
                       
                        var changeEquipDB = eq_ty_finds[0];
                        changeEquipDB.m_norm_lv = wearEquipDB.m_norm_lv;
                        wearEquipDB.m_state = 0;
                        changeEquipDB.m_state = 1;
                        changeEquipDBs.Add(changeEquipDB);

                        if (string.IsNullOrEmpty(changeEquipDB.indate) == true)
                        {
                            changeEquipDB.indate = wearEquipDB.indate;
                            wearEquipDB.indate = string.Empty;
                        }

                        if (string.IsNullOrEmpty(changeEquipDB.indate) == false)
                        {
                            BackendReturnObject bro1 = null, bro2 = null;
                            Param wear_parm = string.IsNullOrEmpty(changeEquipDB.indate) == false && string.IsNullOrEmpty(wearEquipDB.indate) == true ?
                                    GameDatabase.GetInstance().tableDB.EquipParamCollection(changeEquipDB) : ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "m_state", v = changeEquipDB.m_state }, new ParamT.P() { k = "m_norm_lv", v = changeEquipDB.m_norm_lv } });

                            SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_Equipment, changeEquipDB.indate, wear_parm, callback => { bro1 = callback; });

                            if (string.IsNullOrEmpty(wearEquipDB.indate) == false)
                                SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_Equipment, wearEquipDB.indate, ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "m_state", v = 0 } }), callback => { bro2 = callback; });
                            else bro2 = new BackendReturnObject();

                            while (bro1 == null || bro2 == null) { await Task.Delay(100); }

                            GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(changeEquipDB);
                            GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(wearEquipDB);
                            GameDatabase.GetInstance().tableDB.SetTempEquipDbChange(changeEquipDB, wearEquipDB);
                            isChange = true;
                        }
                        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("데이터 식별ID값 오류입니다. 문제가 지속된다면 게임을 재실행 해주시기 바랍니다.");
                    }
                }
            }

            if (isChange)
            {
                NotificationIcon.GetInstance().CheckNoticeAutoWear(default, false, true);
                GameDatabase.GetInstance().characterDB.SetPlayerStatValue();
                MainUI.GetInstance().tapCharacter.wearingEquipmentInfo.SetWearEquipView(-1);
                MainUI.GetInstance().tapCharacter.SetStatView(GameDatabase.GetInstance().characterDB.GetStat());
                MainUI.GetInstance().NewEquipItemInventortRefresh();

                GameMng.GetInstance().myPZ.igp.statValue = GameDatabase.GetInstance().characterDB.GetStat(); // 장비 관련, 스탯 
                GameMng.GetInstance().myPZ.SettingParts(-1); // 필드 좀비 
            }
        }
        catch (System.Exception e)
        {
            LogPrint.Print("e:" + e);
        }

        goAutoWearingLoading.SetActive(false);
    }
    
    /// <summary>
    /// 모든 장비 일반 레벨업 하기 --->>> 사용 않함(장비 숙련도 레벨로 대체됨) 
    /// </summary>
    public async void Click_AllNormalLevelUp()
    {
        bool[] isUps = new bool[11];
        GameDatabase.TableDB.Equipment[] update_equips = new GameDatabase.TableDB.Equipment[11];
        int maxNormLv = GameDatabase.GetInstance ().chartDB.GetDicBalance("eq.normal.max.level").val_int;
        for (int fTy = 0; fTy <= 10; fTy++)
        {
            var eq_db = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(fTy);
            bool isMaxNormLv = eq_db.m_norm_lv >= maxNormLv;
            if (isMaxNormLv == false)
            {
                int fStartNorLv = eq_db.m_norm_lv;
                for (int fLv = fStartNorLv; fLv <= maxNormLv; fLv++)
                {
                    long myGold = GameDatabase.GetInstance().tableDB.GetTableDB_Goods().m_gold;
                    long norLvUpGold = GameDatabase.GetInstance().questDB.GetQuestEquipLevelUpGold(eq_db.eq_rt, eq_db.eq_id, fLv, fLv + 1);
                    if (norLvUpGold > 0)
                    {
                        if (myGold >= norLvUpGold && isMaxNormLv == false)
                        {
                            eq_db.m_norm_lv++;
                            isUps[fTy] = true;
                            update_equips[fTy] = eq_db;
                            isMaxNormLv = eq_db.m_norm_lv >= maxNormLv;
                            GameDatabase.GetInstance().tableDB.SetUpdateGoods("gold", norLvUpGold, "-");
                        }
                        else break;
                    }
                    else break;
                }
            }
        }

        bool isTrans1 = false, isTrans2 = false;
        // 장비 트렌젝션 
        TransactionParam t_equip_param = new TransactionParam();
        for (int fTy = 0; fTy <= 7; fTy++)
        {
            if(isUps[fTy] == true)
            {
                isTrans1 = true;
                Param p1 = new Param();
                p1.Add("m_norm_lv", update_equips[fTy].m_norm_lv);
                t_equip_param.AddUpdateList(BackendGpgsMng.tableName_Equipment, update_equips[fTy].indate, new List<BackEnd.WRITE> { new WRITE { Action = TransactionAction.Update, Param = p1 } });
            }
        }
        
        // 장신구 트렌젝션 
        TransactionParam t_acce_param = new TransactionParam();
        for (int fTy = 8; fTy <= 10; fTy++)
        {
            if (isUps[fTy] == true)
            {
                isTrans2 = true;
                Param p1 = new Param();
                p1.Add("m_norm_lv", update_equips[fTy].m_norm_lv);
                t_acce_param.AddUpdateList(BackendGpgsMng.tableName_Equipment, update_equips[fTy].indate, new List<BackEnd.WRITE> { new WRITE { Action = TransactionAction.Update, Param = p1 } });
            }
        }

        if (isTrans1 || isTrans2)
        {
            goAutoNormalLevelUpLoading.SetActive(true);
            BackendReturnObject trans_bro1 = null;
            BackendReturnObject trans_bro2 = null;

            SendQueue.Enqueue(Backend.GameInfo.TransactionWrite, t_equip_param, callback => { trans_bro1 = callback; });
            if (isTrans1)
                while (trans_bro1 == null) { await Task.Delay(100); }

            SendQueue.Enqueue(Backend.GameInfo.TransactionWrite, t_acce_param, callback => { trans_bro2 = callback; });
            if (isTrans2)
                while (trans_bro2 == null) { await Task.Delay(100); }

            await GameDatabase.GetInstance().tableDB.SetUpdateGoods(GameDatabase.GetInstance().tableDB.GetTableDB_Goods(), false, "gold");
            goAutoNormalLevelUpLoading.SetActive(false);

            for (int fTy = 0; fTy <= 10; fTy++)
            {
                if (isUps[fTy] == true)
                {
                    GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(update_equips[fTy]);
                    MainUI.GetInstance().tapCharacter.wearingEquipmentInfo.SetWearEquipView(fTy);
                }
            }
            
            GameDatabase.GetInstance().characterDB.SetPlayerStatValue();
            MainUI.GetInstance().tapCharacter.SetStatView(GameDatabase.GetInstance().characterDB.GetStat());
            GameMng.GetInstance().myPZ.igp.statValue = GameDatabase.GetInstance().characterDB.GetStat(); // 장비 관련, 스탯 
            
        }
    }

    public async void Click_InventoryLevelUp()
    {
        int max_invn = GameDatabase.GetInstance().chartDB.GetDicBalance("inventory.max.level").val_int;
        var uInfo = GameDatabase.GetInstance().tableDB.GetUserInfo();
        if (uInfo.m_invn_lv >= max_invn)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("인벤토리가 최대치까지 확장 완료된 상태입니다.");
            return;
        }

        int price = GameDatabase.GetInstance().inventoryDB.GetLevelUpPrice();
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        bool isBlueDiaLack = goods_db.m_dia < price;
        int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
        int blue_dia = goods_db.m_dia;
        if (blue_dia + tbc >= price)
        {
            int invn_kan = GameDatabase.GetInstance().inventoryDB.GetLevelInvenCount();
            int invn_nxt_kan = GameDatabase.GetInstance().inventoryDB.GetLevelInvenCount() + 50;
            string _text = string.Format("인벤토리를 {0}칸에서 {1}칸으로 확장시킵니다.\n다이아 <color=#00EEFF>x{2}개</color> <color=red>소모</color>됩니다.", invn_kan, invn_nxt_kan, price);
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(_text, InventoryLevelUp);
        }
        else
        {
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
        }
    }

    async void InventoryLevelUp()
    {
        int price = GameDatabase.GetInstance().inventoryDB.GetLevelUpPrice();
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        int dedDia = goods_db.m_dia -= price; // 내 현재 블루 다이아 차감
        int dedTbc = dedDia < 0 ? System.Math.Abs(dedDia) : 0;

        var userInfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
        userInfo_db.m_invn_lv++;

        Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
        Task tsk2 = GameDatabase.GetInstance().tableDB.DeductionTBC(dedTbc);
        Task tsk3 = GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userInfo_db);
        while (Loading.Bottom(tsk1.IsCompleted, tsk2.IsCompleted, tsk3.IsCompleted) == false) await Task.Delay(100);

        MainUI.GetInstance().inventory.InventoryLevelCount();
        GameDatabase.GetInstance().inventoryDB.CheckIsEmpty(); // 인벤토리 공간 체크 
    }
}
