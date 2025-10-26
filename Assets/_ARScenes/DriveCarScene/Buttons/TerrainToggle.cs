using UnityEngine;

public class TerrainToggle : MonoBehaviour
{
    bool activeTerrain = true;
    [SerializeField] GameObject terrain;

    void Start()
    {
        if(terrain == null)
        {
            Debug.LogError("terrain not assigned in inspector");
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    public void ToggleOnClick()
    {
        activeTerrain = !activeTerrain;
        terrain.SetActive(activeTerrain);
    }
}
