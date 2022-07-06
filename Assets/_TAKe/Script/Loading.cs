using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    static bool isBottomOnAction = false;
    static bool isFullOnAction = false;
    [SerializeField] GameObject goBottomLoading, goBottomBlackLoading;
    static GameObject _goBottomLoading, _goBottomBlackLoading;

    [SerializeField] GameObject goFullLoading;
    static GameObject _goFullLoading;

    [SerializeField] GameObject goFullPvpStartLoading;
    static GameObject _goFullPvpStartLoading;
    [SerializeField]
    Text pvpMyNickname, pvpOrNickname;
    static Text _pvpMyNickname, _pvpOrNickname;

    void Awake()
    {
        _goBottomLoading = goBottomLoading;
        _goBottomBlackLoading = goBottomBlackLoading;
        _goFullLoading = goFullLoading;

        _goFullPvpStartLoading = goFullPvpStartLoading;
        _pvpMyNickname = pvpMyNickname;
        _pvpOrNickname = pvpOrNickname;
    }

    #region ##### 전체 로딩 #####
    static async void AWaitFullClose()
    {
        await Task.Delay(100);
        while (isFullOnAction == true)
            await Task.Delay(100);

        await Task.Delay(100);

        if (!isFullOnAction)
            _goFullLoading.SetActive(false);
    }
    static async void AWaitFullMaxClose()
    {
        await Task.Delay(2000);
        isFullOnAction = false;
        _goFullLoading.SetActive(false);
    }

    public static bool Full(bool isComp1, bool isMaxCloseTimeIngr = false)
    {
        bool isComplete = isComp1 == true;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!isComplete)
            {
                if (!_goFullLoading.activeSelf)
                {
                    isFullOnAction = true;
                    _goFullLoading.SetActive(true);
                    AWaitFullClose();

                    if(!isMaxCloseTimeIngr)
                        AWaitFullMaxClose();
                }
                else isFullOnAction = true;
            }
            else isFullOnAction = false;
        }
        return isComplete;
    }
    public static bool Full(bool isComp1, bool isComp2, bool isMaxCloseTimeIngr = false)
    {
        bool isComplete = isComp1 == true && isComp2 == true;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!isComplete)
            {
                if (!_goFullLoading.activeSelf)
                {
                    isFullOnAction = true;
                    _goFullLoading.SetActive(true);
                    AWaitFullClose();

                    if (!isMaxCloseTimeIngr)
                        AWaitFullMaxClose();
                }
                else isFullOnAction = true;
            }
            else isFullOnAction = false;
        }
        return isComplete;
    }

    public static bool Full(bool isComp1, bool isComp2, bool isComp3, bool isMaxCloseTimeIngr = false)
    {
        bool isComplete = isComp1 == true && isComp2 == true && isComp3 == true;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!isComplete)
            {
                if (!_goFullLoading.activeSelf)
                {
                    isFullOnAction = true;
                    _goFullLoading.SetActive(true);
                    AWaitFullClose();

                    if (!isMaxCloseTimeIngr)
                        AWaitFullMaxClose();
                }
                else isFullOnAction = true;
            }
            else isFullOnAction = false;
        }
        return isComplete;
    }

    public static bool Full(bool isComp1, bool isComp2, bool isComp3, bool isComp4, bool isMaxCloseTimeIngr = false)
    {
        bool isComplete = isComp1 == true && isComp2 == true && isComp3 == true && isComp4 == true;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!isComplete)
            {
                if (!_goFullLoading.activeSelf)
                {
                    isFullOnAction = true;
                    _goFullLoading.SetActive(true);
                    AWaitFullClose();

                    if(!isMaxCloseTimeIngr)
                        AWaitFullMaxClose();
                }
                else isFullOnAction = true;
            }
            else isFullOnAction = false;
        }
        return isComplete;
    }

    public static bool Full(BackendReturnObject bro1, bool isMaxCloseTimeIngr = false)
    {
        bool isComplete = bro1 != null;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!isComplete)
            {
                if (!_goFullLoading.activeSelf)
                {
                    isFullOnAction = true;
                    _goFullLoading.SetActive(true);
                    AWaitFullClose();

                    if (!isMaxCloseTimeIngr)
                        AWaitFullMaxClose();
                }
                else isFullOnAction = true;
            }
            else isFullOnAction = false;
        }
        return isComplete;
    }
    public static bool Full(BackendReturnObject bro1, BackendReturnObject bro2, bool isMaxCloseTimeIngr = false)
    {
        bool isComplete = bro1 != null && bro2 != null;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!isComplete)
            {
                if (!_goFullLoading.activeSelf)
                {
                    isFullOnAction = true;
                    _goFullLoading.SetActive(true);
                    AWaitFullClose();

                    if (!isMaxCloseTimeIngr)
                        AWaitFullMaxClose();
                }
                else isFullOnAction = true;
            }
            else isFullOnAction = false;
        }
        return isComplete;
    }
    #endregion

    #region ##### 하단 TAP 로딩 #####
    static async void AWaitBottomClose(bool isBlack)
    {
        await Task.Delay(100);
        while (isBottomOnAction == true)
            await Task.Delay(100);

        await Task.Delay(100);

        if(!isBottomOnAction)
        {
            if (isBlack)
                _goBottomBlackLoading.SetActive(false);
            else
                _goBottomLoading.SetActive(false);
        }
    }
    static async void AWaitBottomMaxClose(bool isBlack)
    {
        await Task.Delay(2000);
        isBottomOnAction = false;
        if (isBlack)
            _goBottomBlackLoading.SetActive(false);
        else
            _goBottomLoading.SetActive(false);
    }

    public static bool Bottom (BackendReturnObject bro1, bool isBlack = false)
    {
        bool isComplete = bro1 != null;
        GameObject loading = isBlack ? _goBottomBlackLoading : _goBottomLoading;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!isComplete)
            {
                if (!loading.activeSelf)
                {
                    isBottomOnAction = true;
                    loading.SetActive(true);
                    AWaitBottomClose(isBlack);
                    AWaitBottomMaxClose(isBlack);
                }
                else isBottomOnAction = true;
            }
            else isBottomOnAction = false;
        }
        return isComplete;
    }
    
    public static bool Bottom(BackendReturnObject bro1, BackendReturnObject bro2, bool isBlack = false)
    {
        bool isComplete = bro1 != null && bro2 != null;
        GameObject loading = isBlack ? _goBottomBlackLoading : _goBottomLoading;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!isComplete)
            {
                if (!loading.activeSelf)
                {
                    isBottomOnAction = true;
                    loading.SetActive(true);
                    AWaitBottomClose(isBlack);
                    AWaitBottomMaxClose(isBlack);
                }
                else isBottomOnAction = true;
            }
            else isBottomOnAction = false;
        }
        return isComplete;
    }

    public static bool Bottom(bool isComp1, bool isBlack = false)
    {
        bool isComplete = isComp1 == true;
        GameObject loading = isBlack ? _goBottomBlackLoading : _goBottomLoading;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!isComplete)
            {
                if (!loading.activeSelf)
                {
                    isBottomOnAction = true;
                    loading.SetActive(true);
                    AWaitBottomClose(isBlack);
                    AWaitBottomMaxClose(isBlack);
                }
                else isBottomOnAction = true;
            }
            else isBottomOnAction = false;
        }
        return isComplete;
    }

    public static bool Bottom(bool isComp1, bool isComp2, bool isBlack = false)
    {
        bool isComplete = isComp1 == true && isComp2 == true;
        GameObject loading = isBlack ? _goBottomBlackLoading : _goBottomLoading;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!isComplete)
            {
                if (!loading.activeSelf)
                {
                    isBottomOnAction = true;
                    loading.SetActive(true);
                    AWaitBottomClose(isBlack);
                    AWaitBottomMaxClose(isBlack);
                }
                else isBottomOnAction = true;
            }
            else isBottomOnAction = false;
        }
        return isComplete;
    }

    public static bool Bottom(bool isComp1, bool isComp2, bool isComp3, bool isBlack = false)
    {
        bool isComplete = isComp1 == true && isComp2 == true && isComp3 == true;
        GameObject loading = isBlack ? _goBottomBlackLoading : _goBottomLoading;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!isComplete)
            {
                if (!loading.activeSelf)
                {
                    isBottomOnAction = true;
                    loading.SetActive(true);
                    AWaitBottomClose(isBlack);
                    AWaitBottomMaxClose(isBlack);
                }
                else isBottomOnAction = true;
            }
            else isBottomOnAction = false;
        }
        return isComplete;
    }

    public static bool Bottom(bool isComp1, bool isComp2, bool isComp3, bool isComp4, bool isBlack = false)
    {
        bool isComplete = isComp1 == true && isComp2 == true && isComp3 == true && isComp4 == true;
        GameObject loading = isBlack ? _goBottomBlackLoading: _goBottomLoading;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!isComplete)
            {
                if (!loading.activeSelf)
                {
                    isBottomOnAction = true;
                    loading.SetActive(true);
                    AWaitBottomClose(isBlack);
                    AWaitBottomMaxClose(isBlack);
                }
                else isBottomOnAction = true;
            }
            else isBottomOnAction = false;
        }
        return isComplete;
    }
    #endregion

    #region ##### PvP 배틀 로딩 #####
    public static void Open_PvpStartFull(string myNick, string orNick)
    {
        _pvpMyNickname.text = myNick;
        _pvpOrNickname.text = orNick;
        _goFullPvpStartLoading.SetActive(true);
    }

    public static void Close_PvpStartFull() => _goFullPvpStartLoading.SetActive(false);
    #endregion
}
