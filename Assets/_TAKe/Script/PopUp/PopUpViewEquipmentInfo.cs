using BackEnd;
using Coffee.UIExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopUpViewEquipmentInfo : MonoBehaviour
{
    [SerializeField] InfoViewEquip wear;
    [SerializeField] InfoViewEquip select;
    // 착용중인 장비 
    [System.Serializable]
    public struct InfoViewEquip
    {
        public GameObject goTap;
        public GameDatabase.TableDB.Equipment eqDb;

        public Info_icon info_icon;
        public Info_stat info_stat;
        public Info_button info_button;
        public Info_etc info_etc;

        [System.Serializable]
        public struct Info_icon
        {
            public Image imIcon;
            public Image imRatingBg;
            public Text txRating;
            public GameObject goLock; // 잠금 image 
            public Color coOkLock, coNoLock; // 잠금 Color 

            public Text txLevel;
            public Text txEnhantLevel;
            public Text txEquipName;
            public Text txAllCombat; // 총 전투력 
            public Text txAllCombatCompare;

            public GameObject goRootNorLv100;
            public Image imRtOutlineNorLv100;
            public ParticleSystem psNorLv100;

            public GameObject goRootEnhant30;
            public Image imRtOutlineEnhant30;
            public UIGradient ugEnhant30;
            public GameObject goInvenNew;
        }

        [System.Serializable]
        public struct Info_stat
        {
            // 스탯 
            public Text txStatCombat; // 전투력 
            public Text txStatCombatCompare;
            public Image imStatIcon;
            public Text txStatName;
            public Text txStatValue;

            // 옵션 스탯
            public Text txOpStatCombat; // 전투력 
            public Text txOpStatCombatCompare;
            public Image[] imOpStatIcon;
            public Text[] txOpStatNames;
            public Text[] txOpStatValues;

            // 장신구 옵션 스탯
            public GameObject goAcceOpStatRoot;
            public Image imAcceOpStatIcon;
            public Text txAccOpStatTitle;
            public Text txAccOpStatName;
            public Text txAcceOpStatValue;

            // 마석 옵션 스탯
            public GameObject goMagicOpStatRoot;
            public Image imMagicOpStatIcon;
            public Text txMagicOpStatName;
            public Text txMagicOpStatValue;
        }

        [System.Serializable]
        public struct Info_button
        {
            public GameObject goBtnBottomGroup; // 하단 버튼 그룹 
            public GameObject goTopBtnGroup; // 상단 판매/분해 버튼 그룹 
            public GameObject goBottomBtnGroup1; // 하단 버튼 그룹 1. 전승, 강화, 레벨, 장착  
            public GameObject goBottomBtnGroup2; // 하단 버튼 그룹 2. 판매, 분해 
            public GameObject goBottomBtnGroup3; // 하단 버튼 그룹 3. 레벨업 100, 10, 1, 닫기 

            public Text txBottomGroupLvCnt1;
            public Text txBottomGroupLvCnt2;
            public Text txBottomGroupLvCnt3;
            public GameObject goBottimGroup3BtnLv1;
            public GameObject goBottimGroup3BtnLv10;
            public GameObject goBottimGroup3BtnLv100;
            public GameObject goBottomGroup3UpBtn;

            public Toggle toBasicStat; // 기본 스탯 보기  
            public Image imBtnLock; // 잠금 버튼 image 

            public Button[] btnDbChangeActions;
            public GameObject btnWearUpDown;

            public Button btnSale;
            public Button btnDecomp;
            public GameObject btnMoveAcceOpChange;

            public GameObject btnLegendUpgrade;
        }

        [System.Serializable]
        public struct Info_etc
        {
            public RectTransform retRoot;
            public Text txNormLevel1UpPrice;
            public Text txNormLevel10UpPrice;
            public Text txNormLevel100UpPrice;
            public Image imNormLevel1UpBtnBg;
            public Image imNormLevel10UpBtnBg;
            public Image imNormLevel100UpBtnBg;
            public Image imLockBig;
            public DateTime dtNextLock;
            public Text txDtNextLock;
        }
    }

    [SerializeField] EquipValue wearEquipValue;
    [SerializeField] EquipValue selectEquipValue;

    [System.Serializable]
    public struct EquipValue
    {
        public int m_parts_type;
        public int m_main_stat_id;
        public int m_lock;
        public int m_rating;
        public int m_idx;
        public int m_enhant_level;
        public int m_normal_level;
        public int m_legend;
        public int m_legend_sop_id;
        public int m_legend_sop_rlv;

        public long m_combat_total;
        public long m_combat_main_stat;
        public long m_combat_op_stat;
        public long m_combat_sop_stat;

        public StatValue statValue;
        [System.Serializable]
        public struct StatValue
        {
            public object[] main_stat_value; // [0] -> 기본값, 강화, 레벨업 스탯 총합, [1] -> 강화,레벨업으로 증가한 스탯 값 
            public StatOp[] stat_op;
            [System.Serializable]
            public struct StatOp
            {
                public GameDatabase.TableDB.StatOp op;
                public object op_stat_value;
            }
        }
    }

    [SerializeField] private bool isOpenShop = false;
    [SerializeField]
    private bool isNoneWear = false;
    UnityAction actionRefresh;
    [SerializeField] bool isToggleInit = false;
    Sprite transp;
    [SerializeField] Sprite[] statIcon = new Sprite[10];
    [SerializeField]  Sprite[] acceOpStatIcon = new Sprite[11];

    void Awake()
    {
        transp = SpriteAtlasMng.GetInstance().GetTransparency();
        for (int i = 0; i < statIcon.Length; i++)
        {
            statIcon[i] = SpriteAtlasMng.GetInstance().GetSpriteStatIcon(i);
        }

        for (int i = 0; i < acceOpStatIcon.Length; i++)
        {
            acceOpStatIcon[i] = SpriteAtlasMng.GetInstance().GetSpriteAcceSpOpStatIcon(i);
        }
    }

    public void SetData(GameDatabase.TableDB.Equipment equip_db, bool isBotmBtnHide, UnityAction dbChngAction = null, bool isOpShop = false, bool isMyDb = true)
    {
        int t = equip_db.eq_ty;
        bool isEquipAcce = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(t);

        isOpenShop = isOpShop;
        isLevelUp = false;
        isWearChange = false;
        isWearLockChange = false;
        isSelectLockChange = false;

        ToggleInit();
        actionRefresh = dbChngAction;

        if (isMyDb)
        {
            #region ##### 내 장비 #####
            // 선택한 장비가 찾용 장비인지 먼저 체크 
            var nowWearEquip = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(equip_db.eq_ty);
            isNoneWear = string.IsNullOrEmpty(nowWearEquip.indate);
            bool isSelectWear = string.Equals(nowWearEquip.indate, equip_db.indate) && string.Equals(nowWearEquip.aInUid, equip_db.aInUid);
            if (isSelectWear) // 착용중인 장비를 선택헀다. 1개만 보여줌 
            {
                wear.goTap.SetActive(true);
                select.goTap.SetActive(false);
                SetWearEquipOn(nowWearEquip);
            }
            else
            {
                equip_db.m_norm_lv = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(equip_db.eq_ty).m_norm_lv;
                // 착용 중인 장비를 선택하지 않았고, 현재 착용중인 장비가 있다. 
                if (isNoneWear == false)
                {
                    wear.goTap.SetActive(true);
                    select.goTap.SetActive(true);
                    SetWearEquipOn(nowWearEquip);
                    SetSelectEquipOn(equip_db);
                }
                // 착용 중인 장비를 선택하지 않았고, 현재 착용중인 장비가 없다 
                else
                {
                    wear.goTap.SetActive(false);
                    select.goTap.SetActive(true);
                    equip_db.m_norm_lv = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(equip_db.eq_ty).m_norm_lv;
                    SetSelectEquipOn(equip_db);
                }
            }

            if (isBotmBtnHide == false)
            {
                wear.info_button.goBottomBtnGroup1.SetActive(true);
                wear.info_button.goBottomBtnGroup2.SetActive(false);
                wear.info_button.goBottomBtnGroup3.SetActive(false);

                select.info_button.goBottomBtnGroup1.SetActive(true);
                select.info_button.goBottomBtnGroup2.SetActive(false);
                select.info_button.goBottomBtnGroup3.SetActive(false);

                //wear.info_button.btnWearUpDown.SetActive(wear.goTap.activeSelf && select.goTap.activeSelf);
                if (string.IsNullOrEmpty(nowWearEquip.indate))
                {
                    select.info_button.btnWearUpDown.SetActive(true);
                }
                else select.info_button.btnWearUpDown.SetActive(wear.goTap.activeSelf && select.goTap.activeSelf);
            }

            StatCompare();
            #endregion
        }
        else
        {
            #region ##### 다른 유저 장비 #####
            wear.goTap.SetActive(true);
            select.goTap.SetActive(false);
            SetWearEquipOn(equip_db);

            wear.info_button.goBottomBtnGroup2.SetActive(false);
            wear.info_button.goBottomBtnGroup3.SetActive(false);
            #endregion
        }

        if (wear.goTap.activeSelf)
        {
            wear.info_button.btnLegendUpgrade.SetActive(wear.eqDb.eq_rt == 7 && wear.eqDb.eq_legend == 0 && wear.eqDb.eq_ty <= 7);
            wear.info_button.btnMoveAcceOpChange.SetActive(isEquipAcce || (wear.eqDb.eq_rt == 7 && wear.eqDb.eq_legend == 1 && wear.eqDb.eq_ty <= 7));
            wear.info_button.goBtnBottomGroup.SetActive(!isBotmBtnHide);
            wear.info_etc.retRoot.sizeDelta = new Vector2(910, isBotmBtnHide == true ? 400 : 485);
            wear.info_button.toBasicStat.gameObject.SetActive(!isBotmBtnHide);
            wear.info_button.imBtnLock.gameObject.SetActive(!isBotmBtnHide);
        }

        if (select.goTap.activeSelf)
        {
            select.info_button.btnLegendUpgrade.SetActive(select.eqDb.eq_rt == 7 && select.eqDb.eq_legend == 0 && select.eqDb.eq_ty <= 7);
            select.info_button.btnMoveAcceOpChange.SetActive(isEquipAcce || (select.eqDb.eq_rt == 7 && select.eqDb.eq_legend == 1 && select.eqDb.eq_ty <= 7));
            select.info_button.goBtnBottomGroup.SetActive(!isBotmBtnHide);
            select.info_etc.retRoot.sizeDelta = new Vector2(910, isBotmBtnHide == true ? 400 : 485);
            select.info_button.toBasicStat.gameObject.SetActive(!isBotmBtnHide);
            select.info_button.imBtnLock.gameObject.SetActive(!isBotmBtnHide);
        }
        
        //if (isBotmBtnHide == false)
        //{
        //    wear.info_button.goBottomBtnGroup1.SetActive(true);
        //    wear.info_button.goBottomBtnGroup2.SetActive(false);
        //    wear.info_button.goBottomBtnGroup3.SetActive(false);

        //    select.info_button.goBottomBtnGroup1.SetActive(true);
        //    select.info_button.goBottomBtnGroup2.SetActive(false);
        //    select.info_button.goBottomBtnGroup3.SetActive(false);

        //    //wear.info_button.btnWearUpDown.SetActive(wear.goTap.activeSelf && select.goTap.activeSelf);
        //    if(string.IsNullOrEmpty(nowWearEquip.indate))
        //    {
        //        select.info_button.btnWearUpDown.SetActive(true);
        //    }
        //    else select.info_button.btnWearUpDown.SetActive(wear.goTap.activeSelf && select.goTap.activeSelf);
        //}

        StopCoroutine("WearLockDate"); StartCoroutine("WearLockDate");
        StopCoroutine("SelectLockDate"); StartCoroutine("SelectLockDate");
    }

    void ToggleInit()
    {
        isToggleInit = false;
        wear.info_button.toBasicStat.isOn = false;
        select.info_button.toBasicStat.isOn = false;
        isToggleInit = true;
    }

    /// <summary>
    /// 선택한 장비와 착용중인 장비 스탯 비교 
    /// </summary>
    void StatCompare() //GameDatabase.TableDB.Equipment wearEqDb, GameDatabase.TableDB.Equipment selectEqDb)
    {
        if (wear.goTap.activeSelf && select.goTap.activeSelf)
        {
            // 총 전투력 
            select.info_icon.txAllCombatCompare.text = GameDatabase.GetInstance().tableDB.GetCombatCompare(wearEquipValue.m_combat_total, selectEquipValue.m_combat_total);

            //----- 매인 스탯 -----
            //-- 전투력 --
            select.info_stat.txStatCombatCompare.text = GameDatabase.GetInstance().tableDB.GetCombatCompare(wearEquipValue.m_combat_main_stat, selectEquipValue.m_combat_main_stat);
           
            //-- 스탯 --
            object[] wer_stat_val = wearEquipValue.statValue.main_stat_value;
            object[] select_stat_val = selectEquipValue.statValue.main_stat_value;
            Type val_type = wer_stat_val[0].GetType();
            if (val_type == typeof(float))
                select.info_stat.txStatValue.text = string.Format("{0:0.000} {1}", (float)select_stat_val[0], GameDatabase.GetInstance().tableDB.GetStatCompare((float)wer_stat_val[0], (float)select_stat_val[0]));
            else if (val_type == typeof(long))
                select.info_stat.txStatValue.text = string.Format("{0:#,0} {1}", (long)select_stat_val[0], GameDatabase.GetInstance().tableDB.GetStatCompare((long)wer_stat_val[0], (long)select_stat_val[0]));

            //----- 옵션 스탯 -----
            //-- 전투력 --
            select.info_stat.txOpStatCombatCompare.text = GameDatabase.GetInstance().tableDB.GetStatCompare(wearEquipValue.m_combat_op_stat, selectEquipValue.m_combat_op_stat);
        }
        else
        {
            select.info_icon.txAllCombatCompare.text = "";
            select.info_stat.txStatCombatCompare.text = "";
            select.info_stat.txOpStatCombatCompare.text = "";
        }
    }

    /// <summary>
    /// 장비 데이터를 가지고 스탯 및 전투력 데이터 리턴 
    /// </summary>
    private EquipValue SettingEquipValue(GameDatabase.TableDB.Equipment eqDb)
    {
        try
        {
            //EquipValue ev = new EquipValue();
            //ev.m_parts_type = eqDb.eq_ty;
            //ev.m_main_stat_id = eqDb.ma_st_id;
            //ev.m_lock = eqDb.m_lck;
            //ev.m_rating = eqDb.eq_rt;
            //ev.m_idx = eqDb.eq_id;
            //ev.m_enhant_level = eqDb.m_ehnt_lv;
            //ev.m_normal_level = eqDb.m_norm_lv;
            //ev.m_legend = eqDb.eq_legend;
            //ev.m_legend_sop_id = eqDb.eq_legend_sop_id;
            //ev.m_legend_sop_rlv = eqDb.eq_legend_sop_rlv;
            //LogPrint.EditorPrint("1");
            //ev.m_combat_main_stat = GameDatabase.GetInstance().tableDB.GetEquipCombatPower(eqDb, "main");
            //LogPrint.EditorPrint("2");
            //ev.m_combat_op_stat = GameDatabase.GetInstance().tableDB.GetEquipCombatPower(eqDb, "op");
            //LogPrint.EditorPrint("3");
            //ev.m_combat_sop_stat = GameDatabase.GetInstance().tableDB.GetEquipCombatPower(eqDb, "sop");
            //LogPrint.EditorPrint("4");
            //ev.m_combat_total = GameDatabase.GetInstance().tableDB.GetEquipCombatPower(eqDb, "total");
            //LogPrint.EditorPrint("5");
            //ev.statValue.main_stat_value = GameDatabase.GetInstance().chartDB.GetMainStatValue(eqDb);
            //LogPrint.EditorPrint("6");
            //LogPrint.EditorPrint("eqDb.st_op ; " + JsonUtility.ToJson(eqDb.st_op));
            //ev.statValue.stat_op = new EquipValue.StatValue.StatOp[4];
            //ev.statValue.stat_op[0] = new EquipValue.StatValue.StatOp() { op = eqDb.st_op.op1, op_stat_value = (long)GameDatabase.GetInstance().chartDB.GetEquipOptionStatValue(eqDb.st_op.op1.id,eqDb.st_op.op1.rlv, eqDb.eq_rt, eqDb.eq_id, eqDb.m_ehnt_lv, true, eqDb.eq_legend, eqDb.eq_legend_sop_id, eqDb.eq_legend_sop_rlv) };
            //ev.statValue.stat_op[1] = new EquipValue.StatValue.StatOp() { op = eqDb.st_op.op2, op_stat_value = (long)GameDatabase.GetInstance().chartDB.GetEquipOptionStatValue(eqDb.st_op.op2.id,eqDb.st_op.op2.rlv, eqDb.eq_rt, eqDb.eq_id, eqDb.m_ehnt_lv, true, eqDb.eq_legend, eqDb.eq_legend_sop_id, eqDb.eq_legend_sop_rlv) };
            //ev.statValue.stat_op[2] = new EquipValue.StatValue.StatOp() { op = eqDb.st_op.op3, op_stat_value = (long)GameDatabase.GetInstance().chartDB.GetEquipOptionStatValue(eqDb.st_op.op3.id,eqDb.st_op.op3.rlv, eqDb.eq_rt, eqDb.eq_id, eqDb.m_ehnt_lv, true, eqDb.eq_legend, eqDb.eq_legend_sop_id, eqDb.eq_legend_sop_rlv) };
            //ev.statValue.stat_op[3] = new EquipValue.StatValue.StatOp() { op = eqDb.st_op.op4, op_stat_value = (long)GameDatabase.GetInstance().chartDB.GetEquipOptionStatValue(eqDb.st_op.op4.id, eqDb.st_op.op4.rlv, eqDb.eq_rt, eqDb.eq_id, eqDb.m_ehnt_lv, true, eqDb.eq_legend, eqDb.eq_legend_sop_id, eqDb.eq_legend_sop_rlv) };
            //LogPrint.EditorPrint("7");
            //return ev;

            return new EquipValue()
            {
                m_parts_type = eqDb.eq_ty,
                m_main_stat_id = eqDb.ma_st_id,
                m_lock = eqDb.m_lck,
                m_rating = eqDb.eq_rt,
                m_idx = eqDb.eq_id,
                m_enhant_level = eqDb.m_ehnt_lv,
                m_normal_level = eqDb.m_norm_lv,
                m_legend = eqDb.eq_legend,
                m_legend_sop_id = eqDb.eq_legend_sop_id,
                m_legend_sop_rlv = eqDb.eq_legend_sop_rlv,

                m_combat_main_stat = GameDatabase.GetInstance().tableDB.GetEquipCombatPower(eqDb, "main"),
                m_combat_op_stat = GameDatabase.GetInstance().tableDB.GetEquipCombatPower(eqDb, "op"),
                m_combat_sop_stat = GameDatabase.GetInstance().tableDB.GetEquipCombatPower(eqDb, "sop"),
                m_combat_total = GameDatabase.GetInstance().tableDB.GetEquipCombatPower(eqDb, "total"),

                statValue = new EquipValue.StatValue()
                {
                    main_stat_value = GameDatabase.GetInstance().chartDB.GetMainStatValue(eqDb),
                    stat_op = new EquipValue.StatValue.StatOp[]
                    {
                        new EquipValue.StatValue.StatOp()
                            { op = eqDb.st_op.op1, op_stat_value = (long)GameDatabase.GetInstance().chartDB.GetEquipOptionStatValue(eqDb.st_op.op1.id,eqDb.st_op.op1.rlv, eqDb.eq_rt, eqDb.eq_id, eqDb.m_ehnt_lv, true, eqDb.eq_legend, eqDb.eq_legend_sop_id, eqDb.eq_legend_sop_rlv) },
                        new EquipValue.StatValue.StatOp()
                            { op = eqDb.st_op.op2, op_stat_value = (long)GameDatabase.GetInstance().chartDB.GetEquipOptionStatValue(eqDb.st_op.op2.id,eqDb.st_op.op2.rlv, eqDb.eq_rt, eqDb.eq_id, eqDb.m_ehnt_lv, true, eqDb.eq_legend, eqDb.eq_legend_sop_id, eqDb.eq_legend_sop_rlv) },
                        new EquipValue.StatValue.StatOp()
                            { op = eqDb.st_op.op3, op_stat_value = (long)GameDatabase.GetInstance().chartDB.GetEquipOptionStatValue(eqDb.st_op.op3.id,eqDb.st_op.op3.rlv, eqDb.eq_rt, eqDb.eq_id, eqDb.m_ehnt_lv, true, eqDb.eq_legend, eqDb.eq_legend_sop_id, eqDb.eq_legend_sop_rlv) },
                        new EquipValue.StatValue.StatOp()
                            { op = eqDb.st_op.op4, op_stat_value = (long)GameDatabase.GetInstance().chartDB.GetEquipOptionStatValue(eqDb.st_op.op4.id,eqDb.st_op.op4.rlv, eqDb.eq_rt, eqDb.eq_id, eqDb.m_ehnt_lv, true, eqDb.eq_legend, eqDb.eq_legend_sop_id, eqDb.eq_legend_sop_rlv) },
                    }
                },
            };
        }
        catch (Exception) { return new EquipValue(); }
    }

    // -----------------------------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------------------------
    #region 장비 정보 세팅 
    // 착용중 
    void SetWearEquipOn(GameDatabase.TableDB.Equipment eqDb, bool isBasic = false)
    {
        // 데이터 세팅 
        LogPrint.Print("isBasic : " + isBasic);
        if (isBasic == false)
            wear.eqDb = eqDb;

        wearEquipValue = SettingEquipValue(eqDb);
        LogPrint.EditorPrint("<<<<< 착용 >>>>> eqDb ================== : " + JsonUtility.ToJson(eqDb));
        LogPrint.EditorPrint("<<<<< 착용 >>>>> ================== : " + JsonUtility.ToJson(wearEquipValue.statValue.main_stat_value));

        // 아이콘 정보
        wear.info_icon.imIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(wearEquipValue.m_parts_type, wearEquipValue.m_rating, wearEquipValue.m_idx);
        wear.info_icon.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(wearEquipValue.m_rating, wearEquipValue.m_legend);
        wear.info_icon.txRating.text = GameDatabase.StringFormat.GetRatingColorText(wearEquipValue.m_rating, false);
        wear.info_icon.goLock.SetActive(wearEquipValue.m_lock == 1);
        wear.info_icon.txEnhantLevel.text = string.Format("+{0}", wearEquipValue.m_enhant_level);
        wear.info_icon.txLevel.text = string.Format("Lv.{0}", wearEquipValue.m_normal_level);
        wear.info_icon.txEquipName.text = GameDatabase.StringFormat.GetEquipName(wearEquipValue.m_parts_type, wearEquipValue.m_rating, wearEquipValue.m_idx);
        wear.info_etc.imLockBig.color = wearEquipValue.m_lock == 1 ? wear.info_icon.coOkLock : wear.info_icon.coNoLock;

        // 레벨 이펙트 
        if (wear.info_icon.goRootEnhant30.activeSelf == !(wearEquipValue.m_enhant_level >= 30))
            wear.info_icon.goRootEnhant30.SetActive(wearEquipValue.m_enhant_level >= 30);

        if (wearEquipValue.m_enhant_level >= 30)
        {
            wear.info_icon.imRtOutlineEnhant30.color = ResourceDatabase.GetInstance().GetItemColor(wearEquipValue.m_rating);
            wear.info_icon.ugEnhant30.color1 = ResourceDatabase.GetInstance().GetItemGradientColor(wearEquipValue.m_rating);
            wear.info_icon.ugEnhant30.color2 = ResourceDatabase.GetInstance().GetItemGradientColor(wearEquipValue.m_rating);
        }

        if (wear.info_icon.goRootNorLv100.activeSelf == !(wearEquipValue.m_normal_level >= 100))
            wear.info_icon.goRootNorLv100.SetActive(wearEquipValue.m_normal_level >= 100);

        if (wearEquipValue.m_normal_level >= 100)
        {
            wear.info_icon.imRtOutlineNorLv100.color = ResourceDatabase.GetInstance().GetItemColor(wearEquipValue.m_rating);
            wear.info_icon.psNorLv100.startColor = ResourceDatabase.GetInstance().GetItemColor(wearEquipValue.m_rating);
        }

        // 스탯 정보
        wear.info_stat.imStatIcon.sprite = statIcon[wearEquipValue.m_main_stat_id];
        wear.info_stat.txStatName.text = GameDatabase.StringFormat.GetEquipStatName(wearEquipValue.m_main_stat_id); // 매인 스탯 이름 
        wear.info_stat.txStatCombat.text = string.Format("{0:#,0}", wearEquipValue.m_combat_main_stat);
        
        object[] wer_mast_val = wearEquipValue.statValue.main_stat_value;
        LogPrint.EditorPrint("wer_mast_val[0]:" + wer_mast_val[0]);
        LogPrint.EditorPrint("wer_mast_val[1]:" + wer_mast_val[1]);
        Type val_type = wer_mast_val[0].GetType();
        if (val_type == typeof(float))
            wear.info_stat.txStatValue.text = string.Format("{0:0.000}(+{1:0.000})", (float)wer_mast_val[0], (float)wer_mast_val[1]);
        else if (val_type == typeof(long))
            wear.info_stat.txStatValue.text = string.Format("{0:#,0}(+{1:#,0})", (long)wer_mast_val[0], (long)wer_mast_val[1]);

        // 옵션 
        wear.info_stat.txOpStatCombat.text = string.Format("{0:#,0}", wearEquipValue.m_combat_op_stat);
        for (int i = 0; i < 4; i++)
        {
            int so_id = wearEquipValue.statValue.stat_op[i].op.id;
            if (so_id > 0)
            {
                object opst_val = wearEquipValue.statValue.stat_op[i].op_stat_value;
                Type wer_opst_val = opst_val.GetType();
                if (wer_opst_val == typeof(float))
                    wear.info_stat.txOpStatValues[i].text = string.Format("+{0:0.000}", (float)opst_val);
                else if (wer_opst_val == typeof(long))
                    wear.info_stat.txOpStatValues[i].text = string.Format("+{0:#,0}", (long)opst_val);

                wear.info_stat.txOpStatNames[i].text = so_id == 0 ? "" : GameDatabase.StringFormat.GetEquipStatName(so_id);
                wear.info_stat.imOpStatIcon[i].sprite = statIcon[so_id];
                Color cor = so_id > 0 && int.Equals(wearEquipValue.m_legend, 1) && int.Equals(so_id, wearEquipValue.m_legend_sop_id) ? Color.yellow : Color.white;
                wear.info_stat.txOpStatNames[i].color = cor;
                wear.info_stat.txOpStatValues[i].color = cor;
            }
            else
            {
                wear.info_stat.txOpStatNames[i].text = "-";
                wear.info_stat.txOpStatValues[i].text = "-";
                wear.info_stat.imOpStatIcon[i].sprite = transp;
                wear.info_stat.txOpStatNames[i].color = Color.white;
                wear.info_stat.txOpStatValues[i].color = Color.white;
            }
        }

        wear.info_stat.goMagicOpStatRoot.SetActive(false);

        // 장신구 옵션 
        if (GameDatabase.GetInstance().chartDB.GetIsPartsTypeAcce(wearEquipValue.m_parts_type))
        {
            wear.info_stat.goAcceOpStatRoot.SetActive(true);
            wear.info_stat.goMagicOpStatRoot.SetActive(false);
            if (eqDb.st_sop_ac.id > 0)
            {
                float[] wer_acSop_statVal = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionValue(eqDb);
                wear.info_stat.imAcceOpStatIcon.sprite = acceOpStatIcon[eqDb.st_sop_ac.id];
                wear.info_stat.txAccOpStatTitle.text = "<color=#FFAD00>장신구 전용 옵션</color>";
                wear.info_stat.txAccOpStatName.text = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionName(eqDb.st_sop_ac.id);
                wear.info_stat.txAcceOpStatValue.text = string.Format("+{0:0.000}%", wer_acSop_statVal[0]);
            }
            else
            {
                wear.info_stat.imAcceOpStatIcon.sprite = transp;
                wear.info_stat.txAccOpStatTitle.text = "<color=#FFAD00>장신구 전용 옵션</color>";
                wear.info_stat.txAccOpStatName.text = "";
                wear.info_stat.txAcceOpStatValue.text = "-";
            }
        }
        else
        {
            wear.info_stat.goAcceOpStatRoot.SetActive(eqDb.eq_legend >= 1);
            if (eqDb.eq_legend >= 1)
            {
                wear.info_stat.txAccOpStatTitle.text = "<color=#FFE800>장비 전용 옵션</color>";
                wear.info_stat.txAccOpStatName.text = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("equip.sop.id_{0}", eqDb.eq_legend_sop_id)).val_string;
                wear.info_stat.txAcceOpStatValue.text = string.Format("+{0:0.000}%", GameDatabase.GetInstance().chartDB.GetEquipSpecialOptionValue(eqDb.eq_legend_sop_id, eqDb.eq_legend_sop_rlv));
            }
        }

        // 총 전투력 총합 
        wear.info_icon.txAllCombat.text = string.Format("{0:#,0}", wearEquipValue.m_combat_total);

        // 버튼 정보 
        int maxNormLv = GameDatabase.GetInstance().chartDB.GetDicBalance("eq.normal.max.level").val_int;
        //wear.info_button.goBottomGroup3UpBtn.SetActive(eqDb.m_norm_lv < maxNormLv);
        if (eqDb.m_norm_lv < maxNormLv)
        {
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            wear.info_button.goBottimGroup3BtnLv1.SetActive(maxNormLv - eqDb.m_norm_lv >= 1);
            wear.info_button.goBottimGroup3BtnLv10.SetActive(maxNormLv - eqDb.m_norm_lv >= 10);
            wear.info_button.goBottimGroup3BtnLv100.SetActive(maxNormLv - eqDb.m_norm_lv > 10);
            if (wear.info_button.goBottimGroup3BtnLv1.activeSelf)
            {
                long lv_up_gold = GameDatabase.GetInstance().questDB.GetQuestEquipLevelUpGold(eqDb.eq_rt, eqDb.eq_id, eqDb.m_norm_lv, eqDb.m_norm_lv + 1);
                wear.info_etc.txNormLevel1UpPrice.text = string.Format("{0:#,0}", lv_up_gold);
                wear.info_button.txBottomGroupLvCnt1.text = maxNormLv - eqDb.m_norm_lv >= 1 ? "+1 UP" : "";
                wear.info_etc.imNormLevel1UpBtnBg.color = goods_db.m_gold >= lv_up_gold ? new Color(0, 0.58f, 1, 1) : new Color(0.67f, 0.67f, 0.67f, 0.67f);
            }
            if (wear.info_button.goBottimGroup3BtnLv10.activeSelf)
            {
                long lv_up_gold = GameDatabase.GetInstance().questDB.GetQuestEquipLevelUpGold(eqDb.eq_rt, eqDb.eq_id, eqDb.m_norm_lv, eqDb.m_norm_lv + 10);
                wear.info_etc.txNormLevel10UpPrice.text = string.Format("{0:#,0}", lv_up_gold);
                wear.info_button.txBottomGroupLvCnt2.text = maxNormLv - eqDb.m_norm_lv >= 10 ? "+10 UP" : string.Format("+{0} UP", (maxNormLv - eqDb.m_norm_lv).ToString());
                wear.info_etc.imNormLevel10UpBtnBg.color = goods_db.m_gold >= lv_up_gold ? new Color(0, 0.58f, 1, 1) : new Color(0.67f, 0.67f, 0.67f, 0.67f);
            }
            if (wear.info_button.goBottimGroup3BtnLv100.activeSelf)
            {
                long lv_up_gold = GameDatabase.GetInstance().questDB.GetQuestEquipLevelUpGold(eqDb.eq_rt, eqDb.eq_id, eqDb.m_norm_lv, 100);
                wear.info_etc.txNormLevel100UpPrice.text = string.Format("{0:#,0}", lv_up_gold);
                wear.info_button.txBottomGroupLvCnt3.text = maxNormLv - eqDb.m_norm_lv >= 100 ? "+100 UP" : string.Format("+{0} UP", (maxNormLv - eqDb.m_norm_lv).ToString());
                wear.info_etc.imNormLevel100UpBtnBg.color = goods_db.m_gold >= lv_up_gold ? new Color(0, 0.58f, 1, 1) : new Color(0.67f, 0.67f, 0.67f, 0.67f);
            }
        }
        else
        {
            if(wear.info_button.goBottomBtnGroup3.gameObject.activeSelf)
            {
                wear.info_button.goBottomBtnGroup1.SetActive(true);
                wear.info_button.goBottomBtnGroup3.SetActive(false);
            }
        }
    }

    // 선택함 
    void SetSelectEquipOn(GameDatabase.TableDB.Equipment eqDb, bool isBasic = false)
    {
        // 데이터 세팅 
        if (isBasic == false)
            select.eqDb = eqDb;
        
        selectEquipValue = SettingEquipValue(eqDb);
        LogPrint.EditorPrint("<<<<< 선택 >>>>> eqDb ================== : " + JsonUtility.ToJson(eqDb));
        LogPrint.EditorPrint("<<<<< 선택 >>>>> ================== : " + JsonUtility.ToJson(selectEquipValue.statValue.main_stat_value));

        // 아이콘 정보
        select.info_icon.imIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(selectEquipValue.m_parts_type, selectEquipValue.m_rating, selectEquipValue.m_idx);
        select.info_icon.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(selectEquipValue.m_rating, selectEquipValue.m_legend);
        select.info_icon.txRating.text = GameDatabase.StringFormat.GetRatingColorText(selectEquipValue.m_rating, false);
        select.info_icon.goLock.SetActive(selectEquipValue.m_lock == 1);
        select.info_icon.txEnhantLevel.text = string.Format("+{0}", selectEquipValue.m_enhant_level);
        select.info_icon.txLevel.text = string.Format("Lv.{0}", selectEquipValue.m_normal_level);
        select.info_icon.txEquipName.text = GameDatabase.StringFormat.GetEquipName(selectEquipValue.m_parts_type, selectEquipValue.m_rating, selectEquipValue.m_idx);
        select.info_icon.goInvenNew.SetActive(eqDb.client_add_sp > MainUI.GetInstance().inventory.tapOpenSP);
        select.info_etc.imLockBig.color = selectEquipValue.m_lock == 1 ? select.info_icon.coOkLock : select.info_icon.coNoLock;

        // 레벨 이펙트 
        if (select.info_icon.goRootEnhant30.activeSelf == !(selectEquipValue.m_enhant_level >= 30))
            select.info_icon.goRootEnhant30.SetActive(selectEquipValue.m_enhant_level >= 30);

        if (selectEquipValue.m_enhant_level >= 30)
        {
            select.info_icon.imRtOutlineEnhant30.color = ResourceDatabase.GetInstance().GetItemColor(selectEquipValue.m_rating);
            select.info_icon.ugEnhant30.color1 = ResourceDatabase.GetInstance().GetItemGradientColor(selectEquipValue.m_rating);
            select.info_icon.ugEnhant30.color2 = ResourceDatabase.GetInstance().GetItemGradientColor(selectEquipValue.m_rating);
        }

        if (select.info_icon.goRootNorLv100.activeSelf == !(selectEquipValue.m_normal_level >= 100))
            select.info_icon.goRootNorLv100.SetActive(selectEquipValue.m_normal_level >= 100);

        if (selectEquipValue.m_normal_level >= 100)
        {
            select.info_icon.imRtOutlineNorLv100.color = ResourceDatabase.GetInstance().GetItemColor(selectEquipValue.m_rating);
            select.info_icon.psNorLv100.startColor = ResourceDatabase.GetInstance().GetItemColor(selectEquipValue.m_rating);
        }

        // 스탯 정보
        select.info_stat.imStatIcon.sprite = statIcon[selectEquipValue.m_main_stat_id];
        select.info_stat.txStatName.text = GameDatabase.StringFormat.GetEquipStatName(selectEquipValue.m_main_stat_id); // 매인 스탯 이름 
        select.info_stat.txStatCombat.text = string.Format("{0:#,0}", selectEquipValue.m_combat_main_stat);
        object[] sel_mast_val = selectEquipValue.statValue.main_stat_value;
        LogPrint.EditorPrint("sel_mast_val[0]:" + sel_mast_val[0]);
        LogPrint.EditorPrint("sel_mast_val[1]:" + sel_mast_val[1]);
        Type val_type = sel_mast_val[0].GetType();
        if (val_type == typeof(float))
            select.info_stat.txStatValue.text = string.Format("{0:0.000}(+{1:0.000})", (float)sel_mast_val[0], (float)sel_mast_val[1]);
        else if (val_type == typeof(long))
            select.info_stat.txStatValue.text = string.Format("{0:#,0}(+{1:#,0})", (long)sel_mast_val[0], (long)sel_mast_val[1]);

        // 옵션 
        select.info_stat.txOpStatCombat.text = string.Format("{0:#,0}", selectEquipValue.m_combat_op_stat);
        for (int i = 0; i < 4; i++)
        {
            int so_id = selectEquipValue.statValue.stat_op[i].op.id;
            if (so_id > 0)
            {
                object opst_val = selectEquipValue.statValue.stat_op[i].op_stat_value;
                Type wer_opst_val = opst_val.GetType();
                if (wer_opst_val == typeof(float))
                    select.info_stat.txOpStatValues[i].text = string.Format("+{0:0.000}", (float)opst_val);
                else if (wer_opst_val == typeof(long))
                    select.info_stat.txOpStatValues[i].text = string.Format("+{0:#,0}", (long)opst_val);

                select.info_stat.txOpStatNames[i].text = so_id == 0 ? "" : GameDatabase.StringFormat.GetEquipStatName(so_id);
                select.info_stat.imOpStatIcon[i].sprite = statIcon[so_id];

                Color cor = so_id > 0 && int.Equals(selectEquipValue.m_legend, 1) && int.Equals(so_id, selectEquipValue.m_legend_sop_id) ? Color.yellow : Color.white;
                select.info_stat.txOpStatNames[i].color = cor;
                select.info_stat.txOpStatValues[i].color = cor;
            }
            else
            {
                select.info_stat.txOpStatNames[i].text = "-";
                select.info_stat.txOpStatValues[i].text = "-";
                select.info_stat.imOpStatIcon[i].sprite = transp;
                select.info_stat.txOpStatNames[i].color = Color.white;
                select.info_stat.txOpStatValues[i].color = Color.white;
            }
        }

        select.info_stat.goMagicOpStatRoot.SetActive(false);

        // 전용 옵션 
        if (GameDatabase.GetInstance().chartDB.GetIsPartsTypeAcce(selectEquipValue.m_parts_type))
        {
            select.info_stat.goAcceOpStatRoot.SetActive(true);
            select.info_stat.goMagicOpStatRoot.SetActive(false);
            if (eqDb.st_sop_ac.id > 0)
            {
                float[] wer_acSop_statVal = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionValue(eqDb);
                select.info_stat.imAcceOpStatIcon.sprite = acceOpStatIcon[eqDb.st_sop_ac.id];
                select.info_stat.txAccOpStatTitle.text = "<color=#FFAD00>장신구 전용 옵션</color>";
                select.info_stat.txAccOpStatName.text = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionName(eqDb.st_sop_ac.id);
                select.info_stat.txAcceOpStatValue.text = string.Format("+{0:0.000}%", wer_acSop_statVal[0]);
            }
            else
            {
                select.info_stat.imAcceOpStatIcon.sprite = transp;
                select.info_stat.txAccOpStatTitle.text = "";
                select.info_stat.txAccOpStatName.text = "";
                select.info_stat.txAcceOpStatValue.text = "-";
            }
        }
        else
        {
            select.info_stat.goAcceOpStatRoot.SetActive(eqDb.eq_legend >= 1);
            if (eqDb.eq_legend >= 1)
            {
                select.info_stat.txAccOpStatTitle.text = "<color=#FFE800>장비 전용 옵션</color>";
                select.info_stat.txAccOpStatName.text = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("equip.sop.id_{0}", eqDb.eq_legend_sop_id)).val_string;
                select.info_stat.txAcceOpStatValue.text = string.Format("+{0:0.000}%", GameDatabase.GetInstance().chartDB.GetEquipSpecialOptionValue(eqDb.eq_legend_sop_id, eqDb.eq_legend_sop_rlv));
            }
        }

        // 총 전투력 총합 
        select.info_icon.txAllCombat.text = string.Format("{0:#,0}", selectEquipValue.m_combat_total);

        // 버튼 정보 
        int maxNormLv = GameDatabase.GetInstance().chartDB.GetDicBalance("eq.normal.max.level").val_int;
        //select.info_button.goBottomGroup3UpBtn.SetActive(eqDb.m_norm_lv < maxNormLv);
        if (eqDb.m_norm_lv < maxNormLv)
        {
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            select.info_button.goBottimGroup3BtnLv1.SetActive(maxNormLv - eqDb.m_norm_lv >= 1);
            select.info_button.goBottimGroup3BtnLv10.SetActive(maxNormLv - eqDb.m_norm_lv >= 10);
            select.info_button.goBottimGroup3BtnLv100.SetActive(maxNormLv - eqDb.m_norm_lv > 10);
            if (select.info_button.goBottimGroup3BtnLv1.activeSelf)
            {
                long lv_up_gold = GameDatabase.GetInstance().questDB.GetQuestEquipLevelUpGold(eqDb.eq_rt, eqDb.eq_id, eqDb.m_norm_lv, eqDb.m_norm_lv + 1);
                select.info_etc.txNormLevel1UpPrice.text = string.Format("{0:#,0}", lv_up_gold);
                select.info_button.txBottomGroupLvCnt1.text = maxNormLv - eqDb.m_norm_lv >= 1 ? "+1 UP" : "";
                select.info_etc.imNormLevel1UpBtnBg.color = goods_db.m_gold >= lv_up_gold ? new Color(0, 0.58f, 1, 1) : new Color(0.67f, 0.67f, 0.67f, 0.67f);
            }
            if (select.info_button.goBottimGroup3BtnLv10.activeSelf)
            {
                long lv_up_gold = GameDatabase.GetInstance().questDB.GetQuestEquipLevelUpGold(eqDb.eq_rt, eqDb.eq_id, eqDb.m_norm_lv, eqDb.m_norm_lv + 10);
                select.info_etc.txNormLevel10UpPrice.text = string.Format("{0:#,0}", lv_up_gold);
                select.info_button.txBottomGroupLvCnt2.text = maxNormLv - eqDb.m_norm_lv >= 10 ? "+10 UP" : string.Format("+{0} UP", (maxNormLv - eqDb.m_norm_lv).ToString());
                select.info_etc.imNormLevel10UpBtnBg.color = goods_db.m_gold >= lv_up_gold ? new Color(0, 0.58f, 1, 1) : new Color(0.67f, 0.67f, 0.67f, 0.67f);
            }
            if (select.info_button.goBottimGroup3BtnLv100.activeSelf)
            {
                long lv_up_gold = GameDatabase.GetInstance().questDB.GetQuestEquipLevelUpGold(eqDb.eq_rt, eqDb.eq_id, eqDb.m_norm_lv, 100);
                select.info_etc.txNormLevel100UpPrice.text = string.Format("{0:#,0}", lv_up_gold);
                select.info_button.txBottomGroupLvCnt3.text = maxNormLv - eqDb.m_norm_lv >= 100 ? "+100 UP" : string.Format("+{0} UP", (maxNormLv - eqDb.m_norm_lv).ToString());
                select.info_etc.imNormLevel100UpBtnBg.color = goods_db.m_gold >= lv_up_gold ? new Color(0, 0.58f, 1, 1) : new Color(0.67f, 0.67f, 0.67f, 0.67f);
            }
        }
        else
        {
            if (select.info_button.goBottomBtnGroup3.gameObject.activeSelf)
            {
                select.info_button.goBottomBtnGroup1.SetActive(true);
                select.info_button.goBottomBtnGroup3.SetActive(false);
            }
        }
    }
    #endregion

    // -----------------------------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------------------------
    #region 기본 스탯보기 
    public void Toggle_WearBasicStatInfoView() => BasicInfoView(wear.eqDb, true);
    public void Toggle_SelectBasicStatInfoView() => BasicInfoView(select.eqDb, false);

    /// <summary>
    /// 기본 스탯으로 장비 정보 보기 
    /// </summary>
    void BasicInfoView(GameDatabase.TableDB.Equipment eqDb, bool isWear)
    {
        if (isToggleInit == false)
            return;

        if (isWear == true)
        {
            if (wear.info_button.toBasicStat.isOn)
            {
                eqDb.m_norm_lv = 0;
                eqDb.m_ehnt_lv = 0;
                eqDb.eq_legend = 0;
            }

            SetWearEquipOn(eqDb, wear.info_button.toBasicStat.isOn);
        }
        else
        {
            if (select.info_button.toBasicStat.isOn)
            {
                eqDb.m_norm_lv = 0;
                eqDb.m_ehnt_lv = 0;
                eqDb.eq_legend = 0;
            }

            SetSelectEquipOn(eqDb, select.info_button.toBasicStat.isOn);
        }

        StatCompare();
    }
    #endregion

    // -----------------------------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------------------------
    #region 레벨업
    //레벨업 버튼 탭으로 변경 
    public void Click_LevelUpGroupOn(GameObject goMainTap)
    {
        if (object.Equals(goMainTap, wear.goTap))
        {
            wear.info_button.goBottomBtnGroup3.SetActive(true);
            wear.info_button.goBottomBtnGroup1.SetActive(false);
        }
        else
        {
            select.info_button.goBottomBtnGroup3.SetActive(true);
            select.info_button.goBottomBtnGroup1.SetActive(false);
        }
    }

    // 레벨업 버튼 탭 닫기 
    public void Click_LevelUpClose(GameObject goMainTap)
    {
        if (object.Equals(goMainTap, wear.goTap))
        {
            wear.info_button.goBottomBtnGroup3.SetActive(false);
            wear.info_button.goBottomBtnGroup1.SetActive(true);
        }
        else
        {
            select.info_button.goBottomBtnGroup3.SetActive(false);
            select.info_button.goBottomBtnGroup1.SetActive(true);
        }
    }

    public void Click_WearNormalLevelUp(int cnt) => NormalLevelUp(wear.eqDb, true, cnt);
    public void Click_SelectNormalLevelUp(int cnt) => NormalLevelUp(select.eqDb, false, cnt);

    [SerializeField] bool isLevelUp = false;
    /// <summary>
    ///  레벨업 !~~
    /// </summary>
    async void NormalLevelUp(GameDatabase.TableDB.Equipment eqDb, bool isWear, int cnt)
    {
        int maxNormLv = GameDatabase.GetInstance().chartDB.GetDicBalance("eq.normal.max.level").val_int;
        bool isMaxNormLv = eqDb.m_norm_lv >= maxNormLv;
        if (isMaxNormLv == false)
        {
            int leavingsNormLv = maxNormLv - eqDb.m_norm_lv; // 남은 레벨 
            int upCnt = cnt >= leavingsNormLv ? cnt - (maxNormLv - leavingsNormLv) : cnt;
            if (upCnt > maxNormLv)
                upCnt = maxNormLv;

            long lv_up_gold = GameDatabase.GetInstance().questDB.GetQuestEquipLevelUpGold(eqDb.eq_rt, eqDb.eq_id, eqDb.m_norm_lv, eqDb.m_norm_lv + upCnt);
            //long gold_price = GameDatabase.GetInstance().smithyDB.GetEquipNormalUpGold(eqDb.eq_rt, eqDb.m_norm_lv, eqDb.m_norm_lv + upCnt);
            LogPrint.Print("-------lv_up_gold---------- " + lv_up_gold + ", cnt : " + cnt + ", upCnt : " + upCnt);
           
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            if (goods_db.m_gold >= lv_up_gold)
            {
                goods_db.m_gold -= lv_up_gold;
                eqDb.m_norm_lv += upCnt;
                if (eqDb.m_norm_lv >= maxNormLv)
                    eqDb.m_norm_lv = maxNormLv;

                Task tsk1 = GameDatabase.GetInstance().tableDB.SendDataEquipment(eqDb, "update", ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "m_norm_lv", v = eqDb.m_norm_lv } }));
                Task tsk2 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
                while (Loading.Bottom(tsk1.IsCompleted, tsk2.IsCompleted) == false) await Task.Delay(100);
                
                if (isWear == true)
                {
                    SetWearEquipOn(eqDb);
                }
                else
                {
                    eqDb.m_norm_lv = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(eqDb.eq_ty).m_norm_lv;
                    SetSelectEquipOn(eqDb);
                }

                ToggleInit();
                StatCompare();

                if(!isWear)
                    GameDatabase.GetInstance().tableDB.SetTempEquipDbChange(eqDb);

                actionRefresh?.Invoke();
                isLevelUp = true;
            }
            else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("골드가 부족합니다.\n골드 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("최대 레벨에 도달하였습니다.");
    }
    #endregion

    // -----------------------------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------------------------
    #region 장비 / 장신구 판매or 분해
    public void Click_Sale()
    {
        var eqDb = select.eqDb;
        if(eqDb.m_lck == 1)
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("잠금상태인 장비는 판매할 수 없습니다.");
        else
        {
            float pet_sop1_value = GameMng.GetInstance().myPZ.igp.statValue.petSpOpTotalFigures.sop1_value * 0.01f;
            int rwd_bns = isOpenShop == true ? GameDatabase.GetInstance().tableDB.GetDicBalance("sale.decomposition.bonus").val_int : 1;
            int rwdCnt = GameDatabase.GetInstance().questDB.GetQuestEquipSaleGold(eqDb.eq_rt, eqDb.eq_id) * rwd_bns;
            int result_rwdCnt = rwdCnt + (int)(rwdCnt * pet_sop1_value);

            string rtStr = GameDatabase.StringFormat.GetRatingColorText(eqDb.eq_rt, false);
            string txt = isOpenShop == true ? string.Format("선택한 {0}등급 장비를 판매하시겠습니까?\n보상으로 <color=#FFE800>골드</color> {1:#,0}(+{2:#,0}) 획득합니다.", rtStr, result_rwdCnt, (int)(result_rwdCnt / rwd_bns)) :
                string.Format("선택한 {0}등급 장비를 판매하시겠습니까?\n보상으로 <color=#FFE800>골드</color> {1:#,0} 획득합니다.", rtStr, result_rwdCnt);
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(string.Format("선택한 {0}등급 장비를 판매하시겠습니까?", GameDatabase.StringFormat.GetRatingColorText(eqDb.eq_rt, false)), Listener_Sale);
        }
    }

    private async void Listener_Sale()
    {
        var eqDb = select.eqDb;
        eqDb.m_state = -1;
        Task tsk1 = GameDatabase.GetInstance().tableDB.SendDataEquipment(eqDb, "delete", ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "m_state", v = eqDb.m_state } }));
        while (Loading.Bottom(tsk1.IsCompleted) == false) await Task.Delay(100);

        int rwd_bns = isOpenShop == true ? GameDatabase.GetInstance().tableDB.GetDicBalance("sale.decomposition.bonus").val_int : 1;
        int rwd_gold = GameDatabase.GetInstance().questDB.GetQuestEquipSaleGold(eqDb.eq_rt, eqDb.eq_id) * rwd_bns;
        GameDatabase.GetInstance().tableDB.SetUpdateGoods("gold", rwd_gold, "+");
        PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("장비를 판매하여 <color=#FFE800>골드</color> {0:#,0} 획득하였습니다.", rwd_gold));
        MainUI.GetInstance().inventory.initOnStartInventoryAll.SetInit(true);
        if (isOpenShop)
            PopUpMng.GetInstance().popUpShopLuckEquipResult.CellSaleOrDecomp();

        gameObject.SetActive(false);
    }

    public void Click_Decomposition()
    {
        var eqDb = select.eqDb;
        if (eqDb.m_lck == 1)
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("잠금상태인 장비는 분해할 수 없습니다.");
        else
        {
            int rwd_bns = isOpenShop == true ? GameDatabase.GetInstance().tableDB.GetDicBalance("sale.decomposition.bonus").val_int : 1;
            string rtStr = GameDatabase.StringFormat.GetRatingColorText(eqDb.eq_rt, false);
            if (GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(eqDb.eq_ty) == false)
            {
                int rwd_cnt = GameDatabase.GetInstance().questDB.GetQuestEquipDecompRuby(eqDb.eq_rt, eqDb.eq_id);
                int rwdCnt = rwd_cnt * rwd_bns;
                string txt = isOpenShop == true ? string.Format("선택한 {0}등급 장비를 분해하시겠습니까?\n보상으로 <color=#FF0080>루비</color> x{1:#,0}(+{2:#,0}) 획득합니다.", rtStr, rwdCnt, (int)(rwdCnt / rwd_bns)) :
                    string.Format("선택한 {0}등급 장비를 분해하시겠습니까?\n보상으로 <color=#FF0080>루비</color> x{1:#,0} 획득합니다.", rtStr, rwdCnt, (int)(rwdCnt / rwd_bns));
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_Decomposition);
            }
            else
            {
                int rwd_cnt = GameDatabase.GetInstance().questDB.GetQuestEquipDecompEther(eqDb.eq_rt, eqDb.eq_id);
                int rwdCnt = rwd_cnt * rwd_bns;
                string txt = isOpenShop == true ? string.Format("선택한 {0}등급 장신구를 분해하시겠습니까?\n보상으로 <color=#00FF1E>에테르</color> x{1:#,0}(+{2:#,0}) 획득합니다.", rtStr, rwdCnt, (int)(rwdCnt / rwd_bns)) :
                    string.Format("선택한 {0}등급 장신구를 분해하시겠습니까?\n보상으로 <color=#00FF1E>에테르</color> x{1:#,0} 획득합니다.", rtStr, rwdCnt, (int)(rwdCnt / rwd_bns));
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_Decomposition);
            }
        }
    }

    UnityAction sohwan_confirm_action; // 소환 완료 결과 창에서 판매/분해헀을 때 
    private async void Listener_Decomposition()
    {
        var eqDb = select.eqDb;
        eqDb.m_state = -1;
        Task tsk1 = GameDatabase.GetInstance().tableDB.SendDataEquipment(eqDb, "delete", ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "m_state", v = eqDb.m_state } }));
        while (Loading.Bottom(tsk1.IsCompleted) == false) await Task.Delay(100);

        int rwd_bns = isOpenShop == true ? GameDatabase.GetInstance().tableDB.GetDicBalance("sale.decomposition.bonus").val_int : 1;
        string rtStr = GameDatabase.StringFormat.GetRatingColorText(eqDb.eq_rt, false);
        // 장비 분해 
        if (!GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(eqDb.eq_ty))
        {
            int rwd_cnt = GameDatabase.GetInstance().questDB.GetQuestEquipDecompRuby(eqDb.eq_rt, eqDb.eq_id);
            int rwdCnt = rwd_cnt * rwd_bns;
            GameDatabase.GetInstance().tableDB.SetUpdateGoods("ruby", rwdCnt, "+");
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("{0}등급 장비를 분해 완료하였습니다.\n보상으로 <color=#FF0080>루비</color> x{1:#,0} 획득하였습니다..", rtStr, rwdCnt));
        }
        else // 장신구 분해 
        {
            int rwd_cnt = GameDatabase.GetInstance().questDB.GetQuestEquipDecompEther(eqDb.eq_rt, eqDb.eq_id);
            int rwdCnt = rwd_cnt * rwd_bns;
            GameDatabase.GetInstance().tableDB.SetUpdateGoods("ether", rwdCnt, "+");
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("{0}등급 장신구를 분해 완료하였습니다.\n보상으로 <color=#00FF1E>에테르</color> x{1:#,0} 획득하였습니다.", rtStr, rwdCnt));
        }

        if(MainUI.GetInstance().inventory.gameObject.activeSelf)
            MainUI.GetInstance().inventory.initOnStartInventoryAll.SetInit(true);

        if (isOpenShop)
            PopUpMng.GetInstance().popUpShopLuckEquipResult.CellSaleOrDecomp();

        gameObject.SetActive(false);
    }
    #endregion

    // -----------------------------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------------------------
    #region 장비/장신구 장착 
    [SerializeField] bool isWearChange = false;
    [SerializeField] bool isWearChangeParts = false;
    // 장착 
    public async void Click_Wear(GameObject goTap)
    {
        if (wear.goTap.activeSelf && select.goTap.activeSelf)
        {
            var wearDb = wear.eqDb;
            if(string.IsNullOrEmpty(wearDb.indate) == false)
            {
                var selectDb = select.eqDb;
                wearDb.m_state = 0;
                selectDb.m_state = 1;
                selectDb.m_norm_lv = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(selectDb.eq_ty).m_norm_lv;

                if (string.IsNullOrEmpty(selectDb.indate) == true)
                {
                    selectDb.indate = wearDb.indate;
                    wearDb.indate = string.Empty;
                    wear.eqDb = wearDb;
                    select.eqDb = selectDb;
                }

                if (string.IsNullOrEmpty(selectDb.indate) == false)
                {
                    ToggleInit();
                    SetWearEquipOn(selectDb);
                    SetSelectEquipOn(wearDb);
                    StatCompare();

                    isWearChange = !isWearChange;
                    if (GameDatabase.GetInstance().chartDB.GetIsPartsTypeAcce(selectDb.eq_ty) == false)
                        GameMng.GetInstance().myPZ.SettingPartsPreview(selectDb.eq_ty, selectDb.eq_rt, selectDb.eq_id);
                }
                else PopUpMng.GetInstance().Open_MessageNotif("장착할 장비의 InDate 오류입니다.");
            }
            else PopUpMng.GetInstance().Open_MessageNotif("착용중인 장비의 InDate 오류입니다.");

            bool wearisEquipAcce = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(wear.eqDb.eq_ty);
            wear.info_button.btnLegendUpgrade.SetActive(wear.eqDb.eq_rt == 7 && wear.eqDb.eq_legend == 0 && wear.eqDb.eq_ty <= 7);
            wear.info_button.btnMoveAcceOpChange.SetActive(wearisEquipAcce || (wear.eqDb.eq_rt == 7 && wear.eqDb.eq_legend == 1 && wear.eqDb.eq_ty <= 7));

            bool selectisEquipAcce = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(select.eqDb.eq_ty);
            select.info_button.btnLegendUpgrade.SetActive(select.eqDb.eq_rt == 7 && select.eqDb.eq_legend == 0 && select.eqDb.eq_ty <= 7);
            select.info_button.btnMoveAcceOpChange.SetActive(selectisEquipAcce || (select.eqDb.eq_rt == 7 && select.eqDb.eq_legend == 1 && select.eqDb.eq_ty <= 7));

            Loading.Full(false);
            await RefreshData();
            Loading.Full(true);
        }
        else
        {
            if (isNoneWear) // 착용중인게 없다. 
            {
                var item = select.eqDb;
                item.m_state = GameDatabase.GetInstance().tableDB.GetUseEquipSlot();
                BackendReturnObject bro1 = null;
                Param p = GameDatabase.GetInstance().tableDB.EquipParamCollection(item);
                if (string.IsNullOrEmpty(item.indate) == true)
                {
                    long unUID = GameDatabase.GetInstance().tableDB.GetUnusedUID();
                    string unInDate = GameDatabase.GetInstance().tableDB.GetUIDSearchToInDate(unUID);
                    if (unUID == 0)
                        SendQueue.Enqueue(Backend.GameInfo.Insert, BackendGpgsMng.tableName_Equipment, p, callback => { bro1 = callback; });
                    else
                        SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_Equipment, unInDate, p, callback => { bro1 = callback; });

                    while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }

                    if (unUID == 0)
                    {
                        item.indate = bro1.GetReturnValuetoJSON()["inDate"].ToString();
                    }
                    else
                    {
                        item.indate = unInDate;
                        GameDatabase.GetInstance().tableDB.SetUnusedInDateToEmpty(unUID);
                    }
                }
                else
                {
                    SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_Equipment, item.indate, p, callback => { bro1 = callback; });
                    while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }
                }

                GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(item);
                GameDatabase.GetInstance().tableDB.SetTempEquipDbChange(item);
                MainUI.GetInstance().inventory.initOnStartInventoryAll.SetInit(true);
                Click_Close();
            }
        }
    }
    #endregion

    // -----------------------------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------------------------
    #region 잠금 / 풀기 
    private bool isWearLockChange = false;
    private bool isSelectLockChange = false;
    public async void Click_WearLock ()
    {
        if (wear.info_etc.dtNextLock < BackendGpgsMng.GetInstance().GetNowTime())
        {
            var eqDb = wear.eqDb;
            eqDb.m_lck = eqDb.m_lck == 0 ? 1 : 0;
            isWearLockChange = !isWearLockChange;
            SetWearEquipOn(eqDb);


            if (string.IsNullOrEmpty(eqDb.indate) == false)
            {
                BackendReturnObject bro1 = null;
                Param parm = ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "m_lck", v = eqDb.m_lck } });
                SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_Equipment, eqDb.indate, parm, callback => { bro1 = callback; });
                while (Loading.Bottom(bro1, bro1) == false) { await Task.Delay(100); }
            }

            wear.eqDb = eqDb;
            GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(eqDb);
            MainUI.GetInstance().tapCharacter.wearingEquipmentInfo.SetWearEquipView(eqDb.eq_ty);

            wear.info_etc.dtNextLock = BackendGpgsMng.GetInstance().GetNowTime().AddSeconds(5);
            StopCoroutine("WearLockDate"); StartCoroutine("WearLockDate");
        }
    }
    public async void Click_SelectLock()
    {
        if(select.info_etc.dtNextLock < BackendGpgsMng.GetInstance().GetNowTime())
        {
            var eqDb = select.eqDb;
            eqDb.m_lck = eqDb.m_lck == 0 ? 1 : 0;
            isSelectLockChange = !isSelectLockChange;
            SetSelectEquipOn(eqDb);

            if (string.IsNullOrEmpty(eqDb.indate) == false)
            {
                BackendReturnObject bro1 = null;
                Param parm = ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "m_lck", v = eqDb.m_lck } });
                SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_Equipment, eqDb.indate, parm, callback => { bro1 = callback; });
                while (Loading.Bottom(bro1, bro1) == false) { await Task.Delay(100); }
            }

            select.eqDb = eqDb;
            GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(eqDb);

            select.info_etc.dtNextLock = BackendGpgsMng.GetInstance().GetNowTime().AddSeconds(5);
            StopCoroutine("SelectLockDate"); StartCoroutine("SelectLockDate");
            actionRefresh?.Invoke();
        }
    }

    IEnumerator WearLockDate()
    {
        var nDate = BackendGpgsMng.GetInstance().GetNowTime();
        yield return null;
        while (wear.info_etc.dtNextLock > nDate)
        {
            nDate = BackendGpgsMng.GetInstance().GetNowTime();
            int sec = (int)(wear.info_etc.dtNextLock - BackendGpgsMng.GetInstance().GetNowTime()).TotalSeconds;
            wear.info_etc.txDtNextLock.text = sec.ToString();
            yield return null;
        }

        wear.info_etc.txDtNextLock.text = "";
    }

    IEnumerator SelectLockDate()
    {
        var nDate = BackendGpgsMng.GetInstance().GetNowTime();
        yield return null;
        while (select.info_etc.dtNextLock > nDate)
        {
            nDate = BackendGpgsMng.GetInstance().GetNowTime();
            int sec = (int)(select.info_etc.dtNextLock - BackendGpgsMng.GetInstance().GetNowTime()).TotalSeconds;
            select.info_etc.txDtNextLock.text = sec.ToString();
            yield return null;
        }

        select.info_etc.txDtNextLock.text = "";
    }
    #endregion

    // -----------------------------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------------------------
    #region 탭 변경 : 강화 
    public void Click_MoveEnhant(GameObject goTap)
    {
        var eq_db = GameObject.Equals(goTap, wear.goTap) == true ? wear.eqDb : select.eqDb;
        if (eq_db.m_ehnt_lv < GameDatabase.GetInstance().chartDB.GetDicBalanceEquipMaxEnhantLevel())
        {
            MainUI.GetInstance().OnTap(2);
            int ptType = eq_db.eq_ty;
            if (ptType == 0 || ptType == 1) // 무기or방패 대장간 관련 눌렀을 때
                MainUI.GetInstance().tapSmithy.smithyListType = SmithyListType.EQUIP_WEAPON_SHIELD;
            else if (ptType == 8 || ptType == 9 || ptType == 10) // 장신구 대장간 관련 눌렀을 때
                MainUI.GetInstance().tapSmithy.smithyListType = SmithyListType.EQUIP_ACCE;
            else // 방어구 대장간 관련 눌렀을 때
                MainUI.GetInstance().tapSmithy.smithyListType = SmithyListType.EQUIP_COSTUME;

            PopUpMng.GetInstance().popUpInventorySort.enum_SortInvnHighLow = SortInventorytHighLow.HIGH_TO_LOW;
            PopUpMng.GetInstance().popUpInventorySort.enum_SortInventory = SortInventory.RATING;

            MainUI.GetInstance().tapSmithy.TapOpen(SmithyTapType.Enhancement, eq_db);
            MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
            gameObject.SetActive(false);
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("현재 장비는 최대 강화 레벨에 도달하였습니다.");
    }
    #endregion

    // -----------------------------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------------------------
    #region 탭 변경 : 강화 레벨 전승 
    public void Click_MoveEnhantTransfer(GameObject goTap)
    {
        MainUI.GetInstance().OnTap(2);
        var equip_data = GameObject.Equals(goTap, wear.goTap) == true ? wear.eqDb : select.eqDb;
        bool isEqAc = GameDatabase.GetInstance().chartDB.GetIsPartsTypeAcce(equip_data.eq_ty);
        // 탭 오픈 
        if (isEqAc) // 장신구 대장간 관련 눌렀을 때
            MainUI.GetInstance().tapSmithy.smithyListType = SmithyListType.EQUIP_ACCE;
        else
        {
            if(equip_data.eq_ty == 0 || equip_data.eq_ty == 1) // 무기or방패 대장간 관련 눌렀을 때
                MainUI.GetInstance().tapSmithy.smithyListType = SmithyListType.EQUIP_WEAPON_SHIELD;
            else // 방어구 대장간 관련 눌렀을 때
                MainUI.GetInstance().tapSmithy.smithyListType = SmithyListType.EQUIP_COSTUME;
        }

        PopUpMng.GetInstance().popUpInventorySort.enum_SortInvnHighLow = SortInventorytHighLow.HIGH_TO_LOW;
        PopUpMng.GetInstance().popUpInventorySort.enum_SortInventory = SortInventory.RATING;
        MainUI.GetInstance().tapSmithy.TapOpen(SmithyTapType.LevelTransfer, new GameDatabase.TableDB.Equipment() { eq_ty = equip_data.eq_ty }, default);
        MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
        MainUI.GetInstance().tapSmithy.equipmentLevelTransfer.SetInitData(equip_data);
        foreach (var item in GameObject.FindObjectsOfType<ScrollIndexCallbackSmithy>())
            item.ReleaseEnhantTransferMat(equip_data);

        gameObject.SetActive(false);
    }
    #endregion

    // -----------------------------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------------------------
    #region 탭 변경 : 장신구 옵션 변경
    public void Click_MoveOptionChange(GameObject goTap)
    {
        var eq_db = GameObject.Equals(goTap, wear.goTap) == true ? wear.eqDb : select.eqDb;
        bool isEqAc = GameDatabase.GetInstance().chartDB.GetIsPartsTypeAcce(eq_db.eq_ty);
        if((isEqAc == true && eq_db.eq_rt >= 3) || (eq_db.eq_ty <= 7 && eq_db.eq_legend == 1))
        {
            MainUI.GetInstance().OnTap(2);

            if(isEqAc == true)
                MainUI.GetInstance().tapSmithy.smithyListType = SmithyListType.EQUIP_ACCE;
            else if(eq_db.eq_legend == 1)
            {
                if(eq_db.eq_ty <= 1)
                {
                    MainUI.GetInstance().tapSmithy.smithyListType = SmithyListType.EQUIP_WEAPON_SHIELD;
                }
                else
                {
                    MainUI.GetInstance().tapSmithy.smithyListType = SmithyListType.EQUIP_COSTUME;
                }
            }

            PopUpMng.GetInstance().popUpInventorySort.enum_SortInvnHighLow = SortInventorytHighLow.HIGH_TO_LOW;
            PopUpMng.GetInstance().popUpInventorySort.enum_SortInventory = SortInventory.RATING;
          
            MainUI.GetInstance().tapSmithy.TapOpen(SmithyTapType.OrnamentChangeOptions, eq_db);
            MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();

            gameObject.SetActive(false);
        }
        else
        {
            if(isEqAc == true && eq_db.eq_rt < 3)
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("장신구 옵션 변경은 고급 등급부터 가능합니다.");
            } else if(eq_db.eq_ty <= 7 && eq_db.eq_legend == 0)
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("장비 옵션 변경은 진화된 전설 장비부터 가능합니다.");
            }
        }
    }
    #endregion

    // -----------------------------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------------------------
    #region 전설 레전더리 진화 팝업 
    public void ClickOpen_LegendUpdrage(GameObject goTap)
    {
        var eq_db = GameObject.Equals(goTap, wear.goTap) == true ? wear.eqDb : select.eqDb;
        if(eq_db.eq_rt == 7)
        {
            PopUpMng.GetInstance().Open_PopUpEquipLegendUpdrage(eq_db);
        }
    }
    #endregion
    public async void Click_Close()
    {
        await RefreshData();
        gameObject.SetActive(false);
    }

    async Task RefreshData()
    {
        if (wear.goTap.activeSelf && select.goTap.activeSelf && !isNoneWear)
        {
            // 데이터 서버 전송 
            if (isWearChange)
            {
                bool isSendMat1 = false, isSendMat2 = false;
                TransactionParam tParam = new TransactionParam();
                if (string.IsNullOrEmpty(wear.eqDb.indate) == false)
                {
                    Param p1 = GameDatabase.GetInstance().tableDB.EquipParamCollection(wear.eqDb);
                    tParam.AddUpdateList(BackendGpgsMng.tableName_Equipment, wear.eqDb.indate, new List<BackEnd.WRITE> { new WRITE { Action = TransactionAction.Update, Param = p1 } });
                    isSendMat1 = true;
                }

                if (string.IsNullOrEmpty(select.eqDb.indate) == false)
                {
                    Param p2 = new Param();
                    p2.Add("m_state", 0);
                    tParam.AddUpdateList(BackendGpgsMng.tableName_Equipment, select.eqDb.indate, new List<BackEnd.WRITE> { new WRITE { Action = TransactionAction.Update, Param = p2 } });
                    isSendMat2 = true;
                }

                if (isSendMat1 || isSendMat2)
                {
                    BackendReturnObject bro1 = null;
                    SendQueue.Enqueue(Backend.GameInfo.TransactionWrite, tParam, callback => { bro1 = callback; });
                    while (Loading.Full(bro1)) { await Task.Delay(100); }
                }

                GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(wear.eqDb);
                GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(select.eqDb);
                await GameDatabase.GetInstance().tableDB.UpdateMStateServerDB_Equip(wear.eqDb);
                MainUI.GetInstance().NewEquipItemInventortRefresh();
            }
        }

        if (isLevelUp || isWearChange || isNoneWear)
        {
            int refInfoEqTy = wear.goTap.activeSelf ? wear.eqDb.eq_ty : select.eqDb.eq_ty;

            actionRefresh?.Invoke();
            GameDatabase.GetInstance().characterDB.SetPlayerStatValue();
            MainUI.GetInstance().tapCharacter.wearingEquipmentInfo.SetWearEquipView(refInfoEqTy);
            MainUI.GetInstance().tapCharacter.SetStatView(GameDatabase.GetInstance().characterDB.GetStat());

            GameMng.GetInstance().myPZ.igp.statValue = GameDatabase.GetInstance().characterDB.GetStat(); // 장비 관련, 스탯 
            if ((GameDatabase.GetInstance().chartDB.GetIsPartsTypeAcce(refInfoEqTy) == false) && isNoneWear)
            {
                GameMng.GetInstance().myPZ.SettingParts(refInfoEqTy); // 필드 좀비 
            }
        }

        isLevelUp = false;
        isWearChange = false;
        isNoneWear = false;
    }
}