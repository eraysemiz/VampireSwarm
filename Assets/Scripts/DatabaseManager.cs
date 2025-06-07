using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using System;

public class DatabaseManager : MonoBehaviour
{
    SqliteConnection dbConnection;

    string dbPath;
    void Awake()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/VampireSurvivors.db";
        dbConnection = new SqliteConnection(dbPath);
        dbConnection.Open();
        CreateTables();
    }

    void CreateTables()
    {
        using (var command = dbConnection.CreateCommand())
        {
            command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS GameHistory (
                        run_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        character TEXT,
                        level INTEGER,
                        minutes REAL,
                        victory INTEGER,
                        played_at TEXT
                    );";
            command.ExecuteNonQuery();
        }
    }


    public void SaveGameResult(string character, int level, float minutes, bool victory)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                INSERT INTO GameHistory (character, level, minutes, victory, played_at)
                VALUES (@Character, @Level, @Minutes, @Victory, @Date);";
                command.Parameters.Add(new SqliteParameter("@Character", character));
                command.Parameters.Add(new SqliteParameter("@Level", level));
                command.Parameters.Add(new SqliteParameter("@Minutes", minutes));
                command.Parameters.Add(new SqliteParameter("@Victory", victory ? 1 : 0));
                command.Parameters.Add(new SqliteParameter("@Date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                command.ExecuteNonQuery();

            }
        }
    }

    public struct GameHistoryEntry
    {
        public int runId;
        public string character;
        public int level;
        public float minutes;
        public bool victory;
        public string playedAt;
    }

    public System.Collections.Generic.List<GameHistoryEntry> LoadGameHistory()
    {
        var list = new System.Collections.Generic.List<GameHistoryEntry>();
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT run_id, character, level, minutes, victory, played_at FROM GameHistory ORDER BY run_id DESC;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        GameHistoryEntry entry = new GameHistoryEntry();
                        entry.runId = reader.GetInt32(0);
                        entry.character = reader.GetString(1);
                        entry.level = reader.GetInt32(2);
                        entry.minutes = (float)reader.GetDouble(3);
                        entry.victory = reader.GetInt32(4) == 1;
                        entry.playedAt = reader.GetString(5);
                        list.Add(entry);
                    }
                }
            }
        }

        return list;
    }

    public void DeleteGameHistoryEntry(int runId)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM GameHistory WHERE run_id = @Id;";
                command.Parameters.Add(new SqliteParameter("@Id", runId));
                command.ExecuteNonQuery();
            }
        }
    }

}