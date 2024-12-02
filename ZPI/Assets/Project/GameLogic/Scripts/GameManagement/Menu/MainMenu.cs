using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    private void Start()
    {
        GameState.IsSpeechRecognitionEnabled = true;
    }
    public void ChangeSpeechRecognitionMode()
    {
        GameState.Instance.ChangeSpeechRecognitionMode();
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
