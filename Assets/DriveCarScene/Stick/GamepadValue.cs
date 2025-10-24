using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadValue : MonoBehaviour
{
    public Vector2 value = Vector2.zero;
    [SerializeField] float deadZone = 0.3f;

    void Start()
    {

    }

    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            Debug.LogError("No gamepad connected");
            return;
        }

        // temp value to check deadzone before saving value
        var temp = gamepad.leftStick.ReadValue();

        // only save x value if greater than deadzone
        if (temp.x > deadZone || temp.x < -deadZone)
        {
            value = new Vector2(temp.x, temp.y);
        }
        else if (temp.x > 0.5 && temp.x < 0.8)
        {
            value = new Vector2(temp.x, 0.6f);
        }
        else if (temp.x < -0.5 && temp.x < -0.8)
        {
            value = new Vector2(temp.x, 0.6f);
        }
        else
        {
            value = new Vector2(0f, temp.y);
        }

        // DEBUGGING -------------------------------
        // if (temp.x > 0 || value.x < 0) {
        //     Debug.Log("value x " + value.x);
        // } else if (value.y > 0 || value.y < 0) {
        //     Debug.Log("value y " + value.y);
        // }
    }
}
