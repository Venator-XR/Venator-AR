using UnityEngine;
using UnityEngine.EventSystems;

public class BrakeButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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
            OnPointerDown(null);
            carController.Brake();
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            OnPointerUp(null);
            carController.AccelReset();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        carController.Brake();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        carController.AccelReset();
    }
}
