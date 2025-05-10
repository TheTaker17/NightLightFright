using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;
using TMPro;  // A�adir esta l�nea

public class GameManager : MonoBehaviour
{
    public float delayBeforeSceneChange = 5f; // Tiempo de espera antes de cambiar de escena (solo para victoria/derrota)

    public TMP_Text distanceText;        // Referencia al objeto de texto
    public TMP_Text scoreText;           // Referencia al score
    public ChainChompController chainChomp;  // Referencia al ChainChomp
    public Transform player;             // Referencia al jugador
    private static float finalDistance;  // Variable est�tica para almacenar la distancia final
    private static int finalScore;      // Variable est�tica para almacenar el puntaje

    void Start()
    {
        // Intentar encontrar los textos en la escena actual
        distanceText = GameObject.Find("DistanceText")?.GetComponent<TMP_Text>();
        scoreText = GameObject.Find("ScoreText")?.GetComponent<TMP_Text>();

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "VictoryScene" || currentScene == "DefeatScene")
        {
            ShowResults();
        }
    }

    public void SaveDistance()
    {
        if (chainChomp != null && player != null)
        {
            finalDistance = Vector3.Distance(chainChomp.transform.position, player.position);
            finalScore = CalculateScore(finalDistance);
            UnityEngine.Debug.Log("Distancia guardada: " + finalDistance.ToString("F2") + " | Puntaje: " + finalScore);
        }
        else
        {
            UnityEngine.Debug.LogWarning("No se pudo calcular la distancia.");
        }
    }

    private int CalculateScore(float distance)
    {
        // Cuanto m�s cerca est� el enemigo, mayor el puntaje (m�ximo 100)
        if (distance <= 60) return 100;
        if (distance <= 80) return 75;
        if (distance <= 110) return 50;
        if (distance <= 150) return 25;
        return 10;
    }

    private void ShowResults()
    {
        if (distanceText != null)
        {
            distanceText.text = "Distancia al ChainChomp: " + finalDistance.ToString("F2") + " m";
            UnityEngine.Debug.Log("Mostrando distancia en pantalla: " + finalDistance);
        }
        else
        {
            UnityEngine.Debug.LogError("No se encontr� el objeto DistanceText en la escena.");
        }

        if (scoreText != null)
        {
            scoreText.text = "Puntaje: " + finalScore;
            UnityEngine.Debug.Log("Mostrando puntaje en pantalla: " + finalScore);
        }
        else
        {
            UnityEngine.Debug.LogError("No se encontr� el objeto ScoreText en la escena.");
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene"); // Cambia por el nombre correcto
    }

    public void Rules()
    {
        SceneManager.LoadScene("PantallaReglas"); // Cambia por el nombre correcto
    }

    public void ExitGame()
    {
        UnityEngine.Application.Quit();
        UnityEngine.Debug.Log("Salir del juego");
    }

    public void VolverAJugar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }

    public void WinGame()
    {
        StartCoroutine(DelayedSceneChange("PantallaVictoria")); // Cambia el nombre de la escena de victoria
    }

    public void LoseGame()
    {
        StartCoroutine(DelayedSceneChange("PantallaDerrota")); // Cambia el nombre de la escena de derrota
    }

    private IEnumerator DelayedSceneChange(string sceneName)
    {
        // Solo aplica el retraso si estamos en la escena del juego
        if (SceneManager.GetActiveScene().name == "SampleScene") // Cambia por el nombre de tu escena de juego
        {
            yield return new WaitForSeconds(delayBeforeSceneChange); // Espera de 5 segundos
        }
        SceneManager.LoadScene(sceneName);
    }
}