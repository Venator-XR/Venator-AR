using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOrScale : MonoBehaviour
{
    public float rotSpeedTouch = 4f;
    public float rotSpeedMouse = 4f;

    public float scrollStep = 1f;
    public float minScaleFactor = 0.5f;
    public float maxScaleFactor = 3f;

    private float initialDistance;
    private float baseScale;
    private Vector3 initialScaleTouch;

    private bool mouseDragging = false;
    private Vector3 lastMousePos;
    private bool mouseIsOverThis = false;
    private Quaternion baseRotation;

    void Start()
    {
        baseScale = transform.localScale.x;
        baseRotation = Quaternion.Euler(-90f, 0f, 0f);
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            if (objIsTouched_Touch())
            {
                if (Input.touchCount == 1)
                {
                    Touch t0 = Input.GetTouch(0);

                    if (t0.phase == TouchPhase.Moved)
                    {
                        float rotZ = -t0.deltaPosition.x * rotSpeedTouch; // horizontal drag -> z-rotation
                        transform.Rotate(0f, rotZ, 0f, Space.World);
                    }
                }

                if (Input.touchCount == 2)
                {
                    Touch t0 = Input.GetTouch(0);
                    Touch t1 = Input.GetTouch(1);

                    if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
                    {
                        initialDistance = Vector2.Distance(t0.position, t1.position);
                        initialScaleTouch = transform.localScale;
                    }
                    else
                    {
                        if (
                            t0.phase != TouchPhase.Ended && t0.phase != TouchPhase.Canceled &&
                            t1.phase != TouchPhase.Ended && t1.phase != TouchPhase.Canceled
                        )
                        {
                            float currentDistance = Vector2.Distance(t0.position, t1.position);
                            if (!Mathf.Approximately(initialDistance, 0f))
                            {
                                float factor = currentDistance / initialDistance;

                                float targetSize = initialScaleTouch.x * factor;

                                targetSize = ClampSize(targetSize);

                                transform.localScale = new Vector3(targetSize, targetSize, targetSize);
                            }
                        }
                    }
                }
            }

            return;
        }

        mouseIsOverThis = IsMouseOverThisObject();

        if (mouseIsOverThis && Input.GetMouseButtonDown(0))
        {
            mouseDragging = true;
            lastMousePos = Input.mousePosition;
        }

        if (mouseDragging && Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;

            float rotZ = -delta.x * rotSpeedMouse; // horizontal drag -> z-rotation
            transform.Rotate(0f, rotZ, 0f, Space.World);
        }

        if (Input.GetMouseButtonUp(0))
        {
            mouseDragging = false;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if ((mouseIsOverThis || mouseDragging) && Mathf.Abs(scroll) > 0.0001f)
        {
            float currentSize = transform.localScale.x;

            float targetSize = currentSize + scroll * scrollStep;

            targetSize = ClampSize(targetSize);

            transform.localScale = new Vector3(targetSize, targetSize, targetSize);
        }
    }

    private float ClampSize(float sizeIn)
    {
        float minAllowed = baseScale * minScaleFactor;
        float maxAllowed = baseScale * maxScaleFactor;
        return Mathf.Clamp(sizeIn, minAllowed, maxAllowed);
    }

    private bool objIsTouched_Touch()
    {
        foreach (Touch t in Input.touches)
        {
            Ray ray = Camera.main.ScreenPointToRay(t.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform == this.transform)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsMouseOverThisObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.transform == this.transform)
            {
                return true;
            }
        }
        return false;
    }

    public void SliderRotate(float value)
    {
        transform.rotation = Quaternion.Euler(-90f, 0f, value);
    }

    public void SliderScale(float value)
    {
        float scale = Mathf.Lerp(baseScale * minScaleFactor, baseScale * maxScaleFactor, value / 2f);
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
