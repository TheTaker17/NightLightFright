using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float delayBeforeSceneChange = 5f;

    public TMP_Text distanceText;
    public TMP_Text scoreText;
    public ChainChompController chainChomp;
    public Transform player;
    private static float finalDistance;
    private static int finalScore;

    void Start()
    {
        distanceText = GameObject.Find("DistanceText")?.GetComponent<TMP_Text>();
        scoreText = GameObject.Find("ScoreText")?.GetComponent<TMP_Text>();

        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene.Contains("Victoria") || currentScene.Contains("Derrota"))
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
        if (distance <= 60) return 100;
        if (distance <= 80) return 75;
        if (distance <= 110) return 50;
        if (distance <= 200) return 25;
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
            UnityEngine.Debug.LogError("No se encontró el objeto DistanceText en la escena.");
        }

        if (scoreText != null)
        {
            scoreText.text = "Puntaje: " + finalScore;
            UnityEngine.Debug.Log("Mostrando puntaje en pantalla: " + finalScore);
        }
        else
        {
            UnityEngine.Debug.LogError("No se encontró el objeto ScoreText en la escena.");
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Rules()
    {
        SceneManager.LoadScene("PantallaReglas");
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
        SaveDistance();
        string victoriaScene = GetVictoryScene(finalScore);
        StartCoroutine(DelayedSceneChange(victoriaScene));
    }

    public void LoseGame()
    {
        SaveDistance();
        StartCoroutine(DelayedSceneChange("PantallaDerrota"));
    }

    private string GetVictoryScene(int score)
    {
        switch (score)
        {
            case 100: return "Victoria100";
            case 75: return "Victoria75";
            case 50: return "Victoria50";
            case 25: return "Victoria25";
            default: return "Victoria10";
        }
    }

    private IEnumerator DelayedSceneChange(string sceneName)
    {
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            yield return new WaitForSeconds(delayBeforeSceneChange);
        }
        SceneManager.LoadScene(sceneName);
    }
}
