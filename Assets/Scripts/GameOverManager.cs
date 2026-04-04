using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public void OnMenuButton()
    {
        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void OnPlayAgainButton()
    {
                // Reload the current scene to play again
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
