using UnityEngine;
using UnityEngine.EventSystems;

public class BrakeButton : MonoBehaviour
{
    CarController carController;

    void Start()
    {
        carController = GetComponentInParent<CarController>();
        if (carController == null)
        {
            Debug.Log("CarController not found");
        }
    }

    // FOR KEYBOARD
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            OnPress();
            carController.Brake();
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            OnRelease();
            carController.AccelReset();
        }
    }

    public void OnPress()
    {
        carController.Brake();
    }

    public void OnRelease()
    {
        carController.AccelReset();
    }
}
