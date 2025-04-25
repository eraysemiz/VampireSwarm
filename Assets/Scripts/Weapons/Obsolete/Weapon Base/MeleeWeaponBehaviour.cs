using UnityEngine;
[System.Obsolete]
public class MeleeWeaponBehaviour : MonoBehaviour
{
    public WeaponScriptableObject weaponData;

    public float destroyAfterSeconds;

    [HideInInspector] public float currentDamage;
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public float currentCooldownDuration;

    public float GetCurrentDamage()
    {
        return currentDamage *= FindAnyObjectByType<PlayerStats>().CurrentMight;
    }

    void Awake()
    {
        currentDamage = weaponData.Damage;
        currentSpeed = weaponData.Speed;
        currentCooldownDuration = weaponData.CooldownDuration;
    }
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") || col.CompareTag("MiniBoss") || col.CompareTag("FinalBoss"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage(), transform.position);
        }
        else if (col.CompareTag("Prop"))
        {
            BreakableProps prop = col.GetComponent<BreakableProps>();
            prop.TakeDamage(GetCurrentDamage());
        }
    }
}
