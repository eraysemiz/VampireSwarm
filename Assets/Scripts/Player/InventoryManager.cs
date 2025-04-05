using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(3);
    public int[] weaponLevels = new int[6];
    public List<Image> weaponUISlots = new List<Image>(3);
    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(3);
    public int[] passiveItemLevels = new int[6];
    public List<Image> passiveItemUISlots = new List<Image>(3);

    public List<WeaponEvolutionBlueprint> weaponEvolutions = new List<WeaponEvolutionBlueprint>();

    [System.Serializable]
    public class WeaponUpgrade
    {
        public int weaponUpgradeIndex;
        public GameObject initialWeapon;
        public WeaponScriptableObject weaponData;
    }

    [System.Serializable]
    public class PassiveItemUpgrade
    {
        public int passiveItemUpgradeIndex;
        public GameObject initialPassiveItem;
        public PassiveItemScriptableObject passiveItemData;
    }

    [System.Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    public List<WeaponUpgrade> weaponUpgradeOptions = new List<WeaponUpgrade>();
    public List<PassiveItemUpgrade> passiveItemUpgradeOptions = new List<PassiveItemUpgrade>(); 
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();

    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();    
    }

    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        weaponSlots[slotIndex] = weapon;
        weaponLevels[slotIndex] = weapon.weaponData.Level;
        weaponUISlots[slotIndex].sprite = weapon.weaponData.Icon;
        weaponUISlots[slotIndex].enabled = true;

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void AddPassiveItem(int slotIndex, PassiveItem passiveItem)
    {
        passiveItemSlots[slotIndex] = passiveItem;
        passiveItemLevels[slotIndex] = passiveItem.passiveItemData.Level;
        passiveItemUISlots[slotIndex].sprite = passiveItem.passiveItemData.Icon;
        passiveItemUISlots[slotIndex].enabled = true;

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (weaponSlots.Count > slotIndex)
        {
            WeaponController weapon = weaponSlots[slotIndex];
            if (!weapon.weaponData.NextLevelPrefab)
            {
                Debug.LogError("NO NEXT LEVEL " + weapon.name );
                return;
            }
            GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedWeapon.transform.SetParent(transform); // Yeni silahý player ýn child ý yap
            AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>());
            Destroy(weapon.gameObject);
            weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().weaponData.Level;

            weaponUpgradeOptions[upgradeIndex].weaponData = upgradedWeapon.GetComponent<WeaponController>().weaponData;

            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if (passiveItemSlots.Count > slotIndex)
        {
            PassiveItem passiveItem = passiveItemSlots[slotIndex];
            if (!passiveItem.passiveItemData.NextLevelPrefab)
            {
                Debug.LogError("NO NEXT LEVEL " + passiveItem.name);
                return;
            }
            GameObject upgradedPassiveItem = Instantiate(passiveItem.passiveItemData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedPassiveItem.transform.SetParent(transform); // Yeni item i player ýn child ý yap
            AddPassiveItem(slotIndex, upgradedPassiveItem.GetComponent<PassiveItem>());
            Destroy(passiveItem.gameObject);
            weaponLevels[slotIndex] = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData.Level;

            passiveItemUpgradeOptions[upgradeIndex].passiveItemData = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData;

            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    void ApplyUpgradeOptions()
    {
        List<WeaponUpgrade> availableWeaponUpgradeOptions = new List<WeaponUpgrade>(weaponUpgradeOptions);
        List<PassiveItemUpgrade> availablePassiveItemUpgradeOptions = new List<PassiveItemUpgrade>(passiveItemUpgradeOptions);

        foreach (var upgradeOption in upgradeUIOptions)
        {
            int upgradeType;
            if (availableWeaponUpgradeOptions.Count == 0 || availablePassiveItemUpgradeOptions.Count == 0)
            {
                if (availableWeaponUpgradeOptions.Count == 0)
                    upgradeType = 2;
                else if (availablePassiveItemUpgradeOptions.Count == 0)
                    upgradeType = 1;
                else
                    return;
            }
            else
            {
                upgradeType = Random.Range(1, 3); // Silah veya pasif eþya arasýnda seçim yap, 1 silah, 2 pasif eþya
            }

            if (upgradeType == 1)
            {
                WeaponUpgrade choosenWeaponUpgrade = availableWeaponUpgradeOptions[Random.Range(0, availableWeaponUpgradeOptions.Count)];

                availableWeaponUpgradeOptions.Remove(choosenWeaponUpgrade);  // causes error

                if (choosenWeaponUpgrade != null)
                {
                    EnableUpgradeIU(upgradeOption);

                    bool newWeapon = false;
                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        if (weaponSlots[i] != null && weaponSlots[i].weaponData == choosenWeaponUpgrade.weaponData)
                        {
                            newWeapon = false;
                            if (!newWeapon)
                            {
                                if (!choosenWeaponUpgrade.weaponData.NextLevelPrefab)
                                {
                                    DisableUpgradeIU(upgradeOption);
                                    break;
                                }

                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, choosenWeaponUpgrade.weaponUpgradeIndex));    // Upgrade butonuna runtime da onclick eventi ekle
                                // Silah yükseltme butonundaki isim ve açýklama kýsmýna silahýn sonraki seviyesinin deðerlerini ata
                                upgradeOption.upgradeDescriptionDisplay.text = choosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Description;
                                upgradeOption.upgradeNameDisplay.text = choosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Name;
                            }
                            break;
                        }
                        else
                        {
                            newWeapon = true;
                        }
                    }
                    if (newWeapon)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnWeapon(choosenWeaponUpgrade.initialWeapon));
                        // eðer silah yeniyse baþlangýç isim ve açýklama deðerlerini ata
                        upgradeOption.upgradeDescriptionDisplay.text = choosenWeaponUpgrade.weaponData.Description;
                        upgradeOption.upgradeNameDisplay.text = choosenWeaponUpgrade.weaponData.Name;
                    }

                    upgradeOption.upgradeIcon.sprite = choosenWeaponUpgrade.weaponData.Icon;
                }
            }
            else if (upgradeType == 2)
            {
                PassiveItemUpgrade choosenPassiveItemUpgrade = availablePassiveItemUpgradeOptions[Random.Range(0, availablePassiveItemUpgradeOptions.Count)];

                availablePassiveItemUpgradeOptions.Remove(choosenPassiveItemUpgrade);       // causes error

                if (choosenPassiveItemUpgrade != null)
                {
                    EnableUpgradeIU(upgradeOption);

                    bool newPassiveItem = false;
                    for (int i = 0; i < passiveItemSlots.Count; i++)
                    {
                        if (passiveItemSlots[i] != null && passiveItemSlots[i].passiveItemData == choosenPassiveItemUpgrade.passiveItemData)
                        {
                            newPassiveItem = false;
                            if (!newPassiveItem)
                            {
                                if (!choosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab)
                                {
                                    DisableUpgradeIU(upgradeOption);
                                    break;
                                }

                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, choosenPassiveItemUpgrade.passiveItemUpgradeIndex));    // Upgrade butonuna runtime da onclick eventi ekle 
                                // Pasif eþya yükseltme butonundaki isim ve açýklama kýsmýna silahýn sonraki seviyesinin deðerlerini ata
                                upgradeOption.upgradeDescriptionDisplay.text = choosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Description;
                                upgradeOption.upgradeNameDisplay.text = choosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Name;
                            }
                            break;
                        }
                        else
                        {
                            newPassiveItem = true;
                        }
                    }
                    if (newPassiveItem)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnPassiveItem(choosenPassiveItemUpgrade.initialPassiveItem));
                        // eðer pasif eþya yeniyse baþlangýç isim ve açýklama deðerlerini ata
                        upgradeOption.upgradeDescriptionDisplay.text = choosenPassiveItemUpgrade.passiveItemData.Description;
                        upgradeOption.upgradeNameDisplay.text = choosenPassiveItemUpgrade.passiveItemData.Name;
                    }

                    upgradeOption.upgradeIcon.sprite = choosenPassiveItemUpgrade.passiveItemData.Icon;
                }
            }
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach (var upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeIU(upgradeOption);
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    void DisableUpgradeIU(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    void EnableUpgradeIU(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }

    public List<WeaponEvolutionBlueprint> GetPossibleEvolutions()
    {
        List<WeaponEvolutionBlueprint> possibleEvolutions = new List<WeaponEvolutionBlueprint>();

        foreach (WeaponController weapon in weaponSlots)
        {
            if (weapon != null)
            {
                foreach (PassiveItem catalyst in passiveItemSlots)
                {
                    if (catalyst != null)
                    {
                        foreach (WeaponEvolutionBlueprint evolution in weaponEvolutions)
                        {
                            if (weapon.weaponData.Level >= evolution.baseWeaponData.Level && catalyst.passiveItemData.Level >= evolution.catalystPassiveItemData.Level)
                                possibleEvolutions.Add(evolution);
                        }
                    }
                }
            }
        }

        return possibleEvolutions;
    }


    public void EvolveWeapon(WeaponEvolutionBlueprint evolution)
    {
        for (int weaponSlotIndex = 0; weaponSlotIndex < weaponSlots.Count; weaponSlotIndex++)
        {
            WeaponController weapon = weaponSlots[weaponSlotIndex];

            if (!weapon)
            {  continue; }

            for (int catalystSlotIndex = 0; catalystSlotIndex < passiveItemSlots.Count; catalystSlotIndex++)
            {
                PassiveItem catalyst = passiveItemSlots[catalystSlotIndex];

                if (!catalyst)
                { continue; }

                if (weapon && catalyst && weapon.weaponData.Level >= evolution.baseWeaponData.Level && catalyst.passiveItemData.Level >= evolution.catalystPassiveItemData.Level)
                {
                    GameObject evolvedWeapon = Instantiate(evolution.evolvedWeapon, transform.position, Quaternion.identity);
                    WeaponController evolvedWeaponController = evolvedWeapon.GetComponent<WeaponController>();

                    evolvedWeapon.transform.SetParent(transform); // Evrim geçirmiþ silahý oyuncunun child ý yap
                    AddWeapon(weaponSlotIndex, evolvedWeaponController);
                    Destroy(weapon.gameObject);

                    // Silah seviyesini ve iconunu güncelle
                    weaponLevels[weaponSlotIndex] = evolvedWeaponController.weaponData.Level;
                    weaponUISlots[weaponSlotIndex].sprite = evolvedWeaponController.weaponData.Icon;

                    // Silah yükseltme seçeneklerini güncelle
                    weaponUpgradeOptions.RemoveAt(evolvedWeaponController.weaponData.EvolvedUpgradeToRemove);

                    Debug.LogWarning("Evolution Successfull");

                    return;
                }
            }
        }
    }
}
 