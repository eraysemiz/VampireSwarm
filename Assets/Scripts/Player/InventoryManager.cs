using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<WeaponController> weaponSlots = new List<WeaponController>(3);
    public int[] weaponLevels = new int[3];
    public List<Image> weaponUISlots = new List<Image>(6);
    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(3);
    public int[] passiveItemLevels = new int[3];
    public List<Image> passiveItemUISlots = new List<Image>(6);

    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        weaponSlots[slotIndex] = weapon;
        weaponLevels[slotIndex] = weapon.weaponData.Level;
        weaponUISlots[slotIndex].sprite = weapon.weaponData.Icon;
        weaponUISlots[slotIndex].enabled = true;
    }

    public void AddPassiveItem(int slotIndex, PassiveItem passiveItem)
    {
        passiveItemSlots[slotIndex] = passiveItem;
        passiveItemLevels[slotIndex] = passiveItem.passiveItemData.Level;
        passiveItemUISlots[slotIndex].sprite = passiveItem.passiveItemData.Icon;
        passiveItemUISlots[slotIndex].enabled = true;
    }

    public void LevelUpWeapon(int slotIndex)
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
        }
    }

    public void LevelUpPassiveItem(int slotIndex)
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
            weaponLevels[slotIndex] = upgradedPassiveItem.GetComponent<WeaponController>().weaponData.Level;

        }
    }

}
