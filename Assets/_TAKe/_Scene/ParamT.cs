using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamT
{
    public struct P
    {
        public string k;
        public object v;
    }
    
    public static Param Collection(P[] pct)
    {
        Param param = new Param();
        foreach (var p in pct)
        {
            switch (p.v.GetType().ToString())
            {
                case "System.Int32": param.Add(p.k.ToString(), (int)p.v); break;
                case "System.Int64": param.Add(p.k.ToString(), (long)p.v); break;
                case "System.Single": param.Add(p.k.ToString(), (float)p.v); break;
                case "System.Double": param.Add(p.k.ToString(), (double)p.v); break;
                case "System.String": param.Add(p.k.ToString(), (string)p.v); break;
            }
        }

        return param;
    }
}
