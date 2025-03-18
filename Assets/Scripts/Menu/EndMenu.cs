using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndMenu : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text scoreText;

    public DatabaseManager db;
    public string username;
    public int score;
    void Start()
    {
        DisplayEndMenu();   
    }

    public void DisplayEndMenu()
    {
        db.SavePlayerScore();
        username = PlayerStats.PlayerData.Username;
        score = PlayerStats.PlayerData.score;

        usernameText.text = username;
        scoreText.text = score.ToString();
    }

    public void NewGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("StartMenu");
    }
}
