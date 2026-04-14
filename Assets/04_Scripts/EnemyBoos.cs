using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    public Animator anim;
    public Transform jugador;

    [Header("Configuración de Persecución")]
    public float rangoDeteccion = 15f;
    public float velocidadPersecucion = 2.5f;

    [Header("Configuración de Ataque")]
    public float rangoAtaque = 3f;
    public float cooldownAtaque = 3f;
    private float ultimoAtaque;

    [Header("Vida del Boss")]
    public int vida = 30;

    private int rutina;
    private float cronometro;
    private float grado;
    private Animator animPlayer;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (jugador == null) jugador = GameObject.FindWithTag("Player").transform;

        // Inicializar Animator del Player
        if (jugador != null)
        {
            animPlayer = jugador.GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (vida <= 0) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia < rangoAtaque)
        {
            Atacar();
        }
        else if (distancia < rangoDeteccion)
        {
            Perseguir();
        }
        else
        {
            Comportamiento_Boss();
        }
    }

    void Perseguir()
    {
        Vector3 posicionJugador = new Vector3(jugador.position.x, transform.position.y, jugador.position.z);
        transform.LookAt(posicionJugador);
        transform.Translate(Vector3.forward * velocidadPersecucion * Time.deltaTime);
        anim.SetBool("Walk", true);
    }

    void Comportamiento_Boss()
    {
        cronometro += Time.deltaTime;
        if (cronometro >= 5)
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
                transform.Translate(Vector3.forward * 1.5f * Time.deltaTime);
                anim.SetBool("Walk", true);
                break;
        }
    }

    void Atacar()
    {
        anim.SetBool("Walk", false);
        Vector3 posicionJugador = new Vector3(jugador.position.x, transform.position.y, jugador.position.z);
        transform.LookAt(posicionJugador);

        if (Time.time > ultimoAtaque + cooldownAtaque)
        {
            if (vida > 20)
                anim.SetTrigger("Attack1");
            else if (vida > 10)
                anim.SetTrigger("Attack2");
            else
                anim.SetTrigger("Attack3");

            ultimoAtaque = Time.time;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WeaponItem"))
        {
            // Verificar si el Player está en animación de ataque
            if (animPlayer != null && animPlayer.GetCurrentAnimatorStateInfo(0).IsName("Standing Melee Attack Downward"))
            {
                RecibirGolpe();
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
}
 