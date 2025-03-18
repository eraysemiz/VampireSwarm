using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    float currentCooldown;

    protected PlayerMovement pmove;
    // virtual fonksiyonun override edilece�ini g�sterir. virtual olmazsa override i�lemi hata verir
    protected virtual void Start()
    {
        pmove = Object.FindAnyObjectByType<PlayerMovement>();   
        currentCooldown = weaponData.CooldownDuration;
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown < 0f)
        {
            Attack();
        }
    }
    protected virtual void Attack()
    {
        currentCooldown = weaponData.CooldownDuration;
    }
}
