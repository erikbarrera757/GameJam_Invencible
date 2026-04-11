using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    public Transform playerTransform; // El Player a seguir
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Altura del hombro
    public float smoothTime = 0.05f; // Suavizado del movimiento (elimina tambaleo)

    [Header("Configuración de Rotación")]
    public float sensitivity = 0.5f;

    private Vector3 currentVelocity = Vector3.zero;
    private float xRotation = 0f;
    private float yRotation = 0f;

    void LateUpdate()
    {
        if (playerTransform == null) return;

        // --- PARTE 1: SEGUIMIENTO (POSICIÓN) ---
        // Calcula dónde debería estar la cámara y se mueve ahí suavemente.
        // SmoothDamp actúa como un amortiguador que filtra las vibraciones del Player.
        Vector3 targetPosition = playerTransform.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

        // --- PARTE 2: ROTACIÓN (MOUSE) ---
        // Si mantienes click derecho, calculamos la rotación.
        if (Mouse.current.rightButton.isPressed)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Vector2 delta = Mouse.current.delta.ReadValue();

            yRotation += delta.x * sensitivity;
            xRotation -= delta.y * sensitivity;
            xRotation = Mathf.Clamp(xRotation, -30f, 60f); // Evita que la cámara dé la vuelta completa

            // Aplicamos la rotación al objeto de forma absoluta.
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}