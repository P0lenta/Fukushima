using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Valores de velocidade")]
    public float _speed = 10f;            
    public float _jumpForce = 10f;
    public float _rotationSpeed = 720f;

    [Header("Ajustes de gravidade")]
    public float _fallMultiplyer = 2.5f; 
    public float _jumpMultiplyer = 2f;

    [Header("Colisores")]
    public Rigidbody _rig;               
    public Collider _col;                
    public LayerMask _floorLayers; 
    public Animator _moveAnimation;
    public FixerPlatform _fixerPlatform;

    private Vector3 _moveInput;
    private Vector2 _input;
    private float _currentMagnitude;
    private bool _isJumpPressed;

    private Rigidbody _currentPlatformRb;
    private Vector3 _lastPlatformPosition;

    private bool _isControlsLocked = false;

    private void OnEnable()
    {
        FuseSelectionUI.OnMenuOpened += TravarControles;
        FuseSelectionUI.OnMenuClosed += DestravarControles;
    }

    private void OnDisable()
    {
        FuseSelectionUI.OnMenuOpened -= TravarControles;
        FuseSelectionUI.OnMenuClosed -= DestravarControles;
    }

    private void TravarControles()
    {
        _isControlsLocked = true;
        StopMovement(); 
    }

    private void DestravarControles()
    {
        _isControlsLocked = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (_isControlsLocked)
        {
            StopMovement();
            return;
        }

        _input = context.ReadValue<Vector2>();
        Vector3 _direction = new Vector3(_input.x, 0, _input.y);
        _moveInput = _direction.normalized * _speed;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (_isControlsLocked) return;

        if (context.started)  
        {
            _isJumpPressed = true;

            if (Physics.Raycast(_col.bounds.center, Vector3.down, _col.bounds.extents.y * 1.1f, _floorLayers))
            {
                _rig.linearVelocity = new Vector3(
                    _rig.linearVelocity.x,
                    _jumpForce,
                    _rig.linearVelocity.z);
            }
        }
        else if (context.canceled) _isJumpPressed = false;
    }

    private void Update() 
    {
        _currentMagnitude = _moveInput.magnitude;

        if (_moveAnimation != null)
            _moveAnimation.SetFloat("MoveMagnitude", _currentMagnitude);
    }

    void FixedUpdate()
    {
        Vector3 movementVector = _isControlsLocked ? Vector3.zero : _moveInput;

        Vector3 _platformVelocity = _fixerPlatform != null ? _fixerPlatform._platformGetVelocity : Vector3.zero;

        float _targetY = _rig.linearVelocity.y;

        if (_platformVelocity.y < 0 && !_isJumpPressed && _rig.linearVelocity.y <= 0.01f) _targetY = _platformVelocity.y;

        _rig.linearVelocity = new Vector3(movementVector.x + _platformVelocity.x, _targetY, movementVector.z + _platformVelocity.z);

        ApplyJumpGravity();

        if (!_isControlsLocked)
        {
            RotateCharacter();
        }

        if (_currentPlatformRb != null)
        {
            _lastPlatformPosition = _currentPlatformRb.position;
        }
    }

    public void StopMovement()
    {
        _input = Vector2.zero;
        _moveInput = Vector3.zero;
    }

    private void ApplyJumpGravity()
    {
        if (_rig.linearVelocity.y < 0)
        {
            _rig.linearVelocity += Vector3.up * Physics.gravity.y * (_fallMultiplyer - 1) * Time.deltaTime;
        }
        else if (_rig.linearVelocity.y > 0 && !_isJumpPressed)
        {
            _rig.linearVelocity += Vector3.up * Physics.gravity.y * (_jumpMultiplyer - 1) * Time.deltaTime;
        }
    }

    void RotateCharacter()
    {
        if (_currentMagnitude > 0.1f)
        {
            Vector3 _snapInput = Vector3.zero;

            if (Mathf.Abs(_input.x) > Mathf.Abs(_input.y))
            {
                _snapInput.x = Mathf.Sign(_input.x);
            }
            else
            {
                _snapInput.z = Mathf.Sign(_input.y);
            }

            Quaternion _rotation = Quaternion.LookRotation(_snapInput, Vector3.up);

            Quaternion targetRotation = Quaternion.RotateTowards(_rig.rotation, _rotation, _rotationSpeed * Time.fixedDeltaTime);
            
            _rig.MoveRotation(targetRotation);
        }
        else
        {
            Vector3 _currentEuler = _rig.rotation.eulerAngles;
            float _snapY = Mathf.Round(_currentEuler.y / 90f) * 90f;
            
            _rig.MoveRotation(Quaternion.Euler(0f, _snapY, 0f));
        }
    }
}