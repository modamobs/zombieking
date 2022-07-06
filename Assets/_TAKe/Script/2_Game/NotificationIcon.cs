using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd.Tcp;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using CodeStage.AntiCheat.ObscuredTypes;

public class NotificationIcon : MonoSingleton<NotificationIcon>
{
    [System.Serializable]
    struct Notice
    {
        public bool isOn;
        public GameObject[] goNotif_1;
        public Text[] txNotif_1;
        public GameObject[] goNotif_2;
        public Text[] txNotif_2;
    }

    void Awake()
    {
        InitNotifPvpReceiveRecord();
        InitNotifInventory();
    }

    async void Start()
    {
        await Task.Delay(1000);
        
        CheckNoticeContentsTicket();
        CheckNoticeAchievement();
        CheckNoticeDailyMission();
        CheckNoticeEncylo();
        CheckNoticeMail(false);
        CheckNoticeQuestLevelUp();
        CheckNoticeSkillLevelUp();
        CheckNoticeShopLuckSohwan();
        CheckEquipProficiencyLevelUp();

        CheckNoticeNewEquipType(-1, -1, true);
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 새로운 PvP 기록 알림 (보상) 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    #region ##########
    [SerializeField] Notice noticePvpRecord;
    void InitNotifPvpReceiveRecord()
    {
        noticePvpRecord.isOn = false;
        foreach (var item in noticePvpRecord.goNotif_1)
            item.SetActive(false);
    }
    public void OnPvpReceiveRecord()
    {
        if (noticePvpRecord.isOn == false)
        {
            foreach (var item in noticePvpRecord.goNotif_1)
                item.SetActive(true);

            noticePvpRecord.isOn = true;
        }
    }

    public void OffPvpReceiveRecord()
    {
        if (noticePvpRecord.isOn == true)
        {
            foreach (var item in noticePvpRecord.goNotif_1)
                item.SetActive(false);

            noticePvpRecord.isOn = false;
        }
    }

    public void Loop()
    {
        LogPrint.Print("Loop");
        if (GameDatabase.GetInstance().pvpBattleRecord.structData.record.isNewReceivNotif == false)
        {
            GameDatabase.GetInstance().pvpBattleRecord.structData.record.isNewReceivNotif = true;
            StartCoroutine(LoopReceive());
        }
    }

    IEnumerator LoopReceive()
    {
        LogPrint.Print("LoopReceive");
        bool isStop = false;
        while (!isStop)
        {
            yield return new WaitForSeconds(1f);

            // 새 쪽지 받은것 (PvP 배틀 기록) 
            bool isNr = GameDatabase.GetInstance().pvpBattleRecord.structData.record.isNewReceivNotif;
            if (isNr)
            {
                LogPrint.Print((GameDatabase.GetInstance().pvpBattleRecord.structData.record.nextLoadTIme - BackendGpgsMng.GetInstance().GetNowTime()).TotalSeconds);
                if ((GameDatabase.GetInstance().pvpBattleRecord.structData.record.nextLoadTIme - BackendGpgsMng.GetInstance().GetNowTime()).Seconds < 0)
                {
                    NotificationIcon.GetInstance().OnPvpReceiveRecord();
                    isStop = true;
                }
            }
            else isStop = false;
        }
    }
    #endregion

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 다이아 Daily 보상 (구매자)
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    #region ##########
    [SerializeField] Notice daily30Day7Day;
    [SerializeField] Notice daily7Day;
    [SerializeField] Notice daily30Day;
    public void OnDaily30Day7Day(bool is7day, bool is30day)
    {
        if (daily30Day7Day.isOn == false && (is7day == true || is30day == true))
        {
            daily30Day7Day.isOn = true;
            foreach (var item in daily30Day7Day.goNotif_1)
                item.SetActive(true);
        }

        if (is7day)
        {
            daily7Day.isOn = true;
            foreach (var item in daily7Day.goNotif_1)
                item.SetActive(true);
        }

        if (is30day)
        {
            daily30Day.isOn = true;
            foreach (var item in daily30Day.goNotif_1)
                item.SetActive(true);
        }
    }

    public void OffDaily30Day7Day(bool is7day, bool is30day)
    {
        if (!is7day)
        {
            daily7Day.isOn = false;
            foreach (var item in daily7Day.goNotif_1)
                item.SetActive(false);
        }

        if (!is30day)
        {
            daily30Day.isOn = false;
            foreach (var item in daily30Day.goNotif_1)
                item.SetActive(false);
        }
    }
    #endregion

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 컨텐츠 버튼 티켓있을 경우 버튼에 알림 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    #region ##########
    [SerializeField] Notice noticeContents;
    [SerializeField] Notice noticeContentsTop;
    [SerializeField] Notice noticeContentsMine;
    [SerializeField] Notice noticeContentsRaid;
    [SerializeField] Notice noticeContentsPvp;
    public void CheckNoticeContentsTicket()
    {
        noticeContentsTop.isOn = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.DUNGEON_TOP) > 0;
        noticeContentsMine.isOn = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.DUNGEON_MINE) > 0;
        noticeContentsRaid.isOn = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.DUNGEON_RAID) > 0;
        noticeContentsPvp.isOn = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.PVP_BATTLE_ARENA) > 0;
        noticeContents.isOn = noticeContentsTop.isOn || noticeContentsMine.isOn || noticeContentsRaid.isOn || noticeContentsPvp.isOn;
        
        noticeContents.goNotif_1[0].SetActive(noticeContents.isOn);
        foreach (var go in noticeContentsTop.goNotif_1)
            go.SetActive(noticeContentsTop.isOn);
        foreach (var go in noticeContentsMine.goNotif_1)
            go.SetActive(noticeContentsMine.isOn);
        foreach (var go in noticeContentsRaid.goNotif_1)
            go.SetActive(noticeContentsRaid.isOn);
        foreach (var go in noticeContentsPvp.goNotif_1)
            go.SetActive(noticeContentsPvp.isOn);
    }
    #endregion

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 오른쪽 상단 알림 (출석부, 업적, 도감, 데일리, 우편함) 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    #region ##########
    [SerializeField] Notice noticeBtnAttend;
    /// <summary>
    /// 출석 알림 
    /// </summary>
    public void CheckNoticeAttend(bool istrue)
    {
        noticeBtnAttend.isOn = istrue;
        foreach (var item in noticeBtnAttend.goNotif_1)
            item.SetActive(istrue);
    }

    [SerializeField] Notice noticeBtnAchievement;
    /// <summary>
    /// 업적 알림 
    /// </summary>
    public void CheckNoticeAchievement(bool isReCheck = false)
    {
        if (isReCheck)
            noticeBtnAchievement.isOn = false;

        if (noticeBtnAchievement.isOn == false)
        {
            noticeBtnAchievement.isOn = GameDatabase.GetInstance().achievementsDB.GetNoticeCompleteCheck();
            foreach (var item in noticeBtnAchievement.goNotif_1)
                item.SetActive(noticeBtnAchievement.isOn);
        }
    }

    [SerializeField] Notice noticeBtnDailyMission;
    /// <summary>
    /// 미션 버튼 알림 
    /// </summary>
    public void CheckNoticeDailyMission(bool isReCheck = false)
    {
        if (isReCheck)
            noticeBtnDailyMission.isOn = false;

        if (noticeBtnDailyMission.isOn == false)
        {
            noticeBtnDailyMission.isOn = GameDatabase.GetInstance().dailyMissionDB.GetNoticeCompleteCheck();
            foreach (var item in noticeBtnDailyMission.goNotif_1)
                item.SetActive(noticeBtnDailyMission.isOn);
        }
    }

    [SerializeField] Notice noticeBtnEncylo;
    /// <summary>
    /// 도감 강화 가능 알림
    /// </summary>
    public void CheckNoticeEncylo(bool isReCheck = false)
    {
        if (noticeBtnEncylo.isOn == false)
        {
            noticeBtnEncylo.isOn = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetNoticeCompleteCheck();
            foreach (var item in noticeBtnEncylo.goNotif_1)
                item.SetActive(noticeBtnEncylo.isOn);
        }
        else
        {
            if (isReCheck)
            {
                noticeBtnEncylo.isOn = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetNoticeCompleteCheck();
                if (!noticeBtnEncylo.isOn)
                {
                    foreach (var item in noticeBtnEncylo.goNotif_1)
                        item.SetActive(false);
                }
            }
        }
    }

    [SerializeField] Notice noticeBtnMail;
    /// <summary>
    /// 우편함 알림 
    /// </summary>
    public async void CheckNoticeMail(bool isUserPost)
    {
        if (!isUserPost)
        {
            Task tsk1 = GameDatabase.GetInstance().mailDB.AGetAll();
            while (tsk1.IsCompleted == false) await Task.Delay(100);
        }

        noticeBtnMail.isOn = isUserPost || GameDatabase.GetInstance().mailDB.GetCount() > 0;
        noticeBtnMail.goNotif_1[0].SetActive(noticeBtnMail.isOn);
    }

    /// <summary>
    /// 우편함 알림 > 아침/점심/저녁 기본 접속 보상 체크 
    /// </summary>
    public async void CheckNoticeTodaysAccessGift(bool isReCheck = false)
    {
        //if (noticeBtnMail.isOn == false)
        //{
        //    noticeBtnMail.isOn = GameDatabase.GetInstance().achievementsDB.GetCompleteCheck();
        //    foreach (var item in noticeBtnMail.goNotif_1)
        //        item.SetActive(noticeBtnMail.isOn);
        //}
        //else
        //{
        //    if (isReCheck)
        //    {
        //        noticeBtnMail.isOn = false;
        //        if (!noticeBtnMail.isOn)
        //        {
        //            noticeBtnMail.isOn = false;
        //            foreach (var item in noticeBtnMail.goNotif_1)
        //                item.SetActive(false);
        //        }
        //    }
        //}
    }
    #endregion

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 인벤토리 알림 -> 판매/분해, 자동장착, 장비 레벨업
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    #region ##########
    [SerializeField] Notice noticeInventortyMax;
    [SerializeField] Notice noticeInventortyNewEquipRating;
    [SerializeField] Notice noticeInventortyInven1NewEquipRating; // 무기 탭 
    [SerializeField] Notice noticeInventortyInven2NewEquipRating; // 방어구 탭 
    [SerializeField] Notice noticeInventortyInven3NewEquipRating; // 장신구 탭 
    [SerializeField] Notice noticeInventoryAutoWear;
    [SerializeField] Notice noticeInventoryProfLevelUp;
    /// <summary>
    /// 인벤토리 공간 부족시 판매/분해 알림 
    /// </summary>
    void InitNotifInventory()
    {
        noticeInventortyMax.isOn = false;
        foreach (var item in noticeInventortyMax.goNotif_1)
            item.SetActive(false);
    }
    public void OnInventoryMax()
    {
        if (noticeInventortyMax.isOn == false)
        {
            foreach (var item in noticeInventortyMax.goNotif_1)
                item.SetActive(true);

            noticeInventortyMax.isOn = true;
        }
    }

    public void OffInventoryMax()
    {
        if (noticeInventortyMax.isOn == true)
        {
            foreach (var item in noticeInventortyMax.goNotif_1)
                item.SetActive(false);

            noticeInventortyMax.isOn = false;
        }
    }

    /// <summary>
    /// 자동 장착 알림 
    /// </summary>
    public void CheckNoticeAutoWear(GameDatabase.TableDB.Equipment newEquipDB, bool isReCheck, bool isChangeComplete = false)
    {
        CheckNoticeNewEquipType(newEquipDB.eq_ty, newEquipDB.eq_rt, isReCheck);

        if ((noticeInventoryAutoWear.isOn == false || isReCheck == true) && !isChangeComplete)
        {
            var wearEqDb = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(newEquipDB.eq_ty); // 현재 착용 중인 장비 DB 
            if (wearEqDb.eq_rt >= newEquipDB.eq_rt)
            {
                long nowEquipCombat = GameDatabase.GetInstance().tableDB.GetEquipCombatPower(wearEqDb, "total");
                long newEquipCombat = GameDatabase.GetInstance().tableDB.GetEquipCombatPower(newEquipDB, "total", wearEqDb.m_norm_lv);
                noticeInventoryAutoWear.isOn = newEquipCombat > nowEquipCombat;
                foreach (var item in noticeInventoryAutoWear.goNotif_1)
                    item.SetActive(noticeInventoryAutoWear.isOn);
            }
        }
        else if (isChangeComplete)
        {
            noticeInventoryAutoWear.isOn = false;
            foreach (var item in noticeInventoryAutoWear.goNotif_1)
                item.SetActive(noticeInventoryAutoWear.isOn);
        }
    }

    [HideInInspector] public long tapLastInventoryOpenSP = 0; // 인벤토리가 마지막으로 닫힌 sp
    [HideInInspector] public long tapLastInventory1OpenSP = 0; // 인벤토리가 무기/방패 마지막으로 닫힌 sp
    [HideInInspector] public long tapLastInventory2OpenSP = 0; // 인벤토리가 방어구 마지막으로 닫힌 sp
    [HideInInspector] public long tapLastInventory3OpenSP = 0; // 인벤토리가 장신구 마지막으로 닫힌 sp
    private int nwEqRt5, nwEqRt6, nwEqRt7; // 매인 인벤 버튼 탭에 표시
    // 인벤토리 내 무기/방패, 방어구, 장신구에 표시 
    private int nwEqTyWeaponShield, nwEqTyArmor, nwEqTyAcce;
    private int nwEqTyWeaponShieldRt5, nwEqTyWeaponShieldRt6, nwEqTyWeaponShieldRt7;
    private int nwEqTyArmorRt5, nwEqTyArmorRt6, nwEqTyArmorRt7;
    private int nwwEqTyAcceRt5, nwwEqTyAcceRt6, nwwEqTyAcceRt7;

    /// <summary>
    /// 장비 드롭 등급 매인 하단 버튼에 알림 영웅, 고대, 전설 
    /// </summary>
    public void CheckNoticeNewEquipRating(int eqRt, bool reset)
    {
        LogPrint.EditorPrint("eqRt :" + eqRt + ", reset : " + reset + ", nwEqRt5 : " + nwEqRt5 + ", nwEqRt6 : " + nwEqRt6 + ", nwEqRt7 : " + nwEqRt7);
        if (reset)
        {
            foreach (var g in noticeInventortyNewEquipRating.goNotif_1)
            {
                if (g != null && g.activeSelf)
                    g.SetActive(false);
            }
        }
        else
        {
            if (eqRt == -1 || (eqRt >= 5 && eqRt <= 7))
            {
                if (eqRt == -1 || eqRt == 5)
                {
                    if (nwEqRt5 > 0)
                    {
                        noticeInventortyNewEquipRating.txNotif_1[5].text = nwEqRt5.ToString();
                        if (!noticeInventortyNewEquipRating.goNotif_1[5].activeSelf)
                            noticeInventortyNewEquipRating.goNotif_1[5].SetActive(true);
                    }
                    else
                    {
                        nwEqRt5 = 0;
                        if (noticeInventortyNewEquipRating.goNotif_1[5].activeSelf)
                            noticeInventortyNewEquipRating.goNotif_1[5].SetActive(false);
                    }
                }

                if (eqRt == -1 || eqRt == 6)
                {
                    if (nwEqRt6 > 0)
                    {
                        noticeInventortyNewEquipRating.txNotif_1[6].text = nwEqRt6.ToString();
                        if (!noticeInventortyNewEquipRating.goNotif_1[6].activeSelf)
                            noticeInventortyNewEquipRating.goNotif_1[6].SetActive(true);
                    }
                    else
                    {
                        nwEqRt6 = 0;
                        if (noticeInventortyNewEquipRating.goNotif_1[6].activeSelf)
                            noticeInventortyNewEquipRating.goNotif_1[6].SetActive(false);
                    }
                }

                if (eqRt == -1 || eqRt == 7)
                {
                    if (nwEqRt7 > 0)
                    {
                        noticeInventortyNewEquipRating.txNotif_1[7].text = nwEqRt7.ToString();
                        if (!noticeInventortyNewEquipRating.goNotif_1[7].activeSelf)
                            noticeInventortyNewEquipRating.goNotif_1[7].SetActive(true);
                    }
                    else
                    {
                        nwEqRt7 = 0;
                        if (noticeInventortyNewEquipRating.goNotif_1[7].activeSelf)
                            noticeInventortyNewEquipRating.goNotif_1[7].SetActive(false);
                    }
                }
            }
        }

        tapLastInventoryOpenSP = GameDatabase.GetInstance().GetUniqueIDX();
    }

    /// <summary>
    /// 장비 드롭 등급 인벤토리의 각 부위 버튼에 알림 영웅, 고대, 전설 
    /// eqTy -> 0~1, eqTy -> 2~7, eqTy -> 8~10
    /// </summary>
    public void CheckNoticeNewEquipType(int eqTy, int eqRt, bool reset)
    {
        if (reset)
        {
            if (eqTy == -1 || (eqTy >= 0 && eqTy <= 1)) // 무기/방패 
            {
                nwEqTyWeaponShield = 0;
                nwEqTyWeaponShieldRt5 = 0;
                nwEqTyWeaponShieldRt6 = 0;
                nwEqTyWeaponShieldRt7 = 0;
                for (int i = 0; i < noticeInventortyInven1NewEquipRating.goNotif_1.Length; i++)
                {
                    GameObject g = noticeInventortyInven1NewEquipRating.goNotif_1[i];
                    if (g != null && g.activeSelf)
                    {
                        int rtCnt = int.Parse(Regex.Replace(noticeInventortyInven1NewEquipRating.txNotif_1[i].text, @"\D", ""));
                        LogPrint.EditorPrint("eqTy == -1 || (eqTy >= 0 && eqTy <= 1) : " + eqTy +", rtCnt:" + rtCnt);
                        if (rtCnt > 0)
                        {
                            if (i == 5)
                                nwEqRt5 -= rtCnt;
                            else if (i == 6)
                                nwEqRt6 -= rtCnt;
                            else if (i == 7)
                                nwEqRt7 -= rtCnt;
                        }

                        g.SetActive(false);
                    }
                }
            }

            if (eqTy == -1 || (eqTy >= 2 && eqTy <= 7)) // 방어구 
            {
                nwEqTyArmor = 0;
                nwEqTyArmorRt5 = 0;
                nwEqTyArmorRt6 = 0;
                nwEqTyArmorRt7 = 0;
                for (int i = 0; i < noticeInventortyInven2NewEquipRating.goNotif_1.Length; i++)
                {
                    GameObject g = noticeInventortyInven2NewEquipRating.goNotif_1[i];
                    if (g != null && g.activeSelf)
                    {
                        int rtCnt = int.Parse(Regex.Replace(noticeInventortyInven2NewEquipRating.txNotif_1[i].text, @"\D", ""));
                        LogPrint.EditorPrint("eqTy == -1 || (eqTy >= 2 && eqTy <= 7) : " + eqTy + ", rtCnt:" + rtCnt);
                        if (rtCnt > 0)
                        {
                            if (i == 5)
                                nwEqRt5 -= rtCnt;
                            else if (i == 6)
                                nwEqRt6 -= rtCnt;
                            else if (i == 7)
                                nwEqRt7 -= rtCnt;
                        }

                        g.SetActive(false);
                    }
                }
            }

            if (eqTy == -1 || (eqTy >= 8 && eqTy <= 10)) // 장신구 
            {
                nwEqTyAcce = 0;
                nwwEqTyAcceRt5 = 0;
                nwwEqTyAcceRt6 = 0;
                nwwEqTyAcceRt7 = 0;
                for (int i = 0; i < noticeInventortyInven3NewEquipRating.goNotif_1.Length; i++)
                {
                    GameObject g = noticeInventortyInven3NewEquipRating.goNotif_1[i];
                    if (g != null && g.activeSelf)
                    {
                        int rtCnt = int.Parse(Regex.Replace(noticeInventortyInven3NewEquipRating.txNotif_1[i].text, @"\D", ""));
                        LogPrint.EditorPrint("eqTy == -1 || (eqTy >= 8 && eqTy <= 10) : " + eqTy + ", rtCnt:" + rtCnt);
                        if (rtCnt > 0)
                        {
                            if (i == 5)
                                nwEqRt5 -= rtCnt;
                            else if (i == 6)
                                nwEqRt6 -= rtCnt;
                            else if (i == 7)
                                nwEqRt7 -= rtCnt;
                        }

                        g.SetActive(false);
                    }
                }
            }

            LogPrint.EditorPrint("무/방 : " + nwEqTyWeaponShield + ", 방어구 : " + nwEqTyArmor + ", 장신구 : " + nwEqTyAcce + ", nwEqRt5 : " + nwEqRt5 + ", nwEqRt6 : " + nwEqRt6 + ", nwEqRt7 : " + nwEqRt7);
            if (nwEqTyWeaponShield == 0 && nwEqTyArmor == 0 && nwEqTyAcce == 0)
            {
                nwEqRt5 = 0;
                nwEqRt6 = 0;
                nwEqRt7 = 0;
                nwEqTyWeaponShieldRt5 = 0;
                nwEqTyWeaponShieldRt6 = 0;
                nwEqTyWeaponShieldRt7 = 0;
                nwEqTyArmorRt5 = 0;
                nwEqTyArmorRt6 = 0;
                nwEqTyArmorRt7 = 0;
                nwwEqTyAcceRt5 = 0;
                nwwEqTyAcceRt6 = 0;
                nwwEqTyAcceRt7 = 0;
                CheckNoticeNewEquipRating(-1, true);
            }
            else
            {
                CheckNoticeNewEquipRating(eqRt, false);
            }
        }
        else
        {
            if (eqRt >= 5 && eqRt <= 7)
            {
                if (int.Equals(eqRt, 5))
                {
                    nwEqRt5++;
                    if (eqTy >= 0 && eqTy <= 1)
                        nwEqTyWeaponShieldRt5++;
                    else if (eqTy >= 2 && eqTy <= 7)
                        nwEqTyArmorRt5++;
                    else if (eqTy >= 8 && eqTy <= 10)
                        nwwEqTyAcceRt5++;
                }
                else if (int.Equals(eqRt, 6))
                {
                    nwEqRt6++;
                    if (eqTy >= 0 && eqTy <= 1)
                        nwEqTyWeaponShieldRt6++;
                    else if (eqTy >= 2 && eqTy <= 7)
                        nwEqTyArmorRt6++;
                    else if (eqTy >= 8 && eqTy <= 10)
                        nwwEqTyAcceRt6++;
                }
                else if (int.Equals(eqRt, 7))
                {

                    nwEqRt7++;
                    if (eqTy >= 0 && eqTy <= 1)
                        nwEqTyWeaponShieldRt7++;
                    else if (eqTy >= 2 && eqTy <= 7)
                        nwEqTyArmorRt7++;
                    else if (eqTy >= 8 && eqTy <= 10)
                        nwwEqTyAcceRt7++;
                }

                if (eqTy >= 0 && eqTy <= 1)
                {
                    if (!noticeInventortyInven1NewEquipRating.goNotif_1[eqRt].activeSelf)
                        noticeInventortyInven1NewEquipRating.goNotif_1[eqRt].SetActive(true);

                    nwEqTyWeaponShield++;
                    noticeInventortyInven1NewEquipRating.txNotif_1[5].text = nwEqTyWeaponShieldRt5.ToString();
                    noticeInventortyInven1NewEquipRating.txNotif_1[6].text = nwEqTyWeaponShieldRt6.ToString();
                    noticeInventortyInven1NewEquipRating.txNotif_1[7].text = nwEqTyWeaponShieldRt7.ToString();
                }
                else if (eqTy >= 2 && eqTy <= 7)
                {
                    if (!noticeInventortyInven2NewEquipRating.goNotif_1[eqRt].activeSelf)
                        noticeInventortyInven2NewEquipRating.goNotif_1[eqRt].SetActive(true);

                    nwEqTyArmor++;
                    noticeInventortyInven2NewEquipRating.txNotif_1[5].text = nwEqTyArmorRt5.ToString();
                    noticeInventortyInven2NewEquipRating.txNotif_1[6].text = nwEqTyArmorRt6.ToString();
                    noticeInventortyInven2NewEquipRating.txNotif_1[7].text = nwEqTyArmorRt7.ToString();
                }
                else if (eqTy >= 8 && eqTy <= 10)
                {
                    if (!noticeInventortyInven3NewEquipRating.goNotif_1[eqRt].activeSelf)
                        noticeInventortyInven3NewEquipRating.goNotif_1[eqRt].SetActive(true);

                    nwEqTyAcce++;
                    noticeInventortyInven3NewEquipRating.txNotif_1[5].text = nwwEqTyAcceRt5.ToString();
                    noticeInventortyInven3NewEquipRating.txNotif_1[6].text = nwwEqTyAcceRt6.ToString();
                    noticeInventortyInven3NewEquipRating.txNotif_1[7].text = nwwEqTyAcceRt7.ToString();
                }

                CheckNoticeNewEquipRating(eqRt, false);
            }
        }

        if (eqTy == -1 || (eqTy >= 0 && eqTy <= 1))
            tapLastInventory1OpenSP = GameDatabase.GetInstance().GetUniqueIDX();

        if (eqTy == -1 || (eqTy >= 2 && eqTy <= 7))
            tapLastInventory2OpenSP = GameDatabase.GetInstance().GetUniqueIDX();

        if (eqTy == -1 || (eqTy >= 8 && eqTy <= 10))
            tapLastInventory3OpenSP = GameDatabase.GetInstance().GetUniqueIDX();
    }

    /// <summary>
    /// 장비 숙련도 레벨업
    /// </summary>
    public void CheckEquipProficiencyLevelUp(bool isReCheck = false)
    {
        if (isReCheck == true)
            noticeInventoryProfLevelUp.isOn = false;

        if (noticeInventoryProfLevelUp.isOn == false)
        {
            ObscuredLong gold = GameDatabase.GetInstance().tableDB.GetTableDB_Goods().m_gold;
            GameDatabase.TableDB.Equipment min_eqDB = GameDatabase.GetInstance().tableDB.GetNowWearingEquipMinNormalLevel();
            if(min_eqDB.m_norm_lv >= 0)
                noticeInventoryProfLevelUp.isOn = gold >= (ObscuredLong)GameDatabase.GetInstance().questDB.GetQuestEquipProficiencyUpGold(min_eqDB.m_norm_lv);

            foreach (var item in noticeInventoryProfLevelUp.goNotif_1)
                item.SetActive(noticeInventoryProfLevelUp.isOn);
        }
    }
    #endregion

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 스킬 알림
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    #region ##########
    [SerializeField] Notice noticeSkill;
    /// <summary>
    /// 스킬 레벨업 가능 수량 알림 
    /// </summary>
    public void CheckNoticeSkillLevelUp(bool isReCheck = false)
    {
        if (isReCheck)
            noticeSkill.isOn = false;

        if (noticeSkill.isOn == false)
        {
            noticeSkill.isOn = GameDatabase.GetInstance().tableDB.GetNoticeCompleteCheckSkill();
            foreach (var item in noticeSkill.goNotif_1)
                item.SetActive(noticeSkill.isOn);
        }
    }
    #endregion

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 퀘스트 레벨업 가능 알림 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    #region ##########
    [SerializeField] Notice noticeQuest;
    /// <summary>
    /// 퀘스트 레벨업 가능 알림 
    /// </summary>
    public void CheckNoticeQuestLevelUp(bool isReCheck =false)
    {
        if (isReCheck == true)
            noticeQuest.isOn = false;

        if (noticeQuest.isOn == false)
        {
            noticeQuest.isOn = GameDatabase.GetInstance().questDB.GetNoticeCompleteCheck();
            noticeQuest.goNotif_1[0].SetActive(noticeQuest.isOn);
        }
    }
    #endregion

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 상점 무료 보상 알림 -> 블루 다이아 광고, 무료 뽑기 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    #region ##########
    [SerializeField] Notice noticeFreeEquipSohwan;
    [SerializeField] Notice noticeFreeAcceSohwan;
    [SerializeField] Notice noticeFreePetSohwan;
    /// <summary>
    /// 행운 상점 > 장비/장신구 무료 뽑기 알림 
    /// </summary>
    public void CheckNoticeShopLuckSohwan(bool isFreEquipReCheck = false, bool isFreAcceReCheck = false, bool isFrePetRecheck = false)
    {
        var userInfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();

        // 무료 장비 소환하기 
        DateTime tryEquipFreeSohwanDate;
        if (DateTime.TryParse(userInfo_db.m_free_equip_sohwan, out tryEquipFreeSohwanDate) == false)
            tryEquipFreeSohwanDate = nDate;

        // 무료 장신구 소환하기 
        DateTime tryAcceFreeSohwanDate;
        if (DateTime.TryParse(userInfo_db.m_free_acce_sohwan, out tryAcceFreeSohwanDate) == false)
            tryAcceFreeSohwanDate = nDate;

        // 무료 펫 소환하기 
        DateTime tryPetFreeSohwanDate;
        if (DateTime.TryParse(userInfo_db.m_free_pet_sohwan, out tryPetFreeSohwanDate) == false)
            tryPetFreeSohwanDate = nDate;

        if (noticeFreeEquipSohwan.isOn == false)
        {
            noticeFreeEquipSohwan.isOn = tryEquipFreeSohwanDate <= nDate;
            foreach (var item in noticeFreeEquipSohwan.goNotif_1)
                item.SetActive(noticeFreeEquipSohwan.isOn);
        }
        else
        {
            if (isFreEquipReCheck)
            {
                noticeFreeEquipSohwan.isOn = tryEquipFreeSohwanDate <= nDate;
                if (!noticeFreeEquipSohwan.isOn)
                {
                    foreach (var item in noticeFreeEquipSohwan.goNotif_1)
                        item.SetActive(false);
                }
            }
        }
        

        if (noticeFreeAcceSohwan.isOn == false)
        {
            noticeFreeAcceSohwan.isOn = tryAcceFreeSohwanDate <= nDate;
            foreach (var item in noticeFreeAcceSohwan.goNotif_1)
                item.SetActive(noticeFreeAcceSohwan.isOn);
        }
        else
        {
            if (isFreAcceReCheck)
            {
                noticeFreeAcceSohwan.isOn = tryAcceFreeSohwanDate <= nDate;
                if (!noticeFreeAcceSohwan.isOn)
                {
                    foreach (var item in noticeFreeAcceSohwan.goNotif_1)
                        item.SetActive(false);
                }
            }
        }

        if (noticeFreePetSohwan.isOn == false)
        {
            noticeFreePetSohwan.isOn = tryPetFreeSohwanDate <= nDate;
            foreach (var item in noticeFreePetSohwan.goNotif_1)
                item.SetActive(noticeFreePetSohwan.isOn);
        }
        else
        {
            if (isFrePetRecheck)
            {
                noticeFreePetSohwan.isOn = tryPetFreeSohwanDate <= nDate;
                if (!noticeFreePetSohwan.isOn)
                {
                    foreach (var item in noticeFreePetSohwan.goNotif_1)
                        item.SetActive(false);
                }
            }
        }
    }
    #endregion

}
