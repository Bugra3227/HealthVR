using System;
using UnityEngine;

public class PathTracking : MonoBehaviour
{
    // Singleton (isteğe bağlı, kullanıyorsan kalsın)
    public static PathTracking Instance { get; private set; }
    
    // Public read-only property
    public bool HasStarted => _hasStarted;
    public bool IsGoalReached => _isGoalReached;

    [Header("References")]
    [SerializeField] private PlayerIdleTracker _playerIdleTracker;
    [SerializeField] private UnityClients _unityClients;
    [SerializeField] private Transform _user; 

    [Header("Path Settings")]
    [SerializeField] private Transform[] _correctPathPoints;
    [SerializeField] private float _correctPathThreshold = 1.0f;

    [Header("Movement Settings")]
    [SerializeField] private float _movementThreshold = 0.01f;

    [Header("Debug")]
    [SerializeField] private bool _enableDebugLogs = false;

    // State
    private float _startTime;
    private bool _hasStarted;
    private bool _isGoalReached;

    private int _wrongPathCount;
    private int _correctPathCount;

    private Vector3 _lastPosition;
    private float _totalDistanceTraveled;

    // Sonuçlar için sadece okunabilir propertiler (UI vs. için kullanılabilir)
    public string TimeText { get; private set; }
    public string WrongCountText { get; private set; }
    public string CorrectCountText { get; private set; }
    public string DistanceText { get; private set; }
    public string StopTimeText { get; private set; }
    public string IdleTimeText { get; private set; }

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple PathTracking instances detected. Destroying the new one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (_user != null)
        {
            _lastPosition = _user.position;
        }
    }

    private void Update()
    {
        if (!_hasStarted || _isGoalReached || _user == null)
            return;

        UpdateElapsedTime();
        UpdateDistance();
        // Eğer doğru/yanlış yol tespitini de buraya eklersen,
        // buradan _correctPathCount ve _wrongPathCount arttırılabilir.
    }

    #endregion

    #region Public API

    public void StartTracking()
    {
        if (_user == null)
        {
            Debug.LogError("PathTracking: User reference is not assigned.");
            return;
        }

        _startTime = Time.time;
        _lastPosition = _user.position;
        _totalDistanceTraveled = 0f;

        _wrongPathCount = 0;
        _correctPathCount = 0;

        _hasStarted = true;
        _isGoalReached = false;

        if (_enableDebugLogs)
        {
            Debug.Log("PathTracking: Tracking started.");
        }
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
        if (_isGoalReached || !_hasStarted)
            return;

        _isGoalReached = true;
        _hasStarted = false;

        float totalTime = Time.time - _startTime;

        TimeText         = totalTime.ToString("F2");
        WrongCountText   = _wrongPathCount.ToString();
        CorrectCountText = _correctPathCount.ToString();
        DistanceText     = _totalDistanceTraveled.ToString("F2");
        StopTimeText     = _playerIdleTracker != null ? _playerIdleTracker.ReturnTotalIdleTime() : "0";
        IdleTimeText     = _playerIdleTracker != null ? _playerIdleTracker.IdleTimeCount() : "0";

        if (_unityClients != null)
        {
            _unityClients.SendFinishRequest(
                TimeText,
                WrongCountText,
                CorrectCountText,
                DistanceText,
                StopTimeText,
                IdleTimeText
            );
        }
        else
        {
            Debug.LogWarning("PathTracking: UnityClients reference is missing. Finish request not sent.");
        }

        if (_enableDebugLogs)
        {
            Debug.Log("PathTracking: Goal reached.");
        }
    }

    public string GetJsonData()
    {
        float totalTime = Time.time - _startTime;

        var requestData = new RequestData
        {
            time       = totalTime.ToString("F2"),
            wrongCount = _wrongPathCount.ToString(),
            correctCount = _correctPathCount.ToString(),
            distance   = _totalDistanceTraveled.ToString("F2"),
            stopTime   = _playerIdleTracker != null ? _playerIdleTracker.ReturnTotalIdleTime() : "0",
            idleTime   = _playerIdleTracker != null ? _playerIdleTracker.IdleTimeCount() : "0"
        };

        return JsonUtility.ToJson(requestData);
    }

    #endregion

    #region Internal Logic

    private void UpdateElapsedTime()
    {
        if (!_enableDebugLogs)
            return;

        float elapsedTime = Time.time - _startTime;
        Debug.Log($"Elapsed time: {elapsedTime:F2} seconds");
    }

    private void UpdateDistance()
    {
        if (_user == null)
            return;

        float distanceTraveled = Vector3.Distance(_lastPosition, _user.position);

        if (distanceTraveled > _movementThreshold)
        {
            _totalDistanceTraveled += distanceTraveled;
            _lastPosition = _user.position;

            if (_enableDebugLogs)
            {
                Debug.Log($"Total distance traveled: {_totalDistanceTraveled:F2} meters");
            }
        }
    }

    #endregion

    [Serializable]
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
