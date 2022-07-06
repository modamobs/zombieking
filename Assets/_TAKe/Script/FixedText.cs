using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixedText : MonoBehaviour
{
    [SerializeField] Text textString;
    [SerializeField] string languageId;

    void Awake()
    {
        if (textString == null)
        {
            textString = GetComponent<Text>();
        }
    }

    void Start ()
    {
        GameDatabase.GetInstance().lanChange += SetText;
        SetText();
    }

    public void SetText()
    {
        if (textString != null)
        {
            textString.text = LanguageGameData.GetInstance().GetString(languageId);
        }
    }
}
