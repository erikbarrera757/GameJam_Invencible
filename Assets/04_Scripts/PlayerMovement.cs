using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;
    private Vector2 moveInput;
    private Vector3 velocity;

    [Header("Ajustes de Movimiento")]
    public float speed = 5f;
    public float rotationSpeed = 10f;

    [Header("Ajustes de Salto y Gravedad")]
    public float jumpHeight = 2f;
    public float gravity = -25f; // Recomendado -20 a -30 para que no sea lento

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        // Bloqueo del cursor para que no estorbe al jugar
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Se activa automáticamente con el componente Player Input (Acción "Move")
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Se activa automáticamente con el componente Player Input (Acción "Jump")
    public void OnJump(InputValue value)
    {
        // Solo saltamos si estamos tocando el suelo
        if (controller.isGrounded)
        {
            // Fórmula física: v = sqrt(h * -2 * g)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (anim != null)
            {
                anim.SetTrigger("Jump");
            }
        }
    }

    void Update()
    {
        // 1. Resetear velocidad vertical al estar en el suelo
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Pequeña fuerza hacia abajo para mantener el contacto
        }

        // 2. Calcular Dirección de Movimiento (Relativa a la Cámara)
        // Obtenemos las direcciones de la cámara y anulamos el eje Y
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Esta es la dirección en la que el JUGADOR desea moverse (basado en el teclado)
        Vector3 desiredMoveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

        // --- SOLUCIÓN PARA SALTO EN EL SITIO ---
        // Variable para el movimiento final de este frame
        Vector3 finalHorizontalMovement = desiredMoveDirection;

        // Si NO estamos en el suelo (estamos saltando), anulamos el control horizontal
        if (!controller.isGrounded)
        {
            // Al hacer esto Vector3.zero, el personaje no avanza, retrocede ni va a los lados.
            // Solo se moverá en el eje Y (vertical) debido a la gravedad.
            finalHorizontalMovement = Vector3.zero;
        }
        // --- FIN DE LA SOLUCIÓN ---

        // 3. Aplicar Movimiento Horizontal
        Vector3 currentMovement = finalHorizontalMovement * speed;

        // 4. Aplicar Gravedad
        velocity.y += gravity * Time.deltaTime;
        currentMovement.y = velocity.y;

        // Mover el controlador (una sola llamada con movimiento combinado)
        controller.Move(currentMovement * Time.deltaTime);

        // 5. Animación de Carrera (basada en el input real, no el final horizontal)
        if (anim != null)
        {
            anim.SetFloat("Speed", moveInput.magnitude);
        }

        // 6. Rotación del personaje hacia donde se mueve
        // Usamos desiredMoveDirection para que rote aunque esté en el aire y no avance
        if (desiredMoveDirection.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(desiredMoveDirection);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * 50f * Time.deltaTime
            );
        }
    }
}