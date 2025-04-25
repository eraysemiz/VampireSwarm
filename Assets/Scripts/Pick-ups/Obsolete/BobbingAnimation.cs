using UnityEngine;
[System.Obsolete]
public class BobbingAnimation : MonoBehaviour
{
    public float frequency;
    public float magnitude;
    public Vector3 direction;
    Vector3 initialPosition;
    Potion potion;
    ExperienceGem gem;
    void Start()
    {
        initialPosition = transform.position;

        gem = GetComponent<ExperienceGem>();
        if (gem == null)
        { 
            potion = GetComponent<Potion>();
            if (potion == null)
            {
                Debug.LogWarning("No ExperienceGem or Potion component found on this object.");
            }
        }
    }

    void Update()
    {
        if (gem && !gem.hasBeenCollected) 
            transform.position = initialPosition + direction * Mathf.Sin(Time.time * frequency) * magnitude;
        else if (potion && !potion.hasBeenCollected)
            transform.position = initialPosition + direction * Mathf.Sin(Time.time * frequency) * magnitude;
    }
}
