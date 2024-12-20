using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public GameObject scoreEntryPrefab;
    public Transform leaderboardPanel;
    private SQLiteManager sqLiteManager;

    private void OnEnable()
    {
        sqLiteManager = FindObjectOfType<SQLiteManager>();
        StartCoroutine(DelayedDisplay());
    }

    private void OnDisable()
    {
        ClearLeaderboard();
    }

    public void DisplayLeaderboard()
    {
        List<Score> topScores = sqLiteManager.GetTopScores(10);

        for (int i = 0; i < topScores.Count; i++)
        {
            GameObject newScore = Instantiate(scoreEntryPrefab, leaderboardPanel);
            var textComponent = newScore.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = $"{(i + 1),2}.   {topScores[i].Nickname,-15} {topScores[i].Time,5:F2}s.";
            }
        }
    }

    private IEnumerator DelayedDisplay()
    {
        yield return null; // Opóźnienie o jedną klatkę
        DisplayLeaderboard();
    }

    private void ClearLeaderboard()
    {
        foreach (Transform child in leaderboardPanel)
        {
            if (child.name.EndsWith("(Clone)"))
            {
                Destroy(child.gameObject);
            }
        }
    }
}
