using UnityEngine;

public class KnifeController : WeaponController
{
    // protected fonksiyona sadece child class tan eri�im sa�lanabilmesini sa�lar
    protected override void Start()
    {
        base.Start(); // base parent class taki Start fonksiyonunu �a��r�r
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnKnife = Instantiate(weaponData.Weapon);
        spawnKnife.transform.position = transform.position;
        spawnKnife.GetComponent<KnifeBehaviour>().DirectionChecker(pmove.lastMovedVector);
    }
}
