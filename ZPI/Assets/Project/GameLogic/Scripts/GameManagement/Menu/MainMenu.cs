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
        AudioManagerMenu.instance.StopPlayingMusic();
        GameState.Instance.IsGameOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Cut Scene Scene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
