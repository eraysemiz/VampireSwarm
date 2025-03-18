using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using TMPro;
using System;

public class DatabaseManager : MonoBehaviour
{
    SqliteConnection dbConnection;
    public TMP_Text usernameText;
    public TMP_Text scoreText;
    public TMP_Text scoreboardText;

    string dbPath;
    void Awake()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/VampireSurvivors.db";
        dbConnection = new SqliteConnection(dbPath);
        dbConnection.Open();
        CreateTable();
    }

    void CreateTable()
    {
        using (var command = dbConnection.CreateCommand())
        {
            command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Scoreboard (
                        user_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        username TEXT,
                        score INTEGER
                    );";
            command.ExecuteNonQuery();
        }
    }

    public void SavePlayerScore()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                INSERT INTO Scoreboard (username, score) 
                VALUES (@Username, @Score);";
                command.Parameters.Add(new SqliteParameter("@Username", PlayerStats.PlayerData.Username));
                command.Parameters.Add(new SqliteParameter("@Score", PlayerStats.PlayerData.score));
                command.ExecuteNonQuery();

            }
        }
    }
    public void LoadScoreBoard()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT username, score FROM Scoreboard ORDER BY score DESC;";
                using (var reader = command.ExecuteReader())
                {
                    string scoreBoardText = "";
                    while (reader.Read())
                    {
                        object usernameObj = reader.GetValue(0);
                        string username = usernameObj != DBNull.Value ? usernameObj.ToString() : "Unknown";
                        int score = reader.GetInt32(1);
                        scoreBoardText += $"{username}: {score}\n";
                    }

                    scoreboardText.text = scoreBoardText;
                }
            }
        }
    }

}
