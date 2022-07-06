using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.U2D;
using BackEnd;

public class ScrollCellPvPBattleArenaRecord : MonoBehaviour
{
    int indexd = 0;
    [SerializeField]
    GameDatabase.PvPBattleRecord.ReceiveRecord db;

    [SerializeField] UI ui;
    [System.Serializable]
    struct UI
    {
        public Image imRank123;
        public Image imAtkerOrDfnder;
        public Text tx_Rank, tx_NickName, tx_Score, tx_MatchDate, tx_WinOrLoser, tx_Comment, tx_RewardCnt;
        public Color coWin, coLose;
        public Sprite spRank1, spRank2, spRank3, spRankEtc, spReadComp, spReadNoComp;
        public GameObject goIWin, goRewardBtn, goRewardComp;
    }

    [SerializeField] Text text;

    string text_none_rank = "-"; // 순위 없음 
    int atk_win_rwd, dfn_win_rwd;
    void Awake()
    {
        text_none_rank = LanguageGameData.GetInstance().GetString("text.none.rank"); // 순위 없음 
        atk_win_rwd = GameDatabase.GetInstance().chartDB.GetDicBalance("pvp.match.atk.win.reward").val_int;
        dfn_win_rwd = GameDatabase.GetInstance().chartDB.GetDicBalance("pvp.match.dfn.win.reward").val_int;
    }

    void ScrollCellIndex(int idx)
    {
        indexd = idx;
        text.text = (idx + 1).ToString();
        db = GameDatabase.GetInstance().pvpBattleRecord.GetPvpBTLRecordIndexDb(idx);
        string my_indate = BackendGpgsMng.backendUserInfo.m_indate;
        bool isISent = string.Equals(db.rcodJSONContents.sent_indate, my_indate);
        bool iWin = string.Equals(db.rcodJSONContents.winner_indate, my_indate);
        
        int rank = db.rcodJSONContents.pvp_rank;
        float score = db.rcodJSONContents.pvp_score;
        string comment = db.rcodJSONContents.msg;
        bool isAI = db.rcodJSONContents.isAI;

        ui.tx_NickName.text = isISent == true ? db.receiver_nickname : db.sender_nickname;
        ui.tx_Rank.text = isAI == true || rank == -1 ? text_none_rank : rank.ToString();
        ui.tx_Score.text = isAI == true ? "???" : score.ToString();
        ui.tx_Comment.text = comment;
        ui.tx_MatchDate.text = System.DateTime.Parse(db.inDate).ToString("yyyy/MM/dd hh:mm:ss"); // db.inDate.ToString("yyyy/MM/dd hh:mm:ss");
        ui.tx_WinOrLoser.text = iWin == true ? "WIN" : "LOSE";
        ui.tx_WinOrLoser.color = iWin == true ? ui.coWin : ui.coLose;
        ui.imAtkerOrDfnder.sprite = SpriteAtlasMng.GetInstance().GetPvpAttackerOrDefender(isISent);
        ui.goIWin.SetActive(iWin);
        if (ui.imRank123 != null)
            ui.imRank123.sprite = rank == 1 ? ui.spRank1 : rank == 2 ? ui.spRank2 : rank == 3 ? ui.spRank3 : ui.spRankEtc;

        // 승리 보상 
        if (iWin)
        {
            if(db.isRead == false) // 승리 보상 안받은 상태 
            {
                ui.goRewardBtn.SetActive(true);
                ui.tx_RewardCnt.text = string.Format("x{0}", isISent == true ? atk_win_rwd.ToString() : dfn_win_rwd.ToString());
            }
            else
            {
                ui.goRewardBtn.SetActive(false);
                ui.goRewardComp.SetActive(true);
            }
        }
        else
        {
            ui.goRewardBtn.SetActive(false);
            ui.goRewardComp.SetActive(false);
        }
    }

    public async void Click_RewardToRead()
    {
        if (db.isRead == false)
        {
            string my_indate = BackendGpgsMng.backendUserInfo.m_indate;
            string sent_indate = db.rcodJSONContents.sent_indate;
            bool isISent = string.Equals(sent_indate, my_indate);
            string record_indate = db.inDate;
            await GameDatabase.GetInstance().pvpBattleRecord.ReceivedMessage(isISent, record_indate);
            db.isRead = true;
            ScrollCellIndex(indexd);
        }
    }
}
