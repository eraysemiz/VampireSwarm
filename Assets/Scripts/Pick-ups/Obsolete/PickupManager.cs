using UnityEngine;
[System.Obsolete]
public class PickupManager : MonoBehaviour
{
    PlayerStats player;
    CircleCollider2D playerCollector;
    public float pullSpeed;
    [HideInInspector] public ExperienceGem gem;
    [HideInInspector] public Potion potion;


    void Start()
    {
        player = Object.FindAnyObjectByType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
        if (playerCollector == null)
            Debug.LogWarning("Circle Collider Bulunamad�");
    }

    void Update()
    {
        playerCollector.radius = player.CurrentMagnet;   
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("ExpGems"))
        {
            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            MagnetEffect(col, rb);

            gem = col.GetComponent<ExperienceGem>();
            gem.Collect();
        }
        else if (col.CompareTag("HealthPotion"))
        {
            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            MagnetEffect(col, rb);

            potion = col.GetComponent<Potion>();
            potion.healthPotion();
        }
        else if (col.CompareTag("SpeedPotion"))
        {
            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            MagnetEffect(col, rb);

            potion = col.GetComponent<Potion>();
            potion.speedPotion();
        }
    }

    void MagnetEffect(Collider2D col, Rigidbody2D rb)
    {
        // E�yadan oyuncuya do�ru vekt�r
        Vector2 forceDirection = (transform.position - col.transform.position).normalized;
        rb.AddForce(forceDirection * pullSpeed);
    }
}
