using UnityEngine;

public class EnemyAnimationController
{
    Animator animator;
    public EnemyAnimationController(Animator animator)
    {
        this.animator = animator;
    }

    public void OnAttack()
    {
        animator.SetBool("IsAttack", true); ;
    }

    public void OnMove()
    {
        animator.SetBool("IsAttack", false);
    }

    public void OnDie()
    {
        animator.SetTrigger("Disappear");
    }
}
