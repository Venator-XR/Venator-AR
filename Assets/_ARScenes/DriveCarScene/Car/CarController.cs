using UnityEngine;
using UnityEngine.UIElements;

public class CarController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] Light brakeLights;

    [Header("Forces")]
    [SerializeField] float brakeForce = 0.4f;
    [SerializeField] float speed = 1f;
    [SerializeField] float steerForce = 1f;

    float acceleration = 0f;
    float steering = 0f;

    [SerializeField] Transform resetPosition;

    // >>> NEW >>> 自动行驶（相机中心 Raycast）与静止阈值
    [Header("Auto Drive (Raycast)")]
    [SerializeField] bool autoDrive = false;            // 勾选启用
    [SerializeField] Camera cam;                        // 主相机/ARCamera
    [SerializeField] LayerMask planeMask;               // 只勾透明平面层
    [SerializeField] float maxRayDistance = 1000f;
    [SerializeField] float chassisHeight = 0.2f;
    [SerializeField] float arriveDistance = 0.2f;       // 到点阈值（原有靠近时减速）
    [SerializeField] float steerResponse = 1.0f;
    [SerializeField] float cruiseSpeedThreshold = 1.2f;

    // —— 新增“静止阈值”（避免抖动/原地小动作）——
    [SerializeField] float deadZoneDistance = 0.75f;    // 距离小于此值则完全不动
    [SerializeField] float deadZoneAngleDeg = 5f;       // 角度小于此值则不转向
    [SerializeField] float deadZoneDamping = 0.5f;     // 进入静止区时额外阻尼
    // <<< NEW <<<

    Vector3 targetPosition; // 命中点
    bool hasTarget = false;

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

    // >>> NEW >>> 仅新增的 Update，不改原有方法
    void Update()
    {
        if (!autoDrive || cam == null) return;

        // 从屏幕中心发射射线
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, planeMask))
        {
            targetPosition = hit.point;
            targetPosition.y = chassisHeight;
            hasTarget = true;

            Debug.DrawLine(ray.origin, hit.point, Color.green);
        }
        else
        {
            hasTarget = false;
        }

        if (!hasTarget)
        {
            acceleration = 0f;
            steering = 0f;
            return;
        }

        // 将命中点转换成 acceleration / steering
        Vector3 pos = transform.position;
        Vector3 to = targetPosition - pos;
        to.y = 0f;

        float dist = to.magnitude;

        // 计算有符号夹角（用于转向）
        Vector3 forward = transform.forward; forward.y = 0f; forward.Normalize();
        Vector3 dir = (dist > 0.0001f) ? to.normalized : forward;
        float angle = Vector3.SignedAngle(forward, dir, Vector3.up);

        // —— 核心：静止阈值 —— 距离很小且角度很小 → 完全不动，且增加阻尼让它稳住
        if (dist <= deadZoneDistance && Mathf.Abs(angle) <= deadZoneAngleDeg)
        {
            acceleration = 0f;
            steering = 0f;
            if (brakeLights) brakeLights.intensity = 0.8f;

            // 轻微刹停/阻尼，避免小幅漂移
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, deadZoneDamping * Time.deltaTime);
            return;
        }

        // —— 非静止区：正常转向与油门 —— 
        if (dist > 0.0001f)
        {
            float steerCmd = Mathf.Clamp(
                angle / (30f / Mathf.Max(0.01f, steerResponse)),
                -1f, 1f
            );
            steering = steerCmd;
        }
        else
        {
            steering = 0f;
        }

        float currentSpeed = rb.velocity.magnitude;
        if (dist > arriveDistance * 2f)
        {
            acceleration = (currentSpeed > cruiseSpeedThreshold) ? 0.3f : 1f;
            if (brakeLights) brakeLights.intensity = 0.1f;
        }
        else if (dist > arriveDistance)
        {
            acceleration = 0.2f;
            if (brakeLights) brakeLights.intensity = 0.1f;
        }
        else
        {
            acceleration = 0f; // 到点不再加速
            if (brakeLights) brakeLights.intensity = 0.8f;
        }
    }
    // <<< NEW <<<

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
        if (brakeLights) brakeLights.intensity = 0.1f;
    }

    public void Brake()
    {
        acceleration = -1;
        if (brakeLights) brakeLights.intensity = 1;
        Debug.Log("Brake()");
    }

    public void AccelReset()
    {
        acceleration = 0;
        if (brakeLights) brakeLights.intensity = 0.1f;
    }

    public void PositionReset()
    {
        transform.position = resetPosition.position;
        transform.rotation = resetPosition.rotation;
        rb.Sleep();
    }
}
