using UnityEngine;

public class ExperienceGem : MonoBehaviour
{
    public int experienceAmount;

    public void Collect()
    {
        PlayerStats player = Object.FindAnyObjectByType<PlayerStats>();
        player.IncreaseExperience(experienceAmount);
        //Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Eþya, oyuncuya temas ederse sil. Mýknatýs efekti tamamlayýcý
        if (col.CompareTag("Player"))
            Destroy(gameObject);
    }
}
