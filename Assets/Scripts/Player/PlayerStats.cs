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
            // currentHealth deðiþkeni her deðiþtirildiðinde set bloðu çalýþýr ve eðer atanmaya çalýþýlan "value", mevcut deðerden farklý ise atama iþlemi yapýlýr
            if (health != value)
            {
                health = value;
                UpdateHealthBar();
            }
        }
    }


    // Oyuncunun seviyesi ve deneyim puaný
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


    // Oyuncu hasar alma kýsýtlamalarý
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible = false;

    public List<LevelRange> levelRanges;

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

    [Header("Visuals")]
    public ParticleSystem damageEffect;
    public ParticleSystem blockedEffect;


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
        Debug.Log("EXP BAR UPDATE");
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
                if (damageEffect) Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);

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
        if (CurrentHealth < actualStats.maxHealth)
        {
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

    /*public void ApplySpeedBoost(float speedMultiplier = 2f, float duration = 5f)
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
    }*/


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

    public static class PlayerData
    {
        public static string Username;
        public static int score;
    }


}
