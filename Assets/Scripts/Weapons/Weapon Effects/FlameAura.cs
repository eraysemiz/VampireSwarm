using UnityEngine;

public class FlameAura : Aura
{
    void Start()
    {
        float life = weapon.GetLifespan();
        if (life > 0)
            Destroy(gameObject, life);
    }
}