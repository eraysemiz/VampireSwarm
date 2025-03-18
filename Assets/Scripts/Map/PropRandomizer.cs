using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PropRandomizer : MonoBehaviour
{
    public List<GameObject> propSpawnPoint;
    public List<GameObject> props;
    void Start()
    {
        SpawnProps();
    }
    void SpawnProps()
    {
        foreach (GameObject point in propSpawnPoint)
        {
            int rand = Random.Range(0, props.Count);
            GameObject prop = Instantiate(props[rand], point.transform.position, Quaternion.identity);
            prop.transform.parent = point.transform;
            
        }
    }
}
