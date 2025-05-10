using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Light linterna;      // Referencia a la linterna
    public Transform player;    // Referencia al personaje
    public float rotationSpeed = 1f;  // Velocidad de rotación
    private bool estaGirando = false;
    public Animator animator;
    public ChainChompController chompController; // Asignar desde el inspector

    // Sonidos del personaje
    public AudioSource audioSource;
    public AudioClip turnClothSound;       // Sonido de ropa cuando gira
    public AudioClip flashlightClickSound; // Click de encendido y apagado
    public AudioClip flashlightHumLoop;
    public AudioClip deathSound;

    private AudioSource humSource; // Fuente separada para el zumbido
    public Transform lightHolder;

    private bool hasWon = false;

    void Start()
    {
        if (linterna != null)
        {
            linterna.enabled = false;  // Apagar linterna al inicio
        }

        // Fuente de audio para el zumbido de la linterna
        humSource = gameObject.AddComponent<AudioSource>();
        humSource.clip = flashlightHumLoop;
        humSource.loop = true;
        humSource.playOnAwake = false;
        humSource.volume = 0.7f;
        humSource.spatialBlend = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !estaGirando) // Evita cortar la animación
        {
            StartCoroutine(TurnAndLight());
        }
    }

    IEnumerator TurnAndLight()
    {
        estaGirando = true;

        // Sonido de ropa al girar
        if (audioSource != null && turnClothSound != null)
            audioSource.PlayOneShot(turnClothSound);

        // Reproducir la animación del giro
        if (animator != null)
            animator.SetTrigger("isTurning");

        // Esperar la duración de la animación
        yield return new WaitForSeconds(0.7f);

        // Sonido de click al encender/apagar
        if (audioSource != null && flashlightClickSound != null)
            audioSource.PlayOneShot(flashlightClickSound);

        // Rotar manualmente la luz si tienes un LightHolder separado
        if (lightHolder != null)
        {
            lightHolder.rotation = Quaternion.Euler(0, player.eulerAngles.y, 0);
        }

        // Encender o apagar la linterna
        if (linterna != null)
            linterna.enabled = !linterna.enabled;

        // Zumbido de linterna (activo solo si está encendida)
        if (humSource != null)
        {
            if (linterna.enabled && flashlightHumLoop != null)
                humSource.Play();
            else
                humSource.Stop();
        }

        // Detener al ChainChomp si está cerca (comprobación adicional para evitar null)
        if (chompController != null)
        {
            chompController.StopMovement();
            hasWon = true;

            // Llamar a la pantalla de victoria desde el GameManager si gana
            if (hasWon)
            {
                GameManager gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.SaveDistance();
                    gameManager.WinGame();

                }
                else
                {
                    UnityEngine.Debug.LogError("GameManager no encontrado en la escena.");
                }
            }
        }
        else
        {
            UnityEngine.Debug.LogError("chompController no está asignado en el script NewBehaviourScript.");
        }

        estaGirando = false;
    }

}
