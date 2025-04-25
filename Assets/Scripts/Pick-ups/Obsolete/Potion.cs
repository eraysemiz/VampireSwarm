using UnityEngine;
[System.Obsolete]
public class Potion : MonoBehaviour
{
    PlayerStats player;
    public bool hasBeenCollected = false;

    void Start()
    {
        player = Object.FindAnyObjectByType<PlayerStats>();
    }

    public void healthPotion()
    {
        if (!hasBeenCollected)
        {
            float healAmount = 300 + (player.level * 10);

            player.RestoreHealth(healAmount);
            hasBeenCollected = true;
        }
    }

    public void speedPotion()
    {
        if (!hasBeenCollected)
        {
            float boostAmount = 1 + (player.level / 20);
            float boostDuration = 20 + player.level;

            //player.BoostSpeed(boostAmount, boostDuration);
            hasBeenCollected = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Eþya, oyuncuya temas ederse sil. Mýknatýs efekti tamamlayýcý
        if (col.CompareTag("Player"))
            Destroy(gameObject);
    }
}
