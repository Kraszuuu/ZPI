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
        // Ustaw œcie¿kê bazy danych
        dbPath = System.IO.Path.Combine(Application.persistentDataPath, "mydatabase.db");
        Debug.Log(dbPath);
        CreateDatabase();
        InsertData(new MyData { Time = 7.09 });
        List<MyData> data = GetData();
        for (int i = 0; i < data.Count; i++)
        {
            Debug.Log(data[i].Time);
        }
       
    }

    private void CreateDatabase()
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            // Tworzenie tabeli, jeœli nie istnieje
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
}

public class MyData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public double Time { get; set; }
}
