using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] CarController carController;
    MinigameSpawner minigameSpawner;
    PlayableDirector playableDirector;

    void Start()
    {
        minigameSpawner = GetComponent<MinigameSpawner>();
        playableDirector = GetComponent<PlayableDirector>();
    }

    void Awake()
    {
        if (carController == null)
        {
            Debug.LogError("carController not assigned in editor!");
        }
        if (carController != null)
        {
            carController.enabled = false;
        }
    }

    void Update()
    {
        double wait = playableDirector.playableAsset.duration;
        if (playableDirector != null && playableDirector.state == PlayState.Playing && playableDirector.time < 0.01)
        {
            StartCoroutine(WaitThenDo((float)wait));
        }

        IEnumerator WaitThenDo(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            
            
            carController.enabled = true;
            minigameSpawner.enabled = true;
        }
    }
}
