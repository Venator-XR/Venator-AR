using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] List<GameObject> scenes;

    void Start()
    {
        if (scenes.Count == 0)
        {
            Debug.LogError("no scenes assigned in the inspector");
        }
    }
    
    public void ChangeScene(GameObject scene)
    {
        if (!scenes.Contains(scene))
        {
            Debug.LogError("scene not included in the scenes list");
        }

        foreach (GameObject s in scenes)
        {
            s.SetActive(false);
        }

        scene.SetActive(true);
    }
}
