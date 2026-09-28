using UnityEngine;

public class PlayerBuildingInventory : MonoBehaviour
{
    [Header("Building Settings")]
    [SerializeField] private GameObject prefab; // El objeto de plataforma que crearemos
    [SerializeField] private GameObject steps_panel; // El panel de interfaz de inventario (Logs)

    private GameObject building_prefab; // Almacena la plataforma creada actualmente
    private int steps = 0; // Número de plataformas en el inventario
    private const int MAX_STEPS = 5; // Límite máximo de inventario

    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        // Fuerza a que todos los botones de la UI inicien desactivados
        if (steps_panel != null)
        {
            foreach (Transform child in steps_panel.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        // Detecta el clic izquierdo del ratón para recoger bloques colocados
        if (Input.GetMouseButtonDown(0))
        {
            TryPickUpPlacedBlock();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprueba la etiqueta 'block' del suelo y el límite de 5 elementos
        if (collision.CompareTag("block"))
        {
            AddBlockToInventory(collision.gameObject);
        }
    }

    private void TryPickUpPlacedBlock()
    {
        if (steps >= MAX_STEPS) return;

        // Convierte la posición del cursor de la pantalla al mundo 2D
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        // Verifica si el Raycast golpeó un collider con el tag 'block_placed'
        if (hit.collider != null && hit.collider.CompareTag("block_placed"))
        {
            AddBlockToInventory(hit.collider.gameObject);
        }
    }

    private void AddBlockToInventory(GameObject blockObject)
    {
        if (steps >= MAX_STEPS) return;

        steps++; // Aumenta el recuento
        Destroy(blockObject); // Destruye el objeto del mundo

        // Activa la primera casilla disponible en el UI
        if (steps_panel != null)
        {
            foreach (Transform child in steps_panel.transform)
            {
                if (!child.gameObject.activeInHierarchy)
                {
                    child.gameObject.SetActive(true);
                    return; // Detiene la iteración
                }
            }
        }
    }

    public void Build_Click()
    {
        if (steps <= 0) return; // Evita construir si no hay plataformas acumuladas

        steps--; // Disminuye el recuento de inventario

        if (steps_panel != null)
        {
            foreach (Transform child in steps_panel.transform)
            {
                if (child.gameObject.activeInHierarchy)
                {
                    child.gameObject.SetActive(false); // Oculta el icono en UI

                    if (prefab != null)
                    {
                        // Calcula el frente del jugador según hacia dónde mira el sprite
                        float facingDirection = (spriteRenderer != null && spriteRenderer.flipX) ? -1.5f : 1.5f;
                        Vector3 spawnPosition = transform.position + new Vector3(facingDirection, 0f, 0f);

                        // Instancia la plataforma
                        building_prefab = Instantiate(prefab, spawnPosition, Quaternion.identity);

                        // Asigna la etiqueta 'block_placed' al nuevo bloque creado
                        building_prefab.tag = "block_placed";
                    }
                    return; // Detiene la iteración
                }
            }
        }
    }
}
