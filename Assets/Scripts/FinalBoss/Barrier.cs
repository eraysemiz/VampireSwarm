using UnityEngine;

public class Barrier : MonoBehaviour
{
    private float health;
    private float lifespan;
    private GameObject deathEffect;
    private float lifeTimer;

    public void Initialize(EnemyWeaponData data)
    {
        health = data.health;
        lifespan = data.lifespan;
        deathEffect = data.impactEffect;

        lifeTimer = lifespan;
    }

    private void Update()
    {
        if (lifespan > 0f)
        {
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f)
            {
                Die();
            }
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
            Die();
    }

    private void Die()
    {
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
