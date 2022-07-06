using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoserScreen : MonoBehaviour
{
    [SerializeField] Text txChapter;
    [SerializeField] Text txStageBoss;
    [SerializeField] Text txBackStage;
    [SerializeField] Animation ani;
    [SerializeField] GameObject go;

    /// <summary>
    /// 챕터 보스 : 몬스터 보스에게 승리 
    /// </summary>
    public async void ChapterMonsterBossWin (int chapter, int stage, int stage_nbr, string zbName)
    {
        string monName = string.IsNullOrEmpty(zbName) ? "몬스터" : string.Format("<color=yellow>[{0}]</color>", zbName);
        txChapter.text = string.Format("- Chapter {0} -", chapter);
        txStageBoss.text = string.Format("스테이지 {0} 보스 {1} 처치 성공 !!", stage, monName);
        txBackStage.text = "다음 스테이지로 이동합니다.";

        await Task.Delay(1000);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 챕터 보스 : 몬스터 보스에게 패배 
    /// </summary>
    public async void ChapterMonsterBossLose (int chapter, int stage, int stage_nbr, string zbName)
    {
        string monName = string.IsNullOrEmpty(zbName) ? "몬스터" : string.Format("<color=yellow>[{0}]</color>", zbName);
        txChapter.text = string.Format("- Chapter {0} -", chapter);
        txStageBoss.text = string.Format("스테이지 {0} 보스 {1} 처치 실패...", stage, stage_nbr, monName);

        if(GameMng.GetInstance ().mode_type == IG.ModeType.CHAPTER_CONTINUE)
            txBackStage.text = string.Format("{0} - 1 부터 다시 시작합니다.", stage);
        else if (GameMng.GetInstance().mode_type == IG.ModeType.CHAPTER_LOOP)
            txBackStage.text = "다음 스테이지 보스 몬스터로 이동합니다.";

        await Task.Delay(1000);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 챕터 일반 : 일반 몬스터에게 패배 
    /// </summary>
    public async void ChapterLose(int chapter, int stage, int stage_nbr)
    {
        txChapter.text = string.Format("- Chapter {0} -", chapter);
        txStageBoss.text = string.Format("{0} - {1} 일반 몬스터에게 패배하다니...", stage, stage_nbr);
        if (GameMng.GetInstance().mode_type == IG.ModeType.CHAPTER_CONTINUE)
            txBackStage.text = string.Format("{0} - 1 부터 다시 시작합니다.", stage);
        else if (GameMng.GetInstance().mode_type == IG.ModeType.CHAPTER_LOOP)
            txBackStage.text = string.Format("다시 이어서 시작합니다.", stage);

        await Task.Delay(1000);
        gameObject.SetActive(false);
    }

    public void PvpWin (string orNick)
    {
        txChapter.text = string.Format("<color=#0098C5>- WIN -</color>");
        txStageBoss.text = string.Format("<size=35>[{0}]</size> <color=#00EAFF>결투에서 승리했습니다!!~</color>", orNick);
        txBackStage.text = "";
    }
    public void PvpLose(string orNick)
    {
        txChapter.text = string.Format("<color=#A4A4A4>- LOSE -</color>");
        txStageBoss.text = string.Format("<size=35>[{0}]</size> <color=#C8C8C8>결투에서 패배했습니다...</color>", orNick);
        txBackStage.text = "";
    }

    public void DungeonLoser ()
    {

    }
}
