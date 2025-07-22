using UnityEngine;

[DisallowMultipleComponent]
public class PlayerCameraPosition : MonoBehaviour
{
    public Transform CameraPositionGameObject;

    private void Update()
    {
        transform.position = CameraPositionGameObject.position;
    }
}
