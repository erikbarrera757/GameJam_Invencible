using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;
    private Vector2 moveInput;
    private Vector3 velocity;

    [Header("Ajustes de Movimiento")]
    public float speed = 5f;
    public float gravity = -25f;
    public float jumpHeight = 2f;
    public float rotationSpeed = 10f;

    [Header("Sistema de Armas")]
    public GameObject axeInHand;
    public GameObject axePrefab;
    private GameObject hachaCercana;

    [Header("Ajustes de Combate (Anti-Spam)")]
    public float cooldownPunos = 0.5f;     // Tiempo entre puñetazos
    public float cooldownHacha = 1.1f;     // El hacha es más lenta
    private float tiempoSiguienteAtaque = 0f; // Reloj interno

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        // Bloqueamos el mouse para que no se salga de la pantalla de juego
        Cursor.lockState = CursorLockMode.Locked;

        if (axeInHand != null) axeInHand.SetActive(false);
    }

    void Update()
    {
        ManejarMovimiento();

        // Lógica de recoger/soltar hacha con la tecla G
        if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
        {
            if (anim != null && anim.GetBool("hasWeapon"))
            {
                SoltarHacha();
            }
            else if (hachaCercana != null)
            {
                RecogerHacha();
            }
        }
    }

    // --- LÓGICA DE ATAQUE CON COOLDOWN ---
    public void OnAttack(InputValue value)
    {
        // Solo atacamos si ya pasó el tiempo de espera Y estamos en el suelo
        if (Time.time >= tiempoSiguienteAtaque && controller.isGrounded && anim != null)
        {
            anim.SetTrigger("Attack");

            // Elegimos el cooldown dependiendo de si tiene el hacha puesta
            float cooldownActual = anim.GetBool("hasWeapon") ? cooldownHacha : cooldownPunos;

            // Seteamos cuándo será el próximo ataque permitido
            tiempoSiguienteAtaque = Time.time + cooldownActual;

            Debug.Log("Ataque realizado. Próximo ataque en: " + cooldownActual + "s");
        }
        else if (Time.time < tiempoSiguienteAtaque)
        {
            Debug.Log("¡Espera el cooldown!");
        }
    }

    // --- SISTEMA DE RECOGER/SOLTAR ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WeaponItem"))
        {
            hachaCercana = other.gameObject;
            Debug.Log("Hacha detectada. Presiona G");
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

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0; right.y = 0;
        forward.Normalize(); right.Normalize();

        Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
        Vector3 currentMovement = moveDirection * speed;

        velocity.y += gravity * Time.deltaTime;
        currentMovement.y = velocity.y;

        controller.Move(currentMovement * Time.deltaTime);

        if (anim != null) anim.SetFloat("Speed", moveInput.magnitude);

        if (moveDirection.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * 50f * Time.deltaTime);
        }
    }

    public void OnMove(InputValue value) { moveInput = value.Get<Vector2>(); }

    public void OnJump(InputValue value)
    {
        if (controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (anim != null) anim.SetTrigger("Jump");
        }
    }
}