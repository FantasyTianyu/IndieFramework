using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;

namespace IndieFramework {
    public static class UnityWebRequestExtensions {
        // 扩展方法将 UnityWebRequestAsyncOperation 转换为 Task
        public static Task<UnityWebRequestAsyncOperation> SendWebRequestTask(this UnityWebRequest request) {
            var completionSource = new TaskCompletionSource<UnityWebRequestAsyncOperation>();
            request.SendWebRequest().completed += operation => completionSource.SetResult(operation as UnityWebRequestAsyncOperation);
            return completionSource.Task;
        }
    }
    public class NetManager : Singleton<NetManager> {
        //服务器地址
        private string SERVER_ADDRESS = "";
        //服务器端口
        private int SERVER_PORT = 10000;

        private TCPClient tcpClient;

        public async Task<T> GetAsync<T>(string url) {
            using (UnityWebRequest request = UnityWebRequest.Get(url)) {
                await request.SendWebRequestTask();

                if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError) {
                    Debug.LogError($"Error: {request.error}");
                    return default;
                } else {
                    return JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
                }
            }
        }

        public async Task<T> PostAsync<T>(string url, object postData) {
            string jsonData = JsonConvert.SerializeObject(postData);
            var data = Encoding.UTF8.GetBytes(jsonData);

            using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)) {
                request.uploadHandler = new UploadHandlerRaw(data);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                await request.SendWebRequestTask();

                if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError) {
                    Debug.LogError($"Error: {request.error}");
                    return default;
                } else {
                    return JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
                }
            }
        }

        public void TCPConnect(Action<bool> connectCallback) {
            if (tcpClient == null) {
                tcpClient = new TCPClient();
                tcpClient.ConnectToServer(SERVER_ADDRESS, SERVER_PORT, connectCallback);
            }
        }
    }
}