using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Light linterna;      // Referencia a la linterna
    public Transform player;    // Referencia al personaje
    public float rotationSpeed = 1f;  // Velocidad de rotación
    private bool estaGirando = false;
    public Animator animator;
    public ChainChompController chompController; // Asignar desde el inspector

    //Sonidos del personaje
    public AudioSource audioSource;
    public AudioClip turnClothSound; //Sonido de ropa cuando gira
    public AudioClip flashlightClickSound; //Click de encencido y apagado
    public AudioClip flashlightHumLoop;
    public AudioClip deathSound;

    private AudioSource humSource; // Fuente separada para el zumbido

    public Transform lightHolder;


    void Start()
    {
        if (linterna != null)
        {
            linterna.enabled = false;  // Apagar linterna al inicio
        }
        // Forzar reproduccion zumbido linterna
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

        // 1. Sonido de ropa al girar
        if (audioSource != null && turnClothSound != null)
            audioSource.PlayOneShot(turnClothSound);

        // Reproducir la animación del giro
        if (animator != null)
            animator.SetTrigger("isTurning");

        // Esperar la duración de la animación (ajústalo si tu animación dura más o menos)
        yield return new WaitForSeconds(0.7f);

        // 2. Sonido de click al encender/apagar
        if (audioSource != null && flashlightClickSound != null)
            audioSource.PlayOneShot(flashlightClickSound);

        // Rotar manualmente la luz si tienes un LightHolder separado
        if (lightHolder != null)
        {
            // Girar 180 grados desde la rotación actual del personaje
            lightHolder.rotation = Quaternion.Euler(0, player.eulerAngles.y, 0);
        }

        // Encender o apagar la linterna
        if (linterna != null)
            linterna.enabled = !linterna.enabled;

        // 3. Zumbido de linterna (activo solo si está encendida)
        if (humSource != null)
        {
            if (linterna.enabled && flashlightHumLoop != null)
                humSource.Play();
            else
                humSource.Stop();
        }

        // Detener al ChainChomp si está cerca
        if (chompController != null)
            chompController.StopMovement();

        estaGirando = false;
    }
}
