using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Valores de velocidade")]
    public float _speed = 10f;            
    public float _jumpForce = 10f;

    [Header("Ajustes de gravidade")]
    public float _fallMultiplyer = 2.5f; 
    public float _jumpMultiplyer = 2f;

    [Header("Colisores e Física")]
    public Rigidbody _rig;               
    public Collider _col;                
    public LayerMask _floorLayers; 
    public FixerPlatform _fixerPlatform;

    [Header("Visuais 2D")]
    public Animator _animator;
    public SpriteRenderer _spriteRenderer;
    [Tooltip("Se o seu sprite 'de lado' foi desenhado olhando para a direita, deixe falso. Se foi para a esquerda, marque verdadeiro.")]
    public bool _inverterFlipSide = false;

    private Vector3 _moveInput;
    private Vector2 _input;
    private float _currentMagnitude;
    private bool _isJumpPressed;

    private Rigidbody _currentPlatformRb;
    private Vector3 _lastPlatformPosition;
    private bool _isControlsLocked = false;

    private float _lastMoveX = 0f;
    private float _lastMoveY = -1f;

    public static PlayerMovement Instance { get; private set; }
    public static System.Action OnPlayerJumped;
    public bool _isMoving => _moveInput.magnitude > 0.1f;

    private void Awake()
    {
        Instance = this;
    }

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

        if (_input.magnitude > 0.1f) 
        {
            if (Mathf.Abs(_input.x) > Mathf.Abs(_input.y) + 0.1f)
            {
                _lastMoveX = _input.x > 0 ? 1f : -1f;
                _lastMoveY = 0f;
            }
            else if (Mathf.Abs(_input.y) > Mathf.Abs(_input.x) + 0.1f)
            {
                _lastMoveX = 0f;
                _lastMoveY = _input.y > 0 ? 1f : -1f;
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (_isControlsLocked) return;

        if (context.started)  
        {
            _isJumpPressed = true;

            if (Physics.Raycast(_col.bounds.center, Vector3.down, _col.bounds.extents.y * 1.2f, _floorLayers))
            {
                _rig.linearVelocity = new Vector3(_rig.linearVelocity.x, _jumpForce, _rig.linearVelocity.z);
                OnPlayerJumped?.Invoke();
                
                if (_animator != null) _animator.SetTrigger("Jump");
            }
        }
        else if (context.canceled) 
        {
            _isJumpPressed = false;
        }
    }

    private void Update() 
    {
        _currentMagnitude = _moveInput.magnitude;
        AtualizarVisuais2D();
    }

    void FixedUpdate()
    {
        Vector3 movementVector = _isControlsLocked ? Vector3.zero : _moveInput;
        Vector3 _platformVelocity = _fixerPlatform != null ? _fixerPlatform._platformGetVelocity : Vector3.zero;

        float _targetY = _rig.linearVelocity.y;
        if (_platformVelocity.y < 0 && !_isJumpPressed && _rig.linearVelocity.y <= 0.01f) _targetY = _platformVelocity.y;

        _rig.linearVelocity = new Vector3(movementVector.x + _platformVelocity.x, _targetY, movementVector.z + _platformVelocity.z);

        ApplyJumpGravity();

        if (_currentPlatformRb != null)
        {
            _lastPlatformPosition = _currentPlatformRb.position;
        }

        _rig.MoveRotation(Quaternion.identity); 
    }

    private void AtualizarVisuais2D()
    {
        if (_animator == null || _spriteRenderer == null) return;

        if (_input.x != 0)
        {
            bool _indoParaEsquerda = _input.x < 0;
            _spriteRenderer.flipX = _inverterFlipSide ? !_indoParaEsquerda : _indoParaEsquerda;
        }

        _animator.SetFloat("MoveX", _lastMoveX);
        _animator.SetFloat("MoveY", _lastMoveY);
        _animator.SetBool("IsMoving", _isMoving);

        bool _isGrounded = Physics.Raycast(_col.bounds.center, Vector3.down, _col.bounds.extents.y * 1.2f, _floorLayers);
        _animator.SetBool("IsGrounded", _isGrounded);
        _animator.SetFloat("VerticalVelocity", _rig.linearVelocity.y);

        Debug.Log($"<color=yellow>MoveX: {_lastMoveX} | MoveY: {_lastMoveY} | IsMoving: {_isMoving}</color>");
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

    public void TriggerInteract()
    {
        if (_animator != null) _animator.SetTrigger("Interact");
    }

    public void TriggerGrab()
    {
        if (_animator != null) _animator.SetTrigger("Grab");
    }

    public void TriggerDeath()
    {
        if (_animator != null) 
        {
            _animator.SetTrigger("Death");
            TravarControles();
        }
    }

    public Vector3 ObterDirecaoVisual()
    {
        Vector3 _direcao = new Vector3(_lastMoveX, 0f, _lastMoveY).normalized;
        return _direcao == Vector3.zero ? Vector3.forward : _direcao;
    }
}