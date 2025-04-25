using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks; // List of chunk prefabs
    public GameObject currentChunk;
    public List<GameObject> chunkLocations; // List to hold the chunk positions

    public int gridSize = 9;      // Grid size (9x9)
    public float spacing = 40f;    // Distance between the chunks

    void Start()
    {
        // First, create the grid with the center at (0,0)
        CreateGrid();

        // If chunkLocations list is not empty, generate the chunks
        if (chunkLocations.Count > 0)
        {
            GenerateChunks();
        }
    }

    // Function to create the grid with the center at (0,0)
    void CreateGrid()
    {
        // Calculate the offset to center the grid
        float offset = (gridSize - 1) * spacing / 2f;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                // Calculate the position with respect to the center
                Vector3 position = new Vector3((x * spacing) - offset, (y * -spacing) + offset, 0); // Offsetting by half the grid size

                // Create a new GameObject for each location
                GameObject location = new GameObject($"Location_{x * gridSize + y + 1}");
                location.transform.position = position; // Set the position

                chunkLocations.Add(location); // Add the location to the list
            }
        }
    }

    // Function to generate and spawn the chunks at the calculated positions
    void GenerateChunks()
    {
        foreach (GameObject location in chunkLocations)
        {
            Vector3 position = location.transform.position; // Get the position of the location

            // Skip center location if a chunk is already placed there
            if (Vector3.Distance(position, Vector3.zero) < 0.1f)
                continue;

            int rand = Random.Range(0, terrainChunks.Count); // Choose a random chunk prefab
            GameObject chunk = Instantiate(terrainChunks[rand], position, Quaternion.identity); // Instantiate the chunk
            chunk.name = location.name; // Name the chunk the same as the location
        }

        Debug.Log("Chunks Spawned!");
    }
}
