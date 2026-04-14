using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float velocidad = 20f;
    public float tiempoVida = 3f;
    public int damage = 1;

    void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        // Avanzar en la dirección del puntoDisparo (rotación al instanciar)
        transform.position += transform.forward * velocidad * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Untagged"))
        {
            // Aquí podrías aplicar daño al Player
            // other.GetComponent<Player>().RecibirDaño(damage);

            Destroy(gameObject);
        }
    }
}
