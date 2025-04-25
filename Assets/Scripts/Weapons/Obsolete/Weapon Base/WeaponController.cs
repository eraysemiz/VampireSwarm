using UnityEngine;
[System.Obsolete]
public class WeaponController : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    float currentCooldown;

    protected PlayerMovement pmove;
    // virtual fonksiyonun override edileceðini gösterir. virtual olmazsa override iþlemi hata verir
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
