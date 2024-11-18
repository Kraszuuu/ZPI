using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }
    public Button SaveButton;
    public TMP_InputField NicknameInput;
    public TextMeshProUGUI MessageTextMesh;
    public TextMeshProUGUI ErrorMessageTextMesh;

    public GameObject EndGamePanel;

    private GameTimer gameTimer;
    private SQLiteManager sqLiteManager;
    private void Start()
    {
        gameTimer = FindObjectOfType<GameTimer>();
        sqLiteManager = FindObjectOfType<SQLiteManager>();
        SaveButton.onClick.AddListener(SaveScoreAndReturnToMainMenu);
    }

    public void EndGame()
    {
        if (!GameState.Instance.IsGameOver)
        {
            GameState.Instance.IsGameOver = true;
            Time.timeScale = 0f;
            FulfillEndGamePanel();
            EndGamePanel.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void FulfillEndGamePanel()
    {
        double finalTime = gameTimer.GetTotalGameTime();
        MessageTextMesh.text =  $"YOU SURVIVED {finalTime:F2}s. ENTER YOUR NICKNAME AND SAVE YOUR RESULT";
    }

    private void SaveScoreAndReturnToMainMenu()
    {
        string nickname = NicknameInput.text;
        double finalTime = gameTimer.GetTotalGameTime();

        if (nickname.Length >= 12)
        {
            ErrorMessageTextMesh.text = "Nickname should be shorter than 12 signs";
        }
        else if (string.IsNullOrEmpty(nickname))
        {
            ErrorMessageTextMesh.text = "Nickname cannot be empty";
        }
        else
        {
            sqLiteManager.InsertData(new Score { Nickname = nickname, Time = finalTime });

            SceneManager.LoadScene("MainMenu");
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
