using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    InputMannager inputManager;
    AnimatorManager animatorManager;
    PlayerManager playerManager;
    Vector3 moveDirection;
    Transform cameraObject;
    public LayerMask groundLayer;
    public Rigidbody playerrigidbody;

    [Header("下落属性")]
    public float inAirTime;
    public float leapingVelocity;    //水平方向速度
    public float fallingVelocity;   //竖直方向速度
    public float rayCastHeightOffSet = 0.5f;

    [Header("行动速度")]
    public float runningSpeed = 5;
    public float sprintingSpeed = 7;
    public float walkingSpeed = 1.5f;
    public float rotationSpeed = 15;

    [Header("跳跃速度")]
    public float gravityIntensity = -15;
    public float jumpHight = 3;


    [Header("人物状态")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isSquating;
    public bool isJumping;


    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        inputManager = GetComponent<InputMannager>();
        playerrigidbody = GetComponent<Rigidbody>();
        animatorManager = GetComponent<AnimatorManager>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        HandleFallingAndLanding();
        if (playerManager.isInteracting)
            return;

        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement() 
    {
        if (isJumping)
            return;

        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;


        if (isSprinting)
        {
            moveDirection = moveDirection * sprintingSpeed;
        }
        else if (isSquating)
        {
            moveDirection = moveDirection * walkingSpeed;
        }
        else
        {
            if (inputManager.moveAmount > 0.5f)
            {
                moveDirection = moveDirection * runningSpeed;
            }
            else
            {
                moveDirection = moveDirection * walkingSpeed;
            }
        }

        Vector3 movementVelocity = moveDirection;
        playerrigidbody.velocity = movementVelocity;
    }

    private void HandleRotation()
    {
        if (isJumping)
            return;

        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;
        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;


        Quaternion targetRotion = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotion, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        Vector3 targetPosition;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffSet;
        targetPosition = transform.position;

        if (!isGrounded&& !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayerTargetAnimation("Falling", true);
            }

            animatorManager.animator.SetBool("isUsingRootMotion",false);
            inAirTime = inAirTime + Time.deltaTime;
            playerrigidbody.AddForce(transform.forward * leapingVelocity);
            playerrigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTime);
        }


        //检测地面
        if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, groundLayer))
        {
            if (!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayerTargetAnimation("Land", true);
            }
            Vector3 rayCastHitpoint = hit.point;
            targetPosition.y = rayCastHitpoint.y;
            inAirTime = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }


        //检测楼梯，将判定点作为移动位置
        if (isGrounded && !isJumping)
        {
            if (playerManager.isInteracting || inputManager.moveAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else 
            {
                transform.position = targetPosition;
            }
        }
    }

    public void HandleJumping() 
    {
        if (isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayerTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerrigidbody.velocity = playerVelocity;
        }
    
    }

    public void HandleDodge()
    {
        if (playerManager.isInteracting)
            return;

        animatorManager.PlayerTargetAnimation("Dodge", true,true);
    }
   
}
