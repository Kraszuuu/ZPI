using UnityEngine;
using System.Collections.Generic;
using System.Data; // Do obsługi SQL w Mono.Data.Sqlite
using Mono.Data.Sqlite;
using System.IO;

public class SQLiteManager : MonoBehaviour
{
    private string dbPath;

    private void Start()
    {
        // Ustaw ścieżkę bazy danych
        dbPath = System.IO.Path.Combine(Application.persistentDataPath, "mydatabase.db");
        CreateDatabase();
    }

    private void CreateDatabase()
    {
        using (var connection = new SqliteConnection($"URI=file:{dbPath}"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Score (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nickname TEXT NOT NULL,
                        Time REAL NOT NULL
                    );";
                command.ExecuteNonQuery();
            }
        }
    }

    public void InsertData(Score data)
    {
        using (var connection = new SqliteConnection($"URI=file:{dbPath}"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Score (Nickname, Time) VALUES (@Nickname, @Time);";
                command.Parameters.AddWithValue("@Nickname", data.Nickname);
                command.Parameters.AddWithValue("@Time", data.Time);
                command.ExecuteNonQuery();
            }
        }
    }

    public List<Score> GetData()
    {
        var scores = new List<Score>();
        using (var connection = new SqliteConnection($"URI=file:{dbPath}"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Score;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        scores.Add(new Score
                        {
                            Id = reader.GetInt32(0),
                            Nickname = reader.GetString(1),
                            Time = reader.GetDouble(2)
                        });
                    }
                }
            }
        }
        return scores;
    }

    public List<Score> GetTopScores(int limit = 10)
    {
        var topScores = new List<Score>();
        using (var connection = new SqliteConnection($"URI=file:{dbPath}"))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM Score ORDER BY Time DESC LIMIT {limit};";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        topScores.Add(new Score
                        {
                            Id = reader.GetInt32(0),
                            Nickname = reader.GetString(1),
                            Time = reader.GetDouble(2)
                        });
                    }
                }
            }
        }
        return topScores;
    }
}

public class Score
{
    public int Id { get; set; }
    public string Nickname { get; set; }
    public double Time { get; set; }
}
