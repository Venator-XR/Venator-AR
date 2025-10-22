using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            Debug.LogError("No gamepad connected");
            return;
        }

        Vector2 move = gamepad.leftStick.ReadValue();
        if (move.x > 0 || move.x < 0) {
            Debug.Log("move x " + move.x);
        } else if (move.y > 0 || move.y < 0) {
            Debug.Log("move y " + move.y);
        }
    }
}   
