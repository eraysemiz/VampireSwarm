using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Enemy Weapons")]
    public List<EnemyWeaponData> weapons = new List<EnemyWeaponData>();

    [Header("Detection Settings")]
    public float detectionRange = 50f; // Oyuncuyu alg�lama menzili

    private Transform player;
    private List<WeaponState> weaponStates = new List<WeaponState>();

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

        foreach (var state in weaponStates)
        {
            state.cooldownTimer -= Time.deltaTime;

            if (state.cooldownTimer <= 0f)
            {
                ShootWeapon(state);
                state.cooldownTimer = state.data.cooldown;
            }
        }
    }

    private void ShootWeapon(WeaponState state)
    {
        EnemyWeaponData weaponData = state.data;
        if (weaponData.weaponPrefab == null) return;

        Vector2 direction;
        float angle;

        switch (weaponData.weaponType)
        {
            case WeaponType.Ray:
                angle = state.currentAngle;
                state.currentAngle = (state.currentAngle + 15f) % 360f;
                direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
                break;

            case WeaponType.Barrier:
                // Oyuncuya yönelir ama hareket etmez
                direction = (player.position - transform.position).normalized;
                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                direction = Vector2.zero; // hareket etmeyecek
                break;

            case WeaponType.Classic:
            default:
                direction = (player.position - transform.position).normalized;
                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                break;
        }

        GameObject weapon = Instantiate(
            weaponData.weaponPrefab,
            transform.position,
            Quaternion.Euler(0, 0, angle)
        );

        Rigidbody2D rb = weapon.GetComponent<Rigidbody2D>();
        if (rb != null && direction != Vector2.zero && weaponData.weaponType == WeaponType.Classic)
            rb.velocity = direction * weaponData.speed;

        if (weaponData.weaponType == WeaponType.Barrier)
        {
            // Sadece Barrier türü için sağlık sistemi
            Barrier barrier = weapon.AddComponent<Barrier>();
            barrier.Initialize(weaponData);
        }
        else
        {
            // Classic ve Ray için çarpışma davranışı
            ProjectileTracker tracker = weapon.AddComponent<ProjectileTracker>();
            tracker.Initialize(weaponData.damage, weaponData.knockback, weaponData.impactEffect, weaponData.lifespan);
        }
    }

    private class ProjectileTracker : MonoBehaviour
    {
        private float damage;
        private float knockback;
        private GameObject impactEffect;

        public void Initialize(float dmg, float kb, GameObject impact, float lifespan)
        {
            damage = dmg;
            knockback = kb;
            impactEffect = impact;
            Destroy(gameObject, lifespan);
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

            if (collision.CompareTag("Player") || collision.CompareTag("Prop"))
            {
                if (impactEffect)
                    Instantiate(impactEffect, transform.position, Quaternion.identity);

                Destroy(gameObject);
            }
        }
    }


}
