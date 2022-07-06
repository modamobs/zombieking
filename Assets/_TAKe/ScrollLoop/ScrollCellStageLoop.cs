using UnityEngine;
using UnityEngine.UI;

public class ScrollCellStageLoop : MonoBehaviour
{
    [SerializeField] UI ui;

    [System.Serializable]
    class UI
    {
        public Text txTitle;
        public Text txCombat;
        public Text[] dropPct; // 드랍 확률 
        public Text txClearRewardCnt; // 보상 수량 

        public CanvasGroup goBtnNoClear; // 버튼 : 미도달
        public CanvasGroup goBtnLoop; // 버튼 : 반복 
        public CanvasGroup goBtnLooping; // 버튼 : 반복중 

        public CanvasGroup goBtnContinueing;
        public CanvasGroup goBtnContinue; // 버튼 : 이어서 계속  

        [System.Serializable]
        public struct Icon
        {
            public int rt;
            public Text txDropPercent;
        }
    }

    [SerializeField] GameDatabase.AchievementsDB.JsonDB achdb;
    [SerializeField] private int cell_chpt_idx;
    [SerializeField] private long progCnt;
    [SerializeField] string xxxxx = "";

    void ScrollCellIndex(int idx)
    {
        cell_chpt_idx = idx + 1;
        var loop_first_db = GameDatabase.GetInstance().chartDB.GetDicChapterLoopArrayFirst(cell_chpt_idx);
        //var loop_last_db = GameDatabase.GetInstance().chartDB.GetDicChapterLoopArrayLast(cell_chpt_idx);
        //int loop_chpt_id = loop_first_db.chpt_id;
        int currt_chpt_id = GameDatabase.GetInstance().monsterDB.GetMyCurrentChapterID(); // 도전 중인 챕터 ID
        int m_chpt_id = GameDatabase.GetInstance().monsterDB.GetChapterDvsNbrFindChapterID(GameMng.GetInstance ().ChapterNbr()); // 진행중인 챕터 ID

        var drop_db = GameDatabase.GetInstance().monsterDB.GetFieldDropRating(loop_first_db.chpt_id);
        achdb = GameDatabase.GetInstance().achievementsDB.Get((GameDatabase.AchievementsDB.Nbr)1);

        long combat1 = GameDatabase.GetInstance().monsterDB.GetChapterMonsterCombat(loop_first_db.chpt_dvs_nbr);
        ui.txTitle.text = string.Format("챕터 {0}, 스테이지 1 ~ {1}", loop_first_db.chpt_id, GameDatabase.GetInstance().chartDB.GetChapterLoopStageCnt(loop_first_db.chpt_id));
        ui.txCombat.text = string.Format("보스 전투력 : {0:#,0} ~", combat1);
        ui.dropPct[0].text = string.Format("{0:0.000}", drop_db.drop_rt7); // 전설 rt 7 
        ui.dropPct[1].text = string.Format("{0:0.000}", drop_db.drop_rt6); //      rt 6 
        ui.dropPct[2].text = string.Format("{0:0.000}", drop_db.drop_rt5); //      rt 5 
        ui.dropPct[3].text = string.Format("{0:0.000}", drop_db.drop_rt4); //      rt 4 
        ui.dropPct[4].text = string.Format("{0:0.000}", drop_db.drop_rt3); //      rt 3 
        ui.dropPct[5].text = string.Format("{0:0.000}", drop_db.drop_rt2); //      rt 2 
        ui.dropPct[6].text = string.Format("{0:0.000}", drop_db.drop_rt1); //      rt 1 
        
        progCnt = achdb.progCnt;

        if (GameMng.GetInstance().loopChapter.isLoop == true) // 현재 챕터 반복 진행중 
        {
            if (cell_chpt_idx == m_chpt_id)
            {
                ui.goBtnLooping.alpha = 1f;
                BtnsDisable(ui.goBtnLooping);
            }
            else if (cell_chpt_idx < m_chpt_id)
            {
                ui.goBtnLoop.alpha = 1f;
                ui.goBtnLoop.blocksRaycasts = true;
                BtnsDisable(ui.goBtnLoop);
            }
            else
            {
                if(cell_chpt_idx == currt_chpt_id)
                {
                    ui.goBtnContinue.alpha = 1f;
                    ui.goBtnContinue.blocksRaycasts = true;
                    BtnsDisable(ui.goBtnContinue);
                }
                else if (cell_chpt_idx > currt_chpt_id)
                {
                    ui.goBtnNoClear.alpha = 1f;
                    BtnsDisable(ui.goBtnNoClear);
                }
                else
                {
                    ui.goBtnLoop.alpha = 1f;
                    ui.goBtnLoop.blocksRaycasts = true;
                    BtnsDisable(ui.goBtnLoop);
                }
            } 
        }
        else
        {
            if (cell_chpt_idx == m_chpt_id)
            {
                ui.goBtnContinueing.alpha = 1f;
                BtnsDisable(ui.goBtnContinueing);
            }  
            else if (cell_chpt_idx < m_chpt_id)
            {
                ui.goBtnLoop.alpha = 1f;
                ui.goBtnLoop.blocksRaycasts = true;
                BtnsDisable(ui.goBtnLoop);
            }
            else
            {
                ui.goBtnNoClear.alpha = 1f;
                BtnsDisable(ui.goBtnNoClear);
            }
        }
    }

    void BtnsDisable (CanvasGroup ingn)
    {
        if(!object.Equals(ingn, ui.goBtnNoClear))
        {
            ui.goBtnNoClear.alpha = 0;
            ui.goBtnNoClear.blocksRaycasts = false;
        }

        if (!object.Equals(ingn, ui.goBtnLoop))
        {
            ui.goBtnLoop.alpha = 0;
            ui.goBtnLoop.blocksRaycasts = false;
        }
        if (!object.Equals(ingn, ui.goBtnLooping))
        {
            ui.goBtnLooping.alpha = 0;
            ui.goBtnLooping.blocksRaycasts = false;
        }
        if (!object.Equals(ingn, ui.goBtnContinue))
        {
            ui.goBtnContinue.alpha = 0;
            ui.goBtnContinue.blocksRaycasts = false;
        }
        if(!object.Equals(ingn, ui.goBtnContinueing))
        {
            ui.goBtnContinueing.alpha = 0;
            ui.goBtnContinueing.blocksRaycasts = false;
        }
    }

    public void Clicl_Loop()
    {
        GameMng.GetInstance().ChapterLoopStart(cell_chpt_idx);
        PopUpMng.GetInstance().Close_StageLoop();
    }

    public void Click_Continue()
    {
        if (GameMng.GetInstance().loopChapter.isLoop)
        {
            GameMng.GetInstance().ChapterLoopStop();
            PopUpMng.GetInstance().Close_StageLoop();
        }
        else
        {
            PopUpMng.GetInstance().Open_MessageNotif("이미 진행중인 챕터 입니다.");
        }
    }
}
