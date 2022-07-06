using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PooledObject
{
    public string poolItemName = string.Empty;
    public ObjectLife prefab = null;
    [SerializeField] private int poonCount = 5;

    //[SerializeField]
    private List<ObjectLife> poolList = new List<ObjectLife>();

    public void Initialize(Transform parent = null)
    {
        for (int ix = 0; ix < poonCount; ++ix)
        {
            poolList.Add(CreateItem(parent));
        }
    }

    public void PushToPool(ObjectLife item, Transform parent = null)
    {
        item.transform.SetParent(parent);
        item.gameObject.SetActive(false);
        poolList.Add(item);
    }

    public ObjectLife PopFromPool(Transform parent = null)
    {
        if (poolList.Count == 0)
            poolList.Add(CreateItem(parent));

        ObjectLife item = poolList[0];
        poolList.RemoveAt(0);

        return item;
    }

    private ObjectLife CreateItem(Transform parent = null)
    {
        if (prefab == null)
            return null;
    
        ObjectLife item = Object.Instantiate(prefab) as ObjectLife;
        item.name = poolItemName;
        item.transform.SetParent(parent);
        item.gameObject.SetActive(false);

        return item;
    }
}
