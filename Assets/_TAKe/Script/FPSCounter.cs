using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.Utility
{
    //[RequireComponent(typeof (Text))]
    public class FPSCounter : MonoBehaviour
    {
        //Text mText;
        float deltaTime = 0.0f;

        GUIStyle style;
        Rect rect;
        float msec;
        float fps;
        float worstFps = 100f;
        string text;

        void Awake()
        {
            //mText = GetComponent<Text>();
            int w = Screen.width, h = Screen.height;
            rect = new Rect(0, h - (h * 2 / 100), w, h * 4 / 100);
            style = new GUIStyle();
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = Color.cyan;

            StartCoroutine("loopUpdate");
            StartCoroutine("worstReset");
        }

        IEnumerator worstReset() //�ڷ�ƾ���� 15�� �������� ���� ������ ����.
        {
            yield return new WaitForSeconds(1.0f);
            while (true)
            {
                yield return new WaitForSeconds(15f);
                worstFps = 100f;
            }
        }

        IEnumerator loopUpdate()
        {
            while (true)
            {
                yield return null;
                deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            }
        }

        void OnGUI()//�ҽ��� GUI ǥ��.
        {

            msec = deltaTime * 1000.0f;
            fps = 1.0f / deltaTime;  //�ʴ� ������ - 1�ʿ�

            if (fps < worstFps)  //���ο� ���� fps�� ���Դٸ� worstFps �ٲ���.
                worstFps = fps;

            text = msec.ToString("F1") + "ms (" + fps.ToString("F1") + ") //worst : " + worstFps.ToString("F1");
            GUI.Label(rect, text, style);
            //mText.text = msec.ToString("F1") + "ms (" + fps.ToString("F1") + "fps) //worst : " + worstFps.ToString("F1");
        }
    }
}
