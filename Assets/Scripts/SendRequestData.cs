using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Sends experiment result data to a backend server as a JSON payload.
/// </summary>
public class SendRequestData : MonoBehaviour
{
    // Server endpoint for receiving the experiment data.
    // In a production setup this could come from a config or NetworkStarter.
    [SerializeField] private string _serverUrl = "http://192.168.1.3:8383/get_data";

    /// <summary>
    /// Public entry point to send all result values to the server.
    /// </summary>
    public void SendRequestAllData(string time, string wrongCount, string correctCount, string distance, string stoppingTime)
    {
        StartCoroutine(SendStringRequest(time, wrongCount, correctCount, distance, stoppingTime));
    }

    /// <summary>
    /// Builds a JSON body from the given values and posts it to the configured server URL.
    /// </summary>
    private IEnumerator SendStringRequest(string value1, string value2, string value3, string value4, string value5)
    {
        // Build JSON payload
        var requestData = new RequestData(value1, value2, value3, value4, value5);
        string json = JsonUtility.ToJson(requestData);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(_serverUrl, UnityWebRequest.kHttpVerbPOST))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("SendRequestData - Error: " + request.error);
            }
            else
            {
                Debug.Log("SendRequestData - Request sent successfully: " + request.downloadHandler.text);
            }
        }
    }

    /// <summary>
    /// Serializable container for the experiment data to be sent as JSON.
    /// </summary>
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
            time         = value1;
            wrongCount   = value2;
            correctCount = value3;
            distance     = value4;
            stoppingTime = value5;
        }
    }
}
