using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.U2D;
using BackEnd;

public class ScrollIndexCallbackRank : MonoBehaviour
{
    [SerializeField] bool isDungeonRank = false;
    [SerializeField] bool isClickLock = false;
    [SerializeField]
    private GameDatabase.RankDB.RankInfo rankData;
    [SerializeField] Image imRank123;
    [SerializeField] Text tx_Rank, tx_NickName, tx_Stage;

    [SerializeField] Color myColor;
    [SerializeField] Color orColor;

    [SerializeField] Sprite spRank1, spRank2, spRank3, spRankEtc;

    void ScrollCellIndex(int idx)
    {
        rankData = GameDatabase.GetInstance().rankDB.GetDicRTRank()[idx];
        tx_Rank.text = (idx + 1).ToString();// rankData.rank.ToString();
        tx_NickName.text = rankData.nickName.ToString();

        if (isDungeonRank)
        {
            tx_Stage.text = (GameDatabase.RankDB.max_time - rankData.score).ToString();
        }
        else
        {
            tx_Stage.text = GameDatabase.GetInstance().monsterDB.GetRankChapterStageString(rankData.score);
        }

        tx_Rank.color = rankData.isMyself ? myColor : orColor;
        tx_NickName.color = rankData.isMyself ? myColor : orColor;
        tx_Stage.color = rankData.isMyself ? myColor : orColor;

        if(imRank123 != null)
        {
            int _rank = rankData.rank;
            imRank123.sprite = _rank == 1 ? spRank1 : _rank == 2 ? spRank2 : _rank == 3 ? spRank3 : spRankEtc;
        }
    }

    public void Click_SelectCell()
    {
        LogPrint.Print("Click_SelectCell");
        if (isClickLock)
            return;

        if (!string.IsNullOrEmpty(rankData.gamer_indate))
        {
            LogPrint.PrintError("gamer_indate : " + rankData.gamer_indate);
            if (!rankData.isMyself && !string.IsNullOrEmpty(rankData.nickName))
            {
                PopUpMng.GetInstance().OpenUserInfo(rankData.nickName, true, false, rankData.gamer_indate);
            }
        }
        else LogPrint.PrintError("Not gamer_indate");
    }
}
