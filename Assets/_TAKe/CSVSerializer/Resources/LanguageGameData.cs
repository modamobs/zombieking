using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageGameData : ScriptableObject
{
    private static LanguageGameData instance;
    public static LanguageGameData GetInstance()
    {
        if (instance == null)
            instance = Resources.Load<LanguageGameData>("language_game_data");

        return instance;
    }

    [System.Serializable]
    public class Item
    {
        public string id;
        public string ko, en, jp, cn, ru, fr;
    }
    public List<Item> m_Items = new List<Item>();
    private Dictionary<string, Item> d_Items = new Dictionary<string, Item>();

    public void IAwake()
    {
        foreach (var item in m_Items)
        {
            if(!string.IsNullOrEmpty(item.id))
            {
                d_Items.Add(item.id, item);
            }
        }
    }

    public string GetString(string id)
    {
        try
        {
            return d_Items[id].ko.Replace("\\n", "\n");
        }
        catch (System.Exception)
        {
            return "err";
        }

        //return d_Items.ContainsKey(id) ? d_Items[id].ko : "err";

        //if (string.IsNullOrEmpty(id))
        //    return "";

        //string getLanguage = "ko";
        //if(string.Equals(getLanguage, "ko"))
        //{
        //    var strKo = m_Items.Find((Item obj) => obj.id == id);
        //    return strKo == null ? "" : strKo.ko.Replace("\\n", "\n");
        //    //return string.IsNullOrEmpty(strKo.id) ? "" : m_Items.Find((Item obj) => obj.id == id).ko.Replace("\\n", "\n");
        //}
        //else if (string.Equals(getLanguage, "jp"))
        //{
        //    return m_Items.Find((Item obj) => obj.id == id).jp.Replace("\\n", "\n");
        //}
        //else if (string.Equals(getLanguage, "cn"))
        //{
        //    return m_Items.Find((Item obj) => obj.id == id).cn.Replace("\\n", "\n");
        //}
        //else if (string.Equals(getLanguage, "ru"))
        //{
        //    return m_Items.Find((Item obj) => obj.id == id).ru.Replace("\\n", "\n");
        //}
        //else if (string.Equals(getLanguage, "fr"))
        //{
        //    return m_Items.Find((Item obj) => obj.id == id).fr.Replace("\\n", "\n");
        //}
        //else
        //{
        //    return m_Items.Find((Item obj) => obj.id == id). en.Replace("\\n", "\n");
        //}
    }
}
