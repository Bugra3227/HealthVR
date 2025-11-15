using UnityEngine;

/// <summary>
/// Handles smooth VR locomotion using the OVRPlayerController's CharacterController.
/// Reads joystick input and applies movement with adjustable target speed.
/// </summary>
public class VRMovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private OVRPlayerController _playerController;

    [Header("Movement Settings")]
    [SerializeField] private float _targetSpeed = 5f;          // Target movement speed (m/s)
    [SerializeField] private float _speedAdjustmentTime = 0.1f; // Lerp smoothing time

    private CharacterController _characterController;
    private float _currentSpeed;
    private Vector3 _currentMovementDirection;

    private void Start()
    {
        // Get CharacterController from OVRPlayerController
        _characterController = _playerController.GetComponent<CharacterController>();

        // Start with the target speed
        _currentSpeed = _targetSpeed;
    }

    private void Update()
    {
        // Smoothly adjust speed to target speed
        _currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, _speedAdjustmentTime * Time.deltaTime);

        // Read joystick direction from the primary thumbstick
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        _currentMovementDirection = new Vector3(input.x, 0f, input.y);

        // Build movement vector
        Vector3 move = _currentMovementDirection * (_currentSpeed * Time.deltaTime);

        // Apply movement through CharacterController
        _characterController.Move(move);
    }
}
