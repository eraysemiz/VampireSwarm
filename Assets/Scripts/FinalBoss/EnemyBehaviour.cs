using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Enemy Weapons")]
    public List<EnemyWeaponData> weapons = new List<EnemyWeaponData>();
    private List<WeaponState> weaponStates = new List<WeaponState>();

    [Header("Detection Settings")]
    public float detectionRange = 50f; // Oyuncuyu alg�lama menzili



    private Transform player;
    private EnemyStats enemyStats;
    private float lastHealth;

    public Image healthBar;
    public GameObject deathEffect;

    private void OnEnable()
    {
        foreach (GameObject gem in GameObject.FindGameObjectsWithTag("ExpGems"))
        {
            Destroy(gem);
        }
    }
    public Vector2 LastMoveDirection { get; private set; } = Vector2.right;

    private class WeaponState
    {
        public EnemyWeaponData data;
        public float cooldownTimer;
        public float currentAngle;

        public WeaponState(EnemyWeaponData data)
        {
            this.data = data;
            cooldownTimer = data.cooldown;
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        enemyStats = GetComponent<EnemyStats>();
        foreach (var weapon in weapons)
        {
            if (weapon != null)
                weaponStates.Add(new WeaponState(weapon));
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > detectionRange) return;

        // Düşmanın yönü takip edilsin
        LastMoveDirection = (player.position - transform.position).normalized;

        foreach (var state in weaponStates)
        {
            state.cooldownTimer -= Time.deltaTime;

            if (state.cooldownTimer <= 0f)
            {
                ShootWeapon(state);
                state.cooldownTimer = state.data.cooldown;
            }
        }
        if (lastHealth != enemyStats.currentHealth) 
            UpdateHealthBar();
    }

    private void ShootWeapon(WeaponState state)
    {
        EnemyWeaponData weaponData = state.data;
        if (weaponData.weaponPrefab == null) return;

        Vector2 direction;
        float angle;
        float radius;

        switch (weaponData.weaponType)
        {
            case WeaponType.Ray:
                radius = 7f;
                angle = state.currentAngle;
                state.currentAngle = (state.currentAngle + 15f) % 360f;
                direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
                break;
            case WeaponType.Barrier:
                // Oyuncuya yönelir ama hareket etmez
                radius = 5f;
                direction = (player.position - transform.position).normalized;
                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                direction = Vector2.zero; // hareket etmeyecek
                break;
            case WeaponType.Star:
                radius = 2f;
                direction = (player.position - transform.position).normalized;
                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                break;
            case WeaponType.Classic:
            default:
                radius = 2f;
                direction = (player.position - transform.position).normalized;
                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                break;
        }

        Vector2 offset = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * radius;

        // Instantiate konumu düşmanın pozisyonuna göre offsetli
        GameObject weapon = Instantiate(
            weaponData.weaponPrefab,
            (Vector2)transform.position + offset,
            Quaternion.Euler(0, 0, angle)
        );

        Rigidbody2D rb = weapon.GetComponent<Rigidbody2D>();
        if (rb != null && direction != Vector2.zero && (weaponData.weaponType == WeaponType.Classic || weaponData.weaponType == WeaponType.Star))
            rb.linearVelocity = direction * weaponData.speed;


        if (weaponData.weaponType == WeaponType.Ray)
        {
            weapon.transform.parent = this.transform; // Ray düşmana bağlı kalır
        }
        if (weaponData.weaponType == WeaponType.Barrier)
        {
            Barrier barrier = weapon.AddComponent<Barrier>();
            barrier.Initialize(weaponData, this.transform); // düşman referansı veriliyor
        }
        else
        {
            // Classic ve Ray için çarpışma davranışı
            ProjectileTracker tracker = weapon.AddComponent<ProjectileTracker>();
            tracker.Initialize(weaponData.damage, weaponData.knockback, weaponData.impactEffect, weaponData.lifespan, weaponData.weaponType);
        }
    }

    public void UpdateHealthBar()
    {
        lastHealth = enemyStats.currentHealth;
        healthBar.fillAmount = enemyStats.currentHealth / enemyStats.MaxHealth;
    }

    private class ProjectileTracker : MonoBehaviour
    {
        private float damage;
        private float knockback;
        private GameObject impactEffect;
        private WeaponType weaponType;
        private float lifespan;
        private float timeAlive;
        private float lastScale = 1f;

        public void Initialize(float dmg, float kb, GameObject impact, float life, WeaponType type)
        {
            damage = dmg;
            knockback = kb;
            impactEffect = impact;
            weaponType = type;
            lifespan = life;
            timeAlive = 0f;
        }

        private void Update()
        {
            timeAlive += Time.deltaTime;

            if (timeAlive >= lifespan)
            {
                if (impactEffect)
                {
                    GameObject effect = Instantiate(impactEffect, transform.position, Quaternion.identity);
                    effect.transform.localScale = Vector3.one * lastScale;
                    Destroy(effect, 2f);
                }

                Destroy(gameObject);
                return;
            }

            // Star tipi sabit büyüme
            if (weaponType == WeaponType.Star)
            {
                float t = timeAlive / lifespan;
                float scale = Mathf.Lerp(1f, 3f, t);

                // Görsel boyut
                transform.localScale = new Vector3(scale, scale, 1f);

                // Collider boyutu
                CircleCollider2D col = GetComponent<CircleCollider2D>();
                if (col != null)
                {
                    col.radius = scale * 1f; // orantılı büyüt, isteğe göre ayarla
                }

                lastScale = scale; // impactEffect için kullanacağız
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                PlayerStats stats = collision.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    stats.TakeDamage(damage);
                }
            }

            if (collision.CompareTag("Player"))
            {
                if (impactEffect)
                    Destroy(Instantiate(impactEffect, transform.position, Quaternion.identity), 2f);

                if (weaponType == WeaponType.Classic || weaponType == WeaponType.Star) 
                    Destroy(gameObject);
            }

            if (collision.CompareTag("Prop"))
            {
                BreakableProps p = collision.GetComponent<BreakableProps>();

                if (impactEffect)
                    Destroy(Instantiate(impactEffect, transform.position, Quaternion.identity), 2f);

                p.TakeDamage(damage);
            }
        }
    }


}
