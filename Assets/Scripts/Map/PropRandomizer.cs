using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PropRandomizer : MonoBehaviour
{
    public List<GameObject> propSpawnPoint;
    public List<GameObject> props;

    [Tooltip("Time in seconds between prop respawns.")]
    public float respawnInterval = 180f;

    void Start()
    {
        SpawnProps();
        StartCoroutine(RespawnRoutine());
    }

    void SpawnProps()
    {
        foreach (GameObject point in propSpawnPoint)
        {
            // Destroy the previous prop if one exists
            if (point.transform.childCount > 0)
                Destroy(point.transform.GetChild(0).gameObject);

            int rand = Random.Range(0, props.Count);
            GameObject prop = Instantiate(props[rand], point.transform.position, Quaternion.identity);
            prop.transform.parent = point.transform;

        }
    }

    System.Collections.IEnumerator RespawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnInterval);
            SpawnProps();
        }
    }
}