using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FlameAura : Aura
{
    void Start()
    {
        float life = weapon.GetLifespan();
        if (life > 0)
            Destroy(gameObject, life);
    }

    [Tooltip("Saniyede ne kadar hasar verilsin?")]
    public float damagePerSecond = 10f;

    private void Awake()
    {
        // Çarpýþmayý "tetikleme" olarak al
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Her karede Time.deltaTime kadar hasar uygula
        if (other.TryGetComponent<EnemyStats>(out var enemy))
        {
            enemy.TakeDamage(damagePerSecond * Time.deltaTime, transform.position);
        }
    }
}