using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;
using Unity.VisualScripting.Dependencies.Sqlite;
using System.Linq;

public class SQLiteManager : MonoBehaviour
{
    private string dbPath;

    private void Start()
    {
        // Ustaw �cie�k� bazy danych
        dbPath = System.IO.Path.Combine(Application.persistentDataPath, "mydatabase.db");
        CreateDatabase();
    }

    private void CreateDatabase()
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.CreateTable<Score>();
        }
    }

    public void InsertData(Score data)
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.Insert(data);
        }
    }

    public List<Score> GetData()
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            return connection.Table<Score>().ToList();
        }
    }

    public List<Score> GetTopScores(int limit = 10)
    {
        if (string.IsNullOrEmpty(dbPath))
        {
            dbPath = System.IO.Path.Combine(Application.persistentDataPath, "mydatabase.db");
        }
        List<Score> topScores = new List<Score>();
        using (var connection = new SQLiteConnection(dbPath))
        {
            topScores = connection.Table<Score>()
                        .OrderByDescending(d => d.Time)
                        .Take(limit)
                        .ToList();
        }
        return topScores;
    }
}

public class Score
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Nickname { get; set; }
    public double Time { get; set; }
}
