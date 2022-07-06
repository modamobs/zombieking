using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoSingleton<ObjectPool>
{
    public List<PooledObject> objectPool_onceFx = new List<PooledObject>();
    public List<PooledObject> objectPool_loopFx = new List<PooledObject>();
    public List<PooledObject> objectPool_etc = new List<PooledObject>();
    public List<PooledObject> objectPool_monster = new List<PooledObject>();

    public const string Game_PlayerZombie = "Game_PlayerZombie";
    public const string Game_NormalMonster = "Game_NormalMonster";
    public const string Game_DungeonMonster = "Game_DungeonMonster";

    public const string OP_Once_GeneralBeHit    = "OP_Once_GeneralBeHit";
    public const string OP_Once_SwordSlash      = "OP_Once_SwordSlash";
    public const string OP_Once_HitDamageText   = "OP_Once_HitDamageText";
    public const string OP_Once_BubbleActionZombieSport = "OP_Once_BubbleActionZombieSport";

    public const string OP_Once_AutoSaleDecompReward = "OP_Once_AutoSaleDecompReward";
    public List<ObjectLife> onActiveLife = new List<ObjectLife>();

    #region ################ Pool ################
    void Awake()
    {
        for (int ix = 0; ix < objectPool_onceFx.Count; ++ix)
        {
            if (!string.Equals(objectPool_onceFx[ix].poolItemName, "x"))
                objectPool_onceFx[ix].Initialize(transform);
        }

        for (int ix = 0; ix < objectPool_loopFx.Count; ix++)
        {
            if(!string.Equals(objectPool_loopFx[ix].poolItemName, "x"))
                objectPool_loopFx[ix].Initialize(transform);
        }

        for (int ix = 0; ix < objectPool_etc.Count; ix++)
        {
            if (!string.Equals(objectPool_etc[ix].poolItemName, "x"))
                objectPool_etc[ix].Initialize(transform);
        }

        for (int ix = 0; ix < objectPool_monster.Count; ix++)
        {
            if (!string.Equals(objectPool_monster[ix].poolItemName, "x"))
                objectPool_monster[ix].Initialize(transform);
        }
    }

    public void ResetOffPool()
    {
        ObjectLife[] ols = Transform.FindObjectsOfType<ObjectLife>();
        foreach (var item in ols)
            item.VoLife();
    }

    public bool PushToPool(string itemName, ObjectLife item, Transform parent = null)
    {
        PooledObject pool = GetPoolItem(itemName);
        if (pool == null)
            return false;
            
        pool.PushToPool(item, parent == null ? transform : parent);
        return true;
    }

    public bool PushToPoolSec(string itemName, ObjectLife item, float ftime, Transform parent = null)
    {
        PooledObject pool = GetPoolItem(itemName);
        if (pool == null)
            return false;
            
        pool.PushToPool(item, parent == null ? transform : parent);
        return true;
    }

    public ObjectLife PopFromPool(string itemName, Vector3 pos = default, Quaternion qua = default, Transform parent = null)
    {
        PooledObject pool = GetPoolItem(itemName);
        if (pool == null)
            return null;

        ObjectLife _go = pool.PopFromPool(parent);
        if(_go == null)
        {
            LogPrint.EditorPrint("<color=red> PopFromPool error : ObjectLife == null </color>");
        }

        if (!System.Object.Equals(pos, default))
        {
            if(parent != null)
            {
                _go.transform.SetParent(parent);
                _go.transform.localPosition = pos;
            }
            
            _go.transform.position = pos;
            _go.transform.rotation = qua;
            _go.gameObject.SetActive(true);
        }

        return _go;
    }

    PooledObject GetPoolItem(string itemName)
    {
        if (objectPool_onceFx.Count > 0)
        {
            int indx1 = objectPool_onceFx.FindIndex(x => string.Equals(x.poolItemName, itemName));
            if (indx1 >= 0)
                return objectPool_onceFx[indx1];
        }

        if (objectPool_loopFx.Count > 0)
        {
            int indx3 = objectPool_loopFx.FindIndex(x => string.Equals(x.poolItemName, itemName));
            if (indx3 >= 0)
                return objectPool_loopFx[indx3];
        }

        if(objectPool_monster.Count > 0)
        {
            int indx2 = objectPool_monster.FindIndex(x => string.Equals(x.poolItemName, itemName));
            if (indx2 >= 0)
                return objectPool_monster[indx2];
        }
        
        if(objectPool_etc.Count > 0)
        {
            int indx2 = objectPool_etc.FindIndex(x => string.Equals(x.poolItemName, itemName));
            if (indx2 >= 0)
                return objectPool_etc[indx2];
        }

#if UNITY_EDITOR
        Debug.LogWarning("There's no matched pool list. itemName : " + itemName);
#endif

        return null;
    }
    #endregion
}
