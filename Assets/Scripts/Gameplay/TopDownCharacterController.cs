using UnityEngine;
using UnityEngine.InputSystem;
using Gameplay;
using System;

[RequireComponent(typeof(Rigidbody))] // Required for FixedUpdate
[RequireComponent(typeof(PlayerScript))]
public class TopDownCharacterController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    
    [SerializeField] private float moveSpeedWithSack = 2f;
    
    private Rigidbody rb;
    private Vector2 moveInput;
    private Animator animator;

    [SerializeField]
    public ControllingType currentControlType = ControllingType.Keyboard;
    
    public event Action<ControllingType> OnControlTypeChanged;
    
    public ControllingType CurrentControlType => currentControlType;
    
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    
    private void Update()
    {
        HandleInput();
    }
    
    private void FixedUpdate()
    {
        Move();
        Rotate();
    }
    
    private void HandleInput()
    {
        // Check for keyboard input
        bool keyboardActive = CheckForKeyboardInput();
        
        // Check for gamepad input
        bool gamepadActive = CheckForGamepadInput();
        
        // Determine which input to use and update control type if needed
        UpdateControlType(keyboardActive, gamepadActive);
        
        // Get input from keyboard and gamepad
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        moveInput = Vector2.ClampMagnitude(new Vector2(horizontalInput, verticalInput), 1);

        if (moveInput.magnitude > 0.1f)
        {
            animator?.SetBool("Walking", true);
        }
        else
        {
            animator?.SetBool("Walking", false);
        }
    }
    
    private bool CheckForKeyboardInput()
    {
        // Check common WASD keys and arrow keys
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || 
               Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) ||
               Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) ||
               Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) ||
               Input.GetMouseButton(0) || Input.GetMouseButton(1);
    }
    
    private bool CheckForGamepadInput()
    {
        // Check for gamepad stick movement
        
        // Check for any gamepad button press
        bool anyGamepadButton = false;
        for (int i = 0; i < 20; i++) // Check common gamepad buttons
        {
            if (Input.GetKey("joystick button " + i))
            {
                anyGamepadButton = true;
                break;
            }
        }
        
        // Return true if there's significant stick movement or any button press
        return (anyGamepadButton);
    }
    
    private void UpdateControlType(bool keyboardActive, bool gamepadActive)
    {
        ControllingType newControlType = currentControlType;
        
        // Prioritize the most recent input
        if (keyboardActive)
        {
            newControlType = ControllingType.Keyboard;
        }
        else if (gamepadActive)
        {
            newControlType = ControllingType.Gamepad;
        }
        
        // If control type changed, notify listeners
        if (newControlType != currentControlType)
        {
            currentControlType = newControlType;
            OnControlTypeChanged?.Invoke(currentControlType);
        }
    }
    
    private void Move()
    {
        float mSpeed = moveSpeed;
        if (PlayerScript.Instance.carryingSack)
        {
            mSpeed = moveSpeedWithSack;
        }

        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y) * mSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }
    
    private void Rotate()
    {
        if (moveInput != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveInput.x, 0f, moveInput.y), Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }
}
