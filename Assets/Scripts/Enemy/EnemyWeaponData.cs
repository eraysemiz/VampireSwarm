using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Weapon Data", menuName = "2D Top-down Rogue-like/Enemy Weapon Data")]
public class EnemyWeaponData : ScriptableObject
{
    [Header("Weapon Type")]
    public WeaponType weaponType = WeaponType.Classic;


    [Header("Stats")]


    [HideIf("IsBarrier")] public float damage = 10f;
    [ShowIf("IsBarrier")] public float health = 20f;
    [HideIf("IsBarrier")] public float speed = 5f;
    public float cooldown = 2f;
    public float lifespan = 5f;
    [HideIf("IsBarrier")] public float knockback = 0f;

    [Header("Weapon")]
    public GameObject weaponPrefab;
    public GameObject impactEffect;

    [HideInInspector] public bool IsBarrier => weaponType == WeaponType.Barrier;
}

public enum WeaponType
{
    Classic,
    Ray,
    Barrier
}