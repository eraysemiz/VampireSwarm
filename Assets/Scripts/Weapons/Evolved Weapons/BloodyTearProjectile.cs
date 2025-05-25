using UnityEngine;

public class BloodyTearProjectile : Projectile
{
    protected override void Start()
    {
        base.Start();
    }

    // If the projectile is homing, it will automatically find a suitable target
    // to move towards.
    public override void AcquireAutoAimFacing()
    {
        base.AcquireAutoAimFacing();
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        //Check if the damage dealt was a critical hit, then heal player
        if (weapon.criticalHit)
        {
            weapon.owner.RestoreHealth(16);
        }
    }
}
