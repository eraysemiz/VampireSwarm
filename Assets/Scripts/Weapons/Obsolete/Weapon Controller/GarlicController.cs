using UnityEngine;
[System.Obsolete]
public class GarlicController : WeaponController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedGarlic = Instantiate(weaponData.Weapon);
        spawnedGarlic.transform.position = transform.position; // garlic in konumunu this objectinin konumuna eþitle
        spawnedGarlic.transform.parent = transform;
    }

}
