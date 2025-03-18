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
    public int weaponIndex;
    public int passiveItemIndex;

    public GameObject firstPassiveItemTest;

    // Þuanki statlar
    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentRecovery;
    [HideInInspector] public float currentMoveSpeed;
    [HideInInspector] public float currentMight;
    [HideInInspector] public float currentProjectileSpeed;
    [HideInInspector] public float currentMagnet;
    

    public float MaxHealth;
    public float MaxMoveSpeed;

    // Oyuncunun seviyesi ve deneyim puaný
    public int experience = 0;
    public int level = 1;
    public int maxLevel;
    public int experienceCap = 100;
    public int experienceCapIncrease;

    // Oyuncu hasar alma kýsýtlamalarý
    [HideInInspector] public float invincibilityDuration;
    [HideInInspector] float invincibilityTimer;
    [HideInInspector] bool isInvincible = false;

    // Potion timerlarý
    [HideInInspector] public float speedBoostDuration;
    [HideInInspector] float speedBoostTimer;
    [HideInInspector] bool isBoosted = false;


    // Database
    [HideInInspector] public int playerScore;
    [HideInInspector] public string playerName;

    [HideInInspector] public Image healthBar;
    [HideInInspector] public TMP_Text levelText;
    [HideInInspector] public ParticleSystem damageEffect;


    void Awake()
    {
        characterData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<InventoryManager>();

        MaxHealth = characterData.MaxHealth;
        MaxMoveSpeed = characterData.MoveSpeed;
        currentHealth = characterData.MaxHealth;
        currentRecovery = characterData.Recovery;
        currentMoveSpeed = characterData.MoveSpeed;
        currentMagnet = characterData.Magnet;
        currentMight = characterData.Might;

        knifeData = UnityEngine.Object.FindAnyObjectByType<ProjectileWeaponBehaviour>();

        garlicData = UnityEngine.Object.FindAnyObjectByType<MeleeWeaponBehaviour>();


        SpawnWeapon(characterData.StartingWeapon);
        SpawnPassiveItem(firstPassiveItemTest);
    }

    void Start()
    {
        UpdateHealthBar();
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

        if (isBoosted)
        {
            if (speedBoostTimer > 0)
            {
                speedBoostTimer -= Time.deltaTime;
            }
            else
            {
                currentMoveSpeed = characterData.MoveSpeed;
                isBoosted = false;
            }
        }
        Recover();
        ScoreCalculator();
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;

        LevelUpChecker();   
    }

    void LevelUpChecker()
    {
        if (experience > experienceCap && level <= maxLevel)
        {
            level++;
            experience -= experienceCap;
            experienceCap += experienceCapIncrease;
            UpdateLevelText();
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isInvincible)
        {
            currentHealth -= damage;
            if (damageEffect) Instantiate(damageEffect, transform.position, Quaternion.identity);

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

            if (currentHealth <= 0)
            {
                Kill();
            }
            UpdateHealthBar();
        }
    }

    void Kill()
    {
        ScoreCalculator();
        SceneManager.LoadScene("EndMenu");
        Time.timeScale = 0;
    }

    public void RestoreHealth(float amount)
    {
        if (currentHealth + amount < characterData.MaxHealth)
            currentHealth += amount;
        else
            currentHealth = characterData.MaxHealth;
    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = currentHealth / characterData.MaxHealth;

    }

    public void UpdateLevelText()
    {
        levelText.text = "Level: " + level.ToString();
    }
    public void SpeedBoost(float amount)
    {
        if (!isBoosted)
        {
            currentMoveSpeed *= amount;
            isBoosted = true;
            speedBoostTimer = speedBoostDuration;
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
        if (currentHealth < characterData.MaxHealth)
        {
            currentHealth += currentRecovery * Time.deltaTime;

            if (currentHealth > characterData.MaxHealth)
                currentHealth = characterData.MaxHealth;
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
