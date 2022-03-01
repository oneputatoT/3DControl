using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public Animator animator;
    PlayerManager playerManager;
    PlayerLocomotion playerLocomotion;
    int vertical;
    int horizontal;
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerManager = GetComponent<PlayerManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    public void PlayerTargetAnimation(string targetAnimation,bool isInteracting,bool useRootMotion = false) 
    { 
        animator.SetBool("isInteracting", isInteracting);    //��Ϊtrueʱ����ס��������
        animator.SetBool("isUsingRootMotion", useRootMotion);
        animator.CrossFade(targetAnimation, 0.2f);
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement,bool Sprinting,bool Squating)
    {
        float snappedHorizontal;
        float snappedVertical;

        #region Snapped Horizontal
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            snappedHorizontal = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            snappedHorizontal = 1;
        }
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            snappedHorizontal = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            snappedHorizontal = -1;
        }
        else snappedHorizontal = 0;
        #endregion

        #region Snapped Vertical
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            snappedVertical = 1;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalMovement < -0.55f)
        {
            snappedVertical = -1;
        }
        else snappedVertical = 0;
        #endregion

        if (Sprinting)
        {
            snappedHorizontal = horizontalMovement;
            snappedVertical = 2;
        }

        if (Squating)
        {
            snappedHorizontal = horizontalMovement;
            snappedVertical = 0.5f;
        }

        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }

    private void OnAnimatorMove()
    {
        if (playerManager.isUsingRootMotion)
        {
            playerLocomotion.playerrigidbody.drag = 0;
            Vector3 deltaPosition = animator.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / Time.deltaTime;
            playerLocomotion.playerrigidbody.velocity = velocity;
        }
    }
}
