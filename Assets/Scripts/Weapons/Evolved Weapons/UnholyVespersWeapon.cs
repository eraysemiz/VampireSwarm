using UnityEngine;

public class UnholyVespersWeapon : KingBibleWeapon
{
    public override bool ActivateCooldown(bool strict = false)
    {
        // When 'strict' is enabled and the cooldown is not yet finished,
        // do not refresh the cooldown.
        if (strict && currentCooldown > 0) return false;

        // Calculate what the cooldown is going to be, factoring in the cooldown
        // reduction stat in the player character.
        // Cooldown no longer depends on duration, so we take it out
        float actualCooldown = (currentStats.lifespan) * Owner.Stats.cooldown;

        // Limit the maximum cooldown to the actual cooldown, so we cannot increase
        // the cooldown above the cooldown stat if we accidentally call this function
        // multiple times.
        currentCooldown = Mathf.Min(actualCooldown, currentCooldown + actualCooldown);
        return true;
    }
}