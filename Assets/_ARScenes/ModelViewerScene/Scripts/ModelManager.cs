using UnityEngine;
using UnityEngine.UI;

public class ControladorModelos : MonoBehaviour
{
    public GameObject coche;
    public GameObject vampiro;
    public GameObject pistola;
    public GameObject linterna;

    private GameObject active;


    [SerializeField] Slider rotateSlider;
    [SerializeField] Slider scaleSlider;
    RotateOrScale rotateOrScale;


    void Start()
    {
        ActivateModel(coche);
        rotateOrScale = active.GetComponent<RotateOrScale>();
    }

    public void ActivateModel(GameObject modelo)
    {
        coche.SetActive(false);
        vampiro.SetActive(false);
        pistola.SetActive(false);
        linterna.SetActive(false);
        
        rotateSlider.value = 0;
        scaleSlider.value = 1;

        modelo.SetActive(true);
        active = modelo;
        rotateOrScale = active.GetComponent<RotateOrScale>();
    }

    public void RotateModel(float value)
    {

        if (active == coche || active == linterna)
        {
            value += 180;
        }
        else if (active == pistola)
        {
            value -= 90;
        }

        rotateOrScale.SliderRotate(value);
    }
    
    public void ScaleModel(float value)
    {
        rotateOrScale.SliderScale(value);
    }
}
