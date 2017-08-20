using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NetManager
{
    //服务器地址
    private string SERVER_ADDRESS = "";
    //服务器端口
    private int SERVER_PORT = 10000;

    private TCPClient tcpClient;

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

    public void TCPConnect(Action<bool> connectCallback)
    {
        if (tcpClient == null)
        {
            tcpClient = new TCPClient();
            tcpClient.ConnectToServer(SERVER_ADDRESS, SERVER_PORT, connectCallback);
        }
    }
}
