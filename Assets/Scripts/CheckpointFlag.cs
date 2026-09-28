using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CheckpointFlag : MonoBehaviour
{
    [Header("Checkpoint Config")]
    [SerializeField] private CheckpointDataSO checkpointData;
    [SerializeField] private GameObject activePS;
    [SerializeField] private int checkpointID;

    private void Start()
    {
        activePS.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jugador") || other.GetComponent<PlayerLife>() != null)
        {
            if (checkpointData != null)
            {
                // Guarda la posición exacta del jugador al tocar la bandera
                checkpointData.SetCheckpoint(other.transform.position, checkpointID);
                activePS.SetActive(true);
                Debug.Log($"<color=green>[Checkpoint {checkpointID}] Guardado en Vector3: {other.transform.position}</color>");

            }
            else
            {
                Debug.LogError($"[CheckpointFlag] Falta asignar el asset 'CheckpointDataSO' en {gameObject.name}");
            }
        }
    }
}