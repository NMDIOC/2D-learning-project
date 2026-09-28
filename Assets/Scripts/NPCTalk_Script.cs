using UnityEngine;

public class NPCTalk_Script : MonoBehaviour
{
    [SerializeField] private GameObject talkPanel;
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (talkPanel != null)
        {
            talkPanel.SetActive(false);
        }

        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject player = GameObject.FindGameObjectWithTag("Jugador");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            // Nota técnica: En 2D, 'LookAt' rota el objeto en el espacio 3D (Z), 
            // lo que suele deformar o aplanar los sprites. Para un juego 2D,
            // es más eficiente voltear el sprite horizontalmente:
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = playerTransform.position.x < transform.position.x;
            }
            else
            {
                transform.LookAt(playerTransform);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Jugador") && talkPanel != null)
        {
            talkPanel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Jugador") && talkPanel != null)
        {
            talkPanel.SetActive(false);
        }
    }
}