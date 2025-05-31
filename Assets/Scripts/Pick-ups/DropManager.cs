using JetBrains.Annotations;
using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static DropManager;

public class DropManager : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public string name;
        public GameObject item;
        public float dropRate;
    }
    public bool active = false;
    public List<Drops> drops;
    public List<GameObject> spawnedDrops;

    [Header("Treasure Chest Settings")]
    public bool dropTreasureChests = false;

    [Tooltip("Chest drop chance in percentage (0–100)")]
    [ShowIf("dropTreasureChests")] [UnityEngine.Range(0, 100)] public int chestDropRate;
    [ShowIf("dropTreasureChests")] public GameObject chestPrefab;



    void OnDestroy()
    {
        if (!active || !gameObject.scene.isLoaded)
            return;


        float rand = Random.Range(0f, 100f);

        if (CompareTag("Enemy") || CompareTag("MiniBoss"))
        {
            GameObject spawnedGem;

            if (CompareTag("MiniBoss"))
            {
                // Belirli bir ihtimalle sandýk düþür
                if (rand <= chestDropRate)
                {
                    GameObject chest = Instantiate(chestPrefab, transform.position, Quaternion.identity);
                    spawnedDrops.Add(chest);
                }
            }


            if (rand <= drops[2].dropRate)
            {
                spawnedGem = Instantiate(drops[2].item, transform.position, Quaternion.identity);
            }
            else if (rand <= drops[1].dropRate)
            {
                spawnedGem = Instantiate(drops[1].item, transform.position, Quaternion.identity);
            }
            else
            {
                spawnedGem = Instantiate(drops[0].item, transform.position, Quaternion.identity);
            }
            spawnedDrops.Add(spawnedGem);
            //Destroy(spawnedGem, 30f);
        }
        if (CompareTag("Prop"))
        {
            foreach (Drops potion in drops)
            {
                if (rand <= potion.dropRate)
                {
                    GameObject spawnedPotion = Instantiate(potion.item, transform.position, Quaternion.identity);
                    spawnedDrops.Add(spawnedPotion);
                    Destroy(spawnedPotion, 30f);
                    break;
                }
            }
        }
        if (CompareTag("FinalBoss"))
        {

            int numberOfGems = 20 ; // Specify how many gems you want to spawn
            for (int i = 0; i < numberOfGems; i++)
            {
                Vector3 spawnPosition = transform.position + Random.insideUnitSphere * 2f; // Slightly randomize the spawn position
                GameObject spawnedGem = Instantiate(drops[2].item, spawnPosition, Quaternion.identity);
                spawnedDrops.Add(spawnedGem);
                Destroy(spawnedGem, 30f);
            }
        }
    }
}
