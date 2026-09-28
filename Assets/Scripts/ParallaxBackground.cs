using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform;

    [Header("Parallax Settings")]
    [Tooltip("0 = Se mueve igual que la cámara (estático). 1 = No se mueve (fondo infinito/lejano).")]
    [SerializeField] private float parallaxFactorX = 0.5f;
    [SerializeField] private float parallaxFactorY = 0.5f;

    [Header("Infinite Loop (Optional)")]
    [SerializeField] private bool infiniteHorizontal = false;

    private Vector3 lastCameraPosition;
    private float textureSizeX;

    private void Start()
    {
        if (cameraTransform == null)
        {
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
            else
            {
                Debug.LogError($"[ParallaxEffect] No se encontró una cámara principal para el fondo en {gameObject.name}.");
                return;
            }
        }

        lastCameraPosition = cameraTransform.position;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            textureSizeX = spriteRenderer.bounds.size.x;
        }
    }

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // Aplica el desplazamiento ajustado por el factor de parallax
        transform.position += new Vector3(deltaMovement.x * parallaxFactorX, deltaMovement.y * parallaxFactorY, 0f);

        lastCameraPosition = cameraTransform.position;

        // Lógica para fondo infinito horizontal
        if (infiniteHorizontal && textureSizeX > 0f)
        {
            float cameraMovementFromOrigin = cameraTransform.position.x - transform.position.x;

            if (Mathf.Abs(cameraMovementFromOrigin) >= textureSizeX)
            {
                float offsetPositionX = cameraMovementFromOrigin % textureSizeX;
                transform.position = new Vector3(cameraTransform.position.x - offsetPositionX, transform.position.y, transform.position.z);
            }
        }
    }
}