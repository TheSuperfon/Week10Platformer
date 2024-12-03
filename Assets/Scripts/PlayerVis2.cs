using UnityEngine;

public class PlayerVis2 : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer bodyRenderer;
    public PlayerControl2 playerController;
    private readonly int IdleHash = Animator.StringToHash("Idle");
    //private readonly int WalkingHash = Animator.StringToHash("Walking");
    private readonly int WalkingHash = Animator.StringToHash("IsWalking");
    //private readonly int JumpingHash = Animator.StringToHash("Jumping");
    private readonly int JumpingHash = Animator.StringToHash("IsGrounded");
    private readonly int DeadHash = Animator.StringToHash("Dead");
    void Update()
    {
        //UpdateVisuals();

        animator.SetBool(WalkingHash, playerController.IsWalking());
        animator.SetBool(JumpingHash, playerController.IsGrounded());


        switch (playerController.GetFacingDirection())
        {
            case PlayerControl2.FacingDirection.left:
                bodyRenderer.flipX = true;
                break;
            case PlayerControl2.FacingDirection.right:
                bodyRenderer.flipX = false;
                break;
        }
    }

    private void UpdateVisuals()
    {
        if (playerController.previousState != playerController.currentState)
        {
            switch (playerController.currentState)
            {
                case PlayerControl2.PlayerState.idle:
                    animator.CrossFade(IdleHash, 0);
                    break;
                case PlayerControl2.PlayerState.walking:
                    animator.CrossFade(WalkingHash, 0);
                    break;
                case PlayerControl2.PlayerState.jumping:
                    animator.CrossFade(JumpingHash, 0);
                    break;
                case PlayerControl2.PlayerState.dead:
                    animator.CrossFade(DeadHash, 0);
                    break;
            }


        }
    }



}
