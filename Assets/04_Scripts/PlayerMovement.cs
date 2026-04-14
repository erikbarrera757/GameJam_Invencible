using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
<<<<<<< Updated upstream
    public float rotationSpeed = 10f;

    [Header("Ajustes de Salto y Gravedad")]
    public float jumpHeight = 2f;
    public float gravity = -25f; // Recomendado -20 a -30 para que no sea lento
=======
    public float gravity = -27f;
    public float jumpHeight = 2f;
    public float rotationSpeed = 10f;

    [Header("Sistema de Armas")]
    public GameObject axeInHand;
    public GameObject axePrefab;
    private GameObject hachaCercana;

    [Header("Ajustes de Combate (Anti-Spam)")]
    public float cooldownPunos = 0.5f;
    public float cooldownHacha = 1.1f;
    private float tiempoSiguienteAtaque = 0f;
    [Header("Vida del Player")]
    public float vida = 5f;
    private bool agotado = false;
    public float vidaMaxima = 5f;
    public Slider vidaSlider;

    [Header("UI de Derrota")]
    public GameObject derrotaCanvas;
>>>>>>> Stashed changes

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

<<<<<<< Updated upstream
        // Bloqueo del cursor para que no estorbe al jugar
=======
>>>>>>> Stashed changes
        Cursor.lockState = CursorLockMode.Locked;
    }

<<<<<<< Updated upstream
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
=======
        if (axeInHand != null) axeInHand.SetActive(false);
        if (vidaSlider != null)
        {
            vidaSlider.maxValue = vidaMaxima;
            vidaSlider.value = vida;
>>>>>>> Stashed changes
        }
    }

    void Update()
    {
<<<<<<< Updated upstream
        // 1. Resetear velocidad vertical al estar en el suelo
        if (controller.isGrounded && velocity.y < 0)
=======
        if (agotado)
        {
            if (!controller.isGrounded)
            {
                velocity.y += gravity * Time.deltaTime;
                controller.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);
            }
            else
            {
                velocity.y = -2f;
            }
            return;
        }


        ManejarMovimiento();

        if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
>>>>>>> Stashed changes
        {
            velocity.y = -2f; // Pequeña fuerza hacia abajo para mantener el contacto
        }
<<<<<<< Updated upstream
=======
    }


    public void OnAttack(InputValue value)
    {
        if (Time.time >= tiempoSiguienteAtaque && controller.isGrounded && anim != null)
        {
            anim.SetTrigger("Attack");

            float cooldownActual = anim.GetBool("hasWeapon") ? cooldownHacha : cooldownPunos;

            tiempoSiguienteAtaque = Time.time + cooldownActual;

            Debug.Log("Ataque realizado. Próximo ataque en: " + cooldownActual + "s");
        }
        else if (Time.time < tiempoSiguienteAtaque)
        {
            Debug.Log("¡Espera el cooldown!");
        }
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("WeaponItem"))
        {
            hachaCercana = other.gameObject;
            Debug.Log("Hacha detectada. Presiona G");
        }


        if (other.CompareTag("Bullet"))
        {
            RecibirDanio(1f);
            Destroy(other.gameObject);
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WeaponItem") && hachaCercana == other.gameObject)
        {
            hachaCercana = null;
            Debug.Log("Te alejaste del hacha");
        }
    }

    void SoltarHacha()
    {
        axeInHand.SetActive(false);
        anim.SetBool("hasWeapon", false);

        Vector3 spawnPos = transform.position + transform.forward * 1.2f + Vector3.up * 1.2f;
        GameObject hachaSoltada = Instantiate(axePrefab, spawnPos, transform.rotation);

        Rigidbody rb = hachaSoltada.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            Vector3 fuerzaLanzamiento = (transform.forward + Vector3.up * 0.5f) * 3f;
            rb.AddForce(fuerzaLanzamiento, ForceMode.Impulse);
        }
        hachaCercana = null;
    }

    void RecogerHacha()
    {
        axeInHand.SetActive(true);
        anim.SetBool("hasWeapon", true);

        // Ajustes de posición en la mano
        axeInHand.transform.localPosition = new Vector3(-0.001f, 0.231f, 0.067f);
        axeInHand.transform.localEulerAngles = Vector3.zero;

        Destroy(hachaCercana);
        hachaCercana = null;
        Debug.Log("Hacha recogida.");
    }

    void ManejarMovimiento()
    {
        bool grounded = controller.isGrounded;
        if (grounded && velocity.y < 0) velocity.y = -2f;

        if (anim != null) anim.SetBool("isGrounded", grounded);
>>>>>>> Stashed changes

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

    public void RecibirDanio(float cantidad)
    {
        // Si ya está agotado, no recibe más daño
        if (agotado) return;

        vida -= cantidad;
        if (vida < 0) vida = 0;

        // Actualizar barra de vida
        if (vidaSlider != null)
        {
            vidaSlider.value = vida;
        }


        if (anim != null)
        {
            anim.SetTrigger("Hit"); // animación de daño
        }

        if (vida <= 0)
        {
            vida = 0;
            agotado = true;

            if (anim != null)
            {
                anim.SetTrigger("Exhausted");
            }

            // Mostrar Canvas de derrota
            if (derrotaCanvas != null)
            {
                derrotaCanvas.SetActive(true);
            }

            velocity = Vector3.zero;
            controller.Move(Vector3.zero);

            Debug.Log("Player agotado, ya no recibe daño.");
        }


        else
        {
            Debug.Log("Player recibió daño. Vida restante: " + vida);
        }
    }


}