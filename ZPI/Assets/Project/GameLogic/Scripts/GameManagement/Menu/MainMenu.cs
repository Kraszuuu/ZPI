using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ChangeSpeechRecognitionMode()
    {
        GameState.Instance.IsSpeechRecognitionEnabled = !GameState.Instance.IsSpeechRecognitionEnabled;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Cut Scene Scene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
