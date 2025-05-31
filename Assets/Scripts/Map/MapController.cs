using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks; // List of chunk prefabs
    [HideInInspector] public GameObject currentChunk;
    public List<GameObject> chunkLocations; // List to hold the chunk positions

    public int gridSize = 9;      // Grid size (9x9)
    public float spacing = 40f;    // Distance between the chunks

    public GameObject borderPrefab; // Inspector'dan atayacaðýn prefab
    [HideInInspector]public GameObject chunksParent;      // Parent object for chunks
    [HideInInspector] public GameObject locationsParent;   // Parent object for location markers (optional)

    void Start()
    {
        chunksParent = new GameObject("Chunks");
        locationsParent = new GameObject("Locations");

        // First, create the grid with the center at (0,0)
        CreateGrid();

        // If chunkLocations list is not empty, generate the chunks
        if (chunkLocations.Count > 0)
        {
            GenerateChunks();
        }

        CreateBorders(); // Sýnýrlarý oluþtur
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

                location.transform.parent = locationsParent.transform;

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

            chunk.transform.parent = chunksParent.transform;
        }

        locationsParent.SetActive(false);
        Debug.Log("Chunks Spawned!");
    }

    void CreateBorders()
    {
        GameObject bordersParent = new GameObject("Borders");

        float offset = (gridSize - 1) * spacing / 2f;

        for (int i = 0; i < gridSize; i++)
        {
            float x = (i * spacing) - offset;
            float y = (i * -spacing) + offset;

            // ÜST KENAR
            Vector3 topPos = new Vector3(x, offset + spacing, 0);
            InstantiateBorder(topPos, bordersParent.transform);

            // ALT KENAR
            Vector3 bottomPos = new Vector3(x, -offset - spacing, 0);
            InstantiateBorder(bottomPos, bordersParent.transform);

            // SOL KENAR
            Vector3 leftPos = new Vector3(-offset - spacing, y, 0);
            InstantiateBorder(leftPos, bordersParent.transform);

            // SAÐ KENAR
            Vector3 rightPos = new Vector3(offset + spacing, y, 0);
            InstantiateBorder(rightPos, bordersParent.transform);
        }
    }

    void InstantiateBorder(Vector3 position, Transform parent)
    {
        GameObject border = Instantiate(borderPrefab, position, Quaternion.identity);
        border.transform.parent = parent;
    }
}
