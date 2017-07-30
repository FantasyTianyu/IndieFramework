using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingletion<T> : MonoBehaviour where T:MonoBehaviour
{
    private static T instance;
    private static GameObject instanceObj;

    public static T Instance
    {
        get
        {
            if (instanceObj == null)
            {
                instanceObj = new GameObject("Single_" + typeof(T).ToString());
                instance = instanceObj.AddComponent<T>();
                DontDestroyOnLoad(instanceObj);
            }
            return instance;
        }
    }
	
}
