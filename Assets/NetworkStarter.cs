using System;
using System.Collections;
using System.Net;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkStarter : MonoBehaviour
{
    public static NetworkStarter instance;
    private HttpListener _httpListener;
    public string experimentIndex;
    public string _serverURL;
    public string _androidIpAdress;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartServer();
    }

    private void StartServer()
    {
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add("http://*:8080/receive_data/"); // POST isteği için
       _httpListener.Prefixes.Add("http://*:8080/get_data/"); // GET isteği için
       _httpListener.Prefixes.Add("http://*:8080/finish_data/");
      //  _httpListener.Prefixes.Add("http://*:8383/send_data"); // GET isteği için
        _httpListener.Start();
        _httpListener.BeginGetContext(HandleRequest, null);
        Debug.Log("HTTP server started on port 8383");
        //SendFinishRequest();
    }
     
     
    private void HandleRequest(IAsyncResult result)
    {
        try
        {
            var context = _httpListener.EndGetContext(result);
            _httpListener.BeginGetContext(HandleRequest, null);

            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/receive_data")
            {
                HandlePostRequest(request, response);
            }
            else if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/get_data" )
            {
               
            }
            else if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/finish_data")
            {
              
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Close();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error handling request: " + ex.Message);
        }
    }

    // POST isteği ile gelen veriyi işleme
    private void HandlePostRequest(HttpListenerRequest request, HttpListenerResponse response)
    {
        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
        {
            var receivedJson = reader.ReadToEnd();
            Debug.Log("Received JSON: " + receivedJson);

            // JSON verisini işleme
            RequestData requestData = JsonUtility.FromJson<RequestData>(receivedJson);
            Debug.Log($"Received values: value1 = {requestData.value1}, value2 = {requestData.value2}");

            experimentIndex = requestData.value1;
            _serverURL = requestData.value2;
            _androidIpAdress = requestData.value3;
            
            // İlgili sahneye yönlendirme
            StartSceneBasedOnIndex();
        }

        byte[] buffer = Encoding.UTF8.GetBytes("{\"message\": \"Data received successfully\"}");
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }

    // GET isteği ile veri gönderme
    // Bu fonksiyon, sunucuya gelen GET isteği olduğunda çağrılacak
    private void HandleGetRequest(HttpListenerResponse response)
    {
         PathTracking.instance.ReachedGoal();
         // JSON formatında cevabı hazırlama
      
        
        byte[] buffer = Encoding.UTF8.GetBytes(PathTracking.instance.GetJsonData());
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }

   
    private void StartSceneBasedOnIndex()
    {
        // Sahneyi başlatma
        SceneManager.LoadScene("Scenes/MainScene");
    }
    public void SendFinishRequest()
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/finish_data");
            request.Method = "POST";
            request.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string jsonData = "{\"message\": \"Finish request triggered\"}";
                streamWriter.Write(jsonData);
            }

            var response = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Debug.Log("Finish Request Response: " + result);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error sending finish request: " + ex.Message);
        }
    }

   


    void OnApplicationQuit()
    {
        if (_httpListener != null)
        {
            _httpListener.Close();
        }
    }

    // Veriyi JSON formatında gönderilecek sınıf
    [System.Serializable]
    public class RequestData
    {
        public string value1;
        public string value2;
        public string value3;
        public string value4;
        public string value5;
        public string time;
        public string wrongCount;
        public string correctCount;
        public string distance;
        public string stoppingTime;

        public RequestData(string value1, string value2, string value3, string value4, string value5)
        {
            this.value1 = value1;
            this.value2 = value2;
            this.value3 = value3;
            this.value4 = value4;
            this.value5 = value5;
        }
    }
  
}