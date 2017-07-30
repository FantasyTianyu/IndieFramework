using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        EventDispatcher.AddEventListener<string>("myEvent", OnEvent);
    }
	
    // Update is called once per frame
    void Update()
    {
		
    }

    private void OnEvent(string param1)
    {
        Logger.DebugLog(gameObject.name + "监听到事件,参数：" + param1);    
    }
}
