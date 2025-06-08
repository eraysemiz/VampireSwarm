// MissileProjectile.cs
using UnityEngine;

public class MissileProjectile : Projectile
{
    [HideInInspector] public Vector2 targetPosition;
    bool exploded = false;

    private Vector2 lastVelocity = Vector2.right;

    // Bu iki de�i�keni hareket ve y�n i�in sakl�yoruz
    private Vector2 _direction;
    private float _speed;

    protected override void Start()
    {
        // Base s�n�f�n Start'� da �al��s�n
        base.Start();
        Weapon.Stats weaponStats = weapon.GetStats();

        // Hedefe giden y�n ve h�z
        _direction = (targetPosition - (Vector2)transform.position).normalized;
        _speed = weapon.GetSpeed();

        // Sprite'�n "up" (yukar�) vekt�r�n� bu y�ne �evir
        transform.right = _direction;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (rb)
        {
            Vector2 vel = rb.bodyType == RigidbodyType2D.Dynamic ? rb.linearVelocity : (Vector2)transform.right * weapon.GetSpeed();
            if (vel.sqrMagnitude > 0.01f)
                lastVelocity = vel;
            float angle = Mathf.Atan2(lastVelocity.y, lastVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        if (!exploded && Vector2.Distance(transform.position, targetPosition) <= 0.1f)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        Weapon.Stats stats = weapon.GetStats();

        // Alan hasar�
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, weapon.GetArea());
        foreach (var t in targets)
        {
            if (t.TryGetComponent<EnemyStats>(out var es))
                es.TakeDamage(GetDamage(), transform.position);
            else if (t.TryGetComponent<BreakableProps>(out var p))
                p.TakeDamage(GetDamage());
            else if (t.TryGetComponent<Barrier>(out var b))
                b.TakeDamage(GetDamage());
        }

        // Vurulma efekti
        if (stats.hitEffect)
            Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);

        // Aura efekti
        if (stats.auraPrefab)
        {
            FlameAura aura = Instantiate(stats.auraPrefab, transform.position, Quaternion.identity) as FlameAura;
            aura.weapon = weapon;
            aura.owner = owner;
            float area = weapon.GetArea();
            aura.transform.localScale = new Vector3(area, area, area);
        }

        Destroy(gameObject);
    }
}
