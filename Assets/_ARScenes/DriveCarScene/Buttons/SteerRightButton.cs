using UnityEngine;
using UnityEngine.EventSystems;

public class SteerRightButton : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.D))
        {
            OnPress();
            carController.SteerRight();
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            OnRelease();
            carController.SteerReset();
        }
    }

    public void OnPress()
    {
        carController.SteerRight();
    }

    public void OnRelease()
    {
        carController.SteerReset();
    }
}
