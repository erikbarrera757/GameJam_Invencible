using UnityEngine;
using UnityEngine.AI;

public class EnemySoldier : MonoBehaviour
{
    public Animator anim;
    public Transform jugador;
    private NavMeshAgent agent;

    [Header("Configuración de Persecución")]
    public float rangoDeteccion = 10f;

    [Header("Configuración de Ataque (Disparo)")]
    public float rangoAtaque = 8f;
    public float cooldownAtaque = 2f;
    private float ultimoAtaque;
    public GameObject balaPrefab;
    public Transform puntoDisparo;
    public GameObject muzzleFlashPrefab;
    private Animator animPlayer;
    [Header("Vida del Enemigo")]
    public float vida = 2;

    [Header("Raycast Configuración")]
    public LayerMask capasVisibles; // Player y Muro

    // Estado: ¿ya vio al jugador?
    private bool jugadorDetectado = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // Configuración de evasión
        agent.avoidancePriority = Random.Range(20, 50); // prioridad distinta para cada enemigo
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.radius = 0.5f; // radio de colisión
        agent.height = 2f;   // altura del agente

        if (jugador == null) jugador = GameObject.FindWithTag("Player").transform;
        if (jugador != null)
        {
            animPlayer = jugador.GetComponent<Animator>();
        }
    }


    void Update()
    {

        if (vida <= 0) return;

        // Dirección hacia el jugador
        Vector3 direccion = (jugador.position - transform.position).normalized;
        RaycastHit hit;
        // Dibuja el raycast en rojo para visualizarlo en la escena
        Debug.DrawRay(puntoDisparo.position, direccion * rangoAtaque, Color.red);

        // Raycast para comprobar visión
        if (Physics.Raycast(transform.position, direccion, out hit, rangoDeteccion, capasVisibles))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                jugadorDetectado = true; // lo vio por primera vez
            }
        }

        if (jugadorDetectado)
        {
            float distancia = Vector3.Distance(transform.position, jugador.position);

            if (distancia < rangoAtaque)
            {
                Atacar(); // dispara solo si tiene visión clara
            }
            else
            {
                Perseguir(); // siempre persigue aunque no tenga visión
            }
        }
        else
        {
            // Si nunca lo vio, se queda quieto
            anim.SetBool("Walk", false);
            agent.ResetPath();
        }
    }

    void Perseguir()
    {
        if (jugador != null)
        {
            agent.isStopped = false; // reactiva el movimiento
            agent.SetDestination(jugador.position);
            anim.SetBool("Walk", true);
        }
    }

    void Atacar()
    {
        // sigue persiguiendo, pero se detiene al disparar
        agent.isStopped = true;
        anim.SetBool("Walk", false);

        Vector3 direccion = (jugador.position - puntoDisparo.position).normalized;
        RaycastHit hit;

        if (Physics.Raycast(puntoDisparo.position, direccion, out hit, rangoAtaque, capasVisibles))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                transform.LookAt(new Vector3(jugador.position.x, transform.position.y, jugador.position.z));

                if (Time.time > ultimoAtaque + cooldownAtaque)
                {
                    anim.SetTrigger("Attack");
                    ultimoAtaque = Time.time;

                    if (balaPrefab != null && puntoDisparo != null)
                    {
                        Instantiate(balaPrefab, puntoDisparo.position, puntoDisparo.rotation);
                    }

                    if (muzzleFlashPrefab != null && puntoDisparo != null)
                    {
                        GameObject flash = Instantiate(muzzleFlashPrefab, puntoDisparo.position, puntoDisparo.rotation, puntoDisparo);
                        Destroy(flash, 0.2f);
                    }
                }
            }
            else
            {
                // Si hay muro, no dispara pero sigue persiguiendo
                Perseguir();
            }
        }
        else
        {
            // Si no golpea nada, sigue persiguiendo
            Perseguir();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WeaponItem")) // El arma del Player debe tener este tag
        {
            // Solo recibe daño si el Player está en animación de ataque
            if (animPlayer != null && animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Standing Melee Attack Downward"))
            {
                RecibirGolpe();
            }
        }
        if (other.CompareTag("mano")) // el Player mismo entra en el collider del enemigo
        {
            if (animPlayer != null && animPlayer.GetCurrentAnimatorStateInfo(0).IsName("punching"))
            {
                RecibirGolpePuño(); // daño reducido
            }
        }

    }
    public void RecibirGolpe()
    {
        vida--;

        if (vida <= 0)
        {
            anim.SetTrigger("Death");
            Destroy(gameObject, 4f);
        }
    }
    public void RecibirGolpePuño()
    {
        vida -= 0.5f; // resta medio punto de vida

        if (vida <= 0)
        {
            anim.SetTrigger("Death");
            Destroy(gameObject, 4f);
        }
        Debug.Log("Golpe de puño detectado");
    }
}
