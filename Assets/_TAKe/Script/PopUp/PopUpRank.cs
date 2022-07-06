using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using static BackEnd.BackendAsyncClass;
using System.Threading.Tasks;
using LitJson;
using System;
using System.Collections;

public class PopUpRank : MonoBehaviour
{
    private float t_next = 600; // get/set 10분 딜레이 
    public InitOnStartRank initOnStartRank;

    [SerializeField] RankUIObject rankUIObject;
    [System.Serializable]
    private struct RankUIObject
    {
        public Color co_seltBtn, co_noneSeltBtn;
        public Text tx_MyRank, tx_MyNickName, tx_MyScore;
        public Image im_RankSpr;

        public Text tx_SubRank, tx_SubNickName, tx_SubScore;
        public GameObject go_BtnTop50, go_BtnAround;
    }
    
    private DateTime nextLoadDate;

    public Sprite spRank1, spRank2, spRank3, spRankEtc;

    void OnEnable()
    {
        //GetRank();
        //GetChapterRankUser(80);
    }

    /// <summary> 랭킹창 열렀을때 스테이지 Top 보이게 </summary>
    public async void OnPopRank() 
    {
        GameDatabase.GetInstance().rankDB.stageRankType = GameDatabase.RankDB.Enum_RTRankType.Rank_RT_ChptStgTop50;
       
        BackendReturnObject bro = null;
        if (BackendGpgsMng.isEditor == false)
        {
            var userinfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
            SendQueue.Enqueue(Backend.GameInfo.UpdateRTRankTable, BackendGpgsMng.tableName_UserInfo, "m_chpt_stg_nbr", (int)userinfo_db.m_chpt_stg_nbr, userinfo_db.indate, callback => { bro = callback; });
        }
        else bro = new BackendReturnObject();

        Task tsk = AGetLoadRank();

        while (Loading.Full(tsk.IsCompleted, bro != null) == false) await Task.Delay(100);

        StartRankView();
    }

    /// <summary>
    /// 랭킹 보여주기 ㄱㄱㄱ 
    /// </summary>
    private void StartRankView()
    {
        var type = GameDatabase.GetInstance().rankDB.stageRankType;
        bool isTop50 = type == GameDatabase.RankDB.Enum_RTRankType.Rank_RT_ChptStgTop50;
        rankUIObject.go_BtnTop50.SetActive(isTop50);
        rankUIObject.go_BtnAround.SetActive(!isTop50);

        MyRank();
        initOnStartRank.SetInit();
    }

    /// <summary>
    /// 자신의 랭킹을 보여줌 
    /// </summary>
    private void MyRank()
    {
        var my_rnk_indx = GameDatabase.GetInstance().rankDB.GetDicRTRank().FindIndex(r => r.isMyself == true);
        if(my_rnk_indx >= 0)
        {
            var rnk_data = GameDatabase.GetInstance().rankDB.GetDicRTRank()[my_rnk_indx];
            rankUIObject.tx_MyRank.text = rnk_data.rank.ToString();
            rankUIObject.tx_MyNickName.text = rnk_data.nickName.ToString();
            rankUIObject.tx_MyScore.text = GameDatabase.GetInstance().monsterDB.GetRankChapterStageString(rnk_data.score);
            rankUIObject.im_RankSpr.sprite = rnk_data.rank == 1 ? spRank1 : rnk_data.rank == 2 ? spRank2 : rnk_data.rank == 3 ? spRank3 : spRankEtc;
        }
        else
        {
            rankUIObject.tx_MyRank.text = "-";
            rankUIObject.tx_MyNickName.text = "-";
            rankUIObject.tx_MyScore.text = "-";
            rankUIObject.im_RankSpr.sprite = spRankEtc;
        }
    }

    //################################################
    #region ##### 버튼 클릭 #####
    /// <summary>
    /// 스테이지 랭킹 TOP50 리스트로 변경 
    /// </summary>
    public async void Click_TapChangeTop50()
    {
        GameDatabase.GetInstance().rankDB.stageRankType = GameDatabase.RankDB.Enum_RTRankType.Rank_RT_ChptStgTop50;
        await AGetLoadRank();
        StartRankView();
    }

    /// <summary>
    /// 스테이지 랭킹 내 주변 리스트로 변경 
    /// </summary>
    public async void Click_TapChangeMyAround()
    {
        GameDatabase.GetInstance().rankDB.stageRankType = GameDatabase.RankDB.Enum_RTRankType.Rank_RT_ChptStgTop50MyAround10;
        await AGetLoadRank();
        StartRankView();
    }
    #endregion

    /// <summary>
    /// 랭킹 유저 리스트 로드 
    /// </summary>
    private async Task AGetLoadRank()
    {
        DateTime ntDate = BackendGpgsMng.GetInstance().GetNowTime();
        DateTime tryNextDate;
        if (DateTime.TryParse(nextLoadDate.ToString(), out tryNextDate) == false)
            tryNextDate = ntDate;

        nextLoadDate = tryNextDate;

        var rank_type = GameDatabase.GetInstance().rankDB.stageRankType;
        if (ntDate >= nextLoadDate)
        {
            LogPrint.EditorPrint("AGetLoadRank --- 1 " + nextLoadDate.ToString());
            
            await GameDatabase.GetInstance().rankDB.AGetRTRank(rank_type);

            nextLoadDate = ntDate.AddMinutes(5);
            LogPrint.EditorPrint("AGetLoadRank --- 2 " + nextLoadDate.ToString());
        }
    }
}
