using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public Vector2 lastMovedVector;

    Rigidbody2D rigidbody;
    public CharacterScriptableObjects characterData;
    public PlayerStats playerData;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        //playerData = Object.FindAnyObjectByType<PlayerStats>();
        playerData = GetComponent<PlayerStats>();
        lastMovedVector = new Vector2 (1, 0f); // bu olmazsa oyun ba�lad���nda e�er oyuncu hareket etmezse b��ak hareketsiz kal�r
    }

    // Update is called once per frame
    void Update()
    {
        InputManagement();
    }

    void FixedUpdate()
    {
        Move();    
    }
    void InputManagement()
    {
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
        rigidbody.linearVelocity = new Vector2(moveDir.x * playerData.currentMoveSpeed, moveDir.y * playerData.currentMoveSpeed);
    }
}
