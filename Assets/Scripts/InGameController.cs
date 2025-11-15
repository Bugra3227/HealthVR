using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Controls the in-game countdown, experiment initialization and 
/// starts the VR movement tracking when the session begins.
/// </summary>
public class InGameController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PathTracking _pathTracking;
    [SerializeField] private TextMeshProUGUI _startTimeText;
    [SerializeField] private Transform _gameCanvas;
    [SerializeField] private OVRPlayerController _ovrPlayerController;
    [SerializeField] private NetworkStarter _networkStarter;

    [Header("Experiment Settings")]
    [SerializeField] private List<MeshRenderer> _keywordsMeshRenderers = new List<MeshRenderer>();
    [SerializeField] private List<Material> _keywordsMaterials = new List<Material>();

    private float _countdownTime = 4f;
    private bool _startButtonPressed;
    private int _experimentIndex;

    private float _initialAcceleration;

    private void Start()
    {
        // Store original acceleration and disable movement until countdown is finished
        _initialAcceleration = _ovrPlayerController.Acceleration;
        _ovrPlayerController.Acceleration = 0f;

        // Get experiment index from network starter
        _experimentIndex = int.Parse(NetworkStarter.instance.experimentIndex);

        InitializeExperimentState(_experimentIndex);
    }

    /// <summary>
    /// Called by UI start button to trigger the countdown.
    /// </summary>
    public void StartGameBtn()
    {
        if (_startButtonPressed)
            return;

        _startButtonPressed = true;
    }

    private void Update()
    {
        if (!_startButtonPressed)
            return;

        _countdownTime -= Time.deltaTime;
        _startTimeText.text = _countdownTime.ToString("0");

        if (_countdownTime < 0.1f)
        {
            _gameCanvas.gameObject.SetActive(false);
            _startButtonPressed = false;
            _countdownTime = 4f;

            // Start path tracking session
            _pathTracking.StartTracking();   // Eğer method adın hâlâ StartTime ise: _pathTracking.StartTime();

            // Re-enable player movement
            _ovrPlayerController.Acceleration = _initialAcceleration;
        }
    }

    /// <summary>
    /// Applies the selected experiment material to all keyword mesh renderers.
    /// </summary>
    private void HandleRandomMaterialOfPlanes()
    {
        if (_experimentIndex < 0 || _experimentIndex >= _keywordsMaterials.Count)
        {
            Debug.LogWarning("InGameController: experiment index is out of range for materials list.");
            return;
        }

        Material selectedMaterial = _keywordsMaterials[_experimentIndex];

        for (int i = 0; i < _keywordsMeshRenderers.Count; i++)
        {
            _keywordsMeshRenderers[i].material = selectedMaterial;
        }
    }

    /// <summary>
    /// Sets experiment index and initializes the visual state.
    /// </summary>
    private void InitializeExperimentState(int index)
    {
        _experimentIndex = index;
        HandleRandomMaterialOfPlanes();
    }
}
