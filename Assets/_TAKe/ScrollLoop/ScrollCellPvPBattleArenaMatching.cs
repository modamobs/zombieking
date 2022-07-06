using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.U2D;
using BackEnd;
using System.Threading.Tasks;
using LitJson;
using System;

public class ScrollCellPvPBattleArenaMatching : MonoBehaviour
{
    int index = 0;
    [SerializeField]
    private GameDatabase.RankDB.RankInfo rankData;
    [SerializeField] Image imRank123;
    [SerializeField] Text tx_Rank, tx_NickName, tx_Score;

    [SerializeField] Color myColor;
    [SerializeField] Color orColor;

    [SerializeField] Sprite spRank1, spRank2, spRank3, spRankEtc;

    [SerializeField] GameObject goBtnMatch, goBtnReMatchDelay;
    [SerializeField] Text txReMatchDate;

    DateTime reDate;

    void ScrollCellIndex(int idx)
    {
        index = idx;
        rankData = GameDatabase.GetInstance().rankDB.GetPvpBTLMatching()[idx];
        tx_Rank.text = rankData.rank == -1 ? "???" : rankData.rank.ToString();
        tx_NickName.text = rankData.nickName.ToString();
        tx_Score.text = rankData.score == -1 ? "???" : rankData.score.ToString();

        if (imRank123 != null)
        {
            int _rank = rankData.rank;
            imRank123.sprite = _rank == 1 ? spRank1 : _rank == 2 ? spRank2 : _rank == 3 ? spRank3 : spRankEtc;
        }

        StopCoroutine("ReMatch");
        bool isReMatch = GameDatabase.GetInstance().pvpBattle.GetIsCheckMatch(rankData.gamer_indate);
        //isReMatch = true;
        if (isReMatch) // 결투 가능 
        {
            goBtnMatch.SetActive(true);
            goBtnReMatchDelay.SetActive(false);
        }
        // 다시 결투할 대기 시간 존재 
        else
        {
            goBtnMatch.SetActive(false);
            goBtnReMatchDelay.SetActive(true);

            reDate = GameDatabase.GetInstance().pvpBattle.GetReMatchDate(rankData.gamer_indate);
            StartCoroutine("ReMatch");
        }
    }

    IEnumerator ReMatch()
    {
        yield return null;
        bool stop = false;
        while (stop == false)
        {
            double sec = (reDate - BackendGpgsMng.GetInstance().GetNowTime()).TotalSeconds;
            if (sec < 0)
                stop = true;

            int minutes = (int)(Math.Floor((double)(sec / 60)));
            int second = (int)(Math.Floor((double)(sec % 3600 % 60)));
            txReMatchDate.text = string.Format("{0:00}:{1:00}", minutes, second);
            yield return new WaitForSeconds(1f);
        }

        ScrollCellIndex(index);
    }

    // 현재 셀의 유저와 결투 데이터 세팅 
    public async void Click_SelectCell()
    {
        if(GameMng.GetInstance().myPZ == null)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("캐릭터 정보 오류입니다.");
            return;
        }
           
        PopUpMng.GetInstance().Open_PvpBattleOpponent();
        // 게임 진행 모드가 챕터(반복) 모드이고, 화면 전환중이라면 잠시 대기 
        var mtyp = GameMng.GetInstance().mode_type;
        if ((mtyp == IG.ModeType.CHAPTER_CONTINUE || mtyp == IG.ModeType.CHAPTER_LOOP) == true && mtyp == IG.ModeType.CHANGE_WAIT)
        {
            while (GameMng.GetInstance().mode_type == IG.ModeType.CHANGE_WAIT || GameMng.GetInstance ().myPZ == null)
                await Task.Delay(100);

            await Task.Delay(100);
        }

        if (!rankData.isMyself && !string.IsNullOrEmpty(rankData.nickName) && !string.IsNullOrEmpty(rankData.gamer_indate))
        {
            var oldDb = GameDatabase.GetInstance().pvpBattle.GetDataBattleDbOr();
            LogPrint.Print("PvP 배틀에 사용될 결투 상대 데이터 oldDb.gamerInfo.gamer_indate : " + oldDb.gamerInfo.gamer_indate + ", rankData.gamer_indate : " + rankData.gamer_indate);
            //if (oldDb.gamerInfo.gamer_indate == rankData.gamer_indate)
            //{
            //    GameDatabase.GetInstance().pvpBattle.SetBattleData(oldDb.gamerInfo, null); 
            //}
            //else
            //{
                LogPrint.PrintError("gamer_indate : " + rankData.gamer_indate);
                GameDatabase.PvPBattle.Data.GamerInfo gmrInfo = new GameDatabase.PvPBattle.Data.GamerInfo()
                {
                    gamer_indate = rankData.gamer_indate,
                    gamer_nickName = rankData.nickName,
                    gamer_rank = rankData.rank,
                    gamer_score = rankData.score,
                    isAI = rankData.isAI,
                    ai_cpht_dvs_nbr = rankData.ai_cpht_dvs_nbr
                };
                MainUI.GetInstance().tapDungeon.LoadPvPBattleOpponentData(gmrInfo);
            //}
        }
        else
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("결투 상대 데이터 정보 오류입니다.");
            PopUpMng.GetInstance().Close_PvpBattleOpponent();
        }
    }

    public void Click_ReMatchDelay()
    {

    }
}
