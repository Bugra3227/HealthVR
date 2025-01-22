using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;

public class InGameController : MonoBehaviour
{
    [SerializeField] private PathTracking pathTracking;
    [SerializeField] private TextMeshProUGUI startTimeText;
    [SerializeField] private Transform gameCanvas;
    [SerializeField] private List<MeshRenderer> keywordsMeshRenderers = new List<MeshRenderer>();
    [SerializeField] private List<Material> keywordsMaterials = new List<Material>();
    [SerializeField] private OVRPlayerController ovrPlayerController;
    private float _time = 4f;
    private bool _isStartBtn;
    private int _indexExperiment;

    private HttpListener _httpListener;
    private float _firstAcceleration;
    private void Start()
    {
        OVRManager.display.RecenterPose();
        _firstAcceleration = ovrPlayerController.Acceleration;
        ovrPlayerController.Acceleration = 0f;
        _indexExperiment = int.Parse(NetworkStarter.instance.experimentIndex);
        InitializeExperimentState(_indexExperiment);
    }

    public void StartGameBtn()
    {
        if (_isStartBtn)
            return;
        _isStartBtn = true;
    }

    private void Update()
    {
        if (!_isStartBtn)
            return;

        _time -= Time.deltaTime;
        startTimeText.text = _time.ToString("0");

        if (_time < 0.1f)
        {
            gameCanvas.gameObject.SetActive(false);
            _isStartBtn = false;
            _time = 4f;
            pathTracking.StartTime();
            ovrPlayerController.Acceleration = _firstAcceleration;
        }
    }

    private void HandleRandomMaterialOfPlanes()
    {
        Material selectedMaterial = keywordsMaterials[_indexExperiment];

        for (int i = 0; i < keywordsMeshRenderers.Count; i++)
        {
            keywordsMeshRenderers[i].material = selectedMaterial;
        }
    }

    private void InitializeExperimentState(int index)
    {
        _indexExperiment = index;
        HandleRandomMaterialOfPlanes();
    }
}