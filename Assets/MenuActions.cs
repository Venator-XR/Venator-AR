using UnityEngine;

public class MenuActions : MonoBehaviour
{
    [SerializeField] SceneManager sceneManager;

    void Start()
    {
        if(sceneManager == null)
        {
            Debug.LogError("sceneManager not assigned in the inspector");
        }
    }
    
    public void ChangeSceneBtn(GameObject scene)
    {
        sceneManager.SetScene(scene);
    }
    
    public void ExitOnClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
