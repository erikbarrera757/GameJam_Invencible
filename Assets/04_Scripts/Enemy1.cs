<<<<<<< Updated upstream
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

=======
using UnityEngine;
>>>>>>> Stashed changes

public class Enemy1 : MonoBehaviour
{
    public int rutina;
    public float cronometro;
    public Animator anim;
    public float grado;

    [Header("Configuración de Persecución")]
<<<<<<< Updated upstream
    public Transform jugador;     
=======
    public Transform jugador;
>>>>>>> Stashed changes
    public float rangoDeteccion = 5f;
    public float velocidadPersecucion = 3f;

    [Header("Configuración de Ataque")]
    public float rangoAtaque = 1.5f;
<<<<<<< Updated upstream
    public float cooldownAtaque = 2f; // Tiempo entre golpes
    private float ultimoAtaque;
    void Start()
    {
        anim = GetComponent<Animator>();

        if (jugador == null) jugador = GameObject.FindWithTag("Player").transform;
=======
    public float cooldownAtaque = 2f;
    private float ultimoAtaque;

    [Header("Vida del Enemigo")]
    public float vida = 2; // Aguanta 2 golpes

    // Referencia al Animator del Player
    private Animator animPlayer;

    [Header("Raycast Configuración")]
    public LayerMask capasVisibles; // Player y Muro

    private bool jugadorDetectado = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (jugador == null) jugador = GameObject.FindWithTag("Player").transform;

        if (jugador != null)
        {
            animPlayer = jugador.GetComponent<Animator>();
        }
>>>>>>> Stashed changes
    }

    void Update()
    {
<<<<<<< Updated upstream
        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia < rangoAtaque)
        {
            Atacar();
        }
        else if (distancia < rangoDeteccion)
        {
           
=======
        if (vida <= 0) return;

        Vector3 direccion = (jugador.position - transform.position).normalized;
        RaycastHit hit;

        // Dibujar raycast en rojo para debug
        Debug.DrawRay(transform.position, direccion * rangoDeteccion, Color.red);

        // Raycast para comprobar visión
        if (Physics.Raycast(transform.position, direccion, out hit, rangoDeteccion, capasVisibles))
        {
            if (hit.collider.CompareTag("Player"))
            {
                jugadorDetectado = true;
            }
            else
            {
                jugadorDetectado = false; // hay muro bloqueando
            }
        }

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (jugadorDetectado && distancia < rangoAtaque)
        {
            Atacar();
        }
        else if (jugadorDetectado && distancia < rangoDeteccion)
        {
>>>>>>> Stashed changes
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                Perseguir();
            }
        }
        else
        {
            Comportamiento_Enemigo();
        }
    }

    public void Perseguir()
    {
        Vector3 posicionJugador = new Vector3(jugador.position.x, transform.position.y, jugador.position.z);
        transform.LookAt(posicionJugador);
<<<<<<< Updated upstream


        transform.Translate(Vector3.forward * velocidadPersecucion * Time.deltaTime);


=======
        transform.Translate(Vector3.forward * velocidadPersecucion * Time.deltaTime);
>>>>>>> Stashed changes
        anim.SetBool("Walk", true);
    }

    public void Comportamiento_Enemigo()
    {
        cronometro += Time.deltaTime;
        if (cronometro >= 4)
        {
            rutina = Random.Range(0, 2);
            cronometro = 0;
            if (rutina == 1) grado = Random.Range(0, 360);
        }

        switch (rutina)
        {
            case 0:
                anim.SetBool("Walk", false);
                break;
            case 1:
                Quaternion angulo = Quaternion.Euler(0, grado, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, angulo, 0.05f);
                transform.Translate(Vector3.forward * 2 * Time.deltaTime);
                anim.SetBool("Walk", true);
                break;
        }
    }
<<<<<<< Updated upstream
    void Atacar()
    {
        anim.SetBool("Walk", false);


=======

    void Atacar()
    {
        anim.SetBool("Walk", false);
>>>>>>> Stashed changes
        Vector3 posicionJugador = new Vector3(jugador.position.x, transform.position.y, jugador.position.z);
        transform.LookAt(posicionJugador);

        if (Time.time > ultimoAtaque + cooldownAtaque)
        {
            anim.SetTrigger("Attack");
            ultimoAtaque = Time.time;
        }
    }
<<<<<<< Updated upstream
}
=======

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WeaponItem"))
        {
            if (animPlayer != null && animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Standing Melee Attack Downward"))
            {
                RecibirGolpe();
            }
        }
        if (other.CompareTag("mano"))
        {
            if (animPlayer != null && animPlayer.GetCurrentAnimatorStateInfo(0).IsName("punching"))
            {
                RecibirGolpePuño();
            }
        }
    }

    public void RecibirGolpePuño()
    {
        vida -= 0.5f;
        if (vida <= 0)
        {
            anim.SetTrigger("Death");
            Destroy(gameObject, 4f);
        }
        Debug.Log("Golpe de puño detectado");
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
}
>>>>>>> Stashed changes
