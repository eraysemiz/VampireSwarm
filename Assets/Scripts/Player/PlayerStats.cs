using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    CharacterData characterData;
    public CharacterData.Stats baseStats;
    PlayerInventory inventory;
    PlayerAnimator playerAnimator;
    [HideInInspector] public int weaponIndex;
    [HideInInspector] public int passiveItemIndex;

    [SerializeField] CharacterData.Stats actualStats;

    float health;

    #region Current Stats
    public float CurrentHealth
    {
        get { return health; }
        set
        {
            // currentHealth deðiþkeni her deðiþtirildiðinde set bloðu çalýþýr ve eðer atanmaya çalýþýlan "value", mevcut deðerden farklý ise atama iþlemi yapýlýr
            if (health != value)
            {
                health = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = string.Format(
                        "Health: {0} / {1}",
                        health, actualStats.maxHealth
                    );
                }
            }
        }
    }

    public float MaxHealth
    {
        get { return actualStats.maxHealth; }

        // If we try and set the max health, the UI interface
        // on the pause screen will also be updated.
        set
        {
            //Check if the value has changed
            if (actualStats.maxHealth != value)
            {
                actualStats.maxHealth = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = string.Format(
                        "Health: {0} / {1}",
                        health, actualStats.maxHealth
                    );
                }
                //Update the real time value of the stat
                //Add any additional logic here that needs to be executed when the value changes
            }
        }
    }

    public float CurrentRecovery
    {
        get { return Recovery; }
        set { Recovery = value; }
    }

    public float Recovery
    {
        get { return actualStats.recovery; }
        set
        {
            //Check if the value has changed
            if (actualStats.recovery != value)
            {
                actualStats.recovery = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + actualStats.recovery;
                }
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get { return MoveSpeed; }
        set { MoveSpeed = value; }
    }
    public float MoveSpeed
    {
        get { return actualStats.moveSpeed; }
        set
        {
            //Check if the value has changed
            if (actualStats.moveSpeed != value)
            {
                actualStats.moveSpeed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + actualStats.moveSpeed;
                }
            }
        }
    }

    public float CurrentMight
    {
        get { return Might; }
        set { Might = value; }
    }
    public float Might
    {
        get { return actualStats.might; }
        set
        {
            //Check if the value has changed
            if (actualStats.might != value)
            {
                actualStats.might = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMightDisplay.text = "Might: " + actualStats.might;
                }
            }
        }
    }

    public float CurrentProjectileSpeed
    {
        get { return Speed; }
        set { Speed = value; }
    }
    public float Speed
    {
        get { return actualStats.speed; }
        set
        {
            //Check if the value has changed
            if (actualStats.speed != value)
            {
                actualStats.speed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + actualStats.speed;
                }
            }
        }
    }

    public float CurrentMagnet
    {
        get { return Magnet; }
        set { Magnet = value; }
    }
    public float Magnet
    {
        get { return actualStats.magnet; }
        set
        {
            //Check if the value has changed
            if (actualStats.magnet != value)
            {
                actualStats.magnet = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetDisplay.text = "Magnet: " + actualStats.magnet;
                }
            }
        }
    }
    #endregion

    // Oyuncunun seviyesi ve deneyim puaný
    [HideInInspector] public int experience = 0;
    [HideInInspector] public int level = 1;
    [HideInInspector] public int experienceCap = 100;
    [HideInInspector] public int experienceCapIncrease;

    // Oyuncu hasar alma kýsýtlamalarý
    [HideInInspector] public float invincibilityDuration;
    [HideInInspector] float invincibilityTimer;
    [HideInInspector] bool isInvincible = false;

    // Potion timerlarý
    [HideInInspector] public float oldMoveSpeed;
    [HideInInspector] public bool isSpeedBoosted = false;
    [HideInInspector] public Coroutine activeSpeedBoostCoroutine;


    // Database
    [HideInInspector] public int playerScore;
    [HideInInspector] public string playerName;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TextMeshProUGUI levelText;

    public ParticleSystem damageEffect;


    void Awake()
    {
        characterData = CharacterSelector.GetData();
        if (CharacterSelector.instance) 
            CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<PlayerInventory>();

        baseStats = actualStats = characterData.stats;
        health = actualStats.maxHealth;

        playerAnimator = GetComponent<PlayerAnimator>();
        playerAnimator.SetAnimatorController(characterData.animationController); 
    }

    void Start()
    {
        inventory.Add(characterData.StartingWeapon);
            
        GameManager.instance.currentHealthDisplay.text = "Health: " + CurrentHealth;
        GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + CurrentRecovery;
        GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + CurrentMoveSpeed;
        GameManager.instance.currentMightDisplay.text = "Might: " + CurrentMight;
        GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + CurrentProjectileSpeed;
        GameManager.instance.currentMagnetDisplay.text = "Magnet: " + CurrentMagnet;

        GameManager.instance.AssingChosenCharacterUI(characterData);

        UpdateHealthBar();
        UpdateExpBar();
        UpdateLevelText();
    }

    void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if (isInvincible)
        {
            isInvincible = false;
        }

        Recover();
        ScoreCalculator();
    }

    public void RecalculateStats()
    {
        actualStats = baseStats;
        foreach (PlayerInventory.Slot s in inventory.passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p)
            {
                actualStats += p.GetBoosts();
            }
        }

        Recovery = actualStats.recovery;
        MoveSpeed = actualStats.moveSpeed;
        Might = actualStats.might;
        Speed = actualStats.speed;
        Magnet = actualStats.magnet;

    }
    public void IncreaseExperience(int amount)
    {
        experience += amount;

        LevelUpChecker();   
        UpdateExpBar();
    }

    void LevelUpChecker()
    {
        if (experience > experienceCap)
        {
            level++;
            experience -= experienceCap;
            experienceCap += experienceCapIncrease;

            UpdateLevelText();
            GameManager.instance.StartLevelUp();
        }
    }

    void UpdateExpBar()
    {
        expBar.fillAmount = (float) experience / experienceCap;
    }

    void UpdateLevelText()
    {
        levelText.text = "LV " + level.ToString();
    }

    public void TakeDamage(float damage)
    {
        if (!isInvincible)
        {
            CurrentHealth -= damage;
            if (damageEffect) Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

            if (CurrentHealth <= 0)
            {
                Kill();
            }
            UpdateHealthBar();
        }
    }

    void Kill()
    {
        
        if (!GameManager.instance.isGameOver)
        {
            ScoreCalculator(); // ??
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.AssingChosenWeaponAndPassiveItemsUI(inventory.weaponSlots, inventory.passiveSlots);
            GameManager.instance.GameOver();
                
        }

    }

    public void RestoreHealth(float amount)
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;

            // Make sure the player's health doesn't exceed their maximum health
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }

        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }

    public void ApplySpeedBoost(float speedMultiplier = 2f, float duration = 5f)
    {
        if (!isSpeedBoosted)
        {
            oldMoveSpeed = MoveSpeed;
            MoveSpeed *= speedMultiplier;
            isSpeedBoosted = true;
        }

        // Reset timer if one is already running
        if (activeSpeedBoostCoroutine != null)
        {
            StopCoroutine(activeSpeedBoostCoroutine);
        }

        activeSpeedBoostCoroutine = StartCoroutine(RemoveSpeedBoostAfterDelay(duration));

        if (GameManager.instance != null)
        {
            Debug.LogWarning("SpeedBoost");
            GameManager.GenerateFloatingText("Speed Boost!", transform);
        }
           
    }

    private IEnumerator RemoveSpeedBoostAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        MoveSpeed = oldMoveSpeed;
        isSpeedBoosted = false;
        activeSpeedBoostCoroutine = null;
    }


    public void ScoreCalculator()
    {
        int minionScore = EnemySpawner.minionKillCount * 10;
        int miniBossScore = EnemySpawner.miniBossKillCount * 50;
        int finalBossScore = EnemySpawner.finalBossKillCount * 1000;

        playerScore = minionScore + miniBossScore + finalBossScore;
        PlayerStats.PlayerData.score = playerScore;
    }

    void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;
            CurrentHealth += Recovery * Time.deltaTime;

            if (CurrentHealth > actualStats.maxHealth)
                CurrentHealth = actualStats.maxHealth;
        }

        UpdateHealthBar();
    }

    [System.Obsolete("Old function that is kept to maintain compatibility with the InventoryManager. Will be removed soon.")]
    public void SpawnWeapon(GameObject weapon)
    {
        // Envanterde boþ slot var mý kontrol
        if (weaponIndex >= inventory.weaponSlots.Count)
            Debug.LogError("Inventory slots are already full");

        // Baþlangýç silahýný spawnla
        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform);   // Silahý player ýn child ý yap 
       // inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>());
        weaponIndex++;
    
    }

    [System.Obsolete("No need to spawn passive items directly now.")]
    public void SpawnPassiveItem(GameObject passiveItem)
    {
        
        

        // Baþlangýç passive itemini spawnla
        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform);  
       // inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>());
        passiveItemIndex++;

    }

    public static class PlayerData
    {
        public static string Username;
        public static int score;
    }


}
