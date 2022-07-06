using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerZombie : MonoBehaviour
{
    public bool isInitZb = false;
    public IG.MonsterType zbType = IG.MonsterType.MINE;

    //[HideInInspector]
    public string zbName;

    public Tr tr;
    [System.Serializable]
    public struct Tr
    {
        public Transform transf;
        public Transform trfPetInitPos;
        public Transform trAnim;
        public Transform targetRespwan;
        public Transform zbCamTarget;
        public Transform buffBackPos;
        public Transform throw_curve_point;
        public Transform throw_end_point;
        public GameObject goOnSkActive;
        public ObjectLife objLife;
    }

    public bool changeSkill = false;
    public IG.Parts[] partsArray = new IG.Parts[9];
    public IG.Player igp = new IG.Player();
    public PlayerZombie targetPz;
    [SerializeField] public Animator anim;

    [HideInInspector] public int eq_rt, eq_id;
    [SerializeField] MonsterMeshColor mnstMeshColor;
    [SerializeField] RuntimeAnimatorController ac_l, ac_r;
    [SerializeField] GameObject wpn_l, wpn_r, shd_l, shd_r;
    [SerializeField] Image hpAmount, hpBgAmount;
    [SerializeField] Text hpValue;

    [SerializeField]
    private long nowHealth { get; set; }
    [SerializeField]
    public long NowHealth
    {
        get
        {
            return nowHealth;
        }
        set
        {
            nowHealth = value < 0 ? 0 : value;
            if (nowHealth > igp.statValue.stat5_valHealth)
                nowHealth = igp.statValue.stat5_valHealth;

            if (hpValue)
            {
                if (isInitZb)
                    hpValue.text = string.Format("{0:#,0} / {1:#,0}", nowHealth.ToString(), igp.statValue.stat5_valHealth.ToString());
                else
                    hpValue.text = "";
            }
        }
    }

    Color cor_hp_hit = new Color(1, 0, 0, 0.5f);
    Color cor_hp_rev = new Color(1, 1, 1, 0.5f);
    Vector3 v3DisablePos = new Vector3(0, 100, 0);

    public long GetHp() => NowHealth;
    public GameObject go_HandThrowBomb, go_HandThrowSmoke;

    [SerializeField]
    public EtcGameObject etcGameObject;
    [System.Serializable]
    public struct EtcGameObject
    {
        public Transform hip_j; // 가운데 
        public Transform head; // 머리 
        public Transform bottom; // 바닥 
        public Transform hitPosTr;

        public Transform hand_l, hand_r;
    }

    void OnDisable()
    {
        tr.transf.localPosition = new Vector3(50, 0.1f, -5.425f);
    }

    void Awake()
    {
        //anim = this.transform.GetComponentInChildren<Animator>();
    }

    public void Reset()
    {
        isInitZb = false;
        zbType = IG.MonsterType.NONE;
        igp.state = IG.ZombieState.Ready;
        for (var i = 0; i < igp.activateSkills.Count; i++)
            igp.activateSkills.Remove(igp.activateSkills[i]);

        igp.parts = new IG.PartsIdx();
        igp.statValue = new GameDatabase.CharacterDB.StatValue();
        igp.activateSkills.Clear();
        igp.activeSkillCancel.Clear();
        igp.etcZbData = new IG.EtcZbData();
        igp.playerSkillAction.isNowSkillAction = false;
    }

    /// <summary> 유저 좀비 데이터 초기화 </summary>
    public void InitPlayer(bool _isPlayer)
    {
        StopCoroutine("IFightLoop");
        isInitZb = false;
        targetPz = null;
        bool isBoss = GameMng.GetInstance().StageNbr() >= 10;
        zbType = _isPlayer == true ? IG.MonsterType.MINE : IG.MonsterType.NONE;
        igp.state = IG.ZombieState.Ready;
        tr.transf.SetParent(igp.parent);
        if (tr.trAnim)
            tr.trAnim.localPosition = Vector3.zero;

        if (_isPlayer)
        {
            tr.transf.SetPositionAndRotation(igp.respwanPos.position, new Quaternion(0, 0, 0, 0));
        }
        else
        {
            tr.transf.SetPositionAndRotation(GameMng.GetInstance ().myPZ.tr.targetRespwan.position, new Quaternion(0, 0, 0, 0));
            if (mnstMeshColor != null)
            {
                var mdty = GameMng.GetInstance().mode_type;
                if (mdty == IG.ModeType.DUNGEON_MINE || mdty == IG.ModeType.DUNGEON_RAID)
                {
                    int dgnID = mdty == IG.ModeType.DUNGEON_MINE ? GameMng.GetInstance().stc_DungeonMine.inDgNbr : GameMng.GetInstance().stc_DungeonRaid.inDgNbr;
                    int dgnMnstID = mdty == IG.ModeType.DUNGEON_MINE ? GameMng.GetInstance().stc_DungeonMine.qStat.Count : GameMng.GetInstance().stc_DungeonRaid.qStat.Count;
                    mnstMeshColor.SetDgnMonster(mdty, dgnID, dgnMnstID);
                }
                else
                {
                    int csn = GameMng.GetInstance().ChapterStageNbr();
                    int mid = csn % mnstMeshColor.NormalMeshLastID;
                    int mco = csn % mnstMeshColor.NormalMeshLastColorID;
                    int mnstID = mid == 0 ? mnstMeshColor.NormalMeshLastID : mid;
                    int mnstCoID = mco == 0 ? mnstMeshColor.NormalMeshLastColorID : mco;
                    mnstMeshColor.SetNormalMonsterColor(mnstID, mnstCoID);
                }
            }
        }

        anim = tr.transf.GetComponentInChildren<Animator>();
        if (ac_l != null && ac_r != null)
            anim.runtimeAnimatorController = _isPlayer == true ? ac_l : ac_r;

        anim.SetFloat("SkillSpeed", GameMng.GetInstance().GameSpeed);
        anim.SetFloat("AttackSpeed", GetMyAttackSpeed());
        anim.speed = GameMng.GetInstance().GameSpeed;
        if (wpn_l != null && wpn_r != null)
        {
            wpn_l.gameObject.SetActive(_isPlayer);
            wpn_r.gameObject.SetActive(!_isPlayer);
            go_HandThrowBomb = _isPlayer ? wpn_l.transform.Find("Throw").transform.Find("Bomb").gameObject : wpn_r.transform.Find("Throw").transform.Find("Bomb").gameObject;
            go_HandThrowSmoke = _isPlayer ? wpn_l.transform.Find("Throw").transform.Find("Smoke").gameObject : wpn_r.transform.Find("Throw").transform.Find("Smoke").gameObject;
            go_HandThrowBomb.SetActive(false);
            go_HandThrowSmoke.SetActive(false);
        }

        if (shd_l != null) shd_l.gameObject.SetActive(!_isPlayer);
        if (shd_r != null) shd_r.gameObject.SetActive(_isPlayer);

        hpValue = MainUI.GetInstance().tapGameBattleInfo.GetHpText(_isPlayer);
        hpAmount = MainUI.GetInstance().tapGameBattleInfo.GetHpAmount(_isPlayer);
        hpBgAmount = MainUI.GetInstance().tapGameBattleInfo.GetHpBgAmount(_isPlayer);

        Setting(); // 스탯, 스킬 세팅 
        StartCoroutine("IFightLoop");
        isInitZb = true;
    }

    // my 스킬 
    public void PlayerSettingSkills()
    {
        Dictionary<int, int[]> skill_data = new Dictionary<int, int[]>();
        // 스킬 
        var myUnusedSkills = GameDatabase.GetInstance().tableDB.GetUnusedSkillAll();
        
        List<int> random = new List<int>();
        for (int i = 0; i < myUnusedSkills.Count; i++)
            random.Add(i);
        List<int> shuffled = new List<int>(Utility.ShuffleArray(random, (int)Time.time));

        int shf_id = 0;
        int slot_id = 0;
        int n_mSlotNum = GameDatabase.GetInstance().tableDB.GetUseMainSlot();

        var data_m_slot = GameDatabase.GetInstance().tableDB.GetSkillSlot(n_mSlotNum);
        foreach (var item in data_m_slot.slot)
        {
            bool isUnusedRandom = true;
            long sk_uid = item.aInUid;
            int sk_id = 0, // 번호 
                sk_lv = 0, // 레벨 
                sk_rt = 0, // 등급 
                sk_pt = 0, // 발동 포인트 
                sk_rn = 0; // 착용중0 or 랜덤1 

            if (sk_uid > 0)
            {
                var slot_data = GameDatabase.GetInstance().tableDB.GetFindSkill(sk_uid);
                if (slot_data.aInUid > 0)
                {
                    sk_id = slot_data.idx;
                    sk_lv = slot_data.level;
                    sk_rn = 0;
                    isUnusedRandom = false;
                }
            }

            if (isUnusedRandom)
            {
                var sdb = myUnusedSkills[shuffled[shf_id]];
                sk_id = sdb.idx;
                sk_lv = sdb.level;
                sk_rn = 1;
                shf_id++;
            }

            var cdb = GameDatabase.GetInstance().chartDB.GetChartSkill_Data(sk_id);
            sk_rt = cdb.s_rating;
            sk_pt = cdb.s_pnt;
            skill_data.Add(slot_id, new int[] { sk_id, sk_lv, sk_rt, sk_pt, sk_rn });
            slot_id++;
        }

        igp.playerSkillAction.InitSkill(skill_data);
        MainUI.GetInstance().tapGameBattleInfo.InitSetBattleInfo(zbType == IG.MonsterType.MINE, skill_data);
        changeSkill = false;
    }

    [SerializeField] cdb_chpt_mnst_stat mnst_stat;
    /// <summary> 좀비 스텟 </summary>
    public async void Setting()
    {
        if (zbType == IG.MonsterType.MINE)
        {
            zbType = IG.MonsterType.MINE;
            // 스탯 
            igp.statValue = GameDatabase.GetInstance().characterDB.GetStat(); // 장비 관련, 스탯 
            PlayerSettingSkills();
        }
        else
        {
            Dictionary<int, int[]> skill_data = new Dictionary<int, int[]>();
            var mdty = GameMng.GetInstance().mode_type;

            // 챕터, 스테이지 진행 
            if (mdty == IG.ModeType.CHAPTER_CONTINUE || mdty == IG.ModeType.CHAPTER_LOOP)
            {
                zbName = "";
                bool isBoss = GameMng.GetInstance().StageNbr() >= 10 || GameMng.GetInstance ().stage_type == IG.StageType.BOSS_MONSTER;
                #region ##### 유저 보스 데이터 로드 #####
                // 보스 몬스터 
                if (isBoss)
                {
                    JsonData row_userBossDb = null;
                    /*
                    if (mdty == IG.ModeType.CHAPTER_CONTINUE)
                    {
                        int chapter_nbr = GameDatabase.GetInstance().tableDB.GetUserInfo().m_chpt_stg_nbr;
                        Task<JsonData> jd_user_boss = GameDatabase.GetInstance().rankDB.AGetChapterRTRankUser(chapter_nbr + 1);
                        await jd_user_boss;
                        row_userBossDb = jd_user_boss.Result;
                        LogPrint.Print("<color=magenta>################# 유저 보스 row_userBossDb : " + row_userBossDb + "#################</color>");
                        // 유저 보스 
                        if (row_userBossDb != null)
                        {
                            try
                            {
                                zbName = "유저 보스 네임";
                                LogPrint.Print("<color=magenta> 유저 보스 DB : " + row_userBossDb.ToJson() + "</color>");
                                // 파츠 
                                igp.parts = JsonUtility.FromJson<IG.PartsIdx>(RowPaser.StrPaser(row_userBossDb, "parts"));
                                // 스탯, 장신구 전용 옵션 스탯
                                igp.statValue = JsonUtility.FromJson<GameDatabase.CharacterDB.StatValue>(RowPaser.StrPaser(row_userBossDb, "statValue"));
                                this.eq_rt = igp.statValue.eq_rt;
                                this.eq_id = igp.statValue.eq_id;

                                // 장신구 옵션 id
                                var pubDb_accSop = JsonUtility.FromJson<GameDatabase.PublicContentDB.PubDB_ClearChapterChar_AcceSop>(RowPaser.StrPaser(row_userBossDb, "accSop"));
                                igp.acceSpecialOps = new List<IG.AcceSpecialOp>()
                            {
                                (IG.AcceSpecialOp)pubDb_accSop.ac1_stat_id,
                                (IG.AcceSpecialOp)pubDb_accSop.ac2_stat_id,
                                (IG.AcceSpecialOp)pubDb_accSop.ac3_stat_id,
                            };

                                // 스킬 
                                var pubDb_skill = JsonUtility.FromJson<GameDatabase.PublicContentDB.PubDB_ClearChapterChar_UseSkill>(RowPaser.StrPaser(row_userBossDb, "useSkill"));
                                int n_mSlotNum = GameDatabase.GetInstance().tableDB.GetUseMainSlot();
                                var data_m_slot = GameDatabase.GetInstance().tableDB.GetSkillSlot(n_mSlotNum);
                                for (int i = 0; i < 6; i++)
                                {
                                    int sk_id = 0; // 번호 
                                    int sk_lv = 0; // 레벨 
                                    switch (i)
                                    {
                                        case 0: sk_id = pubDb_skill.slot1_id; sk_lv = pubDb_skill.slot1_lv; break;
                                        case 1: sk_id = pubDb_skill.slot2_id; sk_lv = pubDb_skill.slot2_lv; break;
                                        case 2: sk_id = pubDb_skill.slot3_id; sk_lv = pubDb_skill.slot3_lv; break;
                                        case 3: sk_id = pubDb_skill.slot4_id; sk_lv = pubDb_skill.slot4_lv; break;
                                        case 4: sk_id = pubDb_skill.slot5_id; sk_lv = pubDb_skill.slot5_lv; break;
                                        case 5: sk_id = pubDb_skill.slot6_id; sk_lv = pubDb_skill.slot6_lv; break;
                                    }

                                    var cdb = GameDatabase.GetInstance().chartDB.GetChartSkill_Data(sk_id);
                                    int sk_rt = cdb.s_rating; // 등급 
                                    int sk_pt = cdb.s_pnt; // 발동 포인트 
                                    int sk_rn = 0; // 착용중 or 랜덤 

                                    skill_data.Add(i, new int[] { sk_id, sk_lv, sk_rt, sk_pt, sk_rn });
                                }
                            }
                            catch (System.Exception e)
                            {
                                LogPrint.PrintError(e);
                                row_userBossDb = null;
                            }
                        }
                    }
                    */

                    zbType = row_userBossDb == null ? IG.MonsterType.BOSS_MONSTER : IG.MonsterType.BOSS_USER;
                }
                else zbType = IG.MonsterType.NORM_MONSTER;
                #endregion

                if (zbType == IG.MonsterType.NORM_MONSTER || zbType == IG.MonsterType.BOSS_MONSTER) // 일반 몬스터 or 보스 몬스터일 경우 
                {
                    if (GameMng.GetInstance().stc_ChapterMonstStats.Count == 0)
                        GameMng.GetInstance().ChapterMonsterStat();

                    igp.statValue = GameMng.GetInstance().stc_ChapterMonstStats.Dequeue(); // 스탯, 장신구 전용 옵션 스탯 

                    // 보스 상대시 나의 좀비 체력 리셋 
                    if(zbType == IG.MonsterType.BOSS_MONSTER)
                        GameMng.GetInstance().myPZ.ResetHealth();

                    // 몬스터 스킬 세팅 (플레이어 자신의 스킬 목록에서 랜덤으로 세팅) 
                    if (zbType == IG.MonsterType.BOSS_MONSTER || GameMng.GetInstance().stage_type == IG.StageType.BOSS_MONSTER)
                        skill_data = GameDatabase.GetInstance().monsterDB.GetMonsterRandomSkills();
                }
            }
            else
            {
                if (mdty == IG.ModeType.DUNGEON_TOP)
                {
                    zbType = GameMng.GetInstance().stc_DungeonTop.qStat.Count == 1 ? IG.MonsterType.BOSS_DGN_MONSTER : IG.MonsterType.NORM_DGN_MONSTER;
                    igp.statValue = GameMng.GetInstance().stc_DungeonTop.qStat.Dequeue();
                }
                else if (mdty == IG.ModeType.DUNGEON_MINE)
                {
                    zbType = GameMng.GetInstance().stc_DungeonMine.qStat.Count == 1 ? IG.MonsterType.BOSS_DGN_MONSTER : IG.MonsterType.NORM_DGN_MONSTER;
                    igp.statValue = GameMng.GetInstance().stc_DungeonMine.qStat.Dequeue();
                }
                else if (mdty == IG.ModeType.DUNGEON_RAID)
                {
                    zbType = GameMng.GetInstance().stc_DungeonRaid.qStat.Count == 1 ? IG.MonsterType.BOSS_DGN_MONSTER : IG.MonsterType.NORM_DGN_MONSTER;
                    igp.statValue = GameMng.GetInstance().stc_DungeonRaid.qStat.Dequeue();
                }
                else if (mdty == IG.ModeType.PVP_BATTLE_ARENA)
                {
                    zbType = IG.MonsterType.PVP_USER;
                    var pvpDb = GameMng.GetInstance().pvpOpponentDb;
                    igp.parts = pvpDb.parts;
                    igp.statValue = pvpDb.statValue;
                    skill_data = pvpDb.skillData;
                }
            }

            // 파츠 pvp모드아니고 챕터모드의 보스일 경우에만 
            if(zbType != IG.MonsterType.PVP_USER)
            {
                if (zbType == IG.MonsterType.BOSS_MONSTER || zbType == IG.MonsterType.BOSS_DGN_MONSTER || GameMng.GetInstance().stage_type == IG.StageType.BOSS_MONSTER)
                {
                    igp.parts = new IG.PartsIdx()
                    {
                        ty0_weapon_rt = igp.statValue.eq_rt,
                        ty0_weapon_id = igp.statValue.eq_id,
                        ty1_shield_rt = igp.statValue.eq_rt,
                        ty1_shield_id = igp.statValue.eq_id,
                        ty2_helmet_rt = igp.statValue.eq_rt,
                        ty2_helmet_id = igp.statValue.eq_id,
                        ty3_shoulder_l_rt = igp.statValue.eq_rt,
                        ty3_shoulder_l_id = igp.statValue.eq_id,
                        ty3_shoulder_r_rt = igp.statValue.eq_rt,
                        ty3_shoulder_r_id = igp.statValue.eq_id,
                        ty4_armor_rt = igp.statValue.eq_rt,
                        ty4_armor_id = igp.statValue.eq_id,
                        ty5_arm_rt = igp.statValue.eq_rt,
                        ty5_arm_id = igp.statValue.eq_id,
                        ty6_pants_rt = igp.statValue.eq_rt,
                        ty6_pants_id = igp.statValue.eq_id,
                        ty7_boots_rt = igp.statValue.eq_rt,
                        ty7_boots_id = igp.statValue.eq_id,
                    };
                }
                else if (zbType == IG.MonsterType.NONE || zbType == IG.MonsterType.NORM_MONSTER || zbType == IG.MonsterType.NORM_DGN_MONSTER)
                {
                    igp.parts = default;
                }
            }

            eq_rt = igp.statValue.eq_rt;
            eq_id = igp.statValue.eq_id;
            igp.playerSkillAction.InitSkill(skill_data);
            MainUI.GetInstance().tapGameBattleInfo.InitSetBattleInfo(zbType == IG.MonsterType.MINE, skill_data);
        }
        
        SettingParts(-1);
        LogPrint.EditorPrint("igp.statValue.stat5_valHealth : " + igp.statValue.stat5_valHealth +", ismine : " + zbType);
        NowHealth = igp.statValue.stat5_valHealth;
    }

    /// <summary> 좀비 장비 파츠 세팅 </summary>
    public void SettingParts(int changed_type = -1) // -1 : all 
    {
        //나의 좀비 장비 파츠 세팅 
        if (zbType == IG.MonsterType.MINE) 
        {
            if (changed_type == -1 || changed_type == 0) // 무기 
            {
                var weapon = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(0);
                igp.parts.ty0_weapon_rt = weapon.eq_rt;
                igp.parts.ty0_weapon_id = weapon.eq_id;
            }

            if (changed_type == -1 || changed_type == 1) // 방패 
            {
                var shield = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(1);
                igp.parts.ty1_shield_rt = shield.eq_rt;
                igp.parts.ty1_shield_id = shield.eq_id;
            }

            if (changed_type == -1 || changed_type == 2) // 헬멧 
            {
                var helmet = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(2);
                igp.parts.ty2_helmet_rt = helmet.eq_rt;
                igp.parts.ty2_helmet_id = helmet.eq_id;
            }

            if (changed_type == -1 || changed_type == 3) // 어깨 
            {
                var shoulder = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(3);
                igp.parts.ty3_shoulder_r_rt = shoulder.eq_rt;
                igp.parts.ty3_shoulder_r_id = shoulder.eq_id;
                igp.parts.ty3_shoulder_l_rt = shoulder.eq_rt;
                igp.parts.ty3_shoulder_l_id = shoulder.eq_id;
            }

            if (changed_type == -1 || changed_type == 4) // 갑옷 
            {
                var armor = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(4);
                igp.parts.ty4_armor_rt = armor.eq_rt;
                igp.parts.ty4_armor_id = armor.eq_id;
            }

            if (changed_type == -1 || changed_type == 5) // 팔 
            {
                var arm = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(5);
                igp.parts.ty5_arm_rt = arm.eq_rt;
                igp.parts.ty5_arm_id = arm.eq_id;
            }

            if (changed_type == -1 || changed_type == 6) // 바지 
            {
                var pants = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(6);
                igp.parts.ty6_pants_rt = pants.eq_rt;
                igp.parts.ty6_pants_id = pants.eq_id;
            }

            if (changed_type == -1 || changed_type == 7) // 부츠 
            {
                var boots = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(7);
                igp.parts.ty7_boots_rt = boots.eq_rt;
                igp.parts.ty7_boots_id = boots.eq_id;
            }
        }
        else
        {

        }

        SettingPartsObject();
    }

    public void SettingPartsPreview(int eq_ty, int eq_rt, int eq_id)
    {
        switch (eq_ty)
        {
            case 0:
                igp.parts.ty0_weapon_rt = eq_rt;
                igp.parts.ty0_weapon_id = eq_id;
                break;
            case 1:
                igp.parts.ty1_shield_rt = eq_rt;
                igp.parts.ty1_shield_id = eq_id;
                break;
            case 2:
                igp.parts.ty2_helmet_rt = eq_rt;
                igp.parts.ty2_helmet_id = eq_id;
                break;
            case 3:
                igp.parts.ty3_shoulder_r_rt = eq_rt;
                igp.parts.ty3_shoulder_l_rt = eq_rt;
                igp.parts.ty3_shoulder_r_id = eq_id;
                igp.parts.ty3_shoulder_l_id = eq_id;
                break;
            case 4:
                igp.parts.ty4_armor_rt = eq_rt;
                igp.parts.ty4_armor_id = eq_id;
                break;
            case 5:
                igp.parts.ty5_arm_rt = eq_rt;
                igp.parts.ty5_arm_id = eq_id;
                break;
            case 6:
                igp.parts.ty6_pants_rt = eq_rt;
                igp.parts.ty6_pants_id = eq_id;
                break;
            case 7:
                igp.parts.ty7_boots_rt = eq_rt;
                igp.parts.ty7_boots_id = eq_id;
                break;
        }

        SettingPartsObject();
    }

    private void SettingPartsObject()
    {
        if (partsArray.Length == 0)
            return;

        // 착용 장비 세팅 
        string[] partsName = new string[] { "weapon_l", "weapon_r", "shield_l", "shield_r", "helmet", "shoulder_l", "shoulder_r", "armor", "arm", "pants", "boots" };
        foreach (var parItem in partsArray)
        {
            var parObj = parItem.parArray;
            int partsRat = 0, partsIdx = 0;
            switch (parItem.partsName)
            {
                case "weapon_l":
                case "weapon_r": partsRat = igp.parts.ty0_weapon_rt; partsIdx = igp.parts.ty0_weapon_id; break;
                case "shield_l":
                case "shield_r": partsRat = igp.parts.ty1_shield_rt; partsIdx = igp.parts.ty1_shield_id; break;
                case "helmet": partsRat = igp.parts.ty2_helmet_rt; partsIdx = igp.parts.ty2_helmet_id; break;
                case "shoulder_l": partsRat = igp.parts.ty3_shoulder_r_rt; partsIdx = igp.parts.ty3_shoulder_r_id; break;
                case "shoulder_r": partsRat = igp.parts.ty3_shoulder_l_rt; partsIdx = igp.parts.ty3_shoulder_l_id; break;
                case "armor": partsRat = igp.parts.ty4_armor_rt; partsIdx = igp.parts.ty4_armor_id; break;
                case "arm": partsRat = igp.parts.ty5_arm_rt; partsIdx = igp.parts.ty5_arm_id; break;
                case "pants": partsRat = igp.parts.ty6_pants_rt; partsIdx = igp.parts.ty6_pants_id; break;
                case "boots": partsRat = igp.parts.ty7_boots_rt; partsIdx = igp.parts.ty7_boots_id; break;
            }

            for (int i = 0; i < parObj.Count; i++)
            {
                var parObjs = parObj[i].partObj;
                for (int f = 0; f < parObjs.Count; f++)
                {
                    if (parObjs[f].mesh != null)
                    {
                        GameObject _eqGoMesh = null;
                        bool isActiveSelf = (i == partsRat && parObjs[f].idx == partsIdx);
                        if (string.Equals(parItem.partsName, "weapon_l") || string.Equals(parItem.partsName, "weapon_r") || string.Equals(parItem.partsName, "shield_l") || string.Equals(parItem.partsName, "shield_r"))
                        {
                            if (zbType == IG.MonsterType.MINE)
                            {
                                if (string.Equals(parItem.partsName, "weapon_l"))
                                {
                                    _eqGoMesh = parObjs[f].mesh.gameObject;
                                }

                                if (string.Equals(parItem.partsName, "shield_r"))
                                {
                                    _eqGoMesh = parObjs[f].mesh.gameObject;
                                }
                            }
                            else
                            {
                                if (string.Equals(parItem.partsName, "weapon_r"))
                                {
                                    _eqGoMesh = parObjs[f].mesh.gameObject;
                                }

                                if (string.Equals(parItem.partsName, "shield_l"))
                                {
                                    _eqGoMesh = parObjs[f].mesh.gameObject;
                                }
                            }
                        }
                        else
                        {
                            _eqGoMesh = parObjs[f].mesh.gameObject;
                        }

                        if (_eqGoMesh != null)
                        {
                            if (isActiveSelf)
                            {
                                if (!_eqGoMesh.activeSelf)
                                {
                                    _eqGoMesh.SetActive(true);
                                }
                            }
                            else
                            {
                                if (_eqGoMesh.activeSelf)
                                {
                                    _eqGoMesh.SetActive(false);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    bool isPauseasde = false;
    private IEnumerator IFightLoop()
    {
        while(targetPz == null)
        {
            SearchTargetPlayerZb();
            yield return null;
            if (targetPz != null)
            {
                //tr.transf.SetPositionAndRotation(targetPz.tr.targetRespwan.position, new Quaternion(0, 0, 0, 0));
                //tr.transf.position = targetPz.tr.targetRespwan.position;
            }
        }

        WaitForSeconds wa = new WaitForSeconds(0.1f);
        bool firstAtk = false;
        while (true)
        {
            if (targetPz != null)
            {
                if (targetPz.igp.state == IG.ZombieState.LOSER || targetPz.zbType == IG.MonsterType.NONE || targetPz.gameObject.activeSelf == false)
                {
                    LogPrint.EditorPrint(" 11111 is Mine : " + zbType + ", GetHp : " + GetHp() + ", targetPz.GetHp() : " + targetPz.GetHp());
                    targetPz = null;
                }
            }

            if (targetPz != null)
            {
                if (GetHp() > 0 && targetPz.GetHp() > 0)
                {
                    float sqr = GameMng.GetSqr(tr.transf.position, targetPz.tr.transf.position);
                    if (sqr > 8.25f) // 상대한테 다가가도록 이동 
                    {
                        firstAtk = false;
                        igp.etcZbData.accuracy_correction = 0;

                        igp.state = IG.ZombieState.MOVE_POS;
                        if (anim.GetBool("Walk") == false)
                            anim.SetBool("Walk", true);
                            
                        anim.speed = GameMng.GetInstance().GameSpeed;
                        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                        {
                            while (GameMng.GetSqr(tr.transf.position, targetPz.tr.transf.position) > 8.25f)
                            {
                                //transform.position = Vector3.MoveTowards(tr.transf.position, targetPz.tr.transf.position, GameMng.GetInstance().GameDeltaTime * 1.5f);
                                transform.position = Vector3.Lerp(tr.transf.position, targetPz.tr.transf.position, GameMng.GetInstance().GameDeltaTime * (zbType == IG.MonsterType.MINE ? 0.4f : 0.5f));
                                yield return null;
                            }
                        }
                    }
                    else  // 공격 
                    {
                        if(GameMng.GetInstance().mode_type != IG.ModeType.CHANGE_WAIT)
                        {
                            igp.state = IG.ZombieState.FIGHT;
                            if (anim.GetBool("Walk") == true)
                                anim.SetBool("Walk", false);

                            yield return null;
                                
                            if(targetPz.GetHp() > 0)
                            {
                                bool isIdle = anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") == true && anim.GetCurrentAnimatorStateInfo(0).IsName("GeneralAttack1") == false;
                                if (isIdle && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.01f)
                                {
                                    bool isStunned = targetPz.igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_4) >= 0; // 상대방이 기절 스킬 발동인가? 
                                    if (isStunned)
                                        isStunned = igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_3) >= 0 ? false : true; // 자신이 기절 방어 상태다 

                                    if (isStunned == false)
                                    {
                                        if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_18) >= 0) // 스킬 18 : 체력 회복 발동 체크 
                                        {
                                            if (igp.etcZbData.last_attack_count != igp.etcZbData.attack_count)
                                            {
                                                igp.etcZbData.last_attack_count = igp.etcZbData.attack_count;
                                                //ObjectLife ol = ObjectPool.GetInstance().PopFromPool("opOnce_Sk18Ef_Aura", this.transform.position);
                                                int sk18lv = igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_18);
                                                long recv_hp = GameDatabase.GetInstance().chartDB.GetSkillAbility18_RecoveryHealth(sk18lv, this.GetHp());
                                                RecoveryHealth(recv_hp);
                                            }
                                        }

                                        if (igp.playerSkillAction.isNowSkillAction == false && targetPz.igp.playerSkillAction.isNowSkillAction == false)
                                        {
                                            if (!firstAtk)
                                            {
                                                if (igp.statValue.atk_spd >= targetPz.igp.statValue.atk_spd)
                                                    yield return new WaitForSeconds(0.25f);

                                                firstAtk = true;
                                            }

                                            bool isSkOn = zbType == IG.MonsterType.MINE ? ConvenienceFunctionMng.GetInstance().convenFun.cfAutoSkill.onOff == IG.ConvenienceFunction.OnOff.ON :
                                                (zbType == IG.MonsterType.PVP_USER ||
                                                zbType == IG.MonsterType.BOSS_MONSTER ||
                                                zbType == IG.MonsterType.BOSS_DGN_MONSTER ||
                                                GameMng.GetInstance().stage_type == IG.StageType.BOSS_MONSTER);
                                            bool isSkillActve = false;
                                            if (isSkOn)
                                            {
                                                Task<bool> task_isSkActv = igp.playerSkillAction.ActiveSkill();
                                                while (task_isSkActv.IsCompleted == false)
                                                    yield return null;

                                                isSkillActve = task_isSkActv.Result;
                                            }

                                            if (isSkillActve == false)
                                            {
                                                GeneralAttack(); // 기본 공격 
                                                while (anim.GetCurrentAnimatorStateInfo(0).IsName("GeneralAttack1") == false)
                                                    yield return null;
                                            }

                                            igp.etcZbData.last_attack_count = igp.etcZbData.attack_count;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    
                    if (targetPz != null && igp.state != IG.ZombieState.WINNER && igp.state != IG.ZombieState.LOSER)
                    {
                        LogPrint.EditorPrint(" 22222 is Mine : " + zbType + ", GetHp : " + GetHp() + ", targetPz.GetHp() : " + targetPz.GetHp());

                        //TakeDamage
                        bool isTkDmg = targetPz.TakeDamage(true, 0, false, true);
                        if (isTkDmg)
                            targetPz = null;

                        //if (targetPz.GetHp() < 0)
                        //{
                        //    Loser(); // 현재 좀비 패배 
                        //    targetPz.Winner(); // 상대 좀비 승리 
                        //}
                    }
                }
            }
            else
            {
                SearchTargetPlayerZb();
            }

            yield return wa;
        }
    }

    void SearchTargetPlayerZb()
    {
        PlayerZombie tgr = zbType == IG.MonsterType.MINE ? GameMng.GetInstance().orPZ : GameMng.GetInstance().myPZ;
        if (tgr && tgr.gameObject.activeSelf)
        {
            //if ((tgr.igp.state == IG.ZombieState.Ready || tgr.igp.state == IG.ZombieState.FIGHT || tgr.igp.state == IG.ZombieState.MOVE_POS) && tgr.zbType != IG.MonsterType.NONE)
            //{
                targetPz = tgr;
            //}
        }
    }

    void Attack()
    {

    }

    /// <summary> 공격자 기준으로 공격 액션 이전에 디버프 체크 후 적용 </summary>
    //IEnumerator CCheckAttackReadyDebuff()
    //{
    //    if (targetPz == null)
    //        yield break;

    //    bool isCheckAtvSk = igp.playerSkillAction.IsCheckActiveSkill();
    //    //igp.playerSkillAction.isNowSkillAction = isCheckAtvSk;
    //    foreach (var item in targetPz.igp.activateSkills) // 상대 스킬 발동 리스트 
    //    {
    //        var chart_db = GameDatabase.GetInstance().chartDB.GetChartSkill_Data((int)item);
    //        if (isCheckAtvSk == false)
    //        {
    //            #region # 디버프 적용 # => 스킬 7번 : 공격 액션 전 경직 
    //            if (item == IG.SkillNumber.NUMBER_7)
    //            {
    //                PauseAnimation();
    //                anim.Play("GetStiffen");
    //                yield return new WaitForSeconds(0.2f);
    //                ResumAnimation();
    //            }
    //            #endregion
    //        }

    //        #region # 디버프 적용 # => 스킬 6번 : 내가 상대를 공격 이후 나 자신은 출혈 대미지를 입는다. 
    //        if (item == IG.SkillNumber.NUMBER_6)
    //        {
    //            // 상대의스킬 6번 : 도트(출혈) 대미지를 입는다 
    //            int sk6_lv = targetPz.igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_6);
    //            int dot_dmg = GameDatabase.GetInstance().chartDB.GetSkillAbility6_DotDamage(sk6_lv, targetPz.igp.statValue.stat1_valPower);
    //            EtcTakeDamage(dot_dmg, ResourceDatabase.GetInstance().hitColor_Bleeding, "opOnce_Sk6Ef_Hit", "arena_hit", etcGameObject.hip_j.position);
    //        }
    //        #endregion
    //    }
    //}

    /// <summary> 기타 대미지 </summary>
    public void EtcTakeDamage (long _dmg, Color _cor, string _pool_name, string _snd_name, Vector3 _ef_pool_pos)
    {
        TakeDamage(false, _dmg, false);
        HitDamageText(_dmg, _cor, etcGameObject.hitPosTr.position);
        ObjectPool.GetInstance().PopFromPool(_pool_name, _ef_pool_pos);
        SoundManager.GetInstance().PlaySound(_snd_name);
    }

    public void PauseAnimation ()
    {
        anim.SetFloat("Speed", 0f);
        anim.SetFloat("SkillSpeed", 0f);
        anim.SetFloat("AttackSpeed", 0f);
    }

    public void ResumAnimation ()
    {
        if (targetPz != null)
        {
            if (targetPz.igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_4) == -1) // 상대가 기절 스킬을 발동중이 아닐때 
            {
                anim.SetFloat("Speed", GameMng.GetInstance().GameSpeed);
                anim.SetFloat("SkillSpeed", GameMng.GetInstance().GameSpeed);
                anim.SetFloat("AttackSpeed", GetMyAttackSpeed());
            }
        }
    }

    public void PauseAnimationSkill ()
    {
        StopCoroutine("OrPlayerAnimationPause");
        StartCoroutine("OrPlayerAnimationPause");
    }
    private IEnumerator OrPlayerAnimationPause ()
    {
        bool isPause = true;
        while (isPause == true)
        {
            yield return null;
            if (targetPz != null)
            {
                anim.SetFloat("Speed", 0f);
                anim.SetFloat("AttackSpeed", 0f);
                isPause = targetPz.igp.playerSkillAction.isNowSkillAction;
            }
        }

        yield return null;
        anim.SetFloat("Speed", GameMng.GetInstance().GameSpeed);
        if (targetPz != null)
        {
            if (targetPz.igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_4) == -1) // 상대가 기절 스킬을 발동중이 아닐때 
            {
                anim.SetFloat("AttackSpeed", GetMyAttackSpeed());
            }
        }
    }

    /// <summary> 기본 공격 </summary>
    private bool GeneralAttack ()
    {
        bool isOk = true; // 공격 가능한 상태인가 체크 후 공격을 한다. 
        if(targetPz != null)
        {
            if (targetPz.igp.state == IG.ZombieState.LOSER || igp.state == IG.ZombieState.WINNER)
            {
                isOk = false;
            }
        }

        if (isOk)
        {
            anim.SetFloat("AttackSpeed", GetMyAttackSpeed());
            anim.Play("GeneralAttack1");
            return true;
        }

        return false;
    }

    // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    #region -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- 스탯 관련 계산 -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    /// <summary> 기본 공격 : 크리 or 일반 대미지 적용 </summary>
    public void TakeGeneralAttack()
    {
        bool is_cri_chnc = this.GetMyCriticalChance(); // 크리 성공 여부 
        long dmg = is_cri_chnc == true ? this.GetMyCriticalAttackPower() : this.GetMyAttackPower(0);
        this.Hit(true, dmg, is_cri_chnc);
        this.igp.etcZbData.attack_count++;
        MainUI.GetInstance().tapGameBattleInfo.RefreshBuff();
    }

    /// <summary> 스탯:회피율 </summary>
    public float GetMyEvasion()
    {
        float val_evsi = igp.statValue.stat7_valEvasion;
        #region --------- this ----------
        float mtp_pls = 0f;
        if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_23) >= 0) // this 스킬 23번 : 나의 체력이 30%이하라면 적용 -> 회피율 증가 
        {
            if (this.GetHp() < (long)(this.igp.statValue.stat5_valHealth * 0.3f)) // 30% 이하인지 체크 
            {
                int sk23lv = igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_23);
                mtp_pls += GameDatabase.GetInstance().chartDB.GetSkillAbility23_UpEvasion(sk23lv);
            }
        }
        val_evsi += (float)(igp.statValue.stat7_valEvasion * mtp_pls); // this 기본 + 증가된 회피율
        #endregion

        #region --------- other ----------

        #endregion

        return val_evsi;
    }

    /// <summary> 스탯:명중률 </summary>
    public float GetMyAccuracy()
    {
        float df_acur = igp.statValue.stat3_valAccuracy;// 자신의 명중률 
        #region --------- this ----------
        // 나의 명중률 증가 관련 스킬 체크 
        if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_10) >= 0) // 스킬 10번 : 명중률 증가 
        {
            int sk10_lv = igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_10);
            df_acur += GameDatabase.GetInstance().chartDB.GetSkillAbility10_Accuracy(sk10_lv, igp.statValue.stat3_valAccuracy);
        }
        #endregion

        #region --------- other ----------
        if (targetPz != null)
        {
            if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_26) == -1) // this 스킬 26 번 : 스탯 감소되는 스킬을 무력화 발동중 아닐 때 
            {
                // 상대방의 명중률 감소 관련 스킬 체크 
                if (targetPz.igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_16) >= 0)
                {
                    int sk16lv = targetPz.igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_16);
                    df_acur = GameDatabase.GetInstance().chartDB.GetSkillAbility16_DownDefense(sk16lv, df_acur);
                }
            }
        }
        #endregion

        if (df_acur <= 0)
            df_acur = 0;

        return df_acur;
    }

    /// <summary> 스탯:방어력, (상대의 대미지를 받아와 자신이 타격받을 대미지를 리턴) </summary>
    public long GetMyDefenseDamage(long _get_dmg, bool _is_gnr_atk) // _is_gnr_atk : (true -> 일반 공격, false -> 스킬 공격) 
    {
        long val_defense = igp.statValue.stat2_valDefense;
        #region --------- this ----------
        float mtp_pls = 0f;
        if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_11) >= 0) // this 스킬 11번 : 방어력 증가 발동 중이라면 
        {
            int sk11_lv = igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_11);
            mtp_pls += GameDatabase.GetInstance().chartDB.GetSkillAbility11_UpDefense(sk11_lv);

            LogPrint.Print("<color=yellow> this 스킬 11번 : 방어력 증가 발동 중이라면 mtp_pls : " + mtp_pls + " </color>");
        }

        if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_23) >= 0) // this 스킬 23번 : 나의 체력이 30%이하라면 적용 -> 방어력 증가 
        {
            if (this.GetHp() < (long)(this.igp.statValue.stat5_valHealth * 0.3f)) // 30% 이하인지 체크 
            {
                int sk23lv = igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_23);
                mtp_pls += GameDatabase.GetInstance().chartDB.GetSkillAbility23_UpDefense(sk23lv);
            }

            LogPrint.Print("<color=yellow> this 스킬 23번 : 나의 체력이 30%이하라면 적용 -> 방어력 증가 mtp_pls : " + mtp_pls + " </color>");
        }
        val_defense += (long)(igp.statValue.stat2_valDefense * mtp_pls); // this 기본 + 증가된 방어력 

        if(mtp_pls > 0)
        {
            LogPrint.Print("<color=yellow> this 기본 + 증가된 방어력 " + val_defense + " </color>");
        }
        #endregion

        #region --------- other ----------
        if (targetPz != null)
        {
            if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_26) == -1) // this 스킬 26 번 : 상대로 부터 받는 스킬 대미지를 감소 발동이 아닐 때 
            {
                float mtp_mns = 0f;
                if (targetPz.igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_14) >= 0) // other 스킬 14번 : 방어력 감소 발동중이라면 
                {
                    int sk14lv = targetPz.igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_14);
                    mtp_mns += GameDatabase.GetInstance().chartDB.GetSkillAbility14_DownDefense(sk14lv);
                }
                val_defense -= (long)(val_defense * mtp_mns); // this 기본 + 감소 방어력
            }
        }
        #endregion

        if (val_defense < 0)
            val_defense = 0;

        long rlt_dmg = _get_dmg - val_defense;
        #region --------- this ----------
        if(_is_gnr_atk) // 상대의 일반 공격 대미지 이다 
        {
            if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_3) >= 0) // this 스킬 3번 : 일반 공격 대미지 감소 보호막 발동 중인가 
            {
                int sk3lv = igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_3);
                rlt_dmg = GameDatabase.GetInstance().chartDB.GetSkillAbility3_ShieldDamage(sk3lv, rlt_dmg);
            }

        }
        else // 상대의 스킬 공격 대미지 이다 
        {
            if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_26) >= 0) // this 스킬 26번 : 스킬 대미지 감소 
            {
                int sk26_lv = igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_26);
                rlt_dmg = GameDatabase.GetInstance().chartDB.GetSkillAbility26_DwSkillDamage(sk26_lv, rlt_dmg);
            }

        }
        #endregion

        if (rlt_dmg < 0)
            rlt_dmg = 0;

        return rlt_dmg;
    }

    bool isGeneralDmgZero = false;
    /// <summary> 스탯:공격력 대미지 </summary>
    public long GetMyAttackPower(int _sk_num = 0, int _sk_lv = -1) // _sk_num == 0 : 기본 대미지
    {
        bool is_gnr_atk = _sk_num == 0;
        long val_df_dmg = is_gnr_atk == true ? igp.statValue.stat1_valPower : GameDatabase.GetInstance ().chartDB.GetValueSkillAttackPower(_sk_num, _sk_lv, igp.statValue.stat1_valPower);
        long val_dmg = val_df_dmg;

        #region --------- 좀비 정보를 가지고 ----------
        float mtp_pls = 0f;
        if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_9) >= 0) // this 스킬 9번 : this 공격력 증가 스킬 ON 
        {
            mtp_pls += GameDatabase.GetInstance().chartDB.GetSkillAbility9_AttackPower(igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_9));
        }
       
        if(targetPz != null)
        {
            if ((zbType == IG.MonsterType.MINE && (targetPz.zbType == IG.MonsterType.BOSS_USER || targetPz.zbType == IG.MonsterType.PVP_USER)) || (zbType == IG.MonsterType.PVP_USER && targetPz.zbType == IG.MonsterType.MINE))
            {
                // 2.PvP피해 증가
                if (igp.statValue.sop2_val > 0)
                    mtp_pls += igp.statValue.sop2_val * 0.01f;
            }
            else
            {
                // 1.PvE피해 증가
                if (igp.statValue.sop1_val > 0)
                    mtp_pls += igp.statValue.sop1_val * 0.01f;               
            }

            if (targetPz.zbType == IG.MonsterType.BOSS_MONSTER || targetPz.zbType == IG.MonsterType.BOSS_DGN_MONSTER) // 몬스터 보스 인 경우 
            {
                // 7.보스 몬스터 피해 증가 
                if (igp.statValue.sop7_val > 0)
                    mtp_pls += igp.statValue.sop7_val * 0.01f;
            }
        }

        if (mtp_pls > 0f)
            val_dmg += (long)(val_dmg * mtp_pls);

        #endregion

        float mtp_mns = 0f;
        if (targetPz != null)
        {
            // 상대방의 방어력을 계산하여 최종 대미지 리턴 
            val_dmg = targetPz.GetMyDefenseDamage(val_dmg, is_gnr_atk);
        }

        #region --------- 상대 정보를 가지고 ----------
        if(targetPz != null)
        {
            if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_26) == -1) // this 스킬 26 번 : 스탯 감소되는 스킬을 무력화 발동중 아닐 때 
            {
                if (targetPz.igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_15) >= 0) // this 스킬 15번 : other 공격력 감소 스킬 ON 
                {
                    mtp_mns += GameDatabase.GetInstance().chartDB.GetSkillAbility15_DownDefense(targetPz.igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_15));
                }
            }

            if ((zbType == IG.MonsterType.MINE && (targetPz.zbType == IG.MonsterType.BOSS_USER || targetPz.zbType == IG.MonsterType.PVP_USER)) ||
                 (zbType == IG.MonsterType.PVP_USER && targetPz.zbType == IG.MonsterType.MINE))
            {
                // 4.PvP피해 감소
                if (targetPz.igp.statValue.sop4_val > 0)
                    mtp_mns += targetPz.igp.statValue.sop4_val * 0.01f;
            }
            else
            {
                // 3.PvE피해 감소
                if (targetPz.igp.statValue.sop3_val > 0)
                    mtp_mns += targetPz.igp.statValue.sop3_val * 0.01f;
            }
        }
        #endregion

        if (targetPz != null)
        {
            if (mtp_mns > 0f)
                val_dmg -= (long)(val_dmg * mtp_mns);
        }

        if (val_dmg < 0)
            val_dmg = 1;

        return val_dmg;
    }

    /// <summary> 스탯:치명타 성공률 </summary>
    public bool GetMyCriticalChance()
    {
        float dftChnc = 25.0f;
        if (Random.Range(0.0f, 100.0f) < dftChnc && targetPz != null) // 기본 크리 발동률 
        {
            float orCriEvasion = targetPz.GetMyCriticalEvasion(); // 상대방의 크리티컬 회피율 
            float myCriChnc = igp.statValue.stat6_valCriChance; // 기본 크리 성공률 
            if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_17) >= 0)
                return true;
            else
            {
                if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_26) == -1) // this 스킬 26 번 : 스탯 감소되는 스킬을 무력화 발동중 아닐 때 
                {
                    if (targetPz.igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_19) >= 0) // 방어자 : 스킬 19 - 치명타 성공 확률 감소 스킬 발동 중
                        myCriChnc *= 0.5f;
                }

                return Random.Range(0f, myCriChnc) > orCriEvasion;
            }
        }
        else return false;
    }

    /// <summary> 스탯: 크리티컬 회피율 </summary>
    public float GetMyCriticalEvasion()
    {
        float val_evsi = igp.statValue.stat9_valCriEvasion;
        #region --------- this ----------
        //float mtp_pls = 0f;
        //if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_23) >= 0) // this 스킬 23번 : 나의 체력이 30%이하라면 적용 -> 회피율 증가 
        //{
        //    if (this.GetHp() < Mathf.RoundToInt(this.igp.statValue.stat5_valHealth * 0.3f)) // 30% 이하인지 체크 
        //    {
        //        int sk23lv = igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_23);
        //        mtp_pls += GameDatabase.GetInstance().chartDB.GetSkillAbility23_UpEvasion(sk23lv);
        //    }
        //}
        //val_evsi += Mathf.RoundToInt(igp.statValue.stat2_valDefense * mtp_pls); // this 기본 + 증가된 회피율
        #endregion

        #region --------- other ----------

        #endregion

        return val_evsi;
    }

    bool isCriticalDmgZero = false;
    /// <summary> 스탯:치명타 대미지 </summary>
    public long GetMyCriticalAttackPower()
    {
        long df_cri_dmg = igp.statValue.stat4_valCriPower + this.GetMyAttackPower(0);

        #region --------- this ----------
        if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_17) >= 0) // 스킬 17번 : 확률 100% 고정 
        {
            int sk17lv = igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_17);
            float mtp = GameDatabase.GetInstance().chartDB.GetSkillAbility17_UpCriticalAttackPower(sk17lv);
            df_cri_dmg += (long)(df_cri_dmg * mtp);
        }
        #endregion

        #region --------- other ----------
        float mtp_mns = 0f;
        if(targetPz != null)
        {
            if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_26) == -1) // this 스킬 26 번 : 스탯 감소되는 스킬을 무력화 발동중 아닐 때 
            {
                if (targetPz.igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_19) >= 0) // 스킬 19 : 확률 50% 감소, 대미지 ? 감소 
                {
                    int sk19lv = igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_19);
                    float mtp = GameDatabase.GetInstance().chartDB.GetSkillAbility19_DwCriticalAttackPower(sk19lv);
                    df_cri_dmg -= (long)(df_cri_dmg * mtp);
                }
            }

            if ((zbType == IG.MonsterType.MINE && (targetPz.zbType == IG.MonsterType.BOSS_USER || targetPz.zbType == IG.MonsterType.PVP_USER)) ||
                 (zbType == IG.MonsterType.PVP_USER && targetPz.zbType == IG.MonsterType.MINE))
            {
                // 4.PvP피해 감소
                if (targetPz.igp.statValue.sop4_val > 0)
                    mtp_mns += targetPz.igp.statValue.sop4_val * 0.01f;
            }
            else
            {
                // 3.PvE피해 감소
                if (targetPz.igp.statValue.sop3_val > 0)
                    mtp_mns += targetPz.igp.statValue.sop3_val * 0.01f;
            }
        }
        if (mtp_mns > 0f)
            df_cri_dmg -= (long)(df_cri_dmg * mtp_mns);

        #endregion

        if (df_cri_dmg < 0) 
            df_cri_dmg = 0;

        if (targetPz != null)
        {
            long rlt_cri_dmf = targetPz.GetMyCriticalDefenseDamage(df_cri_dmg);
            return rlt_cri_dmf;
        }
        else
        {
            return df_cri_dmg;
        }
    }
    /// <summary> 스탯:치명타 방어력, (상대의 치명타 대미지를 받아와 자신이 타격받을 대미지를 리턴) </summary>
    public long GetMyCriticalDefenseDamage(long _get_cri_dmg)
    {
        long val_cri_defense = igp.statValue.stat8_valCriDefense;
        #region --------- this ----------
        float mtp_pls = 0f;
        if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_23) >= 0) // this 스킬 23번 : 나의 체력이 30%이하라면 적용 -> 치명타 방어력 증가 
        {
            if (this.GetHp() < (long)(this.igp.statValue.stat5_valHealth * 0.3f)) // 30% 이하인지 체크 
            {
                int sk23lv = igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_23);
                mtp_pls += GameDatabase.GetInstance().chartDB.GetSkillAbility23_UpCriticalDefense(sk23lv);
            }
        }
        val_cri_defense += (long)(igp.statValue.stat8_valCriDefense * mtp_pls); // this 기본 + 증가된 방어력 
        #endregion

        #region --------- other ----------

        #endregion

        if (val_cri_defense < 0)
            val_cri_defense = 0;

        long rlt_dmg = _get_cri_dmg - val_cri_defense;
        #region --------- this ----------
        if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_3) >= 0) // this 스킬 3번 : 보호막 발동 중인가 
        {
            int sk3lv = igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_3);
            rlt_dmg = GameDatabase.GetInstance().chartDB.GetSkillAbility3_ShieldDamage(sk3lv, rlt_dmg);
        }
        #endregion

        if (rlt_dmg < 0)
            rlt_dmg = 0;

        return rlt_dmg;
    }

    /// <summary> 스탯:공격 속도 </summary>
    public float GetMyAttackSpeed()
    {
        float df_atkSpd = igp.statValue.atk_spd;
        #region --------- other ----------

        #endregion

        #region --------- other ----------
        if (targetPz != null)
        {
            if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_26) == -1) // this 스킬 26 번 : 스탯 감소되는 스킬을 무력화 발동중 아닐 때 
            {
                if (targetPz.igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_20) >= 0) // 스킬 20번 : 상대방이 공격 속도 감소 스킬 발동 중 
                {
                    float tmp_atk_spd = df_atkSpd;
                    int sk20_lv = targetPz.igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_20);
                    df_atkSpd = GameDatabase.GetInstance().chartDB.GetSkillAbility20_DwAttackSpeed(sk20_lv, tmp_atk_spd);
                }
            }
        }
        #endregion

        return df_atkSpd;
    }
    #endregion

    /// <summary> 상대 좀비에게 타격을 입힌다 </summary>
    public void Hit(bool _isGnrAttack, long _dmg, bool _isCri)
    {
        if (targetPz != null && targetPz.igp.state != IG.ZombieState.LOSER)
        {
            if (_isGnrAttack) // 회피 & 명중 계산은 기본 공격일때에만 적용 
            {
                float or_evasion = targetPz.GetMyEvasion(); // 상대방의 회피율 
                float my_accuracy = GetMyAccuracy(); // 시전자의 명중률 

                if (Random.Range(igp.etcZbData.accuracy_correction, my_accuracy) < or_evasion) // 상대가 나의 공격을 회피함
                {
                    ObjectLife ojlf = ObjectPool.GetInstance().PopFromPool(ObjectPool.OP_Once_HitDamageText, targetPz.etcGameObject.hitPosTr.position);
                    if(ojlf != null)
                        ojlf.TextView("MISS", ResourceDatabase.GetInstance().hitColor_Evasion);

                    //igp.etcZbData.accuracy_correction += my_accuracy * 0.1f;
                    igp.etcZbData.accuracy_correction = my_accuracy;
                    return;
                }
                else igp.etcZbData.accuracy_correction = 0f;

                if (zbType == IG.MonsterType.MINE)
                {
                    LogPrint.EditorPrint("_dmg : " + _dmg +", accuracy_correction : " + igp.etcZbData.accuracy_correction + ", my_accuracy : " + my_accuracy + ", or_evasion : " + or_evasion);
                }
            }

            // 펫 전용 옵션 
            if (zbType != IG.MonsterType.MINE)
            {
                if (targetPz.igp.statValue.petSpOpTotalFigures.sop4_value > 0.0f)
                {
                    float sop4_value = targetPz.igp.statValue.petSpOpTotalFigures.sop4_value * 0.01f;
                    _dmg -= (long)(_dmg * sop4_value);
                }
            }

            if (_dmg < 0)
                _dmg = 1;
            if (_dmg > 0)
            {
                if (targetPz.TakeDamage(_isGnrAttack, _dmg, false)) // 상대 좀비에게 대미지를 입힌다. 
                {
                    if (_isGnrAttack)
                    {
                        // 장신구 전용옵션 - 8.최대 체력의 5% 회복 
                        if (igp.statValue.sop8_val > 0)
                        {
                            if(targetPz.GetHp() > 0)
                            {
                                if (GameDatabase.GetInstance().GetRandomPercent() < igp.statValue.sop8_val)
                                {
                                    RecoveryHealth((long)(igp.statValue.stat5_valHealth * (GameDatabase.GetInstance().chartDB.GetDicBalance("ac.sop.stat8.hp.recovery").val_float) * 0.01f));
                                }
                            }
                        }

                        // #10.상대 버블 차감(확률)
                        if(zbType == IG.MonsterType.MINE)
                        {
                            if (igp.statValue.sop10_val > 0)
                            {
                                if (targetPz.GetHp() > 0)
                                {
                                    if (GameDatabase.GetInstance().GetRandomPercent() < igp.statValue.sop10_val)
                                    {
                                        LogPrint.Print("<color=yellow>igp.statValue.sop10_val : " + igp.statValue.sop10_val + "</color>");
                                        targetPz.igp.playerSkillAction.BubbleCountDeduction(1, false); // 장신구 옵션으로 차감 
                                    }
                                }
                            }
                        }

                        if (_isCri)
                        {
                            HitDamageText(_dmg, ResourceDatabase.GetInstance().hitColor_Critical, targetPz.etcGameObject.hitPosTr.position);
                            ObjectPool.GetInstance().PopFromPool(ObjectPool.OP_Once_GeneralBeHit, targetPz.etcGameObject.hitPosTr.position);
                        }
                        else
                        {
                            HitDamageText(_dmg, ResourceDatabase.GetInstance().hitColor_General, targetPz.etcGameObject.hitPosTr.position);
                            ObjectPool.GetInstance().PopFromPool(ObjectPool.OP_Once_GeneralBeHit, targetPz.etcGameObject.hitPosTr.position);
                        }
                    }
                    else
                    {
                        HitDamageText(_dmg, ResourceDatabase.GetInstance().hitColor_Skill, targetPz.etcGameObject.hitPosTr.position);
                        ObjectPool.GetInstance().PopFromPool(ObjectPool.OP_Once_GeneralBeHit, targetPz.etcGameObject.hitPosTr.position);
                    }

                    SoundManager.GetInstance().PlaySound("arena_hit");
                }
            }
        }
    }

    /// <summary>
    /// 패배자 
    /// </summary>
    public async void Loser()
    {
        igp.state = IG.ZombieState.LOSER;
        zbType = IG.MonsterType.NONE;
        anim.SetTrigger("Death");
        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            await Task.Delay(100);

        //SoundManager.GetInstance().PlaySound("arena_die");
    }

    /// <summary>
    /// 승리자 
    /// </summary>
    public void Winner()
    {
        igp.state = IG.ZombieState.WINNER;
        igp.activateSkills.Clear();
        igp.activeSkillCancel.Clear();
        igp.etcZbData = new IG.EtcZbData();
        igp.playerSkillAction.isNowSkillAction = false;
    }

    /// <summary> 상대 좀비로부터 타격을 당했다 </summary>
    public bool TakeDamage(bool _is_gnrAttack, long get_damage, bool _is_reflect, bool reCheck = false)
    {
        if(igp.state != IG.ZombieState.FIGHT)
            return false;

        bool isHit = false;
        if(targetPz != null)
        {
            if(!reCheck)
            {
                if (_is_gnrAttack && !_is_reflect) // 기본 평타일 경우에만 버블이 증가한다. 반사 대미지 X 
                {
                    targetPz.igp.playerSkillAction.BubbleCountIncrease(0); // 상대 좀비의 스킬 발동 포인트를 증가시킨다. 
                }
            }

            if (targetPz.igp.state != IG.ZombieState.LOSER)
            {
                if (NowHealth > 0)
                {
                    isHit = true;
                    igp.etcZbData.last_taken_damage = get_damage;
                    NowHealth -= get_damage;
                    HealthLerp();
                }

                var mdty = GameMng.GetInstance().mode_type;
                if (NowHealth <= 0)
                {
                    Loser(); // 현재 좀비 패배 
                    targetPz.Winner(); // 상대 좀비 승리 

                    try
                    {
                        var objLifes = ObjectPool.GetInstance().onActiveLife;
                        if(objLifes.Count > 0)
                        {
                            foreach (var item in objLifes)
                            {
                                if (item != null)
                                    item.ResetActive();
                            }
                        }
                        
                        ObjectPool.GetInstance().onActiveLife.Clear();
                    }
                    catch (System.Exception e)
                    {
                        LogPrint.Print("onActiveLife Clear e : " + e);
                    }

                    if (mdty == IG.ModeType.CHAPTER_CONTINUE || mdty == IG.ModeType.CHAPTER_LOOP)
                    {
                        LogPrint.Print(" ------------------------------ 1 Winner ------------------------------ ");
                        GameMng.GetInstance().ChapterWinOrLose(zbType == IG.MonsterType.BOSS_USER);
                        LogPrint.Print(" ------------------------------ 2 Winner ------------------------------ ");
                    }
                    else
                    {
                        if (mdty == IG.ModeType.DUNGEON_TOP)
                        {
                            GameMng.GetInstance().DungeonTopWinOrLose();
                        }
                        else if (mdty == IG.ModeType.DUNGEON_MINE)
                        {
                            GameMng.GetInstance().DungeonMineWinOrLose();
                        }
                        else if (mdty == IG.ModeType.DUNGEON_RAID)
                        {
                            GameMng.GetInstance().DungeonRaidWinOrLose();
                        }
                        else if (mdty == IG.ModeType.PVP_BATTLE_ARENA)
                        {
                            GameMng.GetInstance().PvpWinOrLose();
                        }
                    }

                    LogPrint.PrintWarning(" 55555 Winner ------------------------------");
                    MainUI.GetInstance().tapGameBattleInfo.Init_BuffDebuff();
                    LogPrint.PrintWarning(" 66666 Winner ------------------------------");
                }
                else
                {
                    if (_is_gnrAttack)
                        anim.Play("Damage_GeneralAttack");
                    else anim.Play("Damage_SkillAttack");

                    if (!_is_reflect)
                    {
                        if (igp.playerSkillAction.GetCheckActivateCurrentSkill(IG.SkillNumber.NUMBER_5) >= 0) // 스킬 5 : 적으로 받는 대미지 반사 
                        {
                            int sk5_lv = igp.playerSkillAction.GetFindUseSkillLevel(IG.SkillNumber.NUMBER_5);
                            if (sk5_lv != -1)
                            {
                                long ref_dmg = GameDatabase.GetInstance().chartDB.GetSkillAbility5_ReflectDamage(sk5_lv, get_damage);
                                targetPz.TakeDamage(true, ref_dmg, true);
                                HitDamageText(ref_dmg, ResourceDatabase.GetInstance().hitColor_Reflect, targetPz.etcGameObject.hitPosTr.position);
                                ObjectPool.GetInstance().PopFromPool("opOnce_Sk5Ef_Hit", targetPz.etcGameObject.hitPosTr.position);
                                SoundManager.GetInstance().PlaySound("arena_hit");
                            }
                        }
                    }
                }
            }
        }

        return isHit;
    }

    /// <summary> 대미지 텍스트 </summary>
    public void HitDamageText (long _dmg, Color _color, Vector3 _pos)
    {
        if (GameDatabase.GetInstance().option.setting_damage_txt == 0)
        {
            ObjectLife oblf = ObjectPool.GetInstance().PopFromPool(ObjectPool.OP_Once_HitDamageText, _pos);
            if(oblf != null)
                oblf.TextView(_dmg.ToString(), _color);
        }
    }

    /// <summary> 체력 리셋 </summary>
    public void ResetHealth()
    {
        NowHealth = igp.statValue.stat5_valHealth;
        HealthLerp();
    }

    /// <summary> 체력 회복 </summary>
    public void RecoveryHealth (long val)
    {
        if(GetHp() > 0)
        {
            NowHealth += val;
            HealthLerp();
        }
    }
    /// <summary> 물약 -> 체력 회복 </summary>
    public void RecoveryPotionHp (float recoPcr)
    {
        RecoveryHealth((long)(igp.statValue.stat5_valHealth * recoPcr));

        //int maxHp = igp.statValue.stat5_valHealth;
        //int rcovHp1 = (int)(maxHp * recoPcr);
        //int rcovHp2 = rcovHp1 += GetHp();
        //NowHealth = rcovHp2;
        //HealthLerp();
    }

    public void HealthLerp(bool hpTxt = false)
    {
        if (hpTxt)
            hpValue.text = string.Format("{0:#,0} / {1:#,0}", nowHealth.ToString(), igp.statValue.stat5_valHealth.ToString());

        StopCoroutine("IEHealthLerp");
        StartCoroutine("IEHealthLerp");
    }

    /// <summary>
    /// 체력바 Lerp 
    /// </summary>
    private IEnumerator IEHealthLerp()
    {
        yield return null;

        float t_lerp = 0f;
        float v_end = (float)NowHealth / (float)igp.statValue.stat5_valHealth;
        bool isMinus = v_end < hpAmount.fillAmount;

        if (isMinus)
        {
            hpAmount.fillAmount = v_end;
            hpBgAmount.color = cor_hp_hit;
            while (v_end < hpBgAmount.fillAmount)
            {
                yield return null;
                t_lerp += GameMng.GetInstance().GameDeltaTime + (t_lerp * GameMng.GetInstance ().GameDeltaTime);
                hpBgAmount.fillAmount = Mathf.Lerp(hpBgAmount.fillAmount, v_end, t_lerp);
            }
        }
        else
        {
            hpBgAmount.fillAmount = v_end;
            hpBgAmount.color = cor_hp_rev;
            while (v_end > hpAmount.fillAmount)
            {
                yield return null;
                t_lerp += GameMng.GetInstance().GameDeltaTime + (t_lerp * GameMng.GetInstance().GameDeltaTime * 0.1f);
                hpAmount.fillAmount = Mathf.Lerp(hpAmount.fillAmount, v_end, t_lerp);
            }
        }

        yield return null;
        hpBgAmount.fillAmount = v_end;
        hpAmount.fillAmount = v_end;
    }
}
