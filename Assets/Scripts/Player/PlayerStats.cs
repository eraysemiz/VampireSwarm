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
    PlayerCollector collector;
    PlayerAnimator playerAnimator;

    [HideInInspector] public int weaponIndex;
    [HideInInspector] public int passiveItemIndex;

    [SerializeField] CharacterData.Stats actualStats;

    public CharacterData CharacterData
    {
        get { return characterData; }
        set { characterData = value; }
    }
    public CharacterData.Stats Stats
    {
        get { return actualStats; }
        set
        {
            actualStats = value;
        }
    }

    float health;
    public float CurrentHealth
    {
        get { return health; }
        set
        {
            // currentHealth değişkeni her değiştirildiğinde set bloğu çalışır ve eğer atanmaya çalışılan "value", mevcut değerden farklı ise atama işlemi yapılır
            if (health != value)
            {
                health = value;
                UpdateHealthBar();
            }
        }
    }


    // Oyuncunun seviyesi ve deneyim puanı
    public int experience = 0;
    public int level = 1;
    public int experienceCap = 0;

    //Class for defining a level range and the corresponding experience cap increase for that range
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }


    // Oyuncu hasar alma kısıtlamaları
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible = false;

    public List<LevelRange> levelRanges;

    // Potion timerları
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

    [Header("Visuals")]
    public ParticleSystem damageEffect;
    public ParticleSystem blockedEffect;
    public GameObject healEffect;
    public GameObject damageAnimation;

    void Awake()
    {
        characterData = CharacterSelector.GetData();
        if (CharacterSelector.instance) 
            CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<PlayerInventory>();
        collector = GetComponentInChildren<PlayerCollector>();

        baseStats = actualStats = characterData.stats;
        collector.SetRadius(actualStats.magnet);
        health = actualStats.maxHealth;

        playerAnimator = GetComponent<PlayerAnimator>();
        playerAnimator.SetAnimatorController(characterData.animationController); 
    }

    void Start()
    {
        inventory.Add(characterData.StartingWeapon);

        //Initialize the experience cap as the first experience cap increase
        experienceCap = levelRanges[0].experienceCapIncrease;
        
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

        //PositionHealthBarBelowPlayer();
        Recover();
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

        collector.SetRadius(actualStats.magnet);
    }
    public void IncreaseExperience(int amount)
    {
        experience += amount;

        LevelUpChecker();
        UpdateExpBar();
    }

    void LevelUpChecker()
    {
        if (experience >= experienceCap)
        {
            //Level up the player and reduce their experience by the experience cap
            level++;
            experience -= experienceCap;

            //Find the experience cap increase for the current level range
            int experienceCapIncrease = 0;
            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;

            UpdateLevelText();

            GameManager.instance.StartLevelUp();

            // If the experience still exceeds the experience cap, level up again.
            if (experience >= experienceCap) LevelUpChecker();
        }
    }

    void UpdateExpBar()
    {
        expBar.fillAmount = (float)experience / experienceCap;
    }

    void UpdateLevelText()
    {
        levelText.text = "LV " + level.ToString();
    }

    public void TakeDamage(float damage)
    {
        //If the player is not currently invincible, reduce health and start invincibility
        if (!isInvincible)
        {
            // Take armor into account before dealing the damage.
            damage -= actualStats.armor;

            if (damage > 0)
            {
                // Deal the damage.
                CurrentHealth -= damage;

                // If there is a damage effect assigned, play it.
                if (damageAnimation) Destroy(Instantiate(damageAnimation, transform.position, Quaternion.identity), 1f);

                if (CurrentHealth <= 0)
                {
                    Kill();
                }
            }
            else
            {
                // If there is a blocked effect assigned, play it.
                if (blockedEffect) Destroy(Instantiate(blockedEffect, transform.position, Quaternion.identity), 5f);
            }

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;
        }
    }

    void Kill()
    {
        
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.GameOver();
                
        }

    }

    public void RestoreHealth(float amount)
    {
        if (healEffect) Destroy(Instantiate(healEffect, transform.position, Quaternion.identity), 0.7f);
        if (CurrentHealth < actualStats.maxHealth)
        {
            Debug.Log("Restoring Health");
            CurrentHealth += amount;

            // Make sure the player's health doesn't exceed their maximum health
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }

    }

    public void UpdateHealthBar()
    {
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }

    void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += Stats.recovery * Time.deltaTime;

            if (CurrentHealth > actualStats.maxHealth)
                CurrentHealth = actualStats.maxHealth;
        }
        UpdateHealthBar();
    }
}
