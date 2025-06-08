using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Missile evolution that fires at a random on-screen enemy.
/// The missile's target is chosen on spawn and does not track afterwards.
/// </summary>
public class TargetedMissileWeapon : MissileWeapon
{
    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning($"Projectile prefab has not been set for {name}");
            ActivateCooldown(true);
            return false;
        }
        if (!CanAttack()) return false;

        if (currentStats.procEffect)
            Destroy(Instantiate(currentStats.procEffect, owner.transform), 5f);

        // Pick a random visible enemy. Fallback to a random screen point if none exist.
        EnemyStats targetEnemy = PickEnemy();
        Vector2 target = targetEnemy ? (Vector2)targetEnemy.transform.position : GetRandomScreenPoint();

        // Spawn the projectile with no rotation. Orientation handled by projectile.
        var missile = Instantiate(
            currentStats.projectilePrefab as MissileProjectile,
            owner.transform.position,
            Quaternion.identity
        );

        missile.weapon = this;
        missile.owner = owner;
        missile.targetPosition = target;

        ActivateCooldown(true);

        attackCount--;
        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = ((WeaponData)data).baseStats.projectileInterval;
        }

        return true;
    }

    // Randomly selects an on-screen enemy.
    EnemyStats PickEnemy()
    {
        List<EnemyStats> candidates = new List<EnemyStats>(FindObjectsByType<EnemyStats>(FindObjectsSortMode.None));
        while (candidates.Count > 0)
        {
            int idx = Random.Range(0, candidates.Count);
            EnemyStats target = candidates[idx];
            candidates.RemoveAt(idx);

            if (!target) continue;
            Renderer r = target.GetComponent<Renderer>();
            if (r && r.isVisible)
                return target;
        }
        return null;
    }

    // Same helper as base class but duplicated so it is accessible here.
    Vector2 GetRandomScreenPoint()
    {
        Camera cam = Camera.main;
        Vector2 min = cam.ViewportToWorldPoint(Vector2.zero);
        Vector2 max = cam.ViewportToWorldPoint(Vector2.one);
        return new Vector2(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y)
        );
    }
}