using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum PetTapType
{
    Management = 0,
    LevelUp,
    OptionChange,
    Synthesis,
    Encyclopedia
}

[System.Serializable]
public class Pets
{
    [SerializeField] string header;
    public int pet_rt;
    public List<PetIdObjs> petIdObjs = new List<PetIdObjs>();
    [System.Serializable]
    public class PetIdObjs
    {
        public int pet_id;
        public GameObject goPet;
        public Animator aniPet;
    }
}

public class TapPet : MonoBehaviour
{
    public GameDatabase.TableDB.Pet NowPetDB;
    public List<GameDatabase.TableDB.Pet> NowPetDBs = new List<GameDatabase.TableDB.Pet>();

    [SerializeField] cdb_pet cdbpet;
  

    public PetTapType petTapType = PetTapType.Management;

    [SerializeField] TapObject tapObject;
    [SerializeField] InitOnStartPet initOnStartPet;

    [SerializeField] Management management;
    [SerializeField] LevelUp levelUp;
    [SerializeField] OptionChange optionChange;
    [SerializeField] Synthesis synthesis;
    [SerializeField] Encyclopedia encyclopedia;


    [SerializeField] TapChangeButton[] rightButtons = new TapChangeButton[6];


    void OnEnable()
    {
        tapObject.aniIcon.Play("MainButtonActiveOnScale");
        tapObject.txName.fontStyle = FontStyle.Bold;
        tapObject.txName.color = tapObject.onCorSelect;
        tapObject.goOutline.SetActive(true);
    }

    void OnDisable()
    {
        tapObject.txName.fontStyle = FontStyle.Normal;
        tapObject.txName.color = tapObject.noCorSelect;
        tapObject.goOutline.SetActive(false);
    }

    [SerializeField] Sprite[] statIcon = new Sprite[10];
    void Awake()
    {
        for (int i = 0; i < statIcon.Length; i++)
            statIcon[i] = SpriteAtlasMng.GetInstance().GetSpriteStatIcon(i);
    }

    public void SetData()
    {
        petTapType = PetTapType.Management;
        NowPetDB = GameDatabase.GetInstance().tableDB.GetUsePet();
        Init();
        ResetRightButton();
    }

#region ##### 공용 #####
    public void Init()
    {
        management.goRoot.SetActive(petTapType == PetTapType.Management);
        levelUp.goRoot.SetActive(petTapType == PetTapType.LevelUp);
        optionChange.goRoot.SetActive(petTapType == PetTapType.OptionChange);
        synthesis.goRoot.SetActive(petTapType == PetTapType.Synthesis);
        encyclopedia.goRoot.SetActive(petTapType == PetTapType.Encyclopedia);

        NowPetDBs.Clear();
        GameDatabase.GetInstance().tableDB.SetPetTypeAddSort(petTapType);

        if (petTapType == PetTapType.Management)
            UIManagement();
        else if (petTapType == PetTapType.LevelUp)
        {
            LevelUpInit();
            UILevelUp();
        }
        else if (petTapType == PetTapType.OptionChange)
            UIOptionChange();
        else if (petTapType == PetTapType.Synthesis)
            UISynthesis();
        else if (petTapType == PetTapType.Encyclopedia)
            UIEncyclopedia();

        initOnStartPet.SetInit(true);
    }

    [System.Serializable]
    public class Op
    {
        public GameObject goRoot;
        public Image imStatIcon;
        public Text txStatName, txStatValue, txNextArrow, txStatNextValue;
    }

    [Header("펫 3d 목록")]
    // 펫 목록 
    public List<Pets> prvw_PetRdrTexture = new List<Pets>();
    public GameObject prvw_GoRoot;

    public void ResetRightButton()
    {
        for (int i = 0; i < rightButtons.Length; i++)
        {
            if(rightButtons[i].go_button != null)
            {
                rightButtons[i].ui_ani.Play(i == 0 ? "UIInventoryRightButtonOn" : "UIInventoryRightButtonOff");
            }
        }    
    }

    public void ClickRightTapChange(int idx)
    {
        switch (idx)
        {
            case 0: NowPetDB = default; break;
            case 1:
                var _cdb = GameDatabase.GetInstance ().chartDB.GetCdbPet(NowPetDB.p_rt, NowPetDB.p_id);
                if (NowPetDB.p_lv >= _cdb.max_lv)
                {
                    PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("현재 펫의 레벨이 최대 레벨입니다.");
                    return;
                }
                break;
            case 2:
                NowPetDB = default;
                break;
            case 3:
                NowPetDB = default;
                synthesis.MainPetDB = default;
                synthesis.MatPetDB = default;
                break;
        }

        petTapType = (PetTapType)idx;
        Init();

        if(idx != 1)
        {
            foreach (var item in rightButtons)
            {
                if(item.go_button != null)
                {
                    if (item.id == idx)
                        item.ui_ani.Play("UIInventoryRightButtonOn");
                    else // OFF  
                        item.ui_ani.Play("UIInventoryRightButtonOff");
                }
            }
        }
    }
#endregion

#region ##### UI 관리 #####
    [System.Serializable]
    class Management
    {
        public GameObject goRoot;

        // 동생하기 or 동행해제
        public Text txUseSelectOrRelease;
        public Image imUseSelectOrRelease;
        public Slider sldBarExp;
        public Text txLvExp;
        public Text txLvMax;
        public GameObject goLevelUpBtn;
        public Image imLock;
        public GameObject goLck;

        // 이름, 등급/레벨 
        public Text txName;
        public Text txRatingLevelMax;

        [Header("펫 전용 옵션")]
        public Text txSopName1;
        public Text txSopValue1;
        public Text txSopName2;
        public Text txSopValue2;
        public Text txSopName3;
        public Text txSopValue3;

        [Header("펫 옵션 ~8")]
        public Text txOpCombat;
        public Op[] op;
    }

    /// <summary> 관리 </summary>
    public void UIManagement(GameDatabase.TableDB.Pet cell_select_petDB = default)
    {
        if (cell_select_petDB.aInUid > 0)
        {
            NowPetDB = cell_select_petDB;
            initOnStartPet.SetInit(false);
        }
        else
        {
            NowPetDB = GameDatabase.GetInstance().tableDB.GetUsePet();
        }

        management.goLck.SetActive(NowPetDB.aInUid > 0);
        management.txUseSelectOrRelease.text = NowPetDB.aInUid > 0 ? NowPetDB.m_state == 0 ? "동행 하기" : "동행 해제" : "동행할 펫을 선택해주세요.";
        management.imUseSelectOrRelease.sprite = NowPetDB.aInUid > 0 ? NowPetDB.m_state == 0 ? SpriteAtlasMng.GetInstance().GetSpriteButtonBox("blue") : SpriteAtlasMng.GetInstance().GetSpriteButtonBox("purple") : SpriteAtlasMng.GetInstance().GetSpriteButtonBox("gray");
        var _cdbPet = GameDatabase.GetInstance().chartDB.GetCdbPet(NowPetDB.p_rt, NowPetDB.p_id); // 옵션 
        cdbpet = _cdbPet;

        management.goLevelUpBtn.SetActive(NowPetDB.p_rt >= 3);

        // 이름 / 등급 / 레벨 / 최대 레벨 
        management.txName.text = _cdbPet.name;
        string rtCorSTr = GameDatabase.StringFormat.GetRatingColorText(NowPetDB.p_rt);
        if(NowPetDB.p_rt >= 3)
            management.txRatingLevelMax.text = string.Format("{0}, <color=#00FFF9>Lv.{1}</color> / {2}", rtCorSTr, NowPetDB.p_lv, _cdbPet.max_lv);
        else management.txRatingLevelMax.text = string.Format("{0}", rtCorSTr);

        if (NowPetDB.p_rt >= 3)
        {
            management.sldBarExp.value = NowPetDB.p_lv_residual;
            management.txLvExp.text = string.Format("{0:0.00}%", NowPetDB.p_lv_residual * 100.0f);
            management.txLvMax.text = string.Format("<color=#00FFF9>Lv.{0}</color> / {1}", NowPetDB.p_lv, _cdbPet.max_lv);
        }
        else
        {
            management.sldBarExp.value = 0f;
            management.txLvExp.text = "0.00%";
            management.txLvMax.text = "<color=#00FFF9>Lv.0</color>";
        }

        management.imLock.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquipLock(NowPetDB.m_lck);

        // 펫 전용 옵션 
        if (NowPetDB.p_rt >= 3)
        {
            management.txSopName1.text = GameDatabase.GetInstance ().chartDB.GetCdbPetSopName(NowPetDB.sOp1.id);
            management.txSopValue1.text = NowPetDB.sOp1.id <= 0 ? "-" : string.Format("{0:0.00}%", GameDatabase.GetInstance().chartDB.GetPetSpecialOptionStatValue(NowPetDB, 1));
            management.txSopName2.text = GameDatabase.GetInstance().chartDB.GetCdbPetSopName(NowPetDB.sOp2.id);
            management.txSopValue2.text = NowPetDB.sOp2.id <= 0 ? "-" : string.Format("{0:0.00}%", GameDatabase.GetInstance().chartDB.GetPetSpecialOptionStatValue(NowPetDB, 2));
            management.txSopName3.text = GameDatabase.GetInstance().chartDB.GetCdbPetSopName(NowPetDB.sOp3.id);
            management.txSopValue3.text = NowPetDB.sOp3.id <= 0 ? "-" : string.Format("{0:0.00}%", GameDatabase.GetInstance().chartDB.GetPetSpecialOptionStatValue(NowPetDB, 3));
        }
        else
        {
            management.txSopName1.text = NowPetDB.aInUid > 0 ? "없음 (관상용 펫)" : "-";
            management.txSopValue1.text = string.Empty;
            management.txSopName2.text = "-";
            management.txSopValue2.text = string.Empty;
            management.txSopName3.text = "-";
            management.txSopValue3.text = string.Empty;
        }

        // 옵션 (전투력)
        management.txOpCombat.text = "옵션";
        //if (NowPetDB.p_rt >= 3)
        //    management.txOpCombat.text = string.Format("펫 옵션 <size=24>( <color=orange>전투력</color> {0:#,0} )</size>", GameDatabase.GetInstance().tableDB.GetPetCombatPower(NowPetDB));
        //else management.txOpCombat.text = string.Empty;

        float[] opst_val = GameDatabase.GetInstance().chartDB.GetPetOptionStatValue(NowPetDB);
        // 옵션 
        for (int i = 0; i < 8; i++)
        {
            if (NowPetDB.p_rt >= 3)
            {
                var statOp = i == 0 ? NowPetDB.statOp.op1 : i == 1 ? NowPetDB.statOp.op2 : i == 2 ? NowPetDB.statOp.op3 : i == 3 ? NowPetDB.statOp.op4 :
                    i == 4 ? NowPetDB.statOp.op5 : i == 5 ? NowPetDB.statOp.op6 : i == 6 ? NowPetDB.statOp.op7 : i == 7 ? NowPetDB.statOp.op8 : new GameDatabase.TableDB.StatOp();

                management.op[i].goRoot.SetActive(statOp.id > 0);
                if (statOp.id > 0)
                {
                    management.op[i].imStatIcon.sprite = statIcon[statOp.id];
                    management.op[i].txStatName.text = GameDatabase.StringFormat.GetEquipStatName(statOp.id);
                    management.op[i].txStatValue.text = string.Format("+{0:0.00}%", opst_val[i]);
                }
            }
            else management.op[i].goRoot.SetActive(false);
        }

        // 펫 3d on off 
        foreach (var item in prvw_PetRdrTexture)
        {
            if (int.Equals(item.pet_rt, NowPetDB.p_rt))
            {
                foreach (var item2 in item.petIdObjs)
                    item2.goPet.SetActive(int.Equals(item2.pet_id, NowPetDB.p_id));
            }
            else
            {
                foreach (var item2 in item.petIdObjs)
                {
                    if(item2.goPet.activeSelf)
                        item2.goPet.SetActive(false);
                }
            }
        }

        prvw_GoRoot.SetActive(true);
    }

    /// <summary>
    /// 동행 하기 or 동행 해제 
    /// </summary>
    public async void Click_ManagementSelectOrRelease()
    {
        Task tsk = GameDatabase.GetInstance().tableDB.SetPetUseState(petTapType, NowPetDB.aInUid);
        while (Loading.Bottom(tsk.IsCompleted, true) == false) await Task.Delay(250);
        
        UIManagement(GameDatabase.GetInstance().tableDB.GetUniqIdxPet(NowPetDB.aInUid));
        GameMng.GetInstance().myPet.Setting();
        MainUI.GetInstance().RefreshGameStat();
    }

    /// <summary>
    /// 잠금 or 해제 
    /// </summary>
    public async void Click_ManagementLock()
    {
        var tmp = NowPetDB;
        tmp.m_lck = tmp.m_lck == 0 ? 1 : 0;
        Task tsk = GameDatabase.GetInstance().tableDB.SendDataPet(tmp, "update");
        while (Loading.Bottom(tsk.IsCompleted, true) == false) await Task.Delay(250);
        GameDatabase.GetInstance().tableDB.SetPetTypeAddSort(PetTapType.Management);
        UIManagement(tmp);
    }
#endregion

#region ##### UI 레벨 업 #####
    [System.Serializable]
    class LevelUp
    {
        public long upGold;
        public int upRuby;
        public int upEther;

        public GameDatabase.TableDB.Pet NextPetDB = new GameDatabase.TableDB.Pet();
        public GameObject goRoot;

        public GameObject goLevelUpOn;

        // 이름, 등급/레벨 
        public Text txName;
        public Text txRatingLevel;

        [Header("펫 전용 옵션")]
        public Text txSopName1;
        public Text txSopValue1;
        public Text txSopName2;
        public Text txSopValue2;
        public Text txSopName3;
        public Text txSopValue3;

        [Header("펫 옵션 ~8")]
        public Text txOpCombat;
        public Op[] op;

        public List<Toggle> togglesRating;

        public Text txMatCnt; // 재료선택 수량
        public Text txUpGold; // 필요 골드 
        public Text txUpRuby; // 루비 
        public Text txUpEther; // 에테르 
        public Image txUpBtnBg; // 업 버튼 배경 
        public Slider sdrBar; // 레벨 게이지 
        public Text txSdrBarPercent;
        public Text txSdrBarLv;
    }

    void LevelUpInit()
    {
        levelUp.NextPetDB = new GameDatabase.TableDB.Pet();
        levelUp.upGold = 0;
        NowPetDBs.Clear();
        foreach (var item in levelUp.togglesRating)
        {
            item.isOn = false;
        }
    }

    /// <summary> 레벨 업 </summary>
    public void UILevelUp()
    {
        levelUp.NextPetDB = NowPetDB;
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        var _cdbPet = GameDatabase.GetInstance().tableDB.GetCdbPet(NowPetDB.p_rt, NowPetDB.p_id);
        cdbpet = _cdbPet;

        float fLv = LevelUp_ExpLevel(NowPetDB.p_lv_residual);
        levelUp.NextPetDB.p_lv = NowPetDB.p_lv + (int)(fLv) < _cdbPet.max_lv ? NowPetDB.p_lv + (int)(fLv) : _cdbPet.max_lv;

        float ffLv = fLv - NowPetDB.p_lv_residual;
        if (ffLv > 0.0f)
        {
            float ffLvGold = NowPetDB.p_lv + ffLv > _cdbPet.max_lv ? ffLv - ((NowPetDB.p_lv + ffLv) - _cdbPet.max_lv) : ffLv;
            long initGold = (long)(GameDatabase.GetInstance().chartDB.GetDicBalance("pet.up.exp.gold").val_long * ffLvGold);
            float xvalGold = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("pet.exp.rt.{0}", NowPetDB.p_rt)).val_float * 1000.0f;
            levelUp.upGold = (long)(initGold * xvalGold);

            float ffLvRuby = (NowPetDB.p_lv + ffLv > _cdbPet.max_lv ? ffLv - ((NowPetDB.p_lv + ffLv) - _cdbPet.max_lv) : ffLv) * 10.0f;
            long initRuby = (long)(GameDatabase.GetInstance().chartDB.GetDicBalance("pet.up.exp.ruby").val_int * ffLvRuby);
            float xvalRuby = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("pet.exp.rt.{0}", NowPetDB.p_rt)).val_float * 1000.0f;
            levelUp.upRuby = (int)(initRuby * xvalRuby);

            float ffLvEther = (NowPetDB.p_lv + ffLv > _cdbPet.max_lv ? ffLv - ((NowPetDB.p_lv + ffLv) - _cdbPet.max_lv) : ffLv) * 10.0f;
            long initEther = (long)(GameDatabase.GetInstance().chartDB.GetDicBalance("pet.up.exp.ether").val_int * ffLvEther);
            float xvalEther = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("pet.exp.rt.{0}", NowPetDB.p_rt)).val_float * 1000.0f;
            levelUp.upEther = (int)(initEther * xvalEther);
        }
        else
        {
            levelUp.upGold = 0;
            levelUp.upRuby = 0;
            levelUp.upEther = 0;
        }

        levelUp.goLevelUpOn.SetActive((int)(fLv) >= 1.0f);
        if ((int)(fLv) >= 1.0f)
        {
            if (levelUp.NextPetDB.p_lv < _cdbPet.max_lv)
                fLv -= (int)fLv;
            else fLv = 1.0f;
        }
        
        levelUp.NextPetDB.p_lv_residual = fLv;
        levelUp.txUpGold.text = goods_db.m_gold >= levelUp.upGold ? string.Format("{0:#,0}", levelUp.upGold) : string.Format("<color=red>{0:#,0}</color>", levelUp.upGold);
        levelUp.txUpRuby.text = goods_db.m_ruby >= levelUp.upRuby ? string.Format("{0:#,0}", levelUp.upRuby) : string.Format("<color=red>{0:#,0}</color>", levelUp.upRuby);
        levelUp.txUpEther.text = goods_db.m_ether >= levelUp.upEther ? string.Format("{0:#,0}", levelUp.upEther) : string.Format("<color=red>{0:#,0}</color>", levelUp.upEther);
        levelUp.txUpBtnBg.sprite = (goods_db.m_gold >= levelUp.upRuby && goods_db.m_ruby >= levelUp.upRuby  && goods_db.m_ether >= levelUp.upEther) && (levelUp.upGold > 0 && levelUp.upRuby > 0 && levelUp.upEther > 0) ? 
            SpriteAtlasMng.GetInstance().GetSpriteButtonBox("yellow") : SpriteAtlasMng.GetInstance().GetSpriteButtonBox("gray");
        levelUp.txMatCnt.text = string.Format("{0} / 100", NowPetDBs.Count);
        levelUp.txSdrBarLv.text = string.Format("<color=#00FFF9>Lv.{0}</color> / {1}", levelUp.NextPetDB.p_lv, _cdbPet.max_lv);
        levelUp.txSdrBarPercent.text = string.Format("{0:0.00}%", fLv * 100.0f);
        levelUp.sdrBar.value = fLv;

        // 이름 / 등급 / 레벨 / 최대 레벨 
        levelUp.txName.text = _cdbPet.name;
        string rtCorSTr = GameDatabase.StringFormat.GetRatingColorText(NowPetDB.p_rt);
        levelUp.txRatingLevel.text = string.Format("{0}, <color=#00FFF9>Lv.{1}</color>", rtCorSTr, NowPetDB.p_lv);

        // 펫 전용 옵션 
        if (NowPetDB.p_rt >= 3)
        {
            levelUp.txSopName1.text = GameDatabase.GetInstance ().chartDB.GetCdbPetSopName(NowPetDB.sOp1.id);
            levelUp.txSopName2.text = GameDatabase.GetInstance ().chartDB.GetCdbPetSopName(NowPetDB.sOp2.id);
            levelUp.txSopName3.text = GameDatabase.GetInstance ().chartDB.GetCdbPetSopName(NowPetDB.sOp3.id);
            levelUp.txSopValue1.text = NowPetDB.sOp1.id <= 0 ? "-" : string.Format("{0:0.00}%", GameDatabase.GetInstance().chartDB.GetPetSpecialOptionStatValue(NowPetDB, 1));
            levelUp.txSopValue2.text = NowPetDB.sOp2.id <= 0 ? "-" : string.Format("{0:0.00}%", GameDatabase.GetInstance().chartDB.GetPetSpecialOptionStatValue(NowPetDB, 2));
            levelUp.txSopValue3.text = NowPetDB.sOp3.id <= 0 ? "-" : string.Format("{0:0.00}%", GameDatabase.GetInstance().chartDB.GetPetSpecialOptionStatValue(NowPetDB, 3));
        }
        else
        {
            levelUp.txSopName1.text = "-";
            levelUp.txSopValue1.text = string.Empty;
            levelUp.txSopName2.text = "-";
            levelUp.txSopValue2.text = string.Empty;
            levelUp.txSopName3.text = "-";
            levelUp.txSopValue3.text = string.Empty;
        }

        // 옵션 ▲
        levelUp.txOpCombat.text = "옵션";

        float[] opst_val1 = GameDatabase.GetInstance().chartDB.GetPetOptionStatValue(NowPetDB);
        float[] opst_val2 = GameDatabase.GetInstance().chartDB.GetPetOptionStatValue(levelUp.NextPetDB);
        for (int i = 0; i < 8; i++)
        {
            var statOp = i == 0 ? NowPetDB.statOp.op1 : i == 1 ? NowPetDB.statOp.op2 : i == 2 ? NowPetDB.statOp.op3 : i == 3 ? NowPetDB.statOp.op4 :
                i == 4 ? NowPetDB.statOp.op5 : i == 5 ? NowPetDB.statOp.op6 : i == 6 ? NowPetDB.statOp.op7 : i == 7 ? NowPetDB.statOp.op8 : new GameDatabase.TableDB.StatOp();

            levelUp.op[i].goRoot.SetActive(statOp.id > 0);
            if (statOp.id > 0)
            {
                if(NowPetDB.p_lv == levelUp.NextPetDB.p_lv) // 레벨 변동 없음 
                {
                    levelUp.op[i].imStatIcon.sprite = statIcon[statOp.id];
                    levelUp.op[i].txStatName.text = GameDatabase.StringFormat.GetEquipStatName(statOp.id);
                    LogPrint.EditorPrint(statOp.id + ", " + statOp.rlv + ", " + NowPetDB.p_rt + ", " + NowPetDB.p_id + ", " + NowPetDB.p_lv);
                    levelUp.op[i].txStatValue.text = string.Format("+{0:0.00}%", opst_val1[i]);
                    levelUp.op[i].txStatNextValue.gameObject.SetActive(false);
                    levelUp.op[i].txNextArrow.gameObject.SetActive(false);
                }
                else // 레벨 상승 
                {
                    levelUp.op[i].txNextArrow.gameObject.SetActive(true);
                    levelUp.op[i].txStatNextValue.gameObject.SetActive(true);

                    levelUp.op[i].imStatIcon.sprite = statIcon[statOp.id];
                    levelUp.op[i].txStatName.text = GameDatabase.StringFormat.GetEquipStatName(statOp.id);
                    levelUp.op[i].txStatValue.text = string.Format("+{0:0.00}%", opst_val1[i]);
                    levelUp.op[i].txStatNextValue.text = string.Format("<color=#00FF69>+{0:0.00}%</color>", opst_val2[i]);
                }
            }
        }

        prvw_GoRoot.SetActive(true);
    }

   /// <summary>
   /// 레벨 업 EXP 
   /// </summary>
    float LevelUp_ExpLevel(float residual)
    {
        float fval = residual;
        var seltPetMats = NowPetDBs;
        foreach (var db in seltPetMats)
        {
            fval += (db.p_rt * db.p_rt) * GameDatabase.GetInstance ().chartDB.GetDicBalance(string.Format("pet.exp.rt.{0}", db.p_rt)).val_float;
        }

        return fval;
    }

    public bool IsLevelUpSelect(long uniq_idx) => NowPetDBs.FindIndex(obj => obj.aInUid == uniq_idx) >= 0;

    public bool CellClick_LevelUpIsNowSelectState(GameDatabase.TableDB.Pet seltDB)
    {
        return NowPetDBs.FindIndex(obj => obj.aInUid == seltDB.aInUid) >= 0;
    }

    /// <summary> 레벨 업 재료 선택 or 해제 </summary>
    public void CellClick_LevelUpSelectIMat(GameDatabase.TableDB.Pet seltDB)
    {
        int indx = NowPetDBs.FindIndex(obj => obj.aInUid == seltDB.aInUid);
        if(indx == -1) // 새로운 재료 추가 
        {
            var cdbPet = GameDatabase.GetInstance().tableDB.GetCdbPet(NowPetDB.p_rt, NowPetDB.p_id);
            if (levelUp.NextPetDB.p_lv < cdbPet.max_lv && NowPetDBs.Count < 100)
            {
                NowPetDBs.Add(seltDB);
                UILevelUp();
            }
        }
        else // 이미 선택되있는 재료 
        {
            NowPetDBs.Remove(seltDB);
            UILevelUp();
        }

        initOnStartPet.SetInit(false);
    }

    /// <summary>
    /// 등급 모두 선택 
    /// </summary>
    public void ClickUpLevel_SelectRating (int rt)
    {
        List<int> rts = new List<int>();
        foreach (var item in levelUp.togglesRating)
        {
            if(item.isOn)
            {
                string strTmp = Regex.Replace(item.name, @"\D", "");
                if (!string.IsNullOrEmpty(strTmp))
                {
                    rts.Add(int.Parse(strTmp));
                }
            }
        }
        
        NowPetDBs.Clear();
        var all = GameDatabase.GetInstance().tableDB.GetAllPet(petTapType);
        for (int i = 0; i < all.Count; i++)
        {
            if (int.Equals(all[i].m_lck, 0))
            {
                int iq = rts.FindIndex(obj => int.Equals(obj, all[i].p_rt));
                if (iq >= 0)
                {
                    if (NowPetDBs.Count < 100)
                        NowPetDBs.Add(all[i]);
                    else break;
                }
            }
        }

        UILevelUp();
        initOnStartPet.SetInit(false);
    }

    /// <summary>
    /// 레벨 업 시작 
    /// </summary>
    public async void Click_LevelUpStart()
    {
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        if(goods_db.m_gold >= levelUp.upGold && goods_db.m_ruby >= levelUp.upRuby && goods_db.m_ether >= levelUp.upEther)
        {
            List<TransactionParam> TransactionParamList = new List<TransactionParam>();
            TransactionParam tParam = new TransactionParam();

            // 레벨 업 
            if(!string.IsNullOrEmpty(levelUp.NextPetDB.indate)) // 서버에 존재하는 펫 데이터 
            {
                List<BackEnd.WRITE> writes = new List<BackEnd.WRITE>
                { 
                    new WRITE
                    { 
                        Action = TransactionAction.Update, Param = ParamT.Collection 
                        (
                            new ParamT.P[] {
                                new ParamT.P() { k = "p_lv", v = levelUp.NextPetDB.p_lv },
                                new ParamT.P() { k = "p_lv_residual", v = levelUp.NextPetDB.p_lv_residual } 
                            }
                        )
                    }
                };
                tParam.AddUpdateList(BackendGpgsMng.tableName_Pet, levelUp.NextPetDB.indate, writes);
            }

            GameDatabase.GetInstance().tableDB.UpdateClientDB_Pet(levelUp.NextPetDB);

            // 재료 소멸 
            foreach (var item in NowPetDBs)
            {
                var tmp = item;
                tmp.m_state = -1;
                if (!string.IsNullOrEmpty(tmp.indate))
                {
                    if (tParam.GetWriteValues().Count >= 10)
                    {
                        TransactionParamList.Add(tParam);
                        tParam = new TransactionParam();
                    }

                    List<BackEnd.WRITE> writes = new List<BackEnd.WRITE>
                    {
                        new WRITE
                        {
                            Action = TransactionAction.Update, Param =
                            ParamT.Collection
                            (
                                new ParamT.P[] { new ParamT.P() { k = "m_state", v = tmp.m_state } }
                            )
                        }
                    };
                    tParam.AddUpdateList(BackendGpgsMng.tableName_Pet, tmp.indate, writes);
                }

                GameDatabase.GetInstance().tableDB.UpdateClientDB_Pet(tmp, true);
            }

            LogPrint.EditorPrint("tParam.GetWriteValues().Count:" + tParam.GetWriteValues().Count);
            if (tParam.GetWriteValues().Count > 0)
                TransactionParamList.Add(tParam);

            foreach (var send_param in TransactionParamList)
            {
                BackendReturnObject bro = null;
                SendQueue.Enqueue(Backend.GameInfo.TransactionWrite, send_param, callback => { bro = callback; });
                while (bro == null) await Task.Delay(100);
            }

            // 비용 차감 
            GameDatabase.GetInstance().tableDB.SetUpdateGoods("gold", levelUp.upGold, "-");
            GameDatabase.GetInstance().tableDB.SetUpdateGoods("ruby", levelUp.upRuby, "-");
            GameDatabase.GetInstance().tableDB.SetUpdateGoods("ether", levelUp.upEther, "-");

            NowPetDB = levelUp.NextPetDB;
            LevelUpInit();
            UILevelUp();

            GameDatabase.GetInstance().tableDB.SetPetTypeAddSort(PetTapType.LevelUp);
            initOnStartPet.SetInit(false);
            MainUI.GetInstance().RefreshGameStat();
            MainUI.GetInstance().topUI.SetGoodsView();
        }
        else
        {
            if (goods_db.m_gold < levelUp.upGold && goods_db.m_ruby >= levelUp.upRuby && goods_db.m_ether >= levelUp.upEther)
            {
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("골드가 부족합니다.\n골드 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
            }
            else if(goods_db.m_ruby < levelUp.upRuby)
            {
                if (GameDatabase.GetInstance().convenienceFunctionDB.GetUseingConvenFunAutoSale())
                    PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("루비가 부족합니다.\n루비는 장비를 분해하여 획득할 수 있습니다");
                else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("루비가 부족합니다.\n루비는 장비를 분해하여 획득할 수 있습니다.\n장비 자동 판매/분해 구매 탭으로 이동됩니다.", PopUpMng.GetInstance().Open_DailyProductReward);
            }
            else if(goods_db.m_ether < levelUp.upEther)
            {
                if (GameDatabase.GetInstance().convenienceFunctionDB.GetUseingConvenFunAutoSale())
                    PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("에테르가 족합니다.\n에테르는 장비를 분해하여 획득할 수 있습니다");
                else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("에테르가 부족합니다.\n에테르는 장비를 분해하여 획득할 수 있습니다.\n장비 자동 판매/분해 구매 탭으로 이동됩니다.", PopUpMng.GetInstance().Open_DailyProductReward);
            }
        }
    }
#endregion

#region ##### UI 옵션 변경 #####
    [System.Serializable]
    class OptionChange
    {
        public GameDatabase.TableDB.Pet TempChangePetDB;

        public GameObject goRoot;
        public bool isChangeSop;
        public Text txPriceBlueDia, txPriceRedDia;

        [Header("펫 전용 옵션 미리 결과물")]
        public GameObject goSelectOk, goSelectNo;
        public CanvasGroup cgSop;
        public CanvasGroup cgStOp;
        public CanvasGroup cgChangeBtn;
        public GameObject goRootSop, goRootStOp;
        public List<GameObject> listGoRootSop, listGoRootStOp;
        public List<Text> listTxNameSop, listTxNameStOp;
        public List<Text> listTxValueSop, listTxValueStOp;
       
        public Image imBtnBgSop, imBtnBgStOp;

        // 이름, 등급, 레벨 
        public Text txName, txRating, txLevel;

        [Header("펫 전용 옵션")]
        public Text txSopName1;
        public Text txSopValue1;
        public Text txSopName2;
        public Text txSopValue2;
        public Text txSopName3;
        public Text txSopValue3;

        [Header("펫 옵션 ~8")]
        public Text txOpCombat;
        public Op[] op;

        public BlockScreen blockScreen;
        public List<int> arrStOp = new List<int>() { 1, 2, 4, 5, 8 };

        public Image imSopBtnBg, imStOpBtnBg;
        public GameObject goBtnSop, goBtnStOp, goBtnComplete, goBtnReturn;
    }

    /// <summary> 옵션 변경 </summary>
    public void UIOptionChange(bool _isChangeSop = true)
    {
        optionChange.goBtnSop.SetActive(true);
        optionChange.goBtnStOp.SetActive(true);
        optionChange.goBtnComplete.SetActive(false);
        optionChange.goBtnReturn.SetActive(false);
        optionChange.blockScreen.gameObject.SetActive(false);
        optionChange.isChangeSop = _isChangeSop;
        bool isSelectOk = NowPetDB.aInUid > 0;
        optionChange.goSelectOk.SetActive(isSelectOk);
        optionChange.goSelectNo.SetActive(!isSelectOk);

        optionChange.cgChangeBtn.alpha = 1.0f;
        optionChange.cgSop.alpha = optionChange.isChangeSop ? 1.0f : 0.3f;
        optionChange.cgStOp.alpha = !optionChange.isChangeSop ? 1.0f : 0.3f;
        optionChange.goRootSop.SetActive(optionChange.isChangeSop);
        optionChange.goRootStOp.SetActive(!optionChange.isChangeSop);
        optionChange.imSopBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(optionChange.isChangeSop);
        optionChange.imStOpBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(!optionChange.isChangeSop);
        optionChange.txPriceBlueDia.text = string.Format("x{0:#,0}", GameDatabase.GetInstance().chartDB.GetDicBalance("pet.op.change.blue.dia.price").val_int);
        optionChange.txPriceRedDia.text = string.Format("x{0:#,0}", GameDatabase.GetInstance().chartDB.GetDicBalance("pet.op.change.red.dia.price").val_int);

        if (isSelectOk)
        {
            var _cdbPet = GameDatabase.GetInstance().chartDB.GetCdbPet(NowPetDB.p_rt, NowPetDB.p_id); // 옵션 
            cdbpet = _cdbPet;

            // 이름 / 등급 / 레벨 / 최대 레벨 
            string rtCorSTr = GameDatabase.StringFormat.GetRatingColorText(NowPetDB.p_rt);
            optionChange.txName.text = _cdbPet.name;
            optionChange.txRating.text = rtCorSTr;
            optionChange.txLevel.text = string.Format("<color=#00FFF9>Lv.{0}</color>", NowPetDB.p_lv);

            // 펫 전용 옵션 
            optionChange.txSopName1.text = GameDatabase.GetInstance().chartDB.GetCdbPetSopName(NowPetDB.sOp1.id);
            optionChange.txSopValue1.text = string.Format("{0:0.00}%", GameDatabase.GetInstance().chartDB.GetPetSpecialOptionStatValue(NowPetDB, 1));
            optionChange.txSopName2.text = GameDatabase.GetInstance().chartDB.GetCdbPetSopName(NowPetDB.sOp2.id);
            optionChange.txSopValue2.text = string.Format("{0:0.00}%", GameDatabase.GetInstance().chartDB.GetPetSpecialOptionStatValue(NowPetDB, 2));
            optionChange.txSopName3.text = GameDatabase.GetInstance().chartDB.GetCdbPetSopName(NowPetDB.sOp3.id);
            optionChange.txSopValue3.text = string.Format("{0:0.00}%", GameDatabase.GetInstance().chartDB.GetPetSpecialOptionStatValue(NowPetDB, 3));

            // 옵션 (전투력)
            optionChange.txOpCombat.text = "옵션";

            // 옵션 
            float[] opst_val = GameDatabase.GetInstance().chartDB.GetPetOptionStatValue(NowPetDB);
            for (int i = 0; i < opst_val.Length; i++)
            {
                if (NowPetDB.p_rt >= 3)
                {
                    var statOp = i == 0 ? NowPetDB.statOp.op1 : i == 1 ? NowPetDB.statOp.op2 : i == 2 ? NowPetDB.statOp.op3 : i == 3 ? NowPetDB.statOp.op4 :
                        i == 4 ? NowPetDB.statOp.op5 : i == 5 ? NowPetDB.statOp.op6 : i == 6 ? NowPetDB.statOp.op7 : i == 7 ? NowPetDB.statOp.op8 : new GameDatabase.TableDB.StatOp();

                    optionChange.op[i].goRoot.SetActive(statOp.id > 0);
                    if (statOp.id > 0)
                    {
                        optionChange.op[i].imStatIcon.sprite = statIcon[statOp.id];
                        optionChange.op[i].txStatName.text = GameDatabase.StringFormat.GetEquipStatName(statOp.id);
                        optionChange.op[i].txStatValue.text = string.Format("+{0:0.00}%", opst_val[i]);
                    }
                }
                else optionChange.op[i].goRoot.SetActive(false);
            }

            // 펫 3d on off 
            foreach (var item in prvw_PetRdrTexture)
            {
                if (int.Equals(item.pet_rt, NowPetDB.p_rt))
                {
                    foreach (var item2 in item.petIdObjs)
                        item2.goPet.SetActive(int.Equals(item2.pet_id, NowPetDB.p_id));
                }
                else
                {
                    foreach (var item2 in item.petIdObjs)
                    {
                        if (item2.goPet.activeSelf)
                            item2.goPet.SetActive(false);
                    }
                }
            }
        }
        else
        {
            
        }

        // -------------------------------------- 변경 미리 보기 --------------------------------------
        // 전용 옵션 미리보기 
        for (int i = 0; i < optionChange.listGoRootSop.Count; i++)
        {
            optionChange.listGoRootSop[i].SetActive(true);
            optionChange.listTxNameSop[i].text = GameDatabase.GetInstance().chartDB.GetCdbPetSopName(i + 1);
            if (isSelectOk)
            {
                float[] sop_val = GameDatabase.GetInstance().chartDB.GetCdbPetSopValue(i + 1, NowPetDB.p_rt, NowPetDB.p_id);
                optionChange.listTxValueSop[i].text = string.Format("{0:0.00}~{1:0.00}", sop_val[0], sop_val[1]);
            }
            else
            {
                optionChange.listTxValueSop[i].text = "??? ~ ???";
            }
        }

        //// 옵션 미리 보기 
        var min_op_value = GameDatabase.GetInstance().chartDB.GetCdbPetMinMaxStOpValue(true, NowPetDB.p_rt, NowPetDB.p_id, NowPetDB.p_lv);
        var max_op_value = GameDatabase.GetInstance().chartDB.GetCdbPetMinMaxStOpValue(false, NowPetDB.p_rt, NowPetDB.p_id, NowPetDB.p_lv);
        foreach (var item in optionChange.listGoRootStOp)
            item.SetActive(false);
        for (int i = 0; i < 5; i++)
        {
            optionChange.listGoRootStOp[i].SetActive(true);
            optionChange.listTxNameStOp[i].text = GameDatabase.StringFormat.GetEquipStatName(optionChange.arrStOp[i]);
            if (isSelectOk)
            {
                optionChange.listTxValueStOp[i].text = string.Format("{0:0.00}%~{1:0.00}%", min_op_value[i], max_op_value[i]);
            }
            else
            {
                optionChange.listTxValueStOp[i].text = "??? ~ ???";
            }
        }

        prvw_GoRoot.SetActive(isSelectOk);
    }

    public void Click_RandomOpPreview(bool _isSop) => UIOptionChange(_isSop);

    public bool IsOptionChangeSelect(long uniq_idx) => NowPetDB.aInUid == uniq_idx;

    public void CellClick_OpChangePetSelect(GameDatabase.TableDB.Pet _pet)
    {
        NowPetDB = _pet;
        UIOptionChange(optionChange.isChangeSop);
        initOnStartPet.SetInit(false);
    }

    public async void Click_StartOptionChange(bool isTbc)
    {
        if (optionChange.blockScreen.gameObject.activeSelf)
            return;

        Loading.Full(false);
        bool okStart = true;
        var uInfo = GameDatabase.GetInstance().tableDB.GetUserInfo();
        //public ObscuredInt m_pet_opch_red_cnt; // 펫 레드다야 옵션 변경 횟수 
        //public ObscuredInt m_pet_opch_blue_cnt; // 펫 블루다야 옵션 변경 횟수 
        //public ObscuredInt m_pet_synt_red_cnt; // 펫 레드다야 조합 횟수 
        //public ObscuredInt m_pet_synt_blue_cnt; // 펫 블루다야 조합 횟수 

        if (isTbc) // tbc 비용 차감 
        {
            uInfo.m_pet_opch_red_cnt++;
            Task tsk1 = GameDatabase.GetInstance().tableDB.DeductionTBC(GameDatabase.GetInstance().chartDB.GetDicBalance("pet.op.change.red.dia.price").val_int);
            while (tsk1.IsCompleted == false)
                await Task.Delay(100);
        }
        else // 블루 다이아 차감 
        {
            uInfo.m_pet_opch_blue_cnt++;
            int blue_dia_price = GameDatabase.GetInstance ().chartDB.GetDicBalance("pet.op.change.blue.dia.price").val_int;
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            bool isBlueDiaLack = goods_db.m_dia < blue_dia_price;
            int tbc = goods_db.m_dia < blue_dia_price ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;

            if (goods_db.m_dia >= blue_dia_price || (isBlueDiaLack && tbc + goods_db.m_dia >= blue_dia_price))
            {
                int dedDia = goods_db.m_dia -= blue_dia_price;
                int dedTbc = dedDia < 0 ? System.Math.Abs(dedDia) : 0;
                Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
                Task tsk2 = GameDatabase.GetInstance().tableDB.DeductionTBC(dedTbc);
                while (tsk1.IsCompleted == false || tsk2.IsCompleted == false)
                    await Task.Delay(100);
            }
            else okStart = false;
        }

        GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(uInfo);
        
        if(okStart)
        {
            MainUI.GetInstance().topUI.GetInfoViewTBC();
            StartOptionChange(optionChange.isChangeSop);
        }
        else
        {
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
        }
        Loading.Full(true);
    }

    async void StartOptionChange(bool isSop)
    {
        optionChange.blockScreen.gameObject.SetActive(true);
        optionChange.blockScreen.CenterAlphaZero();
        optionChange.cgChangeBtn.alpha = 0.3f;
        optionChange.goBtnSop.SetActive(false);
        optionChange.goBtnStOp.SetActive(false);

        if (isSop)
        {
            foreach (var item in optionChange.listGoRootSop)
                item.SetActive(false);

            var new_rnd_sop = GameDatabase.GetInstance().tableDB.GetPetRandomSpecialOption(NowPetDB.p_rt);

            // 결과 DB
            GameDatabase.TableDB.Pet tmp_PetDB = NowPetDB;
            tmp_PetDB.sOp1 = new_rnd_sop[0];
            tmp_PetDB.sOp2 = new_rnd_sop[1];
            tmp_PetDB.sOp3 = new_rnd_sop[2];
            optionChange.TempChangePetDB = tmp_PetDB;

            for (int i = 0; i < 3; i++)
            {
                int sop_id = i == 0 ? tmp_PetDB.sOp1.id : i == 1 ? tmp_PetDB.sOp2.id : i == 2 ? tmp_PetDB.sOp3.id : 0;
                if(sop_id > 0)
                {
                    float sop_val = GameDatabase.GetInstance().chartDB.GetPetSpecialOptionStatValue(tmp_PetDB, i + 1);
                    optionChange.listTxNameSop[i].text = GameDatabase.GetInstance().chartDB.GetCdbPetSopName(sop_id);
                    optionChange.listTxValueSop[i].text = string.Format("+{0:0.00}%", sop_val);
                    optionChange.listGoRootSop[i].SetActive(true);
                    await Task.Delay(250);
                }
            }
        }
        else
        {
            foreach (var item in optionChange.listGoRootStOp)
                item.SetActive(false);

            // 결과 DB
            GameDatabase.TableDB.Pet tmp_PetDB = NowPetDB;
            tmp_PetDB.statOp = GameDatabase.GetInstance().tableDB.GetPetRandomOption(NowPetDB.p_rt);
            optionChange.TempChangePetDB = tmp_PetDB;

            float[] opst_val = GameDatabase.GetInstance().chartDB.GetPetOptionStatValue(tmp_PetDB);
            for (int i = 0; i < opst_val.Length; i++)
            {
                var statOp = i == 0 ? tmp_PetDB.statOp.op1 : i == 1 ? tmp_PetDB.statOp.op2 : i == 2 ? tmp_PetDB.statOp.op3 : i == 3 ? tmp_PetDB.statOp.op4 :
                    i == 4 ? tmp_PetDB.statOp.op5 : i == 5 ? tmp_PetDB.statOp.op6 : i == 6 ? tmp_PetDB.statOp.op7 : i == 7 ? tmp_PetDB.statOp.op8 : new GameDatabase.TableDB.StatOp();

                if (statOp.id > 0)
                {
                    optionChange.listTxNameStOp[i].text = GameDatabase.StringFormat.GetEquipStatName(statOp.id);
                    optionChange.listTxValueStOp[i].text = string.Format("+{0:0.00}%", opst_val[i]);
                    optionChange.listGoRootStOp[i].SetActive(true);
                    await Task.Delay(250);
                }
            }
        }

        await Task.Delay(500);

        
        optionChange.goBtnComplete.SetActive(true);
        optionChange.goBtnReturn.SetActive(true);

        optionChange.blockScreen.CenterObjectDisable();
        optionChange.blockScreen.OnText(0.5f);
    }

    /// <summary>
    /// 확정 or 복구 
    /// </summary>
    public async void Click_CompleteOrReturn(bool isOk)
    {
        if (isOk)
        {
            var tmp = optionChange.TempChangePetDB;
            Task tsk = GameDatabase.GetInstance().tableDB.SendDataPet(tmp, "update");
            while (Loading.Bottom(tsk.IsCompleted, true) == false) await Task.Delay(250);
            GameDatabase.GetInstance().tableDB.SetPetTypeAddSort(PetTapType.OptionChange);
            NowPetDB = optionChange.TempChangePetDB;
            optionChange.TempChangePetDB = default;
            UIOptionChange(optionChange.isChangeSop);
            MainUI.GetInstance().RefreshGameStat();
        }
        else
        {
            UIOptionChange(optionChange.isChangeSop);
        }
    }
    #endregion

#region ##### UI 조합 #####
    [System.Serializable]
    class Synthesis
    {
        public GameDatabase.TableDB.Pet MainPetDB;
        public GameDatabase.TableDB.Pet MatPetDB;

        public GameObject goRoot;
        public bool isChangeSop;
        
        public ReadyInfo readyInfo;
        public ResultInfo resultInfo;

        // 성공 확률
        public Text txt_SuccessRate;
       
    }

    [System.Serializable]
    public struct IconInfo
    {
        public GameObject go_Root;
        public Text txt_Rating, txt_Lv, txt_Name;
        public Image img_Icon, img_RatingBg, img_RatingLine;
    }
    
    [System.Serializable]
    public struct ReadyInfo
    {
        public IconInfo iconInfo_Main;
        public IconInfo iconInfo_Mat;

        public Text txRate;
        public GameObject goSelectOk, goSelectNo, goSuccessUI;
        public Animator ani;
        public CanvasGroup cgReady;

        public Image imRedDiaStartBtnBg;
        public Text txRedPriceDia;

        public Image imBlueDiaStartBtnBg;
        public Text txBluePriceDia;
    }

    [System.Serializable]
    public struct ResultInfo
    {
        // 결과 아이콘 정보 
        public IconInfo iconInfo_Result;
        public Animator an_ResultIcon;
        public Image img_IconGray;

        public Text txt_SuccessOrFail; // 성공 결과 라벨 

        // 진화 UI
        public BlockScreen blockScreen;
        public GameObject go_RootMat;
        public GameObject go_RootSuccessResult, go_RootFailResult;

        // 정보 결과 정보 
        // 이름, 등급/레벨 
        public Text txName;
        public Text txRating;
        public Text txLevel;

        [Header("펫 전용 옵션")]
        public Text txSopName1;
        public Text txSopValue1;
        public Text txSopName2;
        public Text txSopValue2;
        public Text txSopName3;
        public Text txSopValue3;

        [Header("펫 옵션 ~8")]
        public Text txOpCombat;
        public Op[] op;
    }

    /// <summary> 조합 </summary>
    public void UISynthesis()
    {
        var uInfo = GameDatabase.GetInstance().tableDB.GetUserInfo();
        prvw_GoRoot.SetActive(false);
        synthesis.resultInfo.blockScreen.gameObject.SetActive(false);
        synthesis.readyInfo.cgReady.alpha = 1.0f;
        synthesis.readyInfo.goSelectOk.SetActive(synthesis.MainPetDB.aInUid > 0 && synthesis.MatPetDB.aInUid > 0);
        synthesis.readyInfo.goSelectNo.SetActive(!(synthesis.MainPetDB.aInUid > 0 && synthesis.MatPetDB.aInUid > 0));
        synthesis.readyInfo.iconInfo_Main.go_Root.SetActive(synthesis.MainPetDB.aInUid > 0);
        synthesis.readyInfo.iconInfo_Mat.go_Root.SetActive(synthesis.MatPetDB.aInUid > 0);
        synthesis.readyInfo.txRedPriceDia.text = string.Format("x{0:#,0}", GameDatabase.GetInstance ().chartDB.GetDicBalance("pet.synt.red.dia.price").val_int);
        synthesis.readyInfo.txBluePriceDia.text = string.Format("x{0:#,0}", GameDatabase.GetInstance().chartDB.GetDicBalance("pet.synt.blue.dia.price").val_int);
        synthesis.readyInfo.txRate.text = string.Format("<size=50>성공 확률 <color=orange>{0}%</color></size>\n(보너스 성공률 <color=yellow>+{1}%</color>)",
            GameDatabase.GetInstance().chartDB.GetDicBalance("pet.synt.default.rate").val_float + uInfo.m_pet_synt_bns_pct, uInfo.m_pet_synt_bns_pct);
        synthesis.resultInfo.go_RootFailResult.SetActive(false);
        synthesis.resultInfo.iconInfo_Result.go_Root.SetActive(false);
        synthesis.resultInfo.go_RootMat.SetActive(true);
        synthesis.resultInfo.go_RootSuccessResult.SetActive(false);
        synthesis.resultInfo.txt_SuccessOrFail.text = "펫 진화 준비!";

        if (synthesis.MainPetDB.aInUid > 0)
        {
            var _cdbMain = GameDatabase.GetInstance().chartDB.GetCdbPet(synthesis.MainPetDB.p_rt, synthesis.MainPetDB.p_id);
            synthesis.readyInfo.iconInfo_Main.img_Icon.sprite = SpriteAtlasMng.GetInstance().GetPetIcon(synthesis.MainPetDB.p_rt, synthesis.MainPetDB.p_id);
            synthesis.readyInfo.iconInfo_Main.img_RatingLine.color = ResourceDatabase.GetInstance().GetItemColor(synthesis.MainPetDB.p_rt);
            synthesis.readyInfo.iconInfo_Main.img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(synthesis.MainPetDB.p_rt);
            synthesis.readyInfo.iconInfo_Main.txt_Rating.color = ResourceDatabase.GetInstance().GetItemColor(synthesis.MainPetDB.p_rt);
            synthesis.readyInfo.iconInfo_Main.txt_Rating.text = synthesis.MainPetDB.p_rt > 0 ? LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", synthesis.MainPetDB.p_rt)) : "";
            synthesis.readyInfo.iconInfo_Main.txt_Lv.text = string.Format("Lv.{0}/{1}", synthesis.MainPetDB.p_lv, _cdbMain.max_lv); // 장비 강화 레벨 
            synthesis.readyInfo.iconInfo_Main.txt_Name.text = _cdbMain.name;
        }

        if (synthesis.MatPetDB.aInUid > 0)
        {
            var _cdbMat = GameDatabase.GetInstance().chartDB.GetCdbPet(synthesis.MainPetDB.p_rt, synthesis.MainPetDB.p_id);
            synthesis.readyInfo.iconInfo_Mat.img_Icon.sprite = SpriteAtlasMng.GetInstance().GetPetIcon(synthesis.MatPetDB.p_rt, synthesis.MatPetDB.p_id);
            synthesis.readyInfo.iconInfo_Mat.img_Icon.sprite = SpriteAtlasMng.GetInstance().GetPetIcon(synthesis.MainPetDB.p_rt, synthesis.MainPetDB.p_id);
            synthesis.readyInfo.iconInfo_Mat.img_RatingLine.color = ResourceDatabase.GetInstance().GetItemColor(synthesis.MatPetDB.p_rt);
            synthesis.readyInfo.iconInfo_Mat.img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(synthesis.MatPetDB.p_rt);
            synthesis.readyInfo.iconInfo_Mat.txt_Rating.color = ResourceDatabase.GetInstance().GetItemColor(synthesis.MatPetDB.p_rt);
            synthesis.readyInfo.iconInfo_Mat.txt_Rating.text = synthesis.MatPetDB.p_rt > 0 ? LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", synthesis.MatPetDB.p_rt)) : "";
            synthesis.readyInfo.iconInfo_Mat.txt_Lv.text = string.Format("Lv.{0}/{1}", synthesis.MatPetDB.p_lv, _cdbMat.max_lv); // 장비 강화 레벨 
            synthesis.readyInfo.iconInfo_Mat.txt_Name.text = _cdbMat.name;
        }

        if(synthesis.MainPetDB.aInUid > 0 && synthesis.MatPetDB.aInUid > 0)
        {

        }
    }

    public void CellClick_SyntSelectMain(GameDatabase.TableDB.Pet _petDB)
    {
        synthesis.MainPetDB = _petDB;
        UISynthesis();
        initOnStartPet.SetInit(false);
    }

    public void CellClick_SyntSelectMat(GameDatabase.TableDB.Pet _petDB)
    {
        if(synthesis.MainPetDB.aInUid != _petDB.aInUid)
        {
            synthesis.MatPetDB = _petDB;
            UISynthesis();
            initOnStartPet.SetInit(false);
        }
    }


    public int SyntSelectRating () => synthesis.MainPetDB.p_rt;
    public bool isSynyMainSelect() => synthesis.MainPetDB.aInUid > 0;

    public bool isSynyMainSelectCheck(long uniq_idx) => synthesis.MainPetDB.aInUid == uniq_idx;

    public bool isSynyMatSelectCheck(long uniq_idx) => synthesis.MatPetDB.aInUid == uniq_idx;

    public bool IsSyntSelect(long cell_uniqIDX) => synthesis.MainPetDB.aInUid == cell_uniqIDX || synthesis.MatPetDB.aInUid == cell_uniqIDX;

    public void Click_SyntRelease(bool isMain)
    {
        if (isMain)
        {
            synthesis.MainPetDB = default;
            synthesis.MatPetDB = default;
        }
        else synthesis.MatPetDB = default;

        UISynthesis();
        initOnStartPet.SetInit(false);
    }

    public async void Click_StartSynt(bool isTbc)
    {
        synthesis.resultInfo.blockScreen.gameObject.SetActive(true);
        synthesis.readyInfo.cgReady.alpha = 0.75f;

        var uInfo = GameDatabase.GetInstance().tableDB.GetUserInfo();
        synthesis.MatPetDB.m_state = -1;
        bool isSyntSuccess = GameDatabase.GetInstance().GetRandomPercent() < (ObscuredInt)(GameDatabase.GetInstance ().chartDB.GetDicBalance("pet.synt.default.rate").val_float + uInfo.m_pet_synt_bns_pct);
        if (isSyntSuccess)
        {
            var tmpPetDB = synthesis.MainPetDB;
            tmpPetDB.p_rt++;
            var _cdbPetRatings = GameDatabase.GetInstance().chartDB.GetCdbPetRatingAll(tmpPetDB.p_rt);
            if(_cdbPetRatings.Count > 0)
            {
                tmpPetDB.p_id = _cdbPetRatings[Random.Range(0, _cdbPetRatings.Count)].p_id;
            }

            tmpPetDB.statOp = GameDatabase.GetInstance().tableDB.GetPetRandomOption(synthesis.MainPetDB.p_rt + 1, true, tmpPetDB.statOp);
            synthesis.MainPetDB = tmpPetDB;

            uInfo.m_pet_synt_bns_pct = 0;
            uInfo.m_pet_synt_red_cnt++;
        }
        else
        {
            uInfo.m_pet_synt_bns_pct += isTbc ? (ObscuredInt)GameDatabase.GetInstance().chartDB.GetDicBalance("pet.synt.red.fail.bns.rate").val_float : (ObscuredInt)GameDatabase.GetInstance().chartDB.GetDicBalance("pet.synt.blue.fail.bns.rate").val_float;
            uInfo.m_pet_synt_blue_cnt++;
        }

        #region ##### 재료 및 비용 소모 #####
        bool okStart = true;
        if (isTbc) // tbc 비용 차감 
        {
            Task tsk1 = GameDatabase.GetInstance().tableDB.DeductionTBC(GameDatabase.GetInstance().chartDB.GetDicBalance("pet.synt.red.dia.price").val_int);
            Task tsk2 = GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(uInfo);
            while (tsk1.IsCompleted == false || tsk2.IsCompleted == false)
                await Task.Delay(100);
        }
        else // 블루 다이아 차감 
        {
            int blue_dia_price = GameDatabase.GetInstance().chartDB.GetDicBalance("pet.synt.blue.dia.price").val_int;
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            bool isBlueDiaLack = goods_db.m_dia < blue_dia_price;
            int tbc = goods_db.m_dia < blue_dia_price ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;

            if (goods_db.m_dia >= blue_dia_price || (isBlueDiaLack && tbc + goods_db.m_dia >= blue_dia_price))
            {
                int dedDia = goods_db.m_dia -= blue_dia_price;
                int dedTbc = dedDia < 0 ? System.Math.Abs(dedDia) : 0;
                Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
                Task tsk2 = GameDatabase.GetInstance().tableDB.DeductionTBC(dedTbc);
                Task tsk3 = GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(uInfo);
                while (tsk1.IsCompleted == false || tsk2.IsCompleted == false || tsk3.IsCompleted == false)
                    await Task.Delay(100);
            }
            else okStart = false;
        }

        if (okStart == false)
        {
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
        }
        #endregion

        synthesis.resultInfo.blockScreen.CenterAlphaZero();
        synthesis.readyInfo.ani.Play("AcceSyntStart");

        while (!synthesis.readyInfo.ani.GetCurrentAnimatorStateInfo(0).IsName("AcceSyntStart"))
            await Task.Delay(100);

        while (synthesis.readyInfo.ani.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            await Task.Delay(100);
            if (synthesis.readyInfo.ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f && !synthesis.resultInfo.iconInfo_Result.go_Root.activeSelf)
            {
                // ----- 결과 아이콘 정보 -----
                var _cdbPet = GameDatabase.GetInstance().chartDB.GetCdbPet(synthesis.MainPetDB.p_rt, synthesis.MainPetDB.p_id);
                synthesis.resultInfo.iconInfo_Result.img_Icon.sprite = SpriteAtlasMng.GetInstance().GetPetIcon(synthesis.MainPetDB.p_rt, synthesis.MainPetDB.p_id);
                synthesis.resultInfo.iconInfo_Result.img_Icon.enabled = true;
                synthesis.resultInfo.iconInfo_Result.txt_Lv.text = string.Format("Lv.{0}", synthesis.MainPetDB.p_lv);
                synthesis.resultInfo.iconInfo_Result.txt_Rating.text = synthesis.MainPetDB.p_rt > 0 ? LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", synthesis.MainPetDB.p_rt)) : "";
                synthesis.resultInfo.iconInfo_Result.txt_Name.text = _cdbPet.name;

                if (isSyntSuccess)
                {
                    Color corRat = ResourceDatabase.GetInstance().GetItemColor(synthesis.MainPetDB.p_rt);
                    synthesis.resultInfo.iconInfo_Result.img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(synthesis.MainPetDB.p_rt);
                    synthesis.resultInfo.iconInfo_Result.img_RatingLine.color = corRat;
                    synthesis.resultInfo.iconInfo_Result.txt_Rating.color = corRat;
                    synthesis.resultInfo.img_IconGray.enabled = false;

                    // 이름 / 등급 / 레벨 / 최대 레벨 
                    string rtCorSTr = GameDatabase.StringFormat.GetRatingColorText(NowPetDB.p_rt);
                    synthesis.resultInfo.txName.text = _cdbPet.name;
                    synthesis.resultInfo.txRating.text = rtCorSTr;
                    synthesis.resultInfo.txLevel.text = string.Format("<color=#00FFF9>Lv.{0}</color>", NowPetDB.p_lv);

                    // 펫 전용 옵션 
                    synthesis.resultInfo.txSopName1.text = GameDatabase.GetInstance().chartDB.GetCdbPetSopName(NowPetDB.sOp1.id);
                    synthesis.resultInfo.txSopValue1.text = string.Format("{0:0.00}%", GameDatabase.GetInstance().chartDB.GetPetSpecialOptionStatValue(NowPetDB, 1));
                    synthesis.resultInfo.txSopName2.text = GameDatabase.GetInstance().chartDB.GetCdbPetSopName(NowPetDB.sOp2.id);
                    synthesis.resultInfo.txSopValue2.text = string.Format("{0:0.00}%", GameDatabase.GetInstance().chartDB.GetPetSpecialOptionStatValue(NowPetDB, 2));
                    synthesis.resultInfo.txSopName3.text = GameDatabase.GetInstance().chartDB.GetCdbPetSopName(NowPetDB.sOp3.id);
                    synthesis.resultInfo.txSopValue3.text = string.Format("{0:0.00}%", GameDatabase.GetInstance().chartDB.GetPetSpecialOptionStatValue(NowPetDB, 3));

                    // 옵션 (전투력)
                    synthesis.resultInfo.txOpCombat.text = "옵션";

                    // 옵션 
                    float[] opst_val = GameDatabase.GetInstance().chartDB.GetPetOptionStatValue(NowPetDB);
                    for (int i = 0; i < opst_val.Length; i++)
                    {
                        if (NowPetDB.p_rt >= 3)
                        {
                            var statOp = i == 0 ? NowPetDB.statOp.op1 : i == 1 ? NowPetDB.statOp.op2 : i == 2 ? NowPetDB.statOp.op3 : i == 3 ? NowPetDB.statOp.op4 :
                                i == 4 ? NowPetDB.statOp.op5 : i == 5 ? NowPetDB.statOp.op6 : i == 6 ? NowPetDB.statOp.op7 : i == 7 ? NowPetDB.statOp.op8 : new GameDatabase.TableDB.StatOp();

                            synthesis.resultInfo.op[i].goRoot.SetActive(statOp.id > 0);
                            if (statOp.id > 0)
                            {
                                synthesis.resultInfo.op[i].imStatIcon.sprite = statIcon[statOp.id];
                                synthesis.resultInfo.op[i].txStatName.text = GameDatabase.StringFormat.GetEquipStatName(statOp.id);
                                synthesis.resultInfo.op[i].txStatValue.text = string.Format("+{0:0.00}%", opst_val[i]);
                            }
                        }
                        else synthesis.resultInfo.op[i].goRoot.SetActive(false);
                    }
                }
                else
                {
                    synthesis.resultInfo.iconInfo_Result.img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
                    synthesis.resultInfo.iconInfo_Result.img_RatingLine.color = Color.gray;
                    synthesis.resultInfo.iconInfo_Result.txt_Rating.color = Color.gray;
                    synthesis.resultInfo.img_IconGray.sprite = SpriteAtlasMng.GetInstance().GetPetIcon(synthesis.MainPetDB.p_rt, synthesis.MainPetDB.p_id);
                    synthesis.resultInfo.img_IconGray.enabled = true;
                }

                synthesis.resultInfo.iconInfo_Result.go_Root.SetActive(true); // 합성 결과 아이콘(애니) 탭 켬  
            }
        }

        synthesis.resultInfo.go_RootMat.SetActive(false);            // 선택한 합성 준비 재료 아이콘 탭 
        string anName = isSyntSuccess && synthesis.MainPetDB.p_rt >= 5 ? string.Format("Result_EquipIconInfoStart_rt{0}", synthesis.MainPetDB.p_rt) : "Result_EquipIconInfoStart";
        synthesis.resultInfo.an_ResultIcon.Play(anName);

        while (synthesis.resultInfo.an_ResultIcon.GetCurrentAnimatorStateInfo(0).IsName(anName) == false)
            await Task.Delay(100);

        while (synthesis.resultInfo.an_ResultIcon.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f)
            await Task.Delay(100);

        #region ##### 합성 성공 결과 장비 서버 전송 
        synthesis.readyInfo.goSelectOk.SetActive(false);

        if (isSyntSuccess)
        {
            synthesis.readyInfo.cgReady.alpha = 1.0f;
            synthesis.resultInfo.txt_SuccessOrFail.text = "펫 진화 성공!";
            synthesis.resultInfo.txt_SuccessOrFail.color = Color.white;
            synthesis.resultInfo.iconInfo_Result.txt_Name.color = Color.white;
            synthesis.resultInfo.go_RootSuccessResult.SetActive(true);
            synthesis.resultInfo.go_RootFailResult.SetActive(false);

            // 펫 3d on off 
            foreach (var item in prvw_PetRdrTexture)
            {
                if (int.Equals(item.pet_rt, synthesis.MainPetDB.p_rt))
                {
                    foreach (var item2 in item.petIdObjs)
                        item2.goPet.SetActive(int.Equals(item2.pet_id, synthesis.MainPetDB.p_id));
                }
                else
                {
                    foreach (var item2 in item.petIdObjs)
                    {
                        if (item2.goPet.activeSelf)
                            item2.goPet.SetActive(false);
                    }
                }
            }

            prvw_GoRoot.SetActive(true);
        }
        else
        {
            synthesis.resultInfo.txt_SuccessOrFail.text = "펫 진화 실패...";
            synthesis.resultInfo.txt_SuccessOrFail.color = Color.gray;
            synthesis.resultInfo.iconInfo_Result.txt_Name.color = Color.gray;
            synthesis.resultInfo.go_RootSuccessResult.SetActive(false);
            synthesis.resultInfo.go_RootFailResult.SetActive(true);
        }

        Task tsk11 = GameDatabase.GetInstance().tableDB.SendDataPet(synthesis.MainPetDB, "update");
        Task tsk22 = GameDatabase.GetInstance().tableDB.SendDataPet(synthesis.MatPetDB, "update");
        while (tsk11.IsCompleted == false || tsk22.IsCompleted == false) await Task.Delay(250);
        GameDatabase.GetInstance().tableDB.SetPetTypeAddSort(PetTapType.Synthesis);
        initOnStartPet.SetInit(false);
        #endregion


        synthesis.resultInfo.blockScreen.CenterObjectDisable();
        synthesis.resultInfo.blockScreen.OnText(0.5f);
    }

    public void Click_SynyResultClose()
    {
        synthesis.MainPetDB = default;
        synthesis.MatPetDB = default;
        MainUI.GetInstance().tapPet.UISynthesis();
        initOnStartPet.SetInit(false);
    }
    #endregion

    #region ##### UI 도감 #####
    [System.Serializable]
    class Encyclopedia
    {
        public GameObject goRoot;
    }

    /// <summary> 도감 </summary>
    public void UIEncyclopedia()
    {
        prvw_GoRoot.SetActive(false);
    }
#endregion
}
