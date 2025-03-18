using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    MeleeWeaponBehaviour weaponB;
    DatabaseManager db;
    PauseMenu pauseMenu;
    StartMenu startMenu;
    EndMenu endMenu;


    private void Start()
    {
        pauseMenu = Object.FindAnyObjectByType<PauseMenu>();
        endMenu = Object.FindAnyObjectByType<EndMenu>();
        weaponB = Object.FindAnyObjectByType<MeleeWeaponBehaviour>();
        db = Object.FindAnyObjectByType<DatabaseManager>();
        
    }
    public void OnContinueGameButtonClick()
    {
        pauseMenu.Continue();
    }

    public void OnEndGameButtonClick()
    {
        pauseMenu.EndGame();
    }

    public void OnNewGameButtonClick()
    {
        endMenu.NewGame();
    }
    public void OnStartGameButtonClick()
    {
        SceneManager.LoadScene("Game");
     
    }
    public void OnScoreboardButtonClick()
    {
       db.LoadScoreBoard();
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }

}
