using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ChangeSpeechRecognitionMode(bool value)
    {
        GameSettings.isSpeechRecognitionEnabled = value;
        Debug.Log($"isSpeechRecognitionEnabled set to (GameSettings): {GameSettings.isSpeechRecognitionEnabled}");
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
