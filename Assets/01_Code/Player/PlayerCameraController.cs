using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Look")]
    [SerializeField] private InputHelper inputHelper;
    [SerializeField] private float lookSensitivity = 0.1f;
    [SerializeField] public float clampRange = 90f;
    private float xRotation;
    private float yRotation;
    public Transform orientation;

    private void Update()
    {
        Rotation();
    }
    private void Rotation()
    {
        float lookX = inputHelper.LookInput.x * lookSensitivity;
        float lookY = inputHelper.LookInput.y * lookSensitivity;

        yRotation += lookX;
        xRotation -= lookY;

        xRotation = Mathf.Clamp(xRotation, -clampRange, clampRange);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

    }

}
