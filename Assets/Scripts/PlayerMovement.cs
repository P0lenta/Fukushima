using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

[Header("Valores de velocidade")]
    public float _speed = 10f;            
    public float _jumpForce = 10f;
    public float _crouchmultiplyer = 0.5f;
    public float _rotationSpeed = 720f;

[Header("Ajustes de gravidade")]
    public float _fallMultiplyer = 2.5f; 
    public float _jumpMultiplyer = 2f;

[Header("Colisores")]
    public Rigidbody _rig;               
    public Collider _col;                 
    public LayerMask _floorLayers; 
    public Animator _moveAnimation;
    private Vector3 _moveInput;
    private Vector2 _input;
    private float _currentMagnitude;
    private bool _isJumpPressed;

    public void OnMove(InputAction.CallbackContext context)
    {
            _input = context.ReadValue<Vector2>();

            Vector3 _direction = new Vector3(_input.x, 0, _input.y);

            _moveInput = _direction.normalized * _speed;
    }

    private void Update() 
    {
        _currentMagnitude = _moveInput.magnitude;

        _moveAnimation.SetFloat("MoveMagnitude", _currentMagnitude);
    }
    

    void FixedUpdate()
    {
        Vector3 _targetVelocity = new Vector3(_moveInput.x, _rig.linearVelocity.y,_moveInput.z);
        _rig.linearVelocity = _targetVelocity;

        ApplyJumpGravity();

        RotateCharacter();
    }

    public void StopMovement()
    {
        _input = Vector2.zero;
        _moveInput = Vector3.zero;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
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

    public void OnCrounch(InputAction.CallbackContext context)
    {
        
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

            transform.rotation = Quaternion.RotateTowards
            (transform.rotation, _rotation, _rotationSpeed * Time.deltaTime);
            
        }
        else
        {
            Vector3 _currentEuler = transform.eulerAngles;
            float _snapY = Mathf.Round(_currentEuler.y / 90f) * 90f;
            transform.rotation = Quaternion.Euler(0f, _snapY, 0f);
        }
    }

}