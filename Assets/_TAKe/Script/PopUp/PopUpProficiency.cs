using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PopUpProficiency : MonoBehaviour
{
    [HideInInspector] public bool[] isUpdateEquipDB = new bool[11];
    public InitOnStartEquipmentProficiencyLevel initOnStartEquipmentProficiencyLevel;

    void OnDisable()
    {
        LogPrint.EditorPrint("----------------- disable ----------------");

        GameDatabase.GetInstance().tableDB.ProficiencyUpdate(); // 장비 숙련도 레벨에 변화가 있을 경우 
        for (int i = 0; i < isUpdateEquipDB.Length; i++)
            isUpdateEquipDB[i] = false;
    }

    public void SetData()
    {
        for (int i = 0; i < isUpdateEquipDB.Length; i++)
            isUpdateEquipDB[i] = false;

        initOnStartEquipmentProficiencyLevel.SetInit();
    }

    public void Listener_ChangeMoveShopTap()
    {
        MainUI.GetInstance().Listener_MoveItemShop();
        gameObject.SetActive(false);
    }

    public void Click_Close() => gameObject.SetActive(false);

    //async Task Save()
    //{
    //    //// 착용 장비 전송
    //    //var wear_equip_transaction = GameDatabase.GetInstance().tableDB.ParamTransactionEquipNormalLevel(isUpdateEquipDB);
    //    //if (wear_equip_transaction.Count > 0)
    //    //{
    //    //    foreach (var db in wear_equip_transaction)
    //    //    {
    //    //        BackendReturnObject bro1 = Backend.GameInfo.TransactionWrite(db);
    //    //        while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }
    //    //    }

    //    //    var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
    //    //    BackendReturnObject bro2 = null;
    //    //    Param param = new Param();
    //    //    param.Add("m_gold", goods_db.m_gold);
    //    //    SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_Goods, goods_db.indate, param, callback => { bro2 = callback; });
    //    //    while (Loading.Bottom(bro2) == false) { await Task.Delay(100); }
    //    //}

    //    gameObject.SetActive(false);
    //}
}
