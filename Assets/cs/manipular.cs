using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manipular : MonoBehaviour
{
    // ===== 可调参数 =====
    public float rotSpeedTouch = 4f;     // 手机上单指旋转速度
    public float rotSpeedMouse = 4f;     // 电脑里鼠标拖拽旋转速度

    public float scrollStep = 1f;        // 鼠标滚轮每次缩放多少单位（线性加减）
                                         // 数值越大，缩放越快。先用1，觉得慢就改大一点

    public float minScaleFactor = 0.5f;  // 最小缩放 = 初始大小 * 这个系数
    public float maxScaleFactor = 3f;    // 最大缩放 = 初始大小 * 这个系数

    // ===== 触摸缩放用（双指）=====
    private float initialDistance;
    private float baseScale;             // 物体初始大小（用x轴即可，假设等比缩放）
    private Vector3 initialScaleTouch;   // pinch开始时的scale

    // ===== 鼠标拖拽用 =====
    private bool mouseDragging = false;
    private Vector3 lastMousePos;
    private bool mouseIsOverThis = false;


    void Start()
    {
        // 记录物体一开始的大小（假设是等比缩放，比如 (30,30,30)）
        baseScale = transform.localScale.x;
    }

    void Update()
    {
        // ========== 1. 触摸交互（真机 / 手机） ==========
        if (Input.touchCount > 0)
        {
            if (objIsTouched_Touch())
            {
                // 1.1 单指旋转
                if (Input.touchCount == 1)
                {
                    Touch t0 = Input.GetTouch(0);

                    if (t0.phase == TouchPhase.Moved)
                    {
                        transform.Rotate(
                            t0.deltaPosition.y * rotSpeedTouch,
                            -t0.deltaPosition.x * rotSpeedTouch,
                            0,
                            Space.World
                        );
                    }
                }

                // 1.2 双指缩放（pinch）
                if (Input.touchCount == 2)
                {
                    Touch t0 = Input.GetTouch(0);
                    Touch t1 = Input.GetTouch(1);

                    // 刚开始捏的时候，记录起点
                    if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
                    {
                        initialDistance = Vector2.Distance(t0.position, t1.position);
                        initialScaleTouch = transform.localScale;
                    }
                    else
                    {
                        // 两个手指都还在屏幕
                        if (
                            t0.phase != TouchPhase.Ended && t0.phase != TouchPhase.Canceled &&
                            t1.phase != TouchPhase.Ended && t1.phase != TouchPhase.Canceled
                        )
                        {
                            float currentDistance = Vector2.Distance(t0.position, t1.position);
                            if (!Mathf.Approximately(initialDistance, 0f))
                            {
                                // 计算缩放比例
                                float factor = currentDistance / initialDistance;

                                // 按这个比例放大/缩小原本的大小
                                float targetSize = initialScaleTouch.x * factor;

                                // 限制范围（基于初始大小baseScale的动态上下限）
                                targetSize = ClampSize(targetSize);

                                // 应用回去（等比缩放）
                                transform.localScale = new Vector3(targetSize, targetSize, targetSize);
                            }
                        }
                    }
                }
            }

            // 有触摸时我们就不执行下面的鼠标逻辑
            return;
        }


        // ========== 2. 鼠标交互（在编辑器Play / PC） ==========

        // 2.1 鼠标是否指向这个物体（用于决定可不可以开始操作它）
        mouseIsOverThis = IsMouseOverThisObject();

        // 2.2 鼠标左键按下 → 开始拖拽旋转
        if (mouseIsOverThis && Input.GetMouseButtonDown(0))
        {
            mouseDragging = true;
            lastMousePos = Input.mousePosition;
        }

        // 2.3 正在拖拽 → 旋转
        if (mouseDragging && Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;

            transform.Rotate(
                delta.y * rotSpeedMouse,
                -delta.x * rotSpeedMouse,
                0,
                Space.World
            );
        }

        // 2.4 松开左键 → 停止拖拽
        if (Input.GetMouseButtonUp(0))
        {
            mouseDragging = false;
        }

        // 2.5 鼠标滚轮缩放（线性 + 基于初始大小的动态限制）
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // 上滚>0, 下滚<0
        if ((mouseIsOverThis || mouseDragging) && Mathf.Abs(scroll) > 0.0001f)
        {
            // 当前大小（假设等比缩放）
            float currentSize = transform.localScale.x;

            // 根据滚轮方向加或减固定步长
            float targetSize = currentSize + scroll * scrollStep;

            // 限制范围：最小=baseScale * minScaleFactor，最大=baseScale * maxScaleFactor
            targetSize = ClampSize(targetSize);

            // 回写成均匀缩放
            transform.localScale = new Vector3(targetSize, targetSize, targetSize);
        }
    }


    // ===== 把目标大小夹在允许范围内 =====
    private float ClampSize(float sizeIn)
    {
        float minAllowed = baseScale * minScaleFactor;
        float maxAllowed = baseScale * maxScaleFactor;
        return Mathf.Clamp(sizeIn, minAllowed, maxAllowed);
    }

    // ===== 触摸射线检测：手指是否点到这个物体 =====
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

    // ===== 鼠标射线检测：鼠标是否指在这个物体上 =====
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
}
