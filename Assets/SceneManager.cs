using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] List<GameObject> scenes;
    [SerializeField] GameObject mainMenuScene;
    [SerializeField] GameObject menuBtnCanvas;

    void Start()
    {
        if (scenes.Count == 0)
        {
            Debug.LogError("no scenes assigned in the inspector");
        }
    }

    public void SetScene(GameObject scene)
    {
        if (!scenes.Contains(scene))
        {
            Debug.LogError("scene not included in the scenes list");
        }

        foreach (GameObject s in scenes)
        {
            s.SetActive(false);
        }

        mainMenuScene.SetActive(false);
        scene.SetActive(true);
        menuBtnCanvas.SetActive(true);
    }

    public void BackToMenu()
    {
        menuBtnCanvas.SetActive(false);
        foreach (GameObject s in scenes)
        {
            s.SetActive(false);
        }
        mainMenuScene.SetActive(true);
    }
}
