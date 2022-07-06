using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObscuredRandomCryptoKey : MonoBehaviour
{
    void Start()
    {
        InvokeRepeating("RandomizeCryptoKey", 1.0f, 1.0f);
    }

    private void RandomizeCryptoKey()
    {
        GameDatabase.GetInstance().tableDB.Goods_RandomizeCryptoKey();
        GameDatabase.GetInstance ().tableDB.UserInfo_RandomizeCryptoKey();
    }

    /// <summary>
    /// 잡았다!
    /// </summary>
    public async void GotchBitch()
    {
        // "GotchBitch!!!";

        var userinfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
        userinfo_db.cheat_gotch_bitch = GameDatabase.GetInstance().GetUniqueIDX();
        Task<bool> tsk = GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userinfo_db);
        while (Loading.Full(tsk.IsCompleted) == false) await Task.Delay(250);
        PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("부정 프로그램 감지되었습니다. 게임이 종료됩니다.", Application.Quit);
    }
}
