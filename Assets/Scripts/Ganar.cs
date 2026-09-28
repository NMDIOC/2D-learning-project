using UnityEngine;
using UnityEngine.SceneManagement;

public class Ganar : MonoBehaviour
{
    [SerializeField] private CheckpointDataSO activeCheckpoint;
    [SerializeField] private string nextScene;

    public void Update()
    {
        if (activeCheckpoint.currentCheckpointID == 3)
        {
            SceneManager.LoadScene(nextScene);
            activeCheckpoint.currentCheckpointID = 0;
        }
    }

    public void GAme()
    {
        SceneManager.LoadScene("Anivel");
    }

}
