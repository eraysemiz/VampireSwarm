using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject currentChunk;
    public List<GameObject> chunkLocations;

    void Start()
    {
        int rand = Random.Range(0, terrainChunks.Count);
        foreach (GameObject location in chunkLocations)
        {
            Vector3 position = location.transform.position;
            GameObject Chunks = Instantiate(terrainChunks[rand], position, Quaternion.identity);
        }
    }
}