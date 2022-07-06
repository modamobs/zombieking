using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThrowCurve : MonoBehaviour
{
    public TextMeshPro count;
    public Transform target;
    public Transform curvePoint;
    public float speed = 1f;

    float t = 0f;
    Vector3 m_start;
    public bool isEnd = false;

    public void SetStart (Transform _endTr, Transform _curvePoint, int _count)
    {
        isEnd = false;
        count.text = _count.ToString();
        target = _endTr;
        curvePoint = _curvePoint;
        m_start = transform.position;
        t = 0f;
        count.gameObject.SetActive(true);
    }

    public void SetCount (int _count)
    {
        count.text = _count.ToString();
    }

    void Update()
    {
        if(!isEnd)
        {
            transform.position = Bezier(m_start, curvePoint.position, target.position, t);
            t = Mathf.Min(1, t + Time.deltaTime * speed);
            //debug
            for (float i = 0; i < 19; i++)
            {
                var a = Bezier(m_start, curvePoint.position, target.position, i * 1f / 20f);
                var b = Bezier(m_start, curvePoint.position, target.position, (i + 1f) * 1f / 20f);
                Debug.DrawLine(a, b);
            }

            isEnd = (transform.position - target.position).sqrMagnitude < 0.01f;
        }
    }

    Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        var omt = 1f - t;
        return a * omt * omt + 2f * b * omt * t + c * t * t;
    }
}
