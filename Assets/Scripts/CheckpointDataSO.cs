using UnityEngine;

[CreateAssetMenu(fileName = "NewCheckpointData", menuName = "Game/Checkpoint Data")]
public class CheckpointDataSO : ScriptableObject
{
    public Vector3 spawnPosition;
    public int currentCheckpointID;

    public void SetCheckpoint(Vector3 position, int id)
    {
        spawnPosition = position;
        currentCheckpointID = id;
    }
}