using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PopUpFirstReward : MonoBehaviour
{
    [SerializeField] GameObject goRewardBtn;

    public void SetData()
    {
        goRewardBtn.SetActive(true);
    }

    public async void Click_Close()
    {
        string rwd_name1 = GameDatabase.GetInstance().chartDB.GetDicBalance("first_reward_name").val_string;
        int rwd_cnt1 = GameDatabase.GetInstance().chartDB.GetDicBalance("first_reward_count").val_int;

        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        if (string.Equals(rwd_name1, "dia"))
        {
            goods_db.m_dia += rwd_cnt1;
        }

        goRewardBtn.SetActive(false);
        Task<bool> task = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db, false, "dia");
        while (Loading.Full(task.IsCompleted) == false) await Task.Delay(100);

        gameObject.SetActive(false);
    }
}
