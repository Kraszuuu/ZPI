using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ChangeSpeechRecognitionMode(bool value)
    {
        GameState.IsSpeechRecognitionEnabled = value;
        Debug.Log($"isSpeechRecognitionEnabled set to (GameSettings): {GameState.IsSpeechRecognitionEnabled}");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Final Scene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
