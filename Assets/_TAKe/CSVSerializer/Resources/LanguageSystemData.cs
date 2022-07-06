using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSystemData : ScriptableObject
{
    private static LanguageSystemData instance;
    public static LanguageSystemData GetInstance()
    {
        if (instance == null)
            instance = Resources.Load<LanguageSystemData>("language_system_data");

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
            if (!string.IsNullOrEmpty(item.id))
            {
                d_Items.Add(item.id, item);
            }
        }
    }

    public string GetString(string id)
    {
        if (d_Items.ContainsKey(id))
        {
            return d_Items[id].ko.Replace("\\n", "\n");
        }
        else
        {
            return id;
        }

        ////return "asd";
        //return d_Items.ContainsKey(id) ? d_Items[id].ko : id;
        ////return m_Items.Find((Item obj) => obj.id == id).ko.Replace("\\n", "\n");
    }
}
