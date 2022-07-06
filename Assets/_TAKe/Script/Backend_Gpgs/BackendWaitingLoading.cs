using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using static BackEnd.BackendAsyncClass;
using System.Threading.Tasks;
using System;
using LitJson;

/// <summary>
/// 뒤끝과 통신중일때 로딩 사용 
/// </summary>
public class BackendWaitingLoading : MonoBehaviour
{
    List<Task> arrTask = new List<Task>();
    public async void SetBkendTaskWaiting (Task[] task)
    {
        if (task == null)
            return;

        arrTask.Clear();
        for (int i = 0; i < task.Length; i++)
            arrTask.Add(task[i]);
        
        int t = 0;
        while(arrTask.Count > 0)
        {
            await task[t];
            arrTask.RemoveAt(t);
        }

        gameObject.SetActive(false);
    }

    public async Task Await(List<Task> list)
    {
        // 서버와 통신이 끝날때까지 대기 
        while (list.Count > 0)
        {
            await list[0];
            list.RemoveAt(0);
        }

        gameObject.SetActive(false);
    }
}
