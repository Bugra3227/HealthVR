using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class UnityClients : MonoBehaviour
{
    public string serverUrl ;
    public PathTracking pathTracking;
    public void SendFinishRequest(string time,string wrongCount,string correctCount,string distance,string stopTime,string idleTime)
    {
        string ipAdress = NetworkStarter.instance._androidIpAdress;
       serverUrl = $"http://{ipAdress}:8083";

      
        StartCoroutine(SendFinishDataRequest(time,wrongCount,correctCount,distance,stopTime,idleTime));
    }

    private IEnumerator SendFinishDataRequest(string value1,string value2,string value3,string value4,string value5,string value6)
    {
       
        RequestDataToFinish requestData = new RequestDataToFinish(value1,value2,value3,value4,value5,value6);
        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest($"{serverUrl}/finish_data", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log($"Sending Request to: {serverUrl}/finish_data");
        Debug.Log($"Request Data: {json}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Finish Data Error: {request.error}");
        }
        else
        {
            Debug.Log($"Finish Request Success: {request.downloadHandler.text}");
        }
    }


    [System.Serializable]
    public class RequestDataToFinish
    {
        public string time;
        public string wrongCount;
        public string correctCount;
        public string distance;
        public string stoppingTime;
        public string idleTime;

        public RequestDataToFinish(string value1, string value2, string value3, string value4, string value5,string value6)
        {
            this.time = value1;
            this.wrongCount = value2;
            this.correctCount = value3;
            this.distance = value4;
            this.stoppingTime = value5;
            this.idleTime = value6;
        }
    }
}