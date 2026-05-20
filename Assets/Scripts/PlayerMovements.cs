using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float crouchingSpeed;
    [SerializeField] private float mouseSensitivity;
        
    [Header("Jump and Fall")]
    [SerializeField]  private float jumpForce = 7f;
    [SerializeField] private float gravity = -12f;
    [SerializeField] private float initialFallVelocity = -2f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference crouchAction;
    [SerializeField] private InputActionReference jumpAction;
    
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private Vector2 _sprintInput;
    private Vector2 _crouchInput;
    
    private float _verticalVelocity;
    private float _verticalAngle;

    private bool _isGrounded;
    private bool _isSprinting;
    private bool _isCrouching;
    
    private CharacterController _characterController;
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        _isGrounded = _characterController.isGrounded;
        HandleGravity();
        HandleCamera();
        HandleMovement();
    }

    private void OnEnable()
    {
        moveAction.action.performed += StoreMovementInput;
        moveAction.action.canceled += StoreMovementInput;
        
        lookAction.action.performed += StoreCameraInput;
        lookAction.action.canceled += StoreCameraInput;

        sprintAction.action.performed += Sprint;
        sprintAction.action.canceled += Sprint;

        crouchAction.action.performed += Crouch;
        crouchAction.action.canceled += Crouch;

        jumpAction.action.performed += Jump;
    }
    
    private void OnDisable()
    {
        moveAction.action.performed -= StoreMovementInput;
        moveAction.action.canceled -= StoreMovementInput;
        
        lookAction.action.performed -= StoreCameraInput;
        lookAction.action.canceled -= StoreCameraInput;
        
        sprintAction.action.performed -= Sprint;

        crouchAction.action.performed -= Crouch;
        
        jumpAction.action.performed -= Jump;
    }

    private void StoreMovementInput(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }

    private void StoreCameraInput(InputAction.CallbackContext ctx)
    {
        _lookInput = ctx.ReadValue<Vector2>();
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (_isGrounded)
        {
            _verticalVelocity = jumpForce;
        }
    }

    private void Sprint(InputAction.CallbackContext ctx)
    {
        _isSprinting = !_isSprinting;
    }

    private void Crouch(InputAction.CallbackContext ctx)
    {
        _isCrouching = !_isCrouching;
    }

    private void HandleMovement()
    {
        var move = cameraTransform.TransformDirection(_moveInput.x, 0, _moveInput.y).normalized;
        var finalMove = move * (_isSprinting ? runningSpeed : walkingSpeed);
        finalMove.y = _verticalVelocity;
        
        var collisions = _characterController.Move(finalMove * Time.deltaTime);
        if ((collisions & CollisionFlags.Above) != 0) _verticalVelocity = initialFallVelocity;
    }

    private void HandleCamera()
    {
        transform.Rotate(0, _lookInput.x * mouseSensitivity, 0);
    
        _verticalAngle -= _lookInput.y * mouseSensitivity;
        _verticalAngle = Mathf.Clamp(_verticalAngle, -90f, 90f);

        cameraTransform.localEulerAngles = new Vector3(_verticalAngle, 0, 0);
    }
    
    private void HandleGravity()
    {
        if (_isGrounded && _verticalVelocity < 0) _verticalVelocity = initialFallVelocity;
        _verticalVelocity += gravity * Time.deltaTime;
    }
}
