using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NetManager
{
    private static NetManager instance;

    public static NetManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetManager();
            }
            return instance;
        }
    }

    private NetManager()
    {
        
    }

    public void PostRequest(string url, WWWForm form, Action<Dictionary<string,object>> callBack = null)
    {
        HttpClient.Instance.PostRequest(url, form, callBack);
    }

}
