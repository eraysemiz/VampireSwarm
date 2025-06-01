using UnityEngine;

public class BreakableProps : MonoBehaviour
{
    public float health;

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Kill();
        }
    }

    void Kill()
    {
        DropManager drops = GetComponent<DropManager>();
        if (drops) drops.active = true;


        Destroy(gameObject);
    }
}
