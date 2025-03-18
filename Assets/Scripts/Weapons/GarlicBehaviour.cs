using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarlicBehaviour : MeleeWeaponBehaviour
{

    List<GameObject> markedEnemies = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
        StartCoroutine(ListCleaner());
    }

    void Update()
    {
        transform.Rotate(0, 0, currentSpeed * Time.deltaTime);
    }

    IEnumerator ListCleaner()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.8f);
            markedEnemies.Clear();
        }
    }
    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.CompareTag("Enemy") || col.CompareTag("MiniBoss")) && !markedEnemies.Contains(col.gameObject))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage(), transform.position);

            markedEnemies.Add(col.gameObject);  // Hasar verilen hedefi i�aretle
        }
        else if (col.CompareTag("Prop")&& !markedEnemies.Contains(col.gameObject))
        {
            BreakableProps prop = col.GetComponent<BreakableProps>();
            prop.TakeDamage(GetCurrentDamage());

            markedEnemies.Add(col.gameObject);
        }
    }

}
