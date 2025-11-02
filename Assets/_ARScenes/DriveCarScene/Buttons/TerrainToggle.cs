using UnityEngine;

public class TerrainToggle : MonoBehaviour
{
    bool activeTerrain = true;
    [SerializeField] GameObject terrain;

    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("terrain not assigned in inspector");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    public void ToggleOnClick()
    {
        activeTerrain = !activeTerrain;
        terrain.SetActive(activeTerrain);
    }
}
