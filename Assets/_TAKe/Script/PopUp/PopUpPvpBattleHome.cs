using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpPvpBattleHome : MonoBehaviour
{
    public GameObject goTapContents;
    public Canvas canHome;
    public CanvasGroup canHomeQuest;
    private float tempCanAlphaChat, tempCanAlphaQuest;

    [SerializeField] UI uiMy, uiOr;
    [System.Serializable]
    class UI
    {
        public Text txNickName;
        public Text txCombat;
        public Text txRank;
    }

    void OnEnable()
    {
        goTapContents.SetActive(false);
        canHome.enabled = true;
        tempCanAlphaQuest = canHomeQuest.alpha;
        canHomeQuest.alpha = 0;
        MainUI.GetInstance().QuestChatSizeUpDwHomeSkillEft(true);
    }

    void OnDisable()
    {
        canHome.enabled = false;
        goTapContents.SetActive(true);
        canHomeQuest.alpha = tempCanAlphaQuest;
        MainUI.GetInstance().QuestChatSizeUpDwHomeSkillEft(false);
    }

    public void SetData()
    {
        var dbMy = GameDatabase.GetInstance().rankDB.GetMyRankPvPBTLArena();
        long combatMy = GameDatabase.GetInstance().characterDB.GetStat().combat_power;
        uiMy.txNickName.text = dbMy.nickName;
        uiMy.txCombat.text = string.Format("전투력 {0:#,0}", combatMy.ToString());
        uiMy.txRank.text = string.Format("{0}위", dbMy.rank);

        var dbOr = GameDatabase.GetInstance().pvpBattle.GetDataBattleDbOr();
        uiOr.txNickName.text = dbOr.gamerInfo.gamer_nickName;
        uiOr.txCombat.text = string.Format("전투력 {0:#,0}", dbOr.statValue.combat_power.ToString());
        uiOr.txRank.text = string.Format("{0}위", dbOr.gamerInfo.gamer_rank);
    }
}
