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
        Debug.Log("INICJALIZUJE BAZKE");
        CreateDatabase();
    }

    private void CreateDatabase()
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.CreateTable<MyData>();
        }
    }

    public void InsertData(MyData data)
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.Insert(data);
        }
    }

    public List<MyData> GetData()
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            return connection.Table<MyData>().ToList();
        }
    }

    public List<MyData> GetTopScores(int limit = 10)
    {
        if (string.IsNullOrEmpty(dbPath))
        {
            dbPath = System.IO.Path.Combine(Application.persistentDataPath, "mydatabase.db");
        }
        List<MyData> topScores = new List<MyData>();
        using (var connection = new SQLiteConnection(dbPath))
        {
            topScores = connection.Table<MyData>()
                        .OrderBy(d => d.Time)
                        .Take(limit)
                        .ToList();
        }
        return topScores;
    }
}

public class MyData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Nickname { get; set; }
    public double Time { get; set; }
}
