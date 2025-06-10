// MissileProjectile.cs
using System.Collections.Generic;
using UnityEngine;

public class MissileProjectile : Projectile
{
    [HideInInspector]
    public Vector2 targetPosition;

    private bool exploded = false;
    private Vector2 direction;
    private float speed;
    private Vector2 lastVelocity = Vector2.right;

    public ParticleSystem fireEffect;
    // Damage value calculated when the missile is spawned.
    public float weaponDamage;

    // Prevent double damage by not applying direct contact damage.
    // Instead, collisions simply trigger the explosion.

    protected override void Start()
    {
        base.Start();
        // Hedef y�n�n� ve h�z� hesapla
        speed = weapon.GetSpeed();
        weaponDamage = weapon.GetDamage();
        direction = (targetPosition - (Vector2)transform.position).normalized;

        // Dynamic Rigidbody i�in do�rudan velocity ata
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.linearVelocity = direction * speed;
        }
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Kinematic Rigidbody ise kendimiz ta��yal�m
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Vector2 newPos = (Vector2)transform.position + direction * speed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
        }

        // �u anki h�z vekt�r�n� al; dinamikse rb.velocity, de�ilse sabit direction*speed
        Vector2 currentVelocity = rb.bodyType == RigidbodyType2D.Dynamic
                                  ? rb.linearVelocity
                                  : direction * speed;

        // Yeterince b�y�kse d�nmeyi hesapla
        if (currentVelocity.sqrMagnitude > 0.001f)
        {
            lastVelocity = currentVelocity;
            float angle = Mathf.Atan2(lastVelocity.y, lastVelocity.x) * Mathf.Rad2Deg;
            // E�er sprite'�n burnu yukar� bak�yorsa +90, sa�a bak�yorsa +0
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }

        // Hedefe ula��ld� m� kontrol et
        if (!exploded)
        {
            float threshold = speed * Time.fixedDeltaTime;
            if (Vector2.Distance(transform.position, targetPosition) <= threshold)
            {
                Explode();
            }
        }
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        Weapon.Stats stats = weapon.GetStats();

        // Alan hasar�
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, weapon.GetArea());
        var damagedEnemies = new HashSet<EnemyStats>();
        foreach (var col in hits)
        {
            if (col.TryGetComponent<EnemyStats>(out var es) && damagedEnemies.Add(es))
            {
                es.TakeDamage(weaponDamage, transform.position);
            }
            else if (col.TryGetComponent<BreakableProps>(out var bp))
            {
                // Gerekirse BreakableProps/Barrier i�in de ayr� bir HashSet kullan�n
                bp.TakeDamage(weaponDamage);
            }
            else if (col.TryGetComponent<Barrier>(out var b))
            {
                b.TakeDamage(weaponDamage);
            }
        }

        // Vurulma efekti
        if (stats.hitEffect)
            Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);

        Burn();
        Destroy(gameObject);
    }

    void Burn()
    {
        if (fireEffect)
        {
            var ps = Instantiate(fireEffect, transform.position, Quaternion.identity);
            var flame = ps.GetComponent<Flame>();
            if (flame) flame.AddDamage(weaponDamage / 10f);
        }
    }
}
