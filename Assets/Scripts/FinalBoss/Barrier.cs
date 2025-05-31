using UnityEngine;

public class Barrier : MonoBehaviour
{
    private float health;
    private float lifespan;
    private float lifeTimer;

    private Transform target; // takip edilecek düþman
    private float followDistance = 3f;

    private GameObject impactEffect;

    private Vector2 lastDirection = Vector2.right;

    public void Initialize(EnemyWeaponData data, Transform targetTransform)
    {
        health = data.health;
        lifespan = data.lifespan;
        impactEffect = data.impactEffect;
        lifeTimer = lifespan;
        target = targetTransform;
    }

    private void Update()
    {
        // Ömür süresi kontrolü
        if (lifespan > 0f)
        {
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f)
                Die();
        }

        // Takip sistemi
        if (target != null)
        {
            Vector2 moveDir = target.GetComponent<EnemyBehaviour>()?.LastMoveDirection ?? Vector2.right;

            if (moveDir.sqrMagnitude > 0.01f)
                lastDirection = moveDir;

            transform.position = (Vector2)target.position + lastDirection.normalized * followDistance;

            // Dönüþü yönüne göre ayarla
            float angle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("Barrier Health: " + health);
        if (impactEffect != null)
            Destroy(Instantiate(impactEffect, transform.position, Quaternion.identity), 2f);
        health -= amount;
        if (health <= 0f)
            Die();
    }

    private void Die()
    {
        if (impactEffect != null)
            Destroy(Instantiate(impactEffect, transform.position, Quaternion.identity), 2f);

        Destroy(gameObject);
    }
}
