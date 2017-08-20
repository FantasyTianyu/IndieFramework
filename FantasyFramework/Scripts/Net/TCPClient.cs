using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Threading;

public class TCPClient
{
    private TcpClient client;
    private Thread recvThread;
    private bool isConnected = false;

    public bool IsConnected
    {
        get
        {
            return isConnected;
        }
    }

    public TCPClient()
    {
        client = new TcpClient();
        client.ReceiveBufferSize = 1024;
    }

    private void ConnectCallback(IAsyncResult result)
    {
        client.EndConnect(result);
        if (client.Connected)
        {
            recvThread = new Thread(new ThreadStart(ReceiveData));
            recvThread.Start();
            Action<bool> callBack = result.AsyncState as Action<bool>;
            callBack(true);
        }
        else
        {
            Logger.Error("服务器连接失败");
            Action<bool> callBack = result.AsyncState as Action<bool>;
            callBack(false);
        }
    }

    private void ReceiveData()
    {
        while (client.Connected)
        {
            byte[] recvData = new byte[client.ReceiveBufferSize];  
            try
            {  
                int readLength = client.GetStream().Read(recvData, 0, client.ReceiveBufferSize);
                //TODO 解析服务器发来的消息，根据协议不同，需要做拆包处理
            }
            catch (Exception ex)
            {  
                Logger.Error("异常信息：" + ex.Message);  
            }  
        }
    }

    public void ConnectToServer(string address, int port, Action<bool> connectedCallback)
    {
        client.BeginConnect(address, port, ConnectCallback, connectedCallback);
    }

    public void SendToServer(byte[] sendData)
    {
        client.GetStream().Write(sendData, 0, sendData.Length);
    }
}
