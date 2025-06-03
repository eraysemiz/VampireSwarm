using UnityEngine;

public class DeathSpiralWeapon : ProjectileWeapon
{
    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("Projectile prefab has not been set for {0}", name));
            ActivateCooldown(true);
            return false;
        }

        if (!CanAttack()) return false;

        int projectileCount = Mathf.Max(1, currentStats.number + owner.Stats.amount);
        float angleStep = 360f / projectileCount;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = i * angleStep;
            Projectile prefab = Instantiate(
                currentStats.projectilePrefab,
                owner.transform.position + (Vector3)GetSpawnOffset(angle),
                Quaternion.Euler(0, 0, angle)
            );

            prefab.weapon = this;
            prefab.owner = owner;

            // Manually set velocity in case the projectile does not
            // automatically move on spawn
            Rigidbody2D rb = prefab.GetComponent<Rigidbody2D>();
            if (rb)
            {
                rb.linearVelocity = prefab.transform.right * GetSpeed();
            }
        }

        ActivateCooldown(true);
        return true;
    }

    protected override Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return Quaternion.Euler(0, 0, spawnAngle) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }
}