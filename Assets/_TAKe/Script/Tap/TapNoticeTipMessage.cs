using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapNoticeTipMessage : MonoBehaviour
{
    [SerializeField] List<string> gameTipMsg = new List<string>();
    Queue<string> qMsg = new Queue<string>();
    [SerializeField] Animation ani; // NoticeTipUserDropMessage
    [SerializeField] Text txMsg;

    public int GetTipCount() => gameTipMsg.Count;

    public string GetTip(int idx)
    {
        try
        {
            return gameTipMsg[idx];
        }
        catch (System.Exception)
        {
            return "";
        }
    }

    void Awake()
    {
        for (int i = 0; i < 100; i++)
        {
            string sVal = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("game.tip.msg.{0}", i)).val_string;
            if (string.IsNullOrEmpty(sVal))
                break;
            else gameTipMsg.Add(sVal);
        }
    }

    IEnumerator Start()
    {
        WaitForSeconds s1 = new WaitForSeconds(0.5f);
        WaitForSeconds s5 = new WaitForSeconds(5.0f);
        WaitForSeconds s15 = new WaitForSeconds(15.0f);

        bool isShake = false;
        int tip_indx = 0, tip_max_indx = GetTipCount();
        //string nTipMsg = "";
        while (true)
        {
            if (qMsg.Count == 0)
            {
                //var rTipMsg = gameTipMsg.FindAll(s => !string.Equals(s, nTipMsg));
                //var tipMsg = rTipMsg[Random.Range(0, rTipMsg.Count)];
                //nTipMsg = tipMsg;
                //txMsg.text = tipMsg;

                if (!isShake)
                {
                    txMsg.text = string.Format("<color=yellow>TIP{0}. </color>{1}", (tip_indx + 1), gameTipMsg[tip_indx]);
                    tip_indx++;
                    if (tip_indx >= tip_max_indx)
                    {
                        tip_indx = 0;
                        isShake = true;
                    }
                }
                else
                {
                    int r = Random.Range(0, GetTipCount());
                    txMsg.text = string.Format("<color=yellow>TIP{0}. </color>{1}", (r + 1), gameTipMsg[r]);
                }

                yield return s15;
            }
            else
            {
                txMsg.text = qMsg.Dequeue();
                yield return s5;
            }


            txMsg.text = "";
            yield return s1;
        }
    }

    public void SetUserChatMsg(string msg)
    {

    }
}
