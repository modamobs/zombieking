using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimeRemaining : MonoBehaviour
{
    string rotinName;
    [SerializeField] Text txTR;
    [SerializeField] Text txExternal; // 외부 텍스트 
    [SerializeField] Text txComfirm;
    //[SerializeField] bool isTR_PvpMatching;
    [SerializeField] bool irTR_PvpRank;
    
    void OnEnable()
    {
        //if(irTR_PvpRank == true)
        //{
        //    StopCoroutine("TR_PvpRank");
        //    StartCoroutine("TR_PvpRank");
        //}

        //if(isTR_PvpMatching == true)
        //{
        //    StopCoroutine("TR_PvpMatching");
        //    StartCoroutine("TR_PvpMatching");
        //}
    }

    void OnDisable()
    {
        StopCoroutine(rotinName);
    }

    public void PvpMatchUserLise()
    {
        StopCoroutine("TR_PvpMatching");
        StartCoroutine("TR_PvpMatching");
    }

    // 매칭 다음 새로 고침 
    #region
    IEnumerator TR_PvpMatching()
    {
        yield return null;
        int pic = GameDatabase.GetInstance().chartDB.GetDicBalance("pvp.matching.refresh").val_int;
        rotinName = "TR_PvpMatching";
        double sec = (GameDatabase.GetInstance().pvpBattleRecord.structData.match.nextLoadTime - BackendGpgsMng.GetInstance().GetNowTime()).TotalSeconds;
        if(sec > 0)
        {
            txComfirm.text = "즉시 갱신";
        }

        while (gameObject.activeSelf && sec > 0)
        {
            sec = (GameDatabase.GetInstance().pvpBattleRecord.structData.match.nextLoadTime - BackendGpgsMng.GetInstance().GetNowTime()).TotalSeconds;
            if (sec > 0)
            {
                int minutes = (int)(Math.Floor((double)(sec / 60)));
                int second = (int)(Math.Floor((double)(sec % 3600 % 60)));
                txTR.text = string.Format("{0:00}:{1:00}", minutes, second);
                txExternal.text = string.Format("x{0}", pic.ToString());

            }

            yield return new WaitForSeconds(1.0f);
        }

        txExternal.text = "x0";
        txTR.text = "00:00";
        txComfirm.text = "무료 갱신";
    }
    #endregion

    // 랭킹 다음 새로 고침 
    #region 
    //IEnumerator TR_PvpRank()
    //{
    //    yield return null;
    //    rotinName = "TR_PvpRank";
    //    while (gameObject.activeSelf)
    //    {
    //        double sec = (GameDatabase.GetInstance().pvpBattleRecord.structData.rank.nextLoadTIme - BackendGpgsMng.GetInstance().GetNowTime()).TotalSeconds;
    //        if(sec > 0)
    //        {
    //            int minutes = (int)(Math.Floor((double)(sec / 60)));
    //            int second = (int)(Math.Floor((double)(sec % 3600 % 60)));
    //            txTR.text = string.Format("{0:00}:{1:00}", minutes, second);
    //        }
    //        else
    //        {
    //            txTR.text = "00:00";
    //        }

    //        yield return new WaitForSeconds(1.0f);
    //    }
    //}
    #endregion

   
   
}
