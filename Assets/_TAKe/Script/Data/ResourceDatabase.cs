using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceDatabase : ScriptableObject
{
    private static ResourceDatabase instance;
    public static ResourceDatabase GetInstance ()
    {
        if (instance == null)
            instance = Resources.Load<ResourceDatabase>("ResourceDatabase");

        return instance;
    }

    //#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    //        [UnityEditor.MenuItem("TAKe/Create ResourceDatabase asset")]
    //        public static void Save()
    //        {
    //            if (instance == null) instance = new ResourceDatabase();
    //            if (!System.IO.Directory.Exists("Assets/Resources")) System.IO.Directory.CreateDirectory("Assets/_TAKe/Resources");
    //            UnityEditor.AssetDatabase.CreateAsset(instance, "Assets/_TAKe/Resources/ResourceDatabase.asset");
    //        }
    //#endif

    public GameObject goGameZombie;
    public GameObject goLobbyZombie;

    #region ##### 장비 리소스 #####
    public List<Equipment> res_equipment = new List<Equipment>();
    [System.Serializable]
    public class Equipment
    {
        [SerializeField] string header;
        public string equip_name;
        public int parts_type;
        public List<EquipmentArray> equip_res_array = new List<EquipmentArray>();
        [System.Serializable]
        public class EquipmentArray
        {
            [SerializeField] string header;
            public int rating;

            public List<EquipRes> equip_res = new List<EquipRes>();
            [System.Serializable]
            public struct EquipRes
            {
                public int idx;
                public Sprite icon;
            }
        }
    }

    public Sprite GetEquipment (int get_ptype, int get_rat, int get_idx)
    {
        var eqResAll = res_equipment.Find((Equipment eq) => eq.parts_type == get_ptype).equip_res_array.Find((Equipment.EquipmentArray eq_rat) => eq_rat.rating == get_rat).equip_res.Find((Equipment.EquipmentArray.EquipRes eq_idx) => eq_idx.idx == get_idx);
        return eqResAll.icon;
    }
    #endregion

    #region ##### 컬러 #####
    [SerializeField] Color[] chatRankColor = new Color[10];
    [SerializeField] Color[] itemColors = new Color[8];
    [SerializeField] string[] itemHexColors = new string[8];
    [SerializeField] Color[] itemGradientColors = new Color[8];
    [SerializeField] Color[] itemShinyColors = new Color[8];
    [SerializeField] Color textColorWhite, textColorGray, textColorRed;
    [System.Serializable] public struct ItemColor { public int rating; public Color color; }
    [System.Serializable] public struct ItemGradientColor { public int rating; public Color color; }
    public Color GetChatRankColor(int rank)
    {
        if (rank > 10000)
            return chatRankColor[9];
        else if (rank > 1000)
            return chatRankColor[8];
        else if (rank > 500)
            return chatRankColor[7];
        else if (rank > 100)
            return chatRankColor[6];
        else if (rank > 50)
            return chatRankColor[5];
        else if (rank > 10)
            return chatRankColor[4];
        else if (rank > 3)
            return chatRankColor[3];
        else if (rank == 3)
            return chatRankColor[2];
        else if (rank == 2)
            return chatRankColor[1];
        else if (rank == 1)
            return chatRankColor[0];

        return chatRankColor[9];
    }

    public Color GetItemColor(int rt) => itemColors[rt]; //itemColor.Find((ItemColor co) => co.rating == _rat).color;
    public string GetItemHexColor(int rt) => itemHexColors[rt];
    public Color GetItemGradientColor(int rt) => itemGradientColors[rt]; //itemGradientColor.Find((ItemGradientColor co) => co.rating == _rat).color;
    public Color GetItemShinyColor(int rt) => itemShinyColors[rt];


    public Color GetColorButtonText(bool isWhite)
    {
        if (isWhite) return textColorWhite;
        else return textColorGray;
    }

    public Color hitColor_General; // 기본 대미지 컬러 
    public Color hitColor_Critical; // 치명타 대미지 컬러 
    public Color hitColor_Bomb; // 폭탄 대미지 컬러 
    public Color hitColor_Bonus; // 추가 (보너스) 대미지 
    public Color hitColor_Bleeding; // 출혈 대미지 
    public Color hitColor_Reflect; // 반사 대미지 
    public Color hitColor_Skill; // 스킬 대미지 
    public Color hitColor_Evasion; // 회피 대미지 (MISS)
    #endregion

}
