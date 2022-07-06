using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpChat : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] BringToFront bringToFront;
    [SerializeField] GameObject goRoot;
    [SerializeField] CanvasGroup canGrp;
    [SerializeField] Text txTopLineMsg;
    [SerializeField] CanvasGroup cgTopLineMsg;
    [SerializeField] Slider slider;
    float maxAlpha = 0.75f;

    void Start()
    {
        bringToFront.gameObject.SetActive(true);
        slider.minValue = maxAlpha;
        slider.value = 1.0f;
        canGrp.alpha = 0;
        canGrp.blocksRaycasts = false;

        txTopLineMsg.text = "쌉 좀비 나이트 키우기에 오신걸 환영합니다!";
        StopCoroutine("TopLineMsgAlpha");
        StartCoroutine("TopLineMsgAlpha");
    }

    public void On()
    {
        canGrp.alpha = slider.value >= maxAlpha && slider.value <= 1.0f ? slider.value : 1.0f;
        slider.value = canGrp.alpha;
        canGrp.blocksRaycasts = true;
        bringToFront.OnAsKastSubkubg();
    }

    public void Click_Off()
    {
        canGrp.alpha = 0;
        canGrp.blocksRaycasts = false;
    }

    public void OnTopChatLineMsg(ChatMsg chat_msg)
    {
        if (chat_msg.MsgType == 1)
        {
            txTopLineMsg.text = string.Format("{0}\n{1}", chat_msg.TItle, chat_msg.Contents);
        }
        else if (chat_msg.MsgType == 2)
        {
            txTopLineMsg.text = string.Format(chat_msg.Contents);
        }
        
        StopCoroutine("TopLineMsgAlpha");
        StartCoroutine("TopLineMsgAlpha");
    }

    WaitForSeconds ss = new WaitForSeconds(5f);
    IEnumerator TopLineMsgAlpha()
    {
        cgTopLineMsg.alpha = 1.0f;
        yield return ss;
        while (cgTopLineMsg.alpha > 0)
        {
            cgTopLineMsg.alpha -= Time.deltaTime;
            yield return null;
        }
    }

    public void Slider_CanvasAlpha(Slider s)
    {
        canGrp.alpha = s.value > maxAlpha ? s.value : maxAlpha;
    }

    public void Click_SizeChange()
    {
        int sizeHg = (int)rectTransform.sizeDelta.y;
        sizeHg -= 150;
        if(sizeHg < 300)
            sizeHg = 600;

        rectTransform.sizeDelta = new Vector2(900, sizeHg); 
    }
}