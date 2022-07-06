using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpBackendNotice : MonoBehaviour
{
    [SerializeField] Text txNoticeTitle, txNoticeContent;
    [SerializeField] GameObject goNotSeeToday;

    public void SetData(JsonData rows, bool isClick)
    {
        LogPrint.Print("PopUpBackendNotice rows : " + rows);
        txNoticeTitle.text = RowPaser.StrPaser(rows[0], "title");
        txNoticeContent.text = RowPaser.StrPaser(rows[0], "content");

        goNotSeeToday.SetActive(!isClick);
    }

    public void Click_NotSeeingToday()
    {
        PlayerPrefs.SetString(PrefsKeys.prky_backend_notice_ymd, GameDatabase.GetInstance().attendanceDB.DailyYmd());
        gameObject.SetActive(false);
    }
}
