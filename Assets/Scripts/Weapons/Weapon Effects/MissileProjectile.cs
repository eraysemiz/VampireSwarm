// MissileProjectile.cs
using UnityEngine;

public class MissileProjectile : Projectile
{
    [HideInInspector]
    public Vector2 targetPosition;

    private bool exploded = false;
    private Vector2 direction;
    private float speed;
    private Vector2 lastVelocity = Vector2.right;

    protected override void Start()
    {
        base.Start();

        // Hedef yönünü ve hýzý hesapla
        speed = weapon.GetSpeed();
        direction = (targetPosition - (Vector2)transform.position).normalized;

        // Dynamic Rigidbody için doðrudan velocity ata
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.linearVelocity = direction * speed;
        }
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Kinematic Rigidbody ise kendimiz taþýyalým
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Vector2 newPos = (Vector2)transform.position + direction * speed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
        }

        // Þu anki hýz vektörünü al; dinamikse rb.velocity, deðilse sabit direction*speed
        Vector2 currentVelocity = rb.bodyType == RigidbodyType2D.Dynamic
                                  ? rb.linearVelocity
                                  : direction * speed;

        // Yeterince büyükse dönmeyi hesapla
        if (currentVelocity.sqrMagnitude > 0.001f)
        {
            lastVelocity = currentVelocity;
            float angle = Mathf.Atan2(lastVelocity.y, lastVelocity.x) * Mathf.Rad2Deg;
            // Eðer sprite'ýn burnu yukarý bakýyorsa +90, saða bakýyorsa +0
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }

        // Hedefe ulaþýldý mý kontrol et
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

        // Alan hasarý
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
