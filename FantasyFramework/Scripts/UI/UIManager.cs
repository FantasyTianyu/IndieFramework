using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UIManager();
            }
            return instance;
        }
    }

    private Transform UIRoot;
    private Transform mainLayer;
    private Transform popLayer;

    private UIManager()
    {
        UIRoot = GameObject.Find("UICanvas").transform;
        mainLayer = GetChild(UIRoot.gameObject, "MainLayer").transform;
        popLayer = GetChild(UIRoot.gameObject, "PopLayer").transform;
    }

	
    /// <summary>
    /// 查找某一个GameObject下组件类型为T,名字为childName的子物体
    /// </summary>
    /// <returns>The child.</returns>
    /// <param name="node">要查找的物体</param>
    /// <param name="childName">要查找的物体名字</param>
    /// <typeparam name="T">组件类型</typeparam>
    public static GameObject GetChild<T>(GameObject node, string childName) where T:MonoBehaviour
    {
        Transform[] childs = node.transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; i++)
        {
            if (childs[i].GetComponent<T>() != null && childs[i].name == childName)
            {
                return childs[i].gameObject;
            }
        }
        return null;
    }

    public static GameObject GetChild(GameObject node, string childName)
    {
        Transform[] childs = node.transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; i++)
        {
            if (childs[i].name == childName)
            {
                return childs[i].gameObject;
            }
        }
        return null;
    }

    public void ChangeLayer(Transform target, UILayer layer)
    {
        switch (layer)
        {
            case UILayer.MainLayer:
                target.SetParent(mainLayer);
                break;
            case UILayer.PopLayer:
                target.SetParent(popLayer);
                break;
            default:
                break;
        }       
    }
}
