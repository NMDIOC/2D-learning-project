using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    [Header("Life Settings")]
    [SerializeField] private int maxLives = 3;
    [SerializeField] private CheckpointDataSO activeCheckpoint;

    private int currentLives;
    private Rigidbody2D rb;

    public int CurrentLives => currentLives;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentLives = maxLives;
    }

    private void Start()
    {
        if (activeCheckpoint != null)
        {
            if (activeCheckpoint.spawnPosition == Vector3.zero)
            {
                activeCheckpoint.SetCheckpoint(transform.position, 0);
            }
            else
            {
                TeleportToCheckpoint();
            }
        }
    }

    public void TakeDamage(int damage = 1)
    {
        currentLives -= damage;

        if (currentLives <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Respawn();
    }

    private void Respawn()
    {
        currentLives = maxLives;
        TeleportToCheckpoint();
        if (activeCheckpoint.currentCheckpointID == 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void TeleportToCheckpoint()
    {
        if (activeCheckpoint == null || activeCheckpoint.spawnPosition == Vector3.zero) return;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.position = activeCheckpoint.spawnPosition;
        }

        transform.position = activeCheckpoint.spawnPosition;

        // Fuerza al motor de físicas 2D a aplicar la nueva posición inmediatamente
        Physics2D.SyncTransforms();

        Debug.Log($"<color=cyan>[Respawn] Jugador reaparecido en: {transform.position}</color>");
    }

    private void CheckVoidCollision(GameObject obj)
    {
        if (obj.name.ToLower().Contains("void") || obj.CompareTag("Void"))
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckVoidCollision(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckVoidCollision(collision.gameObject);
    }
}