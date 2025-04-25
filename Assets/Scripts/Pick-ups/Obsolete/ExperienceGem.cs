using Unity.VisualScripting;
using UnityEngine;
[System.Obsolete]
public class ExperienceGem : MonoBehaviour
{
    public int experienceAmount;
    public bool hasBeenCollected = false;

    public void Collect()
    {
        PlayerStats player = Object.FindAnyObjectByType<PlayerStats>();
        if (!hasBeenCollected)
        {
            player.IncreaseExperience(experienceAmount);
            hasBeenCollected = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // E�ya, oyuncuya temas ederse sil. M�knat�s efekti tamamlay�c�
        if (col.CompareTag("Player"))
            Destroy(gameObject);
    }
}
