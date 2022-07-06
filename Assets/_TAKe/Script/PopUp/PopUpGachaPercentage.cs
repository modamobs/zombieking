using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpGachaPercentage : MonoBehaviour
{
    [SerializeField] Text[] txRatingPercentage;

    public void SetData(cdb_gacha_percentage cdb)
    {
        txRatingPercentage[1].text = string.Format("{0} : {1}%", GameDatabase.StringFormat.GetRatingColorText(1), cdb.rt1);
        txRatingPercentage[2].text = string.Format("{0} : {1}%", GameDatabase.StringFormat.GetRatingColorText(2), cdb.rt2);
        txRatingPercentage[3].text = string.Format("{0} : {1}%", GameDatabase.StringFormat.GetRatingColorText(3), cdb.rt3);
        txRatingPercentage[4].text = string.Format("{0} : {1}%", GameDatabase.StringFormat.GetRatingColorText(4), cdb.rt4);
        txRatingPercentage[5].text = string.Format("{0} : {1}%", GameDatabase.StringFormat.GetRatingColorText(5), cdb.rt5);
        txRatingPercentage[6].text = string.Format("{0} : {1}%", GameDatabase.StringFormat.GetRatingColorText(6), cdb.rt6);
        txRatingPercentage[7].text = string.Format("{0} : {1}%", GameDatabase.StringFormat.GetRatingColorText(7), cdb.rt7);
    }
}
