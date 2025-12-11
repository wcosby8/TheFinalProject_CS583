using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public void ShowGameOver() {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }
    public void RestartGame() {
        Time.timeScale = 1f; // Unpause
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitGame() {
        Application.Quit();
    }
}
