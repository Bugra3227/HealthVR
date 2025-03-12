using System;
using TMPro;
using UnityEngine;

public class PathTracking : MonoBehaviour
{
    public static PathTracking instance;
    public bool HasStarted => _hasStarted;
    [SerializeField] private PlayerIdleTracker playerIdleTracker;
    public Transform[] correctPathPoints; // Doğru yol noktaları
    public Transform user; // Kullanıcı objesi (VR kamera veya karakter)
    public float correctPathThreshold = 1.0f; // Doğru yolda olma mesafesi
    public float inactivityThreshold = 0.5f; // Hareketsiz kalma eşiği (çok küçük hareketler sayılmaz)

    private float _startTime; // Zaman ölçümü için başlangıç zamanı
    private bool _hasStarted = false; // Süre başladı mı?
    private int _wrongPathCount = 0; // Yanlış yola sapma sayısı
    private int _correctPathCount = 0; // doğru yola sapma sayısı
    private bool _isOnCorrectPath = false; // Doğru yolda mı?

    private Vector3 _startPosition; // Başlangıç pozisyonu
    private Vector3 _lastPosition; // Başlangıç pozisyonu
    private float _totalDistanceTraveled = 0f; // Toplam mesafe


    public bool isReachedGoal;
    public UnityClients unityClients;

    public string time;
    public string wrongCount;
    public string correctCount;
    public string distance;
    public string stopTime;

    private void Awake()
    {
        instance = this;
    }

    public void StartTime()
    {
        _startTime = Time.time; // Zaman ölçümüne başla
        _startPosition = user.position; // Başlangıç pozisyonunu kaydet
        _hasStarted = true;
    }

    void Update()
    {
        if (!_hasStarted) return;

// Kullanıcının yolculuk süresini güncelle
        float elapsedTime = Time.time - _startTime;
        Debug.Log("Geçen süre: " + elapsedTime.ToString("F2") + " saniye");

// Kullanıcının aldığı mesafeyi güncelle
        float distanceTraveled = Vector3.Distance(_startPosition, user.position);
        _totalDistanceTraveled = distanceTraveled;
        Debug.Log("Yol alınan mesafe: " + _totalDistanceTraveled.ToString("F2") + " metre");
    }


    public void IncreaseWrongPathCount()
    {
        _wrongPathCount++;
    }

    public void IncreaseCorrectPathCount()
    {
        _correctPathCount++;
    }

    public void ReachedGoal()
    {
        if (isReachedGoal)
            return;


        isReachedGoal = true;
        _hasStarted = false;

        float totalTime = Time.time - _startTime;


        time = totalTime.ToString("F2");
        wrongCount = _wrongPathCount.ToString();
        correctCount = _correctPathCount.ToString();
        distance = _totalDistanceTraveled.ToString("F2");
        stopTime = playerIdleTracker.ReturnTotalIdleTime();
         string idleTime = playerIdleTracker.IdleTimeCount();
        unityClients.SendFinishRequest(time,wrongCount,correctCount,distance,stopTime,idleTime);
    }

    public string GetJsonData()
    {
        float totalTime = Time.time - _startTime;
        RequestData requestData = new RequestData
        {
            time = totalTime.ToString("F2"),
            wrongCount = _wrongPathCount.ToString(),
            correctCount = _correctPathCount.ToString(),
            distance = _totalDistanceTraveled.ToString("F2"),
            stopTime = playerIdleTracker.ReturnTotalIdleTime(),
            idleTime = "1",
        };

        string jsonData = JsonUtility.ToJson(requestData);
        return jsonData;
    }

    [System.Serializable]
    public class RequestData
    {
        public string time;
        public string wrongCount;
        public string correctCount;
        public string distance;
        public string stopTime;
        public string idleTime;
    }
}