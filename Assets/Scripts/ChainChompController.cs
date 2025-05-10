using System.Diagnostics;
using UnityEngine;

public class ChainChompController : MonoBehaviour
{
    public Transform player; // Asignar en el inspector
    public float speed = 5f;
    public float acceleration = 0.5f;
    public float stopDistance = 5f;
    public Animator playerAnimator;

    private bool isStopped = false;

    // Sonidos de movimiento del ChainChomp
    public AudioSource moveAudioSource;
    public AudioClip movementSoundLoop;
    public AudioClip stopSound;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerAnimator = playerObj.GetComponent<Animator>();
        }

        if (moveAudioSource == null)
        {
            moveAudioSource = gameObject.AddComponent<AudioSource>();
        }

        moveAudioSource.clip = movementSoundLoop;
        moveAudioSource.loop = true;
        moveAudioSource.playOnAwake = false;
        moveAudioSource.volume = 1f;
        moveAudioSource.spatialBlend = 0f;
    }

    void Update()
    {
        if (!isStopped && player != null)
        {
            Vector3 pos1 = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 pos2 = new Vector3(player.position.x, 0, player.position.z);

            float distanceToPlayer = Vector3.Distance(pos1, pos2);

            if (distanceToPlayer > stopDistance)
            {
                // Movimiento hacia el jugador
                transform.position += -transform.right * speed * Time.deltaTime;
                speed += acceleration * Time.deltaTime;

                // Sonido de movimiento
                if (!moveAudioSource.isPlaying && movementSoundLoop != null)
                {
                    moveAudioSource.pitch = UnityEngine.Random.Range(1f, 1.05f);
                    moveAudioSource.Play();
                }
            }
            else
            {
                isStopped = true;
                UnityEngine.Debug.Log("¡El Chain Chomp se ha detenido cerca del jugador!");

                // Obtener referencia al script del jugador
                var playerScript = player.GetComponent<NewBehaviourScript>();

                // Verificar si la linterna está apagada (indica derrota)
                if (playerScript != null && playerScript.linterna != null && !playerScript.linterna.enabled)
                {
                    // Animación de muerte
                    if (playerAnimator != null)
                    {
                        playerAnimator.SetBool("isDead", true);
                    }

                    // Sonido de muerte
                    if (playerScript.audioSource != null && playerScript.deathSound != null)
                    {
                        playerScript.audioSource.PlayOneShot(playerScript.deathSound, 1f);
                    }

                    // Parar sonido de movimiento
                    if (moveAudioSource != null && moveAudioSource.isPlaying)
                        moveAudioSource.Stop();

                    // Llamar a la pantalla de derrota desde el GameManager

                    FindObjectOfType<GameManager>().SaveDistance();
                    FindObjectOfType<GameManager>().LoseGame();
                }
                else
                {
                    // Si la linterna está encendida, el jugador ha ganado
                    if (playerScript != null && playerScript.linterna.enabled)
                    {
                        FindObjectOfType<GameManager>().WinGame();
                    }
                }
            }
        }
    }

    public void StopMovement()
    {
        isStopped = true;

        // Parar sonido de movimiento
        if (moveAudioSource != null && moveAudioSource.isPlaying)
            moveAudioSource.Stop();

        // Sonido de frenado
        if (moveAudioSource != null && stopSound != null)
        {
            moveAudioSource.PlayOneShot(stopSound);
        }
    }

    public float GetDistanceToPlayer()
    {
        if (player == null) return Mathf.Infinity;

        Vector3 pos1 = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 pos2 = new Vector3(player.position.x, 0, player.position.z);

        return Vector3.Distance(pos1, pos2);
    }
}
