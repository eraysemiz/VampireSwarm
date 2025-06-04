using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Weapon that shoots a lightning bolt to the closest enemy and bounces
/// between enemies creating a chain lightning effect. The number of
/// bounces comes from <see cref="Weapon.Stats.number"/> while the number
/// of simultaneous branches comes from <see cref="Weapon.Stats.piercing"/>.
/// </summary>
public class ChainLightningWeapon : Weapon
{
    // Maximum distance to search for the next enemy in the chain.
    public float chainRadius = 5f;

    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.hitEffect)
        {
            Debug.LogWarning($"Hit effect prefab has not been set for {name}");
            ActivateCooldown(true);
            return false;
        }

        if (!CanAttack()) return false;

        ActivateCooldown();
        StartCoroutine(FireChainLightning());
        return true;
    }

    IEnumerator FireChainLightning()
    {
        HashSet<EnemyStats> visited = new HashSet<EnemyStats>();
        Vector3 origin = owner.transform.position;

        int chains = Mathf.Max(1, currentStats.number);
        int branchCount = Mathf.Max(1, currentStats.piercing + 1);

        yield return ChainStep(origin, chains, branchCount, visited);
    }

    IEnumerator ChainStep(Vector3 from, int chainsLeft, int branchCount, HashSet<EnemyStats> visited)
    {
        if (chainsLeft <= 0) yield break;

        List<EnemyStats> targets = GetClosestEnemies(from, branchCount, visited);
        foreach (EnemyStats target in targets)
        {
            if (!target) continue;

            visited.Add(target);
            SpawnLightning(from, target.transform.position);
            target.TakeDamage(GetDamage(), from, currentStats.knockback);
            if (currentStats.hitEffect)
                Instantiate(currentStats.hitEffect, target.transform.position, Quaternion.identity);

            yield return new WaitForSeconds(0.05f);
            yield return ChainStep(target.transform.position, chainsLeft - 1, branchCount, visited);
        }
    }

    List<EnemyStats> GetClosestEnemies(Vector3 position, int count, HashSet<EnemyStats> ignore)
    {
        EnemyStats[] enemies = FindObjectsByType<EnemyStats>(FindObjectsSortMode.None);
        List<EnemyStats> list = new List<EnemyStats>();
        foreach (EnemyStats e in enemies)
        {
            if (!e || ignore.Contains(e)) continue;
            if (Vector3.Distance(position, e.transform.position) <= chainRadius)
                list.Add(e);
        }

        list.Sort((a, b) =>
            Vector3.Distance(position, a.transform.position).CompareTo(
            Vector3.Distance(position, b.transform.position)));

        if (list.Count > count) list.RemoveRange(count, list.Count - count);
        return list;
    }

    void SpawnLightning(Vector3 start, Vector3 end)
    {
        GameObject go = new GameObject("ChainLightningVFX");
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 5;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = new Color(0.7f, 0f, 1f);
        lr.startWidth = lr.endWidth = 0.1f;

        for (int i = 0; i < lr.positionCount; i++)
        {
            float t = (float)i / (lr.positionCount - 1);
            Vector3 pos = Vector3.Lerp(start, end, t);
            if (i != 0 && i != lr.positionCount - 1)
                pos += (Vector3)Random.insideUnitCircle * 0.2f;
            lr.SetPosition(i, pos);
        }

        Destroy(go, 0.15f);
    }
}
