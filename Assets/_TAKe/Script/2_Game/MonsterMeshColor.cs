using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMeshColor : MonoBehaviour
{
    public List<Monster> monsters = new List<Monster>();
    [System.Serializable]
    public class Monster
    {
        public int ID;
        public GameObject goMnst;
        public SkinnedMeshRenderer sknMeshRdr;
        public Mesh[] meshs;
    }

    public List<GameObject> dgn_mon_all = new List<GameObject>();
    public List<DgnMonster> dgn_mine_monsters = new List<DgnMonster>();
    public List<DgnMonster> dgn_raid_monsters = new List<DgnMonster>();
    [System.Serializable]
    public class DgnMonster
    {
        public int DgnID;
        public List<Mon> monster = new List<Mon>();
        [System.Serializable]
        public class Mon
        {
            public int ID;
            public GameObject goMnst;
        }
    }

    public int NormalMeshLastID => monsters[monsters.Count - 1].ID;
    public int NormalMeshLastColorID => 6;

    public void SetNormalMonsterColor(int _ID, int _CorID)
    {
        int findIndx = monsters.FindIndex(e => e.ID == _ID);
        if(findIndx == -1)
        {
            findIndx = 0;
        }

        for (int i = 0; i < monsters.Count; i++)
        {
            if (monsters[i].goMnst.activeSelf == !(i == findIndx))
            {
                monsters[i].goMnst.SetActive(i == findIndx);
            }
        }
        
        var mnst = monsters[findIndx];
        int corID = _CorID < mnst.meshs.Length ? _CorID : Random.Range(0, mnst.meshs.Length);

        mnst.sknMeshRdr.sharedMesh = mnst.meshs[corID];
        float rScl = Random.Range(1.5f, 2.0f);
        mnst.goMnst.transform.localScale = new Vector3(rScl, rScl, rScl);
        mnst.goMnst.gameObject.SetActive(true);
    }


    public void SetDgnMonster(IG.ModeType _Mdty, int _DgnID, int _DgnMnstID) // mdty, dgnID, dgnMnstID
    {
        foreach (var item in dgn_mon_all)
            item.SetActive(false);

        for (int i = 0; i < dgn_mine_monsters.Count; i++)
        {
            foreach (var item in dgn_mine_monsters[i].monster)
                if(item.goMnst.activeSelf)
                    item.goMnst.SetActive(false);
        }
        for (int i = 0; i < dgn_raid_monsters.Count; i++)
        {
            foreach (var item in dgn_raid_monsters[i].monster)
                if (item.goMnst.activeSelf)
                    item.goMnst.SetActive(false);
        }

        if (_Mdty == IG.ModeType.DUNGEON_MINE)
        {
            int dgMineIndx = dgn_mine_monsters.FindIndex(m => m.DgnID == _DgnID);
            if(dgMineIndx >= 0)
            {
                int mnst_id = dgn_mine_monsters[dgMineIndx].monster.FindIndex(m => m.ID == _DgnMnstID);
                if(mnst_id >= 0)
                {
                    dgn_mine_monsters[dgMineIndx].monster[mnst_id].goMnst.SetActive(true);
                }
            }
        }
        else if(_Mdty == IG.ModeType.DUNGEON_RAID)
        {
            int dgRaidIndx = dgn_raid_monsters.FindIndex(m => m.DgnID == _DgnID);
            if (dgRaidIndx >= 0)
            {
                int mnst_id = dgn_raid_monsters[dgRaidIndx].monster.FindIndex(m => m.ID == _DgnMnstID);
                if (mnst_id >= 0)
                {
                    dgn_raid_monsters[dgRaidIndx].monster[mnst_id].goMnst.SetActive(true);
                }
            }
        }
    }
}
