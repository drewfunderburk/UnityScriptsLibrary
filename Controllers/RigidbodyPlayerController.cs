using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class RigidbodyPlayerController : MonoBehaviour
{
    private enum PlayerState
    {
        NORMAL,
        AIRBORN,
        CROUCHING,
    }

    #region SERIALIZED FIELDS
    [Tooltip("Base movement speed")]
    [SerializeField] private float _baseMoveSpeed;

    [Tooltip("How fast to sprint")]
    [SerializeField] private float _sprintSpeedPercentage = 1.3f;

    [Space]
    [Tooltip("How high in units to jump")]
    [SerializeField] private float _jumpHeight = 1;

    [Tooltip("How much force to apply to the rigidbody when moving while airborn")]
    [SerializeField] private float _airMovementForce;

    [Space]
    [Tooltip("Crouch height reduction percentage")]
    [SerializeField] [Range(0.5f, 1)] private float _crouchHeightPercentage = 0.5f;

    [Tooltip("Crouch speed reduction percentage")]
    [SerializeField] [Range(0, 1)] private float _crouchSpeedPercentage = 0.5f;

    [Tooltip("Crouch camera position")]
    [SerializeField] private Vector3 _crouchCameraPosition = new Vector3();

    [Tooltip("How fast the camera lerps into the crouch")]
    [SerializeField] private float _crouchSpeedFactor = 0.1f;

    [Space]
    [Tooltip("Where to check for ground")]
    [SerializeField] private Vector3 _groundCheck = new Vector3();

    [Tooltip("Ground check size")]
    [SerializeField] private float _groundCheckRadius = 0.75f;

    [Tooltip("What is considered ground")]
    [SerializeField] private LayerMask _groundMask = ~0;

    [Tooltip("Object that holds the camera")]
    [SerializeField] private GameObject _camera;
    #endregion

    #region VARIABLES
    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;

    // Actual speed used internally
    private float _speed;
    private float _airSpeed;
    private Vector3 _camDefaultPosition;
    private Vector3 _colliderDefaultCenter;
    private float _colliderDefaultHeight;
    private Vector3 _camTargetPosition;

    private bool _isGrounded;
    private bool _isMoving = false;
    private bool _canJump = false;
    private bool _updateCrouch = false;
    private bool _needJumpVelocity = true;

    private bool _jumpInput = false;
    private bool _crouchInput = false;
    private bool _sprintInput = false;
    private Vector3 _movementInput = new Vector3();

    private PlayerState _playerState = PlayerState.NORMAL;
    #endregion

    private void Awake()
    {
        // Cache components
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();

        // Cache camera
        _camDefaultPosition = _camera.transform.localPosition;
    }

    private void Start()
    {
        // Set initial speed
        _speed = _baseMoveSpeed;

        // Cache initial CharacterController values for crouching
        _colliderDefaultHeight = _collider.height;
        _colliderDefaultCenter = _collider.center;
    }

    private void FixedUpdate()
    {
        // Ground check
        _isGrounded = Physics.CheckSphere(transform.position + _groundCheck, _groundCheckRadius, _groundMask);

        // Jumping/falling and air velocity
        if (!_isGrounded)
        {
            _playerState = PlayerState.AIRBORN;
        }
        else
        {
            // Zero out horizontal velocity when grounded to prevent slipping
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
        }

        switch (_playerState)
        {
            case PlayerState.NORMAL:
                Sprint();
                Move();

                // Transition to jump
                if (_jumpInput)
                    _playerState = PlayerState.AIRBORN;

                // Transition to crouch
                if (_crouchInput)
                    _playerState = PlayerState.CROUCHING;
                break;
            case PlayerState.AIRBORN:
                Jump();
                Sprint();
                Move();

                // Transition to normal
                if (_isGrounded)
                {
                    _speed = _baseMoveSpeed;
                    _playerState = PlayerState.NORMAL;
                }
                break;
            case PlayerState.CROUCHING:
                SetCrouchPosition();
                Move();

                // Transition to normal
                if (!_crouchInput)
                {
                    SetUncrouchPosition();
                    _playerState = PlayerState.NORMAL;
                }

                // Transition to jump
                if (_jumpInput)
                {
                    SetUncrouchPosition();
                    _playerState = PlayerState.AIRBORN;
                }
                break;
            case PlayerState.SLIDING:
                break;
            case PlayerState.WALLRUNNING:
                break;
        }

        // Update crouch
        if (_updateCrouch)
        {
            // Lerp to the crouch or uncrouch position
            _camera.transform.localPosition = Vector3.Lerp(_camera.transform.localPosition, _camTargetPosition, _crouchSpeedFactor * Time.deltaTime);

            // If cam has arrived, stop updating crouch
            if (Vector3.Distance(_camera.transform.position, _camTargetPosition) < 0.01f)
                _updateCrouch = false;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Draw ground check
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + _groundCheck, _groundCheckRadius);
    }
#endif

    private bool CanUnCrouch()
    {
        // TODO: This is in local space for some reason?
        if (Physics.Raycast(_colliderDefaultCenter, Vector3.up * (_colliderDefaultHeight * 0.5f)))
            return true;
        return false;
    }

    /// <summary>
    /// Enter a crouch
    /// </summary>
    private void SetCrouchPosition()
    {
        // Adjust speed
        _speed = _baseMoveSpeed * _crouchSpeedPercentage;

        // Shrink height
        _collider.height = _colliderDefaultHeight * (1 - _crouchHeightPercentage);

        // Shrink center
        Vector3 center = new Vector3(_colliderDefaultCenter.x, _colliderDefaultCenter.y - (_collider.height * _crouchHeightPercentage), _colliderDefaultCenter.z);
        _collider.center = center;

        // Set cam target position
        _camTargetPosition = _crouchCameraPosition;
        _updateCrouch = true;
    }

    /// <summary>
    /// Exit a crouch
    /// </summary>
    private void SetUncrouchPosition()
    {
        // Reset variables to defaults
        _speed = _baseMoveSpeed;
        _collider.center = _colliderDefaultCenter;
        _collider.height = _colliderDefaultHeight;
        _camTargetPosition = _camDefaultPosition;
        _updateCrouch = true;
    }

    /// <summary>
    /// Handle the player's jump
    /// </summary>
    private void Jump()
    {
        // Only jump if we can
        if (!_canJump)
            return;

        _canJump = false;
        _rigidbody.AddForce(Vector3.up * Mathf.Sqrt(_jumpHeight * -2f * Physics.gravity.y), ForceMode.Impulse);
    }

    /// <summary>
    /// Handle the player's movement
    /// </summary>
    private void Move()
    {
        // Don't move if no input
        if (!_isMoving)
            return;

        // Calculate local movement
        Vector3 xMovement = transform.right * _movementInput.x;
        Vector3 zMovement = transform.forward * _movementInput.z;
        Vector3 movement = xMovement + zMovement;

        if (_isGrounded)
        {
            // Move _characterController based on movement

            _rigidbody.MovePosition(transform.position + (movement * _speed * Time.deltaTime));
        }
        else
        {
            if (_needJumpVelocity)
            {
                // Set airspeed with the speed at jump to prevent max speed changes mid air
                _airSpeed = _speed;

                // Calculate and set the rigidbody's velocity 
                Vector3 velocity = (transform.position + (movement * _speed)) - transform.position;
                _rigidbody.velocity = new Vector3(velocity.x, _rigidbody.velocity.y, velocity.z);
                _needJumpVelocity = false;
            }

            // Add force instead of MovePosition in air for better feel
            _rigidbody.AddForce(movement * _airMovementForce * Time.deltaTime);

            // Clamp horizontal velocity magnitude
            Vector2 clampedVelocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z);
            clampedVelocity = Vector2.ClampMagnitude(clampedVelocity, _airSpeed);
            _rigidbody.velocity = new Vector3(clampedVelocity.x, _rigidbody.velocity.y, clampedVelocity.y);
        }
    }

    /// <summary>
    /// Multiply the base movespeed by a percentage
    /// </summary>
    private void Sprint()
    {
        if (_sprintInput)
            _speed = _baseMoveSpeed * _sprintSpeedPercentage;
        else
            _speed = _baseMoveSpeed;
    }

    #region INPUT METHODS
    /// <summary>
    /// Performs movement actions upon receiving input
    /// </summary>
    public void OnMovement(InputAction.CallbackContext value)
    {
        _isMoving = true;
        Vector2 input = value.ReadValue<Vector2>();
        _movementInput = new Vector3(input.x, 0, input.y);

        if (value.canceled)
            _isMoving = false;
    }

    /// <summary>
    /// Performs sprint actions upon receiving input
    /// </summary>
    public void OnSprint(InputAction.CallbackContext value)
    {
        if (value.started)
            _sprintInput = true;

        if (value.canceled)
            _sprintInput = false;
    }

    /// <summary>
    /// Performs jump actions upon receiving input
    /// </summary>
    public void OnJump(InputAction.CallbackContext value)
    {
        // Do not allow jumping in the air
        if (_isGrounded && value.started)
        {
            _jumpInput = true;
            _canJump = true;
            _needJumpVelocity = true;
        }

        if (value.canceled)
            _jumpInput = false;
    }

    /// <summary>
    /// Performs crouch actions upon receiving input
    /// </summary>
    public void OnCrouch(InputAction.CallbackContext value)
    {
        // Do not allow crouching if we are in the air
        if (_isGrounded && value.started)
            _crouchInput = true;

        if (value.canceled)
            _crouchInput = false;
    }
    #endregion
}
