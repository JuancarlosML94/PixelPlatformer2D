using UnityEngine;

// Zona invisible delante del portal: cuando el jugador entra, se acaba el nivel
// y el personaje deja de moverse.
public class PortalTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo reacciona al jugador, no a cualquier objeto que entre
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Has llegado al portal! Nivel completado.");

            // Desactivamos el script de movimiento y quitamos la velocidad que llevaba
            other.GetComponent<PlayerController2D>().enabled = false;
            other.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }
}
