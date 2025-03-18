using UnityEngine;

public class PickupManager : MonoBehaviour
{
    PlayerStats player;
    CircleCollider2D playerCollector;
    public float pullSpeed;

    float healAmount;
    float boostMultiplier = 2;

    void Start()
    {
        player = Object.FindAnyObjectByType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
        if (playerCollector == null)
            Debug.LogWarning("Circle Collider Bulunamadý");
        healAmount = (player.level * 20) + 400;
        boostMultiplier = 2 + (player.level / 20);
    }

    void Update()
    {
        playerCollector.radius = player.currentMagnet;    
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Mýknatýs animasyonu
        if (col.CompareTag("ExpGems"))
        {
            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            MagnetEffect(col, rb);

            ExperienceGem gem = col.GetComponent<ExperienceGem>();
            gem.Collect();
        }
        else if (col.CompareTag("HealthPotion"))
        {
            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            MagnetEffect(col, rb);

            PlayerStats player = Object.FindAnyObjectByType<PlayerStats>();
            player.RestoreHealth(healAmount);
            player.UpdateHealthBar();
            Destroy(col.gameObject);
        }
        else if (col.CompareTag("SpeedPotion"))
        {
            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            MagnetEffect(col, rb);

            PlayerStats player = Object.FindAnyObjectByType<PlayerStats>();
            player.SpeedBoost(boostMultiplier);
            Destroy(col.gameObject);
        }
    }

    void MagnetEffect(Collider2D col, Rigidbody2D rb)
    {
        // Eþyadan oyuncuya doðru vektör
        Vector2 forceDirection = (transform.position - col.transform.position).normalized;
        rb.AddForce(forceDirection * pullSpeed);
    }
}
