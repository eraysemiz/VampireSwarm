using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    PlayerStats playerData;


    void Start()
    {
        playerData = Object.FindAnyObjectByType<PlayerStats>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
                Continue();
            else
                Pause();
        }
    }

    public void Continue()
    {
        SceneManager.UnloadSceneAsync("PauseMenu");
        Time.timeScale = 1;
        gameIsPaused = false;
    }

    public void Pause()
    {
        SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
        Time.timeScale = 0;
        gameIsPaused = true;
    }
    
    public void EndGame()
    {
        gameIsPaused = false;
        playerData.ScoreCalculator();
        SceneManager.LoadScene("EndMenu");
    }
}
