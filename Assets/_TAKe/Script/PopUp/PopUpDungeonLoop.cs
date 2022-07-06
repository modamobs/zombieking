using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpDungeonLoop : MonoBehaviour
{
    string loop_dg_name = "";
    [SerializeField] Text tx_Title, tx_Btn;
    [SerializeField] private int ticketCnt = 0; // 현재 던전 티켓 수량 
    [SerializeField] private int inLoopCnt = 0; // 반복 진행 횟수 

    [SerializeField] Text tx_InCnt;

    public void SetData(string dg_name)
    {
        ticketCnt = 0;
        inLoopCnt = 0;

        if (string.Equals(dg_name, "top"))
        {
            tx_Title.text = "알 수 없는 탑 반복";
            ticketCnt = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.DUNGEON_TOP);
        }
        else if (string.Equals(dg_name, "mine"))
        {
            tx_Title.text = "광산 반복";
            ticketCnt = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.DUNGEON_MINE);
        }
        else if (string.Equals(dg_name, "raid"))
        {
            tx_Title.text = "레이드 반복";
            ticketCnt = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.DUNGEON_RAID);
        }
        else ticketCnt = -1;

        if (ticketCnt > 1)
        {
            inLoopCnt = 2;
            tx_Btn.text = string.Format("{0}회 반복 진행 시작", inLoopCnt);
            loop_dg_name = (string.Equals(dg_name, "top") || string.Equals(dg_name, "mine") || string.Equals(dg_name, "raid")) ? dg_name : "";
            tx_InCnt.text = string.Format("{0}/{1}", inLoopCnt, ticketCnt);
        }

        
    }

    /// <summary>
    /// 클릭 : 반복 진행 횟수 증가 
    /// </summary>
    public void Click_Plus()
    {
        if (inLoopCnt < ticketCnt)
        {
            inLoopCnt++;
            tx_InCnt.text = string.Format("{0}/{1}", inLoopCnt, ticketCnt);
            tx_Btn.text = string.Format("{0}회 반복 진행 시작", inLoopCnt);
        }
    }

    /// <summary>
    /// 클릭 : 반복 진행 횟수 차감  
    /// </summary>
    public void Click_Minus()
    {
        if (inLoopCnt >= 3 && ticketCnt >= 2)
        {
            inLoopCnt--;
            tx_InCnt.text = string.Format("{0}/{1}", inLoopCnt, ticketCnt);
            tx_Btn.text = string.Format("{0}회 반복 진행 시작", inLoopCnt);
        }
    }

    /// <summary>
    /// 클릭 : 자동 반복 진행 시작 세팅 및 한번더 입장 확인 알림 
    /// </summary>
    public void Click_InLoop()
    {
        if(string.Equals(loop_dg_name, "top"))
        {
            MainUI.GetInstance().tapDungeon.SettingDungeonLoop(IG.ModeType.DUNGEON_TOP, inLoopCnt);
            AskInLoopDungeon("top");
        }
        else if (string.Equals(loop_dg_name, "mine"))
        {
            MainUI.GetInstance().tapDungeon.SettingDungeonLoop(IG.ModeType.DUNGEON_MINE, inLoopCnt);
            AskInLoopDungeon("mine");
        }
        else if (string.Equals(loop_dg_name, "raid"))
        {
            MainUI.GetInstance().tapDungeon.SettingDungeonLoop(IG.ModeType.DUNGEON_RAID, inLoopCnt);
            AskInLoopDungeon("raid");
        }
        else
        {
            if(inLoopCnt < 2)
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("던전 자동 반복 진행 횟수가 잘못되었습니다.");
            else
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("던전 자동 반복 진행 데이터가 잘못되었습니다.");

            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 입장 알림 
    /// </summary>
    void AskInLoopDungeon(string dg_name)
    {
        bool isLoop = MainUI.GetInstance().tapDungeon.dgLoop.isLoop;
        int dgNbr = MainUI.GetInstance().tapDungeon.dgLoop.loopDgNbr;
        int dgLoonCnt = MainUI.GetInstance().tapDungeon.dgLoop.loopCnt;
        if (string.Equals(loop_dg_name, "top") && isLoop && dgLoonCnt > 1)
        {
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(
                string.Format(string.Format("알 수 없는 탑 : {0}층에 입장합니다.\n<color=#698FFF>{1}회 자동 반복 진행됩니다.</color>", dgNbr + 1, dgLoonCnt)),
                Listener_LoopInTop, Listener_LoopCancel);
        }
        else if (string.Equals(loop_dg_name, "mine") && isLoop && dgLoonCnt > 1)
        {
            string rtTxt = LanguageGameData.GetInstance().GetString(string.Format("text.dungeon.difficulty.nbr{0}", dgNbr));
            string txt = string.Format(string.Format("광산 : 난이도 [{0}] 입장합니다.\n<color=#698FFF>{1}회 자동 반복 진행됩니다.</color>", rtTxt, dgLoonCnt));
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt,
                Listener_LoopInMine, Listener_LoopCancel);
        }
        else if (string.Equals(loop_dg_name, "raid") && isLoop && dgLoonCnt > 1)
        {
            string rtTxt = LanguageGameData.GetInstance().GetString(string.Format("text.dungeon.difficulty.nbr{0}", dgNbr));
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(
                string.Format(string.Format("레이드 : 난이도 [{0}] 입장합니다.\n<color=#698FFF>{1}회 자동 반복 진행됩니다.</color>", rtTxt, dgLoonCnt)),
                Listener_LoopInRaid, Listener_LoopCancel);
        }
        else
        {
            MainUI.GetInstance().tapDungeon.dgLoop = new TapDungeon.DgLoop();
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("던전 자동 반복 진행 데이터가 잘못되었습니다.");
        }

        gameObject.SetActive(false);
    }

    void Listener_LoopCancel()
    {
        MainUI.GetInstance().tapDungeon.dgLoop = new TapDungeon.DgLoop();
    }

    void Listener_LoopInTop()
    {
        MainUI.GetInstance().tapDungeon.Listener_InDungeonTop();
    }

    void Listener_LoopInMine()
    {
       
        MainUI.GetInstance().tapDungeon.Listener_InDungeonMine();
    }

    void Listener_LoopInRaid()
    {
        
        MainUI.GetInstance().tapDungeon.Listener_InDungeonRaid();
    }
}
