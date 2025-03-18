using UnityEngine;

public class FireballBehaviour : AttackBehaviour
{
    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        transform.position += direction * currentSpeed * Time.deltaTime;
    }
}
