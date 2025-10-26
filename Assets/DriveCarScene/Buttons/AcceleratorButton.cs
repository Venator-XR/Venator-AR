using UnityEngine;
using UnityEngine.EventSystems;

public class AcceleratorButton : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnPress();
            carController.Accelerate();
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            OnRelease();
            carController.AccelReset();
        }
    }

    public void OnPress()
    {
        carController.Accelerate();
    }

    public void OnRelease()
    {
        carController.AccelReset();
    }
}
