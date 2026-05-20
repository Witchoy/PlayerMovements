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
    [SerializeField] private float initialFallVelocity = -2f;
    [SerializeField] private float gravity = -12f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
 
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    
    private float _verticalVelocity;
    private float _verticalAngle;

    private bool _isGrounded;
    
    private CharacterController _characterController;
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();    
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
    }
    
    private void OnDisable()
    {
        moveAction.action.performed -= StoreMovementInput;
        moveAction.action.canceled -= StoreMovementInput;
        
        lookAction.action.performed -= StoreCameraInput;
        lookAction.action.canceled -= StoreCameraInput;
    }

    private void StoreMovementInput(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }

    private void StoreCameraInput(InputAction.CallbackContext ctx)
    {
        _lookInput = ctx.ReadValue<Vector2>();
    }

    private void HandleMovement()
    {
        var move = cameraTransform.TransformDirection(_moveInput.x, 0, _moveInput.y).normalized;
        var finalMove = move * walkingSpeed;
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
