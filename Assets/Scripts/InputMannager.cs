using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMannager : MonoBehaviour
{
    PlayerControls playerControls;
    PlayerLocomotion playerLocomotion;
    AnimatorManager animatorManager;

    [Header("键盘输入")]
    public Vector2 movementInput;
    public Vector2 cameraInput;
    public float cameraInputX;
    public float cameraInputY;


    [Header("获取输出")]
    public float moveAmount;
    public float verticalInput;
    public float horizontalInput;
    public bool b_Input;
    public bool c_Input;
    public bool x_Input;
    public bool jump_input;


    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerControls.PlayerAction.B.performed += i => b_Input = true;
            playerControls.PlayerAction.B.canceled += i => b_Input=false;

            playerControls.PlayerAction.C.performed += i => c_Input = true;
            playerControls.PlayerAction.C.canceled += i => c_Input = false;

            playerControls.PlayerAction.Jump.performed += i => jump_input = true;

            playerControls.PlayerAction.X.performed += i => x_Input = true;
        }
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs() 
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleSquatInput();
        HandleJumpingInput();
        HandleDodgeInput();
    }


    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount,playerLocomotion.isSprinting,playerLocomotion.isSquating);
    }

    private void HandleSprintingInput() 
    {
        if (b_Input && moveAmount>0.5f)
        {
            playerLocomotion.isSprinting = true;
        }
        else 
        {
            playerLocomotion.isSprinting = false;
        }
    }

    private void HandleSquatInput()
    {
        if (c_Input)
        {
            playerLocomotion.isSquating = true;
        }
        else
        {
            playerLocomotion.isSquating = false;
        }
    }

    private void HandleJumpingInput()
    {
        if (jump_input)
        {
            jump_input = false;
            playerLocomotion.HandleJumping();
        
        }
    
    }

    private void HandleDodgeInput() 
    {
        if (x_Input)
        {
            x_Input = false;
            playerLocomotion.HandleDodge();
        }
    }
}
