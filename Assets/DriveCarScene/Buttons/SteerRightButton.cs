using UnityEngine;
using UnityEngine.EventSystems;

public class SteerRightButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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
            OnPointerDown(null);
            carController.SteerRight();
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            OnPointerUp(null);
            carController.SteerReset();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        carController.SteerRight();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        carController.SteerReset();
    }
}
