using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator animator;
    PlayerMovement pmove;
    SpriteRenderer srender;
    void Start()
    {
        animator = GetComponent<Animator>();
        pmove = GetComponent<PlayerMovement>();
        srender = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (pmove.moveDir.x != 0 || pmove.moveDir.y != 0)
        {
            animator.SetBool("Move", true);
            SpriteDirectionControl();
        }
        else
        {
            animator.SetBool("Move", false);
        }
    }
    void SpriteDirectionControl()
    {
        if (pmove.lastHorizontalVector < 0)
            srender.flipX = true;
        else
            srender.flipX = false;
    }

    public void SetAnimatorController(RuntimeAnimatorController controller)
    {
        if (!animator) animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = controller;
    }

    public void ControlLoop()
    {

    }
}
