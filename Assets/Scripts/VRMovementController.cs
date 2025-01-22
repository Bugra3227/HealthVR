using UnityEngine;

public class VRMovementController : MonoBehaviour
{
    public OVRPlayerController playerController;  // OVRPlayerController referansı
    public float targetSpeed = 5f; // Hedef hız (metre/saniye)
    public float speedAdjustmentTime = 0.1f; // Hızın yumuşak geçiş süresi

    private CharacterController characterController; // Karakterin hareketini kontrol etmek için
    private float currentSpeed; // Mevcut hız
    private Vector3 currentMovementDirection; // Hareket yönü

    void Start()
    {
        // OVRPlayerController'dan CharacterController'ı alıyoruz
        characterController = playerController.GetComponent<CharacterController>();

        // Başlangıçta mevcut hızı hedef hız olarak ayarlıyoruz
        currentSpeed = targetSpeed;
    }

    void Update()
    {
        // Hedef hıza yumuşak geçiş yapıyoruz
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, speedAdjustmentTime * Time.deltaTime);
        
        // Hareket yönünü joystick'ten alıyoruz
        Vector2 joystickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        currentMovementDirection = new Vector3(joystickInput.x, 0, joystickInput.y);

        // Hareketi, hızla birleştirerek karakteri hareket ettiriyoruz
        Vector3 move = currentMovementDirection * (currentSpeed * Time.deltaTime);

        // CharacterController üzerinden hareketi uygula
        characterController.Move(move);
    }
}