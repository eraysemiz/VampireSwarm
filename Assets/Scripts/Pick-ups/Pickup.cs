using UnityEngine;

public class Pickup : Sortable
{
    public float lifespan = 0.5f;
    protected PlayerStats target; // If the pickup has a target, then fly towards the target.
    protected float speed; // The speed at which the pickup travels.
    Vector2 initialPosition;
    float initialOffset;

    // To represent the bobbing animation of the object.
    [System.Serializable]
    public struct BobbingAnimation
    {
        public float frequency;
        public Vector2 direction;
    }
    public BobbingAnimation bobbingAnimation = new BobbingAnimation
    {
        frequency = 2f,
        direction = new Vector2(0, 0.3f)
    };

    [Header("Bonuses")]
    public int experience;
    public int health;
    public bool isMagnet;

    protected override void Start()
    {
        base.Start();
        initialPosition = transform.position;
        initialOffset = Random.Range(0, bobbingAnimation.frequency);
    }


    protected virtual void Update()
    {
        if (target)
        {
            // Move it towards the player and check the distance between.
            Vector2 distance = target.transform.position - transform.position;
            if (distance.sqrMagnitude > speed * speed * Time.deltaTime)
                transform.position += (Vector3)distance.normalized * speed * Time.deltaTime;
            else
                Destroy(gameObject);

        }
        else
        {
            // Handle the animation of the object.
            transform.position = initialPosition + bobbingAnimation.direction * Mathf.Sin(Time.time + initialOffset * bobbingAnimation.frequency);
        }
    }

    public virtual bool Collect(PlayerStats target, float speed, float lifespan = 0f)
    {
        if (!this.target)
        {
            this.target = target;
            this.speed = speed;
            if (lifespan > 0) this.lifespan = lifespan;
            Destroy(gameObject, Mathf.Max(0.01f, this.lifespan));
            return true;
        }
        return false;
    }

    protected virtual void OnDestroy()
    {
        if (!target) return;
        if (experience != 0) target.IncreaseExperience(experience);
        if (health != 0) target.RestoreHealth(health);
        if (isMagnet)
        {
            // T�m sahnedeki di�er pickup'lar� bul ve �ek
            foreach (Pickup pickup in Object.FindObjectsByType<Pickup>(FindObjectsSortMode.None))
            {
                // Kendini hari� tut
                if (pickup == this) continue;

                // Zaten hedefe y�nelmi�se ge�
                if (pickup.target != null) continue;

                // Sadece ExpGems etiketine sahip olanlar� �ek
                if (!pickup.CompareTag("ExpGems")) continue;

                pickup.Collect(target, speed: 12f, lifespan: 2f); // h�z ve s�reyi iste�e g�re ayarla
            }
        }
    }
}