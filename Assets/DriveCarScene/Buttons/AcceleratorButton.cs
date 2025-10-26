using UnityEngine;
using UnityEngine.EventSystems;

public class AcceleratorButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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
            OnPointerDown(null);
            carController.Accelerate();
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            OnPointerUp(null);
            carController.AccelReset();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        carController.Accelerate();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        carController.AccelReset();
    }
}
