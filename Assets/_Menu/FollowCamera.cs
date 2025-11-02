using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Camera currentCamera;
    [SerializeField] float rotationSpeed = 1f;

    bool isRotating;

    void Start()
    {
        if (currentCamera == null)
        {
            Debug.LogError("Camera not assigned in inspector");
        }
    }

    void Update()
    {
        Vector3 cameraPosition = currentCamera.transform.position;
        Vector3 toCamera = cameraPosition - transform.position;
        toCamera.y = 0f; // no vertical rotation
        toCamera = -toCamera; // compensar 180º
        Vector3 desiredDir = toCamera.normalized;

        float angle = Vector3.Angle(transform.forward, desiredDir);

        if (angle > 20f)
        {
            isRotating = true;
        }
        else if (angle < 5f)
        {
            isRotating = false;
        }

        if (isRotating)
        {
            Quaternion targetRotation = Quaternion.LookRotation(desiredDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime * 60f // para que sea más uniforme por frame
            );
        }
    }
}
