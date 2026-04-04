using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void OnPlayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("PlayScene");
    }
}
