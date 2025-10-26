using UnityEngine;

public class ControladorModelos : MonoBehaviour
{
    public GameObject coche;
    public GameObject vampiro;
    public GameObject pistola;
    public GameObject linterna;

    private GameObject active;

    void Start()
    {
        ActivateModel(coche);
    }

    public void ActivateModel(GameObject modelo)
    {
        coche.SetActive(false);
        vampiro.SetActive(false);
        pistola.SetActive(false);
        linterna.SetActive(false);

        modelo.SetActive(true);
        active = modelo;
    }
}
