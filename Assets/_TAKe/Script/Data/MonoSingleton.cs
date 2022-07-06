using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance = null;
    public static T GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(T)) as T;
            //Debug.LogError(" ============== instance (" + typeof(T).ToString() + ") == null ============== ");
            //instance = new GameObject("@" + typeof(T).ToString(), typeof(T)).AddComponent<T>();
            //DontDestroyOnLoad(instance);

            if(instance == null)
                return null;
        }

        return instance;

        //get
        //{
        //    instance = FindObjectOfType(typeof(T)) as T;
        //    if (instance == null)
        //    {
        //        Debug.LogError(" ============== instance (" + typeof(T).ToString()  +") == null ============== ");
        //        //instance = new GameObject("@" + typeof(T).ToString(), typeof(T)).AddComponent<T>();
        //        //DontDestroyOnLoad(instance);
        //        return null;
        //    }

        //    return instance;
        //}
    }


    //public static ResourceDatabase GetInstance()
    //{
    //    if (instance == null)
    //        instance = Resources.Load<ResourceDatabase>("ResourceDatabase");

    //    return instance;
    //}
}