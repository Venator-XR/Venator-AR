using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Camera currentCamera;

    void Start()
    {
        if (currentCamera == null)
        {
            Debug.LogError("camera not assigned in inspector");
        }
    }

    void Update()
    {
        Vector3 cameraPosition = currentCamera.transform.position;

        // direction to camera
        Vector3 toCamera = cameraPosition - transform.position;
        toCamera.y = 0f; // no vertical rotation
        
        // compensate because object is already rotated 180deg sideways
        toCamera = -toCamera;

        if (toCamera.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(toCamera.normalized, Vector3.up);
        }
    }
}
