using UnityEngine;

public class ControladorModelos : MonoBehaviour
{
    public GameObject coche;
    public GameObject vampiro;
    public GameObject pistola;
    public GameObject castillo;

    private GameObject activo;

    void Start()
    {
        MostrarModelo(coche); // Ä¬ÈÏÏÔÊ¾Æû³µ
    }

    public void MostrarModelo(GameObject modelo)
    {
        coche.SetActive(false);
        vampiro.SetActive(false);
        pistola.SetActive(false);
        castillo.SetActive(false);

        modelo.SetActive(true);
        activo = modelo;
    }
}
