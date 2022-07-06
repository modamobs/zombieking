using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public static class RowPaser
{
    // JsonData Int Paser 
    public static int IntPaser(JsonData row, string key)
    {
        if (row.Keys.Contains(key))
        {
            if (row[key].Keys.Contains("N"))
            {
                string val = row[key]["N"].ToString();
                try { return System.Convert.ToInt32(string.IsNullOrEmpty(val) ? "0" : val); }
                catch (System.Exception e) { LogPrint.PrintError("err IntPaser [N] val : " + val + ", e : " + e); }
            }
            else if (row[key].Keys.Contains("S"))
            {
                string val = row[key]["S"].ToString();
                try { return System.Convert.ToInt32(string.IsNullOrEmpty(val) ? "0" : val); }
                catch (System.Exception e) { LogPrint.PrintError("err IntPaser [S] key : " + key + ", val : " + val +", e : " + e); }
            }
        }

        return 0;
    }

    public static long LongPaser(JsonData row, string key)
    {
        if (row.Keys.Contains(key))
        {
            if (row[key].Keys.Contains("N"))
            {
                string val = row[key]["N"].ToString();
                try { return System.Convert.ToInt64(string.IsNullOrEmpty(val) ? "0" : val); }
                catch (System.Exception e) { LogPrint.PrintError("err LongPaser [N] val : " + val + ", e : " + e); }
            }
            else if (row[key].Keys.Contains("S"))
            {
                string val = row[key]["S"].ToString();
                try { return System.Convert.ToInt64(string.IsNullOrEmpty(val) ? "0" : val); }
                catch (System.Exception e) { LogPrint.PrintError("err LongPaser [S] val : " + val + ", e : " + e);  }
            }
        }

        return 0;
    }

    // JsonData Float Paser 
    public static float FloatPaser(JsonData row, string key)
    {
        if (row.Keys.Contains(key))
        {
            if (row[key].Keys.Contains("N"))
            {
                string val = row[key]["N"].ToString();
                try { return System.Convert.ToSingle(string.IsNullOrEmpty(val) ? "0" : val); }
                catch (System.Exception e) { LogPrint.PrintError("err FloatPaser [N] val : " + val + ", e : " + e); }
            }
            else if (row[key].Keys.Contains("S"))
            {
                string val = row[key]["S"].ToString();
                try { return System.Convert.ToSingle(string.IsNullOrEmpty(val) ? "0" : val); }
                catch (System.Exception e) { LogPrint.PrintError("err FloatPaser [S] val : " + val + ", e : " + e); }
            }
        }

        return 0f;
    }

    public static bool BoolPaser(JsonData row, string key)
    {
        if (row.Keys.Contains(key))
        {
            try 
            {
                if (row[key].Keys.Contains("S"))
                {
                    return bool.Parse(row[key]["S"].ToString());
                }
                else if (row[key].Keys.Contains("BOOL"))
                {
                    return bool.Parse(row[key]["BOOL"].ToString());
                }
                else
                {
                    if(string.Equals(row[key].ToString(), "TRUE") || string.Equals(row[key].ToString(), "FALSE"))
                    {
                        return bool.Parse(row[key].ToString());
                    }
                }
            }
            catch (System.Exception e)
            { 
                LogPrint.PrintError("err BoolPaser e : " + e);
            }
        }

        return false;
    }

    // JsonData String Paser
    public static string StrPaser(JsonData row, string key, bool null_str = false)
    {
        try
        {
            string str = string.Empty;
            if (row.Keys.Contains(key))
            {
                if (row[key].Keys.Contains("S"))
                    str = row[key]["S"].ToString();
                else if (row[key].Keys.Contains("N"))
                    str = row[key]["N"].ToString();
                else str = row[key].ToString();

                if (null_str == true)
                    return "null";
                else return str;
            }
        }
        catch (System.Exception e)
        {
            LogPrint.PrintError("err [S] StrPaser e : " + e);
        }

        return string.Empty;
    }

    // JsonData Array Int Paser
    public static List<int> ListIntPaser(JsonData row, string key)
    {
        if (row.Keys.Contains(key))
        {
            string c_value = row[key]["S"].ToString();
            if (!string.IsNullOrEmpty(c_value))
            {
                List<int> temp = new List<int>();
                string[] array = c_value.Split(',');
                for (int i = 0; i < array.Length; i++)
                {
                    temp.Add(System.Convert.ToInt32(array[i]));
                }
                return temp;
            }
        }
        return new List<int>();
    }
    // JsonData Array Float Paser
    public static List<float> ListFloatPaser(JsonData row, string key)
    {
        if (row.Keys.Contains(key))
        {
            string c_value = row[key]["S"].ToString();
            if (!string.IsNullOrEmpty(c_value))
            {
                List<float> temp = new List<float>();
                string[] array = c_value.Split(',');
                for (int i = 0; i < array.Length; i++)
                {
                    temp.Add(System.Convert.ToSingle(array[i]));
                }
                return temp;
            }
        }
        return new List<float>();
    }
}