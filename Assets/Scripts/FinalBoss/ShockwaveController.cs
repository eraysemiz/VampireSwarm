using UnityEngine;

public class ShockwaveController : AttackController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start(); // base parent class taki Start fonksiyonunu �a��r�r
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnShockwave = Instantiate(weaponData.Weapon);
        spawnShockwave.transform.position = transform.position;

        Vector3 playerDirection = (pmove.transform.position - transform.position).normalized;

        spawnShockwave.GetComponent<ShockwaveBehaviour>().DirectionChecker(playerDirection);
    }
}
