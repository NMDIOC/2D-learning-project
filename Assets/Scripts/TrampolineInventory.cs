using UnityEngine;

public class TrampolineInventory : MonoBehaviour
{
    [Header("Trampoline Settings")]
    [SerializeField] private GameObject trampolinePrefab; // Prefab del trampolín físico
    [SerializeField] private float bounceForce = 12f; // Fuerza de impulso al pisar el trampolín

    [Header("Inventory Display Settings")]
    [Tooltip("Distancia desde el borde derecho de la pantalla")]
    [SerializeField] private float paddingRight = 1.0f;
    [Tooltip("Distancia desde el borde inferior de la pantalla")]
    [SerializeField] private float paddingBottom = 1.0f;
    [Tooltip("Espacio horizontal entre trampolines guardados")]
    [SerializeField] private float iconSpacing = 0.8f;

    private const int MAX_TRAMPOLINES = 5;
    private GameObject[] inventoryIcons = new GameObject[MAX_TRAMPOLINES];
    private int trampolineCount = 0;

    private Camera mainCamera;
    private GameObject draggedIcon = null;
    private int draggedIndex = -1;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        UpdateInventoryScreenPositions();

        // 1. Detectar clic inicial sobre un objeto del inventario en pantalla
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            
            // Lanza un Raycast2D que detecte Triggers
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider != null)
            {
                for (int i = 0; i < trampolineCount; i++)
                {
                    if (inventoryIcons[i] == hit.collider.gameObject)
                    {
                        draggedIcon = inventoryIcons[i];
                        draggedIndex = i;
                        break;
                    }
                }
            }
        }

        // 2. Arrastrar el objeto siguiendo el cursor del ratón
        if (draggedIcon != null && Input.GetMouseButton(0))
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            draggedIcon.transform.position = mouseWorldPos;
        }

        // 3. Soltar el objeto en el mundo (colocación permanente)
        if (draggedIcon != null && Input.GetMouseButtonUp(0))
        {
            PlaceTrampoline();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Recolección al colisionar con el objeto suelto en el mapa
        if (collision.CompareTag("trampolin") && trampolineCount < MAX_TRAMPOLINES)
        {
            PickUpTrampoline(collision.gameObject);
        }

        // Impulso si el jugador pisa un trampolín colocado en el mundo
        if (collision.CompareTag("trampolin_placed"))
        {
            Rigidbody2D playerRb = GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, bounceForce);
            }
        }
    }

    private void PickUpTrampoline(GameObject worldObject)
    {
        Destroy(worldObject);

        // Instancia el objeto visual del inventario
        GameObject icon = Instantiate(trampolinePrefab);
        
        // Mantiene el collider ACTIVO pero como Trigger para que el Raycast pueda detectarlo sin crear colisión física sólida
        Collider2D iconCollider = icon.GetComponent<Collider2D>();
        if (iconCollider != null) 
        {
            iconCollider.enabled = true;
            iconCollider.isTrigger = true;
        }

        inventoryIcons[trampolineCount] = icon;
        trampolineCount++;

        UpdateInventoryScreenPositions();
    }

    private void UpdateInventoryScreenPositions()
    {
        if (mainCamera == null) return;

        Vector3 bottomRightWorld = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0f, mainCamera.nearClipPlane));

        for (int i = 0; i < trampolineCount; i++)
        {
            if (inventoryIcons[i] == draggedIcon) continue;

            float xOffset = paddingRight + (i * iconSpacing);
            Vector3 targetPosition = new Vector3(bottomRightWorld.x - xOffset, bottomRightWorld.y + paddingBottom, 0f);

            inventoryIcons[i].transform.position = targetPosition;
        }
    }

    private void PlaceTrampoline()
    {
        // Configura el colisionador según el comportamiento final que desees
        Collider2D col = draggedIcon.GetComponent<Collider2D>();
        if (col != null) 
        {
            col.enabled = true;
            // Mapea a IsTrigger = true para que el jugador pase a través de él y active el OnTriggerEnter2D del salto
            col.isTrigger = true; 
        }

        // Cambia la etiqueta a 'trampolin_placed' para fijarlo definitivamente
        draggedIcon.tag = "trampolin_placed";

        // Reorganiza el arreglo quitando el elemento usado
        for (int i = draggedIndex; i < trampolineCount - 1; i++)
        {
            inventoryIcons[i] = inventoryIcons[i + 1];
        }

        inventoryIcons[trampolineCount - 1] = null;
        trampolineCount--;

        draggedIcon = null;
        draggedIndex = -1;

        UpdateInventoryScreenPositions();
    }
}