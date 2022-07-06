using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpPvpBattleOpponentInfo : MonoBehaviour
{
    
    [SerializeField] UI uiMy, uiOr;
    [System.Serializable]
    public class UI
    {
        public Text txNickName;
        public Text txRank;
        public Text txCombat;
        public Image imEqRtBgAverage;
        public Text txWinningStreak; // 연승 
        public PlayerZombiePreview pzp;
        public Animator ani;
    }

    [SerializeField] UIReward[] uiReward;
    [System.Serializable]
    public class UIReward
    {
        public Image imIcon;
        public Image txRatingMinMax;
        public Text txCount;
        public Text txName;
    }

    public GameObject goRootInfo;
    public Text txVs;

    [SerializeField] Canvas canvas;
    void Awake()
    {
        if(canvas == null)
            canvas = GetComponent<Canvas>();
    }

    public void AWait()
    {
        goRootInfo.SetActive(false);
    }

    public void SetData()
    {
        goRootInfo.SetActive(true);

        var dbOr = GameDatabase.GetInstance().pvpBattle.GetDataBattleDbOr();
        var dbMy = GameDatabase.GetInstance().rankDB.GetMyRankPvPBTLArena();
        long combatMy = GameDatabase.GetInstance().characterDB.GetStat().combat_power;

        txVs.text = string.Format("[{0}]님에게 결투를 신청하시겠습니까?", dbOr.gamerInfo.gamer_nickName);

        // ##### 상대 정보 
        uiOr.txNickName.text = dbOr.gamerInfo.gamer_nickName;
        uiOr.txRank.text = dbOr.gamerInfo.isAI == true ? "???" : string.Format("{0}등", dbOr.gamerInfo.gamer_rank);
        uiOr.txCombat.text = string.Format("전투력 {0:#,0}", dbOr.statValue.combat_power);
        uiOr.pzp.SettingZombieEquipParts(dbOr.parts);
        uiOr.ani.Play("idle");

        // 장비 평균 등급으로 상대의 전투력 컬러 > 차이 많이 날수록 붉은색 
        int pvp_difference_max = GameDatabase.GetInstance().chartDB.GetDicBalance("pvp.win.difference.score.max").val_int; // max 10 
        long difference = dbOr.statValue.combat_power < combatMy ? 0 : (long)(dbOr.statValue.combat_power - combatMy) / (long)(combatMy * 0.1f);
        if (difference > pvp_difference_max)
            difference = pvp_difference_max;

        float nickCor = 1 - (difference * 0.1f);
        uiOr.txCombat.color = new Color(1, nickCor, nickCor, 1);

        List<int> eqRtsOr = new List<int>();
        foreach (var item in dbOr.eqList)
            eqRtsOr.Add(item.eq_rt);
        int averEqRtOr = GameDatabase.GetInstance().tableDB.GetEquipRatingAverage(eqRtsOr);
        Color averEqCorOr = ResourceDatabase.GetInstance().GetItemColor(averEqRtOr);
        uiOr.imEqRtBgAverage.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(averEqRtOr);

        // ##### 내 정보 
        uiMy.txNickName.text = BackendGpgsMng.backendUserInfo.m_nickname;
        uiMy.txRank.text = string.Format("{0}등", dbMy.rank);
        uiMy.txCombat.text = string.Format("전투력 {0:#,0}", combatMy);
        uiMy.txWinningStreak.text = string.Format("연승 : {0}", GameDatabase.GetInstance().tableDB.GetUserInfo().m_pvp_win_streak);
        uiMy.pzp.SettingZombieEquipParts(GameMng.GetInstance ().myPZ.igp.parts);
        uiMy.ani.Play("idle");

        int averEqRtMy = GameDatabase.GetInstance().tableDB.GetEquipRatingAverage();
        Color averEqCorMy = ResourceDatabase.GetInstance().GetItemColor(averEqRtMy);
        uiMy.imEqRtBgAverage.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(averEqRtMy);
    }

    public void Click_Start()
    {
        int tikCnt = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.PVP_BATTLE_ARENA);
        if (tikCnt > 0)
            GameMng.GetInstance().ChangeMode_PvP();
        else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("PvP 배틀 아레나 입장권이 부족합니다.\n입장권 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
    }
}
