using System;
using UnityEngine;

/// <summary>
/// Tracks the player's movement and path quality in the VR color-following experiment.
/// Collects metrics such as time, distance, wrong/correct path decisions and idle durations,
/// and sends them to an external client at the end of the session.
/// </summary>
public class PathTracking : MonoBehaviour
{
    /// <summary>
    /// Optional singleton reference for easy access from other components.
    /// </summary>
    public static PathTracking Instance { get; private set; }
    
    /// <summary>
    /// True while tracking is active and the session has started.
    /// </summary>
    public bool HasStarted => _hasStarted;
    
    /// <summary>
    /// True after the goal has been reached and tracking has stopped.
    /// </summary>
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

    // State fields
    private float _startTime;
    private bool _hasStarted;
    private bool _isGoalReached;

    private int _wrongPathCount;
    private int _correctPathCount;

    private Vector3 _lastPosition;
    private float _totalDistanceTraveled;

    // Read-only result values (for UI, logging or external usage)
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
        // If path correctness logic is added in the future,
        // this is where correct/wrong path counters can be updated.
    }

    #endregion

    #region Public API

    /// <summary>
    /// Initializes and starts tracking the player's movement for the current session.
    /// Resets time, distance and path counters.
    /// </summary>
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

    /// <summary>
    /// Increases the wrong path counter. 
    /// Call this when the user follows an incorrect color or direction.
    /// </summary>
    public void IncreaseWrongPathCount()
    {
        _wrongPathCount++;
    }

    /// <summary>
    /// Increases the correct path counter.
    /// Call this when the user makes a correct decision on the path.
    /// </summary>
    public void IncreaseCorrectPathCount()
    {
        _correctPathCount++;
    }

    /// <summary>
    /// Stops tracking and finalizes the session when the goal is reached.
    /// Calculates all metrics and sends them to the UnityClients endpoint.
    /// </summary>
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

    /// <summary>
    /// Builds a JSON payload containing the current session metrics.
    /// Can be used for logging, debugging or sending data to external endpoints.
    /// </summary>
    public string GetJsonData()
    {
        float totalTime = Time.time - _startTime;

        var requestData = new RequestData
        {
            time         = totalTime.ToString("F2"),
            wrongCount   = _wrongPathCount.ToString(),
            correctCount = _correctPathCount.ToString(),
            distance     = _totalDistanceTraveled.ToString("F2"),
            stopTime     = _playerIdleTracker != null ? _playerIdleTracker.ReturnTotalIdleTime() : "0",
            idleTime     = _playerIdleTracker != null ? _playerIdleTracker.IdleTimeCount() : "0"
        };

        return JsonUtility.ToJson(requestData);
    }

    #endregion

    #region Internal Logic

    /// <summary>
    /// Logs elapsed time while tracking is active (only when debug logs are enabled).
    /// </summary>
    private void UpdateElapsedTime()
    {
        if (!_enableDebugLogs)
            return;

        float elapsedTime = Time.time - _startTime;
        Debug.Log($"Elapsed time: {elapsedTime:F2} seconds");
    }

    /// <summary>
    /// Accumulates the distance traveled by the user if the movement 
    /// exceeds the defined movement threshold.
    /// </summary>
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
