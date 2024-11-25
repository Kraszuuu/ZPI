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
        GameState.IsSpeechRecognitionEnabled = !GameState.IsSpeechRecognitionEnabled;
        Debug.Log($"isSpeechRecognitionEnabled set to (GameSettings): {GameState.IsSpeechRecognitionEnabled}");
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
