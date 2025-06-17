using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void SceneChange(string name)
    {
        Time.timeScale = 1f;
        if (name == "Title Screen")
            MusicManager.PlayBackgroundMusic(true);
        else if (name == "Game")
            MusicManager.PauseBackgroundMusic();
        SceneManager.LoadScene(name);
    }
}
