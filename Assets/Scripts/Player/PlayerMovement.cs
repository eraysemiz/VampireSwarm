using UnityEngine;

public class PlayerMovement : Sortable
{
    public const float DEFAULT_MOVESPEED = 5f;

    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public Vector2 lastMovedVector;

    [Header("Push Settings")]
    public float pushForce = 1f;

    private Rigidbody2D rb;  
    public PlayerStats player;

    protected override void Start()
    {
        base.Start();
        player = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f); //If we don't do this and game starts up and don't move, the projectile weapon will have no momentum
    }

    // Update is called once per frame
    void Update()
    {
        InputManagement();
    }

    void FixedUpdate()
    {
        Move();    
        StayInsideOfBorders();
    }
    void InputManagement()
    {

        if (GameManager.instance.isGameOver)
        {
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;

        if (moveDir.x !=  0 ) 
        { 
            lastHorizontalVector = moveDir.x;
            lastMovedVector = new Vector2(lastHorizontalVector, 0f);
        }
        if (moveDir.y != 0 ) 
        { 
            lastVerticalVector = moveDir.y; 
            lastMovedVector = new Vector2 (0f, lastVerticalVector);
        }

        if (moveDir.x != 0 && moveDir.y != 0)
        {
            lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);
        }
    }

    void Move()
    { 
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        rb.linearVelocity = moveDir * DEFAULT_MOVESPEED * player.Stats.moveSpeed;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyMovement enemy = collision.gameObject.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                Vector2 dir = (collision.transform.position - transform.position).normalized;
                enemy.Knockback(dir * pushForce, 0.1f);
            }
        }
    }

    void StayInsideOfBorders()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, MapController.minX, MapController.maxX);
        pos.y = Mathf.Clamp(pos.y, MapController.minY, MapController.maxY);
        transform.position = pos;
    }
}
