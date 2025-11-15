using UnityEngine;

/// <summary>
/// Tracks how long the player stays idle (not moving) during the VR session.
/// Counts both total idle time and the number of idle events longer than 3 seconds.
/// </summary>
public class PlayerIdleTracker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PathTracking _pathTracking;
    [SerializeField] private Transform _playerHead;  // Usually CenterEyeAnchor in VR

    [Header("Settings")]
    [SerializeField] private float _idleThreshold = 0.01f; // Movement threshold

    // Internal state
    private Vector3 _lastPosition;
    private float _currentIdleTime = 0f;
    private float _totalIdleTime = 0f;
    private bool _isIdle = false;
    private int _idleEventCount = 0;

    private void Start()
    {
        _lastPosition = _playerHead.position;
    }

    private void Update()
    {
        if (!_pathTracking.HasStarted)
            return;

        Vector3 currentPosition = _playerHead.position;
        float movement = Vector3.Distance(currentPosition, _lastPosition);

        // Player is idle (not moving)
        if (movement < _idleThreshold)
        {
            if (!_isIdle)
            {
                _isIdle = true;
                // Debug.Log("Player started idling.");
            }

            _currentIdleTime += Time.deltaTime;
        }
        else
        {
            // Player stops idling
            if (_isIdle)
            {
                _isIdle = false;

                _totalIdleTime += _currentIdleTime;

                // Count idle events longer than 3 seconds
                if (_currentIdleTime >= 3f)
                    _idleEventCount++;

                // Debug.Log($"Idle ended. Duration: {_currentIdleTime:F2} seconds");
            }

            _currentIdleTime = 0f;
        }

        _lastPosition = currentPosition;
    }

    /// <summary>
    /// Returns the total amount of time the player stayed idle (in seconds).
    /// </summary>
    public string ReturnTotalIdleTime()
    {
        return _totalIdleTime.ToString("F2");
    }

    /// <summary>
    /// Returns how many idle events were longer than 3 seconds.
    /// </summary>
    public string IdleTimeCount()
    {
        return _idleEventCount.ToString();
    }
}
