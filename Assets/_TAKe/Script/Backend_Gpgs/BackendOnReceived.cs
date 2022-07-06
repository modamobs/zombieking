using UnityEngine;
using BackEnd;
using System;
using UnityEngine.SceneManagement;
using BackEnd.Tcp;
using System.Collections;

public class BackendOnReceived : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(CallbackLoop());
        Backend.Initialize(HandleBackendCallback);
    }

    void HandleBackendCallback()
    {
        // 새 쪽지 도착 
        Backend.Notification.OnReceivedMessage = () =>
        {
            try
            {
                isNewOnReceivedMessage = true;
                LogPrint.Print("<color=yellow> 1 Backend.Notification.OnReceivedMessage 새 쪽지 도착</color>");
            }
            catch (Exception e)
            {
                LogPrint.Print(e);
            }
        };

        // 새 우편 도착 
        Backend.Notification.OnReceivedUserPost = () =>
        {
            LogPrint.Print("<color=yellow>Backend.Notification.OnReceivedUserPost 새 유저 우편 도착</color>");
            NotificationIcon.GetInstance().CheckNoticeMail(true);
        };
    }


    bool isNewOnReceivedMessage = false;
    IEnumerator CallbackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            if (isNewOnReceivedMessage)
            {
                NotificationIcon.GetInstance().Loop();
                isNewOnReceivedMessage = false;
            }
        }
    }
}
