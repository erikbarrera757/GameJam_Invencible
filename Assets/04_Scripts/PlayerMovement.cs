using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;

    [Header("Ajustes de Movimiento")]
    public float speed = 5f;
    public float gravity = -9.81f;
    public float rotationSpeed = 8f; // Bajamos un poco para más estabilidad

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 move = (forward * moveInput.y + right * moveInput.x).normalized;

        // Mover el personaje
        controller.Move(move * speed * Time.deltaTime);

        // Rotación ultra-estable
        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            // Usamos un ángulo de rotación más suave
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * 40f * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}