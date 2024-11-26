using UnityEngine;

public class PlayerVis2 : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer bodyRenderer;
    public PlayerControl2 playerController;

    private readonly int isWalkingHash = Animator.StringToHash("IsWalking");
    private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");

    void Update()
    {
        animator.SetBool(isWalkingHash, playerController.IsWalking());
        animator.SetBool(isGroundedHash, playerController.IsGrounded());

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
}
