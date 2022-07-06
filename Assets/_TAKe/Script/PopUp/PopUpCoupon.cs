using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PopUpCoupon : MonoBehaviour
{
    [SerializeField] InputField inputField;
    [SerializeField] Text txtCoupon;

    public void SetData()
    {
        ResetInputField();
        //LogPrint.Print("----------------A CouponList ");
        //BackendReturnObject bro1 = null;
        //SendQueue.Enqueue(Backend.Coupon.CouponList, callback => { bro1 = callback; });
        //while (Loading.Full(bro1) == false) { await Task.Delay(100); }
        //LogPrint.Print("----------------A CouponList " + bro1);
    }

    public async void Click_Coupon(Text txt)
    {
        LogPrint.Print("----------------A Coupon ");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.Coupon.UseCoupon, txt.text.ToString(), callback => { bro1 = callback; });
        while (Loading.Full(bro1) == false) { await Task.Delay(100); }
        LogPrint.Print("----------------A Coupon " + bro1);
        
        if (bro1.IsSuccess())
        {
            var jd = bro1.GetReturnValuetoJSON();
            await GameDatabase.GetInstance().mailDB.ASetGiftRewarded(
                new GameDatabase.MailDB.Item()
                {
                    item_name = jd["items"]["item_name"].ToString(),
                    gift_type = jd["items"]["gift_type"].ToString(),
                    ty = int.Parse(jd["items"]["ty"].ToString()),
                    rt = int.Parse(jd["items"]["rt"].ToString()),
                    idx = int.Parse(jd["items"]["idx"].ToString()),
                    count = int.Parse(jd["itemsCount"].ToString())
                });

            Close();
        }
        else
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(bro1.GetMessage(), ResetInputField);
        }
    }

    void ResetInputField() => inputField.text = string.Empty;

    void Close()
    {
        ResetInputField();
        gameObject.SetActive(false);
    }
}
