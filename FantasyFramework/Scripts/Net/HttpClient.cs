using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using fastJSON;

public class HttpClient : MonoSingletion<HttpClient>
{

    public void PostRequest(string url, WWWForm form, Action<Dictionary<string,object>> callBack = null)
    {
        StartCoroutine(DoPost(url, form, callBack));
    }

    private IEnumerator DoPost(string url, WWWForm form, Action<Dictionary<string,object>> callBack = null)
    {
        WWW www = new WWW(url, form);
        yield return www;
        if (www.error != null)
        {
            Logger.DebugLog("WWW error:" + www.error);
        }
        else
        {
            Logger.DebugLog(www.text);
            Dictionary<string,object> responseData = JSON.Parse(www.text) as Dictionary<string,object>;
            if (callBack != null)
            {
                callBack(responseData);
            }
        }
    }
}
