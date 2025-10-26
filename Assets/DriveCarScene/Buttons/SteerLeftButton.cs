using UnityEngine;
using UnityEngine.EventSystems;

public class SteerLeftButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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
            OnPointerDown(null);
            carController.SteerLeft();
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            OnPointerUp(null);
            carController.SteerReset();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        carController.SteerLeft();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        carController.SteerReset();
    }
}
