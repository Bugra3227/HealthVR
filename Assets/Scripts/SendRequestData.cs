using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System.Text;
using UnityEngine.Serialization;

public class SendRequestData : MonoBehaviour
{
    private string serverUrl;
   

   

    public void SendRequestAllData(string time, string wrongCount, string correctCount, string distance,
        string stopingTime)
    {
        // serverUrl = $"http://{NetworkStarter.instance._serverURL}:8080/get_data";
        serverUrl = "http://192.168.1.3:8383/get_data";
        
        StartCoroutine(SendStringRequest(time, wrongCount, correctCount, distance, stopingTime));
    }

    public IEnumerator SendStringRequest(string value1, string value2, string value3, string value4, string value5)
    {
        // JSON formatında veri oluştur
        RequestData requestData = new RequestData(value1, value2, value3, value4, value5);
        string json = JsonUtility.ToJson(requestData); // JSON formatında veri oluştur

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json); // JSON verisini byte dizisine dönüştür

        UnityWebRequest request = new UnityWebRequest(serverUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json"); // Content-Type olarak JSON gönderiyoruz
        
        yield return request.SendWebRequest(); // Veriyi gönder

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
           
            Debug.LogError("Error: " + request.error);
        }
        else
        {
           
            Debug.Log("Request sent successfully: " + request.downloadHandler.text);
        }
    }

    // JSON verisi için sınıf
    [System.Serializable]
    public class RequestData
    {
        public string time;
        public string wrongCount;
        public string correctCount;
        public string distance;
        public string stoppingTime;

        public RequestData(string value1, string value2, string value3, string value4, string value5)
        {
            this.time = value1;
            this.wrongCount = value2;
            this.correctCount = value3;
            this.distance = value4;
            this.stoppingTime = value5;
        }
    }
}