using UnityEngine;
using UnityEngine.UIElements;

public class CarController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] Light brakeLights;

    [SerializeField] float brakeForce = 0.4f;
    [SerializeField] float speed = 1f;
    [SerializeField] float steerForce = 1f;

    float acceleration = 0f;
    float steering = 0f;

    [SerializeField] Transform resetPosition;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on the GameObject");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        if (brakeLights == null)
        {
            Debug.LogError("brakeLights not assigned in the inspector");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        if (resetPosition == null)
        {
            Debug.LogError("resetPosition not assigned in the inspector");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    void FixedUpdate()
    {
        Move(acceleration, steering);

        if (acceleration == 0)
        {
            Vector3 accumulated = rb.GetAccumulatedForce();
            rb.AddRelativeForce(-accumulated.z, 0f, accumulated.z);
        }
    }

    void Move(float accel, float steer)
    {
        if (accel > 0)
        {
            rb.AddRelativeForce(0f, 0f, accel * speed);
            // brakeLights.intensity = 1;
        }
        else if (accel < 0)
        {
            rb.AddRelativeForce(0f, 0f, accel * brakeForce);
            // brakeLights.intensity = 5;
        }

        rb.AddRelativeTorque(0f, steer * steerForce, 0f);
    }

    public void SteerRight()
    {
        steering = 1;
        Debug.Log("SteerRight()");
    }

    public void SteerLeft()
    {
        steering = -1;
        Debug.Log("SteerLeft()");

    }

    public void SteerReset()
    {
        steering = 0;
    }

    public void Accelerate()
    {
        acceleration = 1;
        Debug.Log("Accelerate()");
        brakeLights.intensity = 0.1f;
    }

    public void Brake()
    {
        acceleration = -1;
        brakeLights.intensity = 1;
        Debug.Log("Brake()");
    }

    public void AccelReset()
    {
        acceleration = 0;
        brakeLights.intensity = 0.1f;
    }

    public void PositionReset()
    {
        transform.position = resetPosition.position;
        transform.rotation = resetPosition.rotation;
        rb.Sleep();
    }
}
