using UnityEngine;
using UnityEngine.EventSystems;

public class SteerLeftButton : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnPress();
            carController.SteerLeft();
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            OnRelease();
            carController.SteerReset();
        }
    }

    public void OnPress()
    {
        carController.SteerLeft();
    }

    public void OnRelease()
    {
        carController.SteerReset();
    }
}
