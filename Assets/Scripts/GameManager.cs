using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Oyun durumlarýný tanýmla
    public enum GameState 
    {
        Gameplay,
        Paused,
        GameOver,
        LevelUp
    }

    public GameState currentState;
    public GameState previousState;

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;

    [Header("Current Stat Displays")]
    public Text currentHealthDisplay;
    public Text currentRecoveryDisplay;
    public Text currentMoveSpeedDisplay;
    public Text currentMightDisplay;
    public Text currentProjectileSpeedDisplay;
    public Text currentMagnetDisplay;

    [Header("Result Screen Displays")]
    public Image chosenCharacterImage;
    public TextMeshProUGUI chosenCharacterName;
    public TextMeshProUGUI levelReachedDisplay;
    public TextMeshProUGUI timeSurvivedDisplay;
    public List<Image> chosenWeaponsUI = new List<Image>(3);
    public List<Image> chosenPassiveItemsUI = new List<Image>(3);

    [Header("StopWatch")]
    public float timeLimit; // Zaman limiti
    float stopwatchTime;    // Oyunda geçen süre
    public TextMeshProUGUI stopwatchDisplay;


    // Flags
    public bool isGameOver = false; // oyun bitti mi kontrol flag
    public bool choosingUpgrade = false;    // oyuncu seviye atlama ekranýnda mý kontrol

    public GameObject playerObject;     // Oyuncunun gameObjectine referans, sendMessage için kullanýlacak

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("EXTRA " + this + " DELETED");
            Destroy(gameObject);
        }
            

        DisableScreens();    
    }
    void Update()
    {
        switch (currentState)
        {
            case GameState.Gameplay:
                CheckForPauseAndResume();
                UpdateStopWatch();
                break;
            case GameState.Paused:
                CheckForPauseAndResume();
                break;
            case GameState.GameOver:
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f;
                    Debug.Log("GAME IS OVER");
                    DisplayResults();
                }
                break;
            case GameState.LevelUp:
                if (!choosingUpgrade)
                {
                    choosingUpgrade = true;
                    Time.timeScale = 0f;
                    Debug.Log("UPGRADE SCREEN LOADED");
                    levelUpScreen.SetActive(true);
                }
                break;
            default:
                Debug.LogWarning("STATE DOES NOT EXIST");
                break;
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }

    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
            Debug.Log("Game is paused");
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            /* oyuna devam ederken direkt oyun durumuna deðilde bir önceki duruma geçmemizin sebebi, eðer oyuncu oyun harici bir durumda oyunu durduysa
               o duruma dönmesini saðlamak için (level atlama durumunda dondurulan oyunun devam ettirildiðinde level atlama ekranýna dönmesi gibi) */
            ChangeState(previousState);
            Time.timeScale = 1;
            pauseScreen.SetActive(false);
            Debug.Log("Game is resumed");
        }
    }

    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }

    public void GameOver()
    {
        timeSurvivedDisplay.text = stopwatchDisplay.text;
        ChangeState(GameState.GameOver);
    }

    void DisplayResults()
    {
        resultsScreen.SetActive(true);
    }

    public void AssingChosenCharacterUI(CharacterScriptableObjects chosenCharacterData)
    {
        chosenCharacterImage.sprite = chosenCharacterData.Icon;
        chosenCharacterName.text = chosenCharacterData.Name;
    }

    public void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }

    public void AssingChosenWeaponAndPassiveItemsUI(List<Image> chosenWeaponsData, List<Image> chosenPassiveItemsData)
    {
        if (chosenWeaponsData.Count != chosenPassiveItemsUI.Count || chosenPassiveItemsData.Count != chosenPassiveItemsUI.Count)
        {
            Debug.Log("Chosen weapon and passive items data lists have different lengths");
            return;
        }

        // Seçilmiþ silahlarýn ve pasif eþyalarýn verisini sonuç ekranýna ata
        for (int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            // Seçilmiþ silahýn sprite ý var mý kontrol
            if (chosenWeaponsData[i].sprite)
            {
                // Seçilen silahýn UI daki karþýlýk gelen yerini aktif hale getir ve UI daki image ý silahýn iconu yap
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].sprite = chosenWeaponsData[i].sprite;
            }
            else
            {
                // Eðer sprite yoksa UI daki karþýlýk gelen yeri devre dýþý býrak
                chosenWeaponsUI[i].enabled = false;
            }
        }

        for (int i = 0; i < chosenPassiveItemsUI.Count; i++)
        {
            // Seçilmiþ eþyanýn sprite ý var mý kontrol
            if (chosenPassiveItemsData[i].sprite)
            {
                // Seçilen eþyanýn UI daki karþýlýk gelen yerini aktif hale getir ve UI daki image ý eþyanýn iconu yap
                chosenPassiveItemsUI[i].enabled = true;
                chosenPassiveItemsUI[i].sprite = chosenPassiveItemsData[i].sprite;
            }
            else
            {
                // Eðer sprite yoksa UI daki karþýlýk gelen yeri devre dýþý býrak
                chosenPassiveItemsUI[i].enabled = false;
            }
        }
    }

    void UpdateStopWatch()
    {
        stopwatchTime += Time.deltaTime;

        UpdateStopWatchDisplay();

        if (stopwatchTime > timeLimit)
        {
            GameOver();
        }
    }

    void UpdateStopWatchDisplay()
    {
        int minutes = Mathf.FloorToInt(stopwatchTime / 60);
        int seconds = Mathf.FloorToInt(stopwatchTime % 60);

        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        playerObject.SendMessage("RemoveAndApplyUpgrades");
    }

    public void EndLevelUp()
    {
        choosingUpgrade = false;
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
    }
}
