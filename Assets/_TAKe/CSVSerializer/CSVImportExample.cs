using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CSVImportExample : ScriptableObject
{
    [System.Serializable]
    public class Sample
    {
        public int year;
        public string make;
        public string model;
        public string description;
        public float price;
    }
    public Sample[] m_Sample;
}

#if UNITY_EDITOR
public class CSVImportExamplePostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        Debug.Log("OnPostprocessAllAssets cnt : " + importedAssets.Length);
        foreach (string str in importedAssets)
        {
            Debug.Log("OnPostprocessAllAssets str : " + str);
            // ### 게임 데이터 언어 ###
            if (str.IndexOf("/language_game_data.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                LanguageGameData gm = AssetDatabase.LoadAssetAtPath<LanguageGameData>(assetfile);
                if (gm == null)
                {
                    gm = new LanguageGameData();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                LanguageGameData.Item[] items = CSVSerializer.Deserialize<LanguageGameData.Item>(data.text);
                //gm.m_Items =
                gm.m_Items.Clear();
                foreach (var item in items)
                    gm.m_Items.Add(item);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }

            // ### 시스템 언어 ###
            if (str.IndexOf("/language_system_data.csv") != -1)
            {
                TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                string assetfile = str.Replace(".csv", ".asset");
                LanguageSystemData gm = AssetDatabase.LoadAssetAtPath<LanguageSystemData>(assetfile);
                if (gm == null)
                {
                    gm = new LanguageSystemData();
                    AssetDatabase.CreateAsset(gm, assetfile);
                }

                LanguageSystemData.Item[] items = CSVSerializer.Deserialize<LanguageSystemData.Item>(data.text);
                gm.m_Items.Clear();
                foreach (var item in items)
                    gm.m_Items.Add(item);

                EditorUtility.SetDirty(gm);
                AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                Debug.Log("Reimported Asset: " + str);
#endif
            }
        }
    }
}
#endif