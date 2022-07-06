using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.U2D;
using BackEnd;

public class ScrollCellPvPBattleArenaRank : MonoBehaviour
{
    [SerializeField]
    private GameDatabase.RankDB.RankInfo rankData;
    [SerializeField] Image imRank123;
    [SerializeField] Text tx_Rank, tx_NickName, tx_Score;

    [SerializeField] Color myColor;
    [SerializeField] Color orColor;

    [SerializeField] Sprite spRank1, spRank2, spRank3, spRankEtc;

    void ScrollCellIndex(int idx)
    {
        rankData = GameDatabase.GetInstance().rankDB.GetListRTRankPvpBTLArenaRank()[idx];
        tx_Rank.text = rankData.rank.ToString();
        tx_NickName.text = rankData.nickName.ToString();
        tx_Score.text = rankData.score.ToString();

        tx_Rank.color = rankData.isMyself ? myColor : orColor;
        tx_NickName.color = rankData.isMyself ? myColor : orColor;
        tx_Score.color = rankData.isMyself ? myColor : orColor;

        if (imRank123 != null)
        {
            int _rank = rankData.rank;
            imRank123.sprite = _rank == 1 ? spRank1 : _rank == 2 ? spRank2 : _rank == 3 ? spRank3 : spRankEtc;
        }
    }

    // 유저 정보 보기 
    public void Click_SelectCell()
    {
        if (!string.IsNullOrEmpty(rankData.gamer_indate))
        {
            LogPrint.PrintError("gamer_indate : " + rankData.gamer_indate);
            if (!rankData.isMyself && !string.IsNullOrEmpty(rankData.nickName))
            {
                PopUpMng.GetInstance().OpenUserInfo(rankData.nickName, true, false, rankData.gamer_indate);
            }
            else
            {
                PopUpMng.GetInstance().OpenUserInfo(rankData.nickName, true, false, rankData.gamer_indate);
                LogPrint.Print("자기 자신 정보");
            }
        }
        else LogPrint.PrintError("Not gamer_indate");
    }
}
