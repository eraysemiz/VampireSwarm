using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    List<RewardInfo> rewardInfos;

    private void OnTriggerEnter2D(Collider2D col)
    {
        PlayerInventory p = col.GetComponent<PlayerInventory>();
        if (p)
        {
            bool randomBool = Random.Range(0, 2) == 0;

            OpenTreasureChest(p, randomBool);
        }
    }

    public void OpenTreasureChest(PlayerInventory inventory, bool isHigherTier)
    {
        foreach (var slot in inventory.weaponSlots)
        {
            Weapon w = slot.item as Weapon;
            if (w == null || w.data?.evolutionData == null)
                continue;

            foreach (ItemData.Evolution evo in w.data.evolutionData)
            {
                if (evo.condition == ItemData.Evolution.Condition.treasureChest
                    && w.AttemptEvolution(evo, 0))
                {
                    // Outcome data’sýný al
                    var data = evo.outcome.itemType;

                    // Bu yeni silah 1. seviyeden baþlar
                    var lvlData = data.GetLevelData(1);

                    rewardInfos = new List<RewardInfo>();
                    rewardInfos.Add(new RewardInfo
                    {
                        icon = data.icon,
                        displayName = lvlData.name,
                        description = lvlData.description,
                        levelText = "New"
                    });

                    GameManager.instance.StartTreasureChestScreen(rewardInfos);
                    return;
                }
            }
        }

        List<ItemData> pool = new List<ItemData>();
        foreach (var s in inventory.weaponSlots)
            if (s.item) pool.Add(s.item.data);
        foreach (var s in inventory.passiveSlots)
            if (s.item) pool.Add(s.item.data);

        if (pool.Count == 0) return;

        // 2) Rastgele 1–3 item seç (her item bir kez)
        int giftCount = Random.Range(1, Mathf.Min(pool.Count, 3) + 1);
        var selected = new List<ItemData>();
        for (int i = 0; i < giftCount; i++)
        {
            int idx = Random.Range(0, pool.Count);
            selected.Add(pool[idx]);
            pool.RemoveAt(idx);
        }

        rewardInfos = new List<RewardInfo>();
        foreach (var data in selected)
        {
            var item = inventory.Get(data);
            int nextLevel = item != null ? Mathf.Min(item.currentLevel + 1, data.maxLevel) : 1;
            var lvlData = data.GetLevelData(nextLevel);

            // envantere uygula
            if (item != null) inventory.LevelUp(item);
            else inventory.Add(data);

            // RewardInfo yarat
            rewardInfos.Add(new RewardInfo
            {
                icon = data.icon,
                displayName = lvlData.name,
                description = lvlData.description,
                levelText = item != null ? $"Level {nextLevel}" : "New"
            });
        }

        GameManager.instance.StartTreasureChestScreen(rewardInfos);
        Destroy(gameObject);
    }

}
