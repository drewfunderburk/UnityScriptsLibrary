using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles input binding. Override event methods for functionality
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2DBase : MonoBehaviour
{
    [SerializeField] protected float BaseMoveSpeed = 10;
    [Space]
    [SerializeField] protected float JumpHeight = 2;
    [SerializeField] protected Vector2 GroundCheck;
    [SerializeField] protected float GroundCheckRadius = 0.5f;
    [SerializeField] protected LayerMask GroundMask = ~0;
    [Space]

    protected Vector2 MovementInput = new Vector2();
    protected bool JumpInput = false;
    protected bool SwitchInput = false;
    protected bool AbilityInput = false;

    // Controls
    private PlayerControls _controls;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _switchAction;
    private InputAction _abilityAction;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        // Bind actions
        _controls = new PlayerControls();
        _moveAction = _controls.Player.Move;
        _jumpAction = _controls.Player.Jump;
        _switchAction = _controls.Player.Switch;
        _abilityAction = _controls.Player.Ability;
    }

    private void OnEnable()
    {
        // Enable actions
        _moveAction.Enable();
        _jumpAction.Enable();
        _switchAction.Enable();
        _abilityAction.Enable();

        // Attach functions to input events
        _controls.Player.Move.performed += OnMoveBegin;
        _controls.Player.Move.canceled += OnMoveEnd;
        _controls.Player.Move.Enable();
        _controls.Player.Jump.performed += OnJumpBegin;
        _controls.Player.Jump.canceled += OnJumpEnd;
        _controls.Player.Jump.Enable();
        _controls.Player.Switch.performed += OnSwitchBegin;
        _controls.Player.Switch.canceled += OnSwitchEnd;
        _controls.Player.Switch.Enable();
        _controls.Player.Ability.performed += OnAbilityBegin;
        _controls.Player.Ability.canceled += OnAbilityEnd;
        _controls.Player.Ability.Enable();
    }

    private void OnDisable()
    {
        // Disable actions
        _moveAction.Disable();
        _jumpAction.Disable();
        _switchAction.Disable();
        _abilityAction.Disable();

        // Detatch functions from input events
        _controls.Player.Move.performed -= OnMoveBegin;
        _controls.Player.Move.canceled -= OnMoveEnd;
        _controls.Player.Move.Disable();
        _controls.Player.Jump.performed -= OnJumpBegin;
        _controls.Player.Jump.canceled -= OnJumpEnd;
        _controls.Player.Jump.Disable();
        _controls.Player.Switch.performed -= OnSwitchBegin;
        _controls.Player.Switch.canceled -= OnSwitchEnd;
        _controls.Player.Switch.Disable();
        _controls.Player.Ability.performed -= OnAbilityBegin;
        _controls.Player.Ability.canceled -= OnAbilityEnd;
        _controls.Player.Ability.Disable();
    }

    private void FixedUpdate()
    {
        Vector2 movement = MovementInput * BaseMoveSpeed * Time.deltaTime;
        if (movement != Vector2.zero)
            _rigidbody.MovePosition((Vector2)transform.position + movement);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw GroundCheck
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + GroundCheck, GroundCheckRadius);
    }

    private bool IsGrounded()
    {
        // Check for colliders in GroundCheck
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)transform.position + GroundCheck, GroundCheckRadius, GroundMask);

        // Loop through colliders
        foreach (Collider2D collider in colliders)
        {
            // If there is one that's not this object
            if (collider.gameObject != gameObject)
            {
                // Apply jump force
                _rigidbody.AddForce(Vector2.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode2D.Impulse);
                return true;
            }
        }

        return false;
    }

    public void OnMoveBegin(InputAction.CallbackContext context)
    {
        MovementInput.x = context.ReadValue<Vector2>().x;
    }

    public void OnMoveEnd(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    public virtual void OnJumpBegin(InputAction.CallbackContext context)
    {
        JumpInput = context.ReadValue<float>() > 0.5f;

        // Check for colliders in GroundCheck
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)transform.position + GroundCheck, GroundCheckRadius, GroundMask);

        // Loop through colliders
        foreach (Collider2D collider in colliders)
        {
            // If there is one that's not this object
            if (collider.gameObject != gameObject)
            {
                // Apply jump force
                _rigidbody.AddForce(Vector2.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode2D.Impulse);
                return;
            }
        }
    }

    public virtual void OnJumpEnd(InputAction.CallbackContext context)
    {
        JumpInput = context.ReadValue<bool>();
    }

    public virtual void OnSwitchBegin(InputAction.CallbackContext context)
    {
        SwitchInput = context.ReadValue<float>() > 0.5f;
    }

    public virtual void OnSwitchEnd(InputAction.CallbackContext context)
    {
        SwitchInput = context.ReadValue<bool>();
    }

    public virtual void OnAbilityBegin(InputAction.CallbackContext context)
    {
        AbilityInput = context.ReadValue<float>() > 0.5f;
    }

    public virtual void OnAbilityEnd(InputAction.CallbackContext context)
    {
        AbilityInput = context.ReadValue<bool>();
    }
}