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
    CharacterScriptableObjects characterData;
    public static ProjectileWeaponBehaviour knifeData;
    public static MeleeWeaponBehaviour garlicData;
    InventoryManager inventory;
    [HideInInspector] public int weaponIndex;
    [HideInInspector] public int passiveItemIndex;

    //public GameObject firstPassiveItemTest;

    // Þuanki statlar
    float currentHealth;
    float currentRecovery;
    float currentMoveSpeed;
    float currentMight;
    float currentProjectileSpeed;
    float currentMagnet;

    #region Current Stats
    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            // currentHealth deðiþkeni her deðiþtirildiðinde set bloðu çalýþýr ve eðer atanmaya çalýþýlan "value", mevcut deðerden farklý ise atama iþlemi yapýlýr
            if (currentHealth != value)
            {
                currentHealth = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = "Health: " + currentHealth;
                }
            }
        }
    }
    
    public float CurrentRecovery
    {
        get { return currentRecovery; }
        set
        {
            if (currentRecovery != value)
            {
                currentRecovery = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery;
                }
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set
        {
            if (currentMoveSpeed != value)
            {
                currentMoveSpeed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = "MoveSpeed: " + currentMoveSpeed;
                }
            }
        }
    }

    public float CurrentMight
    {
        get { return currentMight; }
        set
        {
            if (currentMight != value)
            {
                currentMight = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMightDisplay.text = "Might: " + currentMight;
                }
            }
        }
    }

    public float CurrentProjectileSpeed
    {
        get { return currentProjectileSpeed; }
        set
        {
            if (currentProjectileSpeed != value)
            {
                currentProjectileSpeed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedDisplay.text = "ProjectileSpeed: " + currentProjectileSpeed;
                }
            }
        }
    }

    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set
        {
            if (currentMagnet != value)
            {
                currentMagnet = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet;
                }
            }
        }
    }
    #endregion

    float baseMoveSpeed;

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
    public float moveSpeedMultiplier = 1f;
    float speedBoostTimer;
    float speedBoostAmount = 0f;


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
        CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<InventoryManager>();

        baseMoveSpeed = characterData.MoveSpeed;
        CurrentHealth = characterData.MaxHealth;
        CurrentRecovery = characterData.Recovery;
        CurrentMoveSpeed = baseMoveSpeed;
        CurrentProjectileSpeed = characterData.ProjectileSpeed;
        CurrentMagnet = characterData.Magnet;
        CurrentMight = characterData.Might;

        knifeData = UnityEngine.Object.FindAnyObjectByType<ProjectileWeaponBehaviour>();

        garlicData = UnityEngine.Object.FindAnyObjectByType<MeleeWeaponBehaviour>();


        SpawnWeapon(characterData.StartingWeapon);
        //SpawnPassiveItem(firstPassiveItemTest);
    }

    void Start()
    {
        UpdateHealthBar();
        UpdateExpBar();
        UpdateLevelText();

        GameManager.instance.currentHealthDisplay.text = "Health: " + currentHealth;
        GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery;
        GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed;
        GameManager.instance.currentMightDisplay.text = "Might: " + currentMight;
        GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed;
        GameManager.instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet;

        GameManager.instance.AssingChosenCharacterUI(characterData);
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

        if (isSpeedBoosted)
        {
            if (speedBoostTimer > 0)
            {
                speedBoostTimer -= Time.deltaTime;
            }
            else
            {
                speedBoostAmount = 0;
                isSpeedBoosted = false;
                UpdateMoveSpeed();
            }
        }
        Recover();
        ScoreCalculator();
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
            if (damageEffect) Instantiate(damageEffect, transform.position, Quaternion.identity);

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
            GameManager.instance.AssingChosenWeaponAndPassiveItemsUI(inventory.weaponUISlots, inventory.passiveItemUISlots);
            GameManager.instance.GameOver();
                
        }

    }

    public void RestoreHealth(float amount)
    {
        if (CurrentHealth + amount < characterData.MaxHealth)
            CurrentHealth += amount;
        else
            CurrentHealth = characterData.MaxHealth;
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = CurrentHealth / characterData.MaxHealth;

    }

    public void UpdateMoveSpeed()
    {
        CurrentMoveSpeed = baseMoveSpeed * moveSpeedMultiplier + speedBoostAmount;
    }

    public void BoostSpeed(float boostAmount, float boostDuration)
    {
        if (!isSpeedBoosted)
        {
            speedBoostAmount = boostAmount;
            isSpeedBoosted = true;
            speedBoostTimer = boostDuration;
            UpdateMoveSpeed();
        }
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
        if (CurrentHealth < characterData.MaxHealth)
        {
            CurrentHealth += currentRecovery * Time.deltaTime;

            if (CurrentHealth > characterData.MaxHealth)
                CurrentHealth = characterData.MaxHealth;
        }
    }

    public void SpawnWeapon(GameObject weapon)
    {
        // Envanterde boþ slot var mý kontrol
        if (weaponIndex >= inventory.weaponSlots.Count)
            Debug.LogError("Inventory slots are already full");

        // Baþlangýç silahýný spawnla
        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform);   // Silahý player ýn child ý yap 
        inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>());
        weaponIndex++;
    
    }

    public void SpawnPassiveItem(GameObject passiveItem)
    {
        // Envanterde boþ slot var mý kontrol
        if (passiveItemIndex >= inventory.passiveItemSlots.Count)
            Debug.LogError("Inventory slots are already full");

        // Baþlangýç passive itemini spawnla
        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform);  
        inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>());
        passiveItemIndex++;

    }

    public static class PlayerData
    {
        public static string Username;
        public static int score;
    }


}
