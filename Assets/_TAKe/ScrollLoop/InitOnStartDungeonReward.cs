using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SG;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]
public class InitOnStartDungeonReward : MonoBehaviour
{
    [SerializeField] LoopScrollRect ls;
    [SerializeField] bool is_preview = false;
    public int totalCount = -1;
    public void InitStart(bool sweep = false)
    {
        if(is_preview == false)
        {
            var md_ty = GameMng.GetInstance().gameUIObject.dungeonTop.pu_DungeonReward.rwdMty;
            if (md_ty == IG.ModeType.DUNGEON_TOP)
            {
                totalCount = GameMng.GetInstance().gameUIObject.dungeonTop.pu_DungeonReward.resultRewards.Count;
            }
            else if (md_ty == IG.ModeType.DUNGEON_MINE)
            {
                totalCount = GameMng.GetInstance().gameUIObject.dungeonMine.pu_DungeonReward.resultRewards.Count;
            }
            else if (md_ty == IG.ModeType.DUNGEON_RAID)
            {
                totalCount = GameMng.GetInstance().gameUIObject.dungeonRaid.pu_DungeonReward.resultRewards.Count;
            }
        }
        else
        {
            totalCount = MainUI.GetInstance().tapDungeon.resultRewardPreview.Count;
        }

        ls.totalCount = totalCount;
        ls.RefillCells();
    }
}
