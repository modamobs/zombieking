using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapShopExchange : MonoBehaviour
{

    [SerializeField] List<UI> uiEquip = new List<UI>();
    [SerializeField] List<UI> uiAcce = new List<UI>();
    [SerializeField] List<UI> uiPotion = new List<UI>();
    [System.Serializable]
    public struct UI
    {
        public int rt;
        public Text txCnt;
        public Image imBtnBg;
    }

    void Awake()
    {

    }

    void OnEnable()
    {
        SetData();
    }

    public void SetData()
    {
        InitInfoEquipPiece();
        InitInfoAccePiece();
        InitInfoPotion();
    }

    //  28	        2	        중급 장비 조각
    //  28	        3	        고급 장비 조각
    //  28	        4	        희귀 장비 조각
    //  28	        5	        영웅 장비 조각
    //  28	        6	        고대 장비 조각
    //  28	        7	        전설 장비 조각

    void InitInfoEquipPiece()
    {
        var item_db = GameDatabase.GetInstance().tableDB.GetItemAll().FindAll(x => x.type == 28);
        foreach (var item in uiEquip)
        {
            if (item.rt >= 2)
            {
                int iIndx = item_db.FindIndex(x => x.rating == item.rt);
                if (iIndx >= 0)
                    item.txCnt.text = string.Format("x{0:#,0}", item_db[iIndx].count);
                else
                    item.txCnt.text = "x0";
            }
        }
    }

    /// <summary>
    /// 버튼 : 장비 조각 교환 팝업 
    /// </summary>
    public void Click_OpenEquipExchange(int btnRt) => PopUpMng.GetInstance().Open_ShopPieceExchange(GameDatabase.GetInstance().tableDB.GetItem(28, btnRt), InitInfoEquipPiece);


    //  29	        2	        중급 장신구 조각
    //  29	        3	        고급 장신구 조각
    //  29	        4	        희귀 장신구 조각
    //  29	        5	        영웅 장신구 조각
    //  29	        6	        고대 장신구 조각
    //  29	        7	        전설 장신구 조각
    void InitInfoAccePiece()
    {
        var item_db = GameDatabase.GetInstance().tableDB.GetItemAll().FindAll(x => x.type == 29);
        foreach (var item in uiAcce)
        {
            if (item.rt >= 2)
            {
                int iIndx = item_db.FindIndex(x => x.rating == item.rt);
                if (iIndx >= 0)
                    item.txCnt.text = string.Format("x{0:#,0}", item_db[iIndx].count);
                else
                    item.txCnt.text = "x0";
            }
        }
    }

    // 20   1   일반 물약
    // 20   2   중급 물약
    // 20   3   고급 물약 
    void InitInfoPotion()
    {
        var item_db = GameDatabase.GetInstance().tableDB.GetItemAll().FindAll(x => x.type == 20);
        foreach (var item in uiPotion)
        {
            if (item.rt >= 1)
            {
                int iIndx = item_db.FindIndex(x => x.rating == item.rt);
                if (iIndx >= 0)
                    item.txCnt.text = string.Format("x{0:#,0}", item_db[iIndx].count);
                else
                    item.txCnt.text = "x0";
            }
        }
    }

    /// <summary>
    /// 버튼 : 장신구 조각 교환 팝업 
    /// </summary>
    public void Click_OpenAcceExchange(int btnRt) => PopUpMng.GetInstance().Open_ShopPieceExchange(GameDatabase.GetInstance().tableDB.GetItem(29, btnRt), InitInfoAccePiece);
}
