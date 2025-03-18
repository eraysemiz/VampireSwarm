using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    GameObject minion;
    [SerializeField]
    GameObject miniBoss;
    [SerializeField]
    GameObject FinalBoss;

    PlayerStats playerData;
    Transform playerLocation;

    int maxMinions = 4;
    int maxMiniBosses = 1;
    bool isFinalBossSpawned = false;
    int waveCount = 0;

    float spawnInterval;


    // Database
    public static int minionKillCount = 0;
    public static int miniBossKillCount = 0;
    public static int finalBossKillCount = 0;

    void Start()
    {
        playerLocation = Object.FindAnyObjectByType<PlayerStats>().transform;
        playerData = Object.FindAnyObjectByType<PlayerStats>();

        StartCoroutine(SpawnMobs());
    }

    IEnumerator SpawnMobs()
    {
        while (true)
        {
            spawnInterval = 10;

            // Spawn minions
            for (int i = 0; i < maxMinions; i++)
            {
                Vector2 spawnPosition = new Vector2(
                    playerLocation.position.x + Random.Range(-20f, 20f),
                    playerLocation.position.y + Random.Range(-20f, 20f));
                Instantiate(minion, spawnPosition, Quaternion.identity);
            }

            // Spawn mini-bosses
            for (int i = 0; i < maxMiniBosses; i++)
            {
                Vector2 spawnPosition = new Vector2(
                    playerLocation.position.x + Random.Range(-20f, 20f),
                    playerLocation.position.y + Random.Range(-20f, 20f));
                Instantiate(miniBoss, spawnPosition, Quaternion.identity);
            }
            maxMinions++;
            waveCount++;
            if (waveCount % 10 == 0) { maxMiniBosses++; }

            if (playerData.level >= 20 && !isFinalBossSpawned)
            {
                Vector2 spawnPosition = new Vector2(
                        playerLocation.position.x + Random.Range(-20f, 20f),
                        playerLocation.position.y + Random.Range(-20f, 20f));
                Instantiate(FinalBoss, spawnPosition, Quaternion.identity);
                isFinalBossSpawned = true;
                break;
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        
    }
}
