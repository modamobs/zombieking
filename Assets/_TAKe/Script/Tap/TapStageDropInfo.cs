using BackEnd.RealTime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapStageDropInfo : MonoBehaviour
{
    [SerializeField] UI ui;
    [System.Serializable]
    class UI
    {
        public Text[] dropPct; // 드랍 확률
        public Text txCombat; // 전투력 
    }

    public void SetDropInfoView()
    {
        int chpt_dvs_nbr = GameMng.GetInstance().ChapterNbr();
        var mnst_db = GameDatabase.GetInstance().monsterDB.GetMonsterStatDb(chpt_dvs_nbr);
        var drop_db = GameDatabase.GetInstance().monsterDB.GetFieldDropRating(mnst_db.chpt_id);

        ui.txCombat.text = string.Format("전투력 : {0:#,0}", GameDatabase.GetInstance().monsterDB.GetChapterMonsterCombat(chpt_dvs_nbr));
        ui.dropPct[0].text = string.Format("{0:0.000}", drop_db.drop_rt7); // 전설 rt 7 
        ui.dropPct[1].text = string.Format("{0:0.000}", drop_db.drop_rt6); //      rt 6 
        ui.dropPct[2].text = string.Format("{0:0.000}", drop_db.drop_rt5); //      rt 5 
        ui.dropPct[3].text = string.Format("{0:0.000}", drop_db.drop_rt4); //      rt 4 
        ui.dropPct[4].text = string.Format("{0:0.000}", drop_db.drop_rt3); //      rt 3 
        ui.dropPct[5].text = string.Format("{0:0.000}", drop_db.drop_rt2); //      rt 2 
        ui.dropPct[6].text = string.Format("{0:0.000}", drop_db.drop_rt1); //      rt 1

    }
}
