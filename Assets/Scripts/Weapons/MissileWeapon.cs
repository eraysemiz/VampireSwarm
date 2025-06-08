// MissileWeapon.cs
using UnityEngine;

public class MissileWeapon : ProjectileWeapon
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

        // Rastgele bir ekrandaki hedef noktasý seç
        Vector2 target = GetRandomScreenPoint();
        Vector2 direction = (target - (Vector2)owner.transform.position).normalized;

        // Füze prefab'ýný spawn ederken rotasyonu sýfýr býrakýyoruz.
        // Yönlendirmeyi Projectile.Start()'ta yapacaðýz.
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
