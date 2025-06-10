using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Flame : MonoBehaviour
{
    [Tooltip("Çarptığında düşmana kaç hasar uygulasın?")]
    public float damage = 10f;

    private void Awake()
    {
        // Collider'ın trigger olduğundan emin ol
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    public void AddDamage(float amount)
    {
        damage += amount;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // İlk temas anında hasar uygula
        if (other.TryGetComponent<EnemyStats>(out var enemy))
        {
            enemy.TakeDamage(damage, transform.position);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent<EnemyStats>(out var enemy))
        {
            enemy.TakeDamage(damage * Time.deltaTime, transform.position);
        }
    }
}