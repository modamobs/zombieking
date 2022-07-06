using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollIndexCallbackAchievement : MonoBehaviour
{
    [SerializeField] bool isDaily;
    [SerializeField]
    private int idxxxx;
    [SerializeField]
    private int nbr;
    [SerializeField]
    private GameDatabase.AchievementsDB.JsonDB jDb;
    [SerializeField]
    private cdb_achievements cdb;
    [SerializeField] UI ui;
    [System.Serializable]
    class UI
    {
        public Image imRwdIcon;
        public Image imRatingBg;
        public Image imBg;
        public Image imBox;
        public Text txRwdCnt;
        public Text txName;
        public Slider sdProg;
        public Text txProgMaxCnt;
        public Color coBgDaily, coBgAchie, CoBoxDaily, coBoxAchie, coAllComp;

        public GameObject goAllComp, goProgBox;
        public Button btnMove; // 해당 진행 위치로 이동 
        public Button btComp;
    }

    void ScrollCellIndex(int idx)
    {
        idxxxx = idx;
        isDaily = PopUpMng.GetInstance().AchievementType() == PopUpAchievement.Type.DailyMission;
        cdb = default;
        bool isRwdComp = false;

        try
        {
            if (isDaily)
            {
                cdb = GameDatabase.GetInstance().chartDB.list_cdb_daily_mission[idx];
                jDb = GameDatabase.GetInstance().dailyMissionDB.Get((GameDatabase.DailyMissionDB.Nbr)cdb.nbr);
                isRwdComp = jDb.progLv > cdb.lv;
            }
            else
            {
                GameDatabase.AchievementsDB.Nbr achieNbrKey = GameDatabase.GetInstance().achievementsDB.GetSortIndex(idx);
                jDb = GameDatabase.GetInstance().achievementsDB.Get(achieNbrKey);
                int lastLv = GameDatabase.GetInstance().achievementsDB.GetLastAchieLv(achieNbrKey);
                isRwdComp = jDb.progLv > lastLv;
                cdb = GameDatabase.GetInstance().chartDB.dic_cdb_achievements[(int)achieNbrKey][isRwdComp == true ? lastLv : jDb.progLv];

                // 장비 도감 강화 업적 
                switch (cdb.nbr)
                {
                    case 11: jDb.progCnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetEncyProgressEnhantLevel(1, cdb.lv); break; // 
                    case 12: jDb.progCnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetEncyProgressEnhantLevel(2, cdb.lv); break; // 
                    case 13: jDb.progCnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetEncyProgressEnhantLevel(3, cdb.lv); break; // 
                    case 14: jDb.progCnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetEncyProgressEnhantLevel(4, cdb.lv); break; // 
                    case 15: jDb.progCnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetEncyProgressEnhantLevel(5, cdb.lv); break; // 
                    case 16: jDb.progCnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetEncyProgressEnhantLevel(6, cdb.lv); break; // 
                    case 17: jDb.progCnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetEncyProgressEnhantLevel(7, cdb.lv); break; // 
                }
            }
        }
        catch (System.Exception e)
        {
            nbr = -1;
            AllRewardComplete();
            return;
        }

        nbr = cdb.nbr;
        bool isCntComp = jDb.progCnt >= cdb.prog_cmp_cnt;  // 보상 완료 카운트까지 달성 
        if (string.Equals(cdb.item.gift_type, "goods"))
        {
            ui.imRwdIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteGoods(cdb.item.ty);
        }

        ui.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(cdb.item.rt);
        ui.txRwdCnt.text = string.Format("x{0}", cdb.item.count);
        ui.txName.text = cdb.name;
        ui.sdProg.value = (float)jDb.progCnt / (float)cdb.prog_cmp_cnt;
        
        if(int.Equals(cdb.prog_cnt_type, 1)) // 일반 카운트 표시 
        {
            ui.txProgMaxCnt.text = string.Format("{0} / {1}", jDb.progCnt, cdb.prog_cmp_cnt);
        }
        else // % 진행 표시 
        {
            float prog_f = (float)((float)jDb.progCnt / (float)cdb.prog_cmp_cnt);
            ui.txProgMaxCnt.text = string.Format("{0:0.0}% / 100%", (float)(prog_f * 100f));
        }

        if (isCntComp == false)
        {
            ui.imBg.color = isDaily == true ? ui.coBgDaily : ui.coBgAchie;
            ui.imBox.color = isDaily == true ? ui.CoBoxDaily : ui.coBoxAchie;
            ui.btComp.gameObject.SetActive(false);
            ui.goAllComp.SetActive(false);

            ui.btnMove.gameObject.SetActive(cdb.prog_tap_move);
            ui.goProgBox.gameObject.SetActive(!cdb.prog_tap_move);
        }
        else // 카운트 달성은 하였다.  
        {
            ui.btnMove.gameObject.SetActive(false);
            ui.goProgBox.gameObject.SetActive(false);
            if (isRwdComp == false) // 보상은 아직 받지 않은 상태 
            {
                ui.imBg.color = isDaily == true ? ui.coBgDaily : ui.coBgAchie;
                ui.imBox.color = isDaily == true ? ui.CoBoxDaily : ui.coBoxAchie;
                ui.btComp.gameObject.SetActive(true);
                ui.goAllComp.SetActive(false);
            }
            else //  보상 받기 완료 된상태 
            {
                AllRewardComplete();
            }
        }
    }

    public void MoveTap()
    {
        PopUpMng.GetInstance().Open_AchievementsMoveTap(isDaily, cdb.nbr);
    }

    void AllRewardComplete()
    {
        ui.imBg.color = ui.coAllComp;
        ui.imBox.color = ui.coAllComp;
        ui.btComp.gameObject.SetActive(false);
        ui.goAllComp.SetActive(true);
    }

    public async void Click_GetReward()
    {
        if (nbr == -1)
            return;

        var type = PopUpMng.GetInstance().AchievementType();
        if(type == PopUpAchievement.Type.DailyMission)
        {
            await GameDatabase.GetInstance().dailyMissionDB.AReward((GameDatabase.DailyMissionDB.Nbr)nbr, cdb.item);
            PopUpMng.GetInstance().Refresh_Achievemet();
            NotificationIcon.GetInstance().CheckNoticeDailyMission(true);
        }
        else if(type == PopUpAchievement.Type.Achievement)
        {
            await GameDatabase.GetInstance().achievementsDB.AReward((GameDatabase.AchievementsDB.Nbr)nbr, cdb.item);
            PopUpMng.GetInstance().Refresh_Achievemet();
            NotificationIcon.GetInstance().CheckNoticeAchievement(true);
        }
    }
}
