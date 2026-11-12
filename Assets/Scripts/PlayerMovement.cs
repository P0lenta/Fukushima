using System.Collections;
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

    [Header("Agarrar Beirada (Ledge Grab)")]
    [Tooltip("Distância à frente do corpo para detectar a parede da plataforma.")]
    public float _ledgeReach = 0.4f;
    [Tooltip("Altura (a partir do centro do colisor) onde ficam as mãos do personagem.")]
    public float _handHeightOffset = 0.3f;
    [Tooltip("Distância acima das mãos que precisa estar livre para considerar uma quina.")]
    public float _headClearance = 0.5f;
    [Tooltip("Tempo que leva para o personagem subir na plataforma ao apertar Espaço.")]
    public float _climbDuration = 0.25f;

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

    // Estados do Ledge Grab
    private bool _isGrabbingLedge = false;
    private bool _isClimbingLedge = false;
    private float _ledgeCooldownTimer = 0f;
    private Vector3 _climbTargetPosition;

    private float _lastMoveX = 0f;
    private float _lastMoveY = -1f;

    public static PlayerMovement Instance { get; private set; }
    public static System.Action OnPlayerJumped;
    public bool _isMoving => !_isGrabbingLedge && !_isClimbingLedge && _moveInput.magnitude > 0.1f;

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
        if (_isGrabbingLedge) SoltarBeirada();
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

        if (!_isGrabbingLedge && !_isClimbingLedge && _input.magnitude > 0.1f) 
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
        if (_isControlsLocked || _isClimbingLedge) return;

        if (context.started)  
        {
            _isJumpPressed = true;

            if (_isGrabbingLedge)
            {
                StartCoroutine(SubirBeiradaRoutine());
                return;
            }

            if (IsGrounded())
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
        if (_ledgeCooldownTimer > 0f)
        {
            _ledgeCooldownTimer -= Time.deltaTime;
        }

        _currentMagnitude = _moveInput.magnitude;
        AtualizarVisuais2D();
    }

    void FixedUpdate()
    {
        _rig.MoveRotation(Quaternion.identity);

        if (_isGrabbingLedge || _isClimbingLedge)
        {
            _rig.linearVelocity = Vector3.zero;
            return;
        }

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

        if (!_isControlsLocked && _ledgeCooldownTimer <= 0f && !IsGrounded() && _rig.linearVelocity.y <= 2f)
        {
            DetectarBeirada();
        }
    }

    private void DetectarBeirada()
    {
        Vector3 _forwardDir = ObterDirecaoVisual();
        float _radius = Mathf.Max(_col.bounds.extents.x, _col.bounds.extents.z);
        float _totalReach = _radius + _ledgeReach;

        Vector3 _handOrigin = _col.bounds.center + Vector3.up * _handHeightOffset;
        Vector3 _headOrigin = _handOrigin + Vector3.up * _headClearance;

        if (Physics.Raycast(_handOrigin, _forwardDir, out RaycastHit _wallHit, _totalReach, _floorLayers) &&
            !Physics.Raycast(_headOrigin, _forwardDir, _totalReach + 0.15f, _floorLayers))
        {
            Vector3 _topRayOrigin = new Vector3(_wallHit.point.x, _headOrigin.y, _wallHit.point.z) + _forwardDir * 0.25f;

            if (Physics.Raycast(_topRayOrigin, Vector3.down, out RaycastHit _topHit, _headClearance + 0.5f, _floorLayers))
            {
                IniciarAgarrar(_wallHit, _topHit, _forwardDir, _radius);
            }
        }
    }

    private void IniciarAgarrar(RaycastHit _wallHit, RaycastHit _topHit, Vector3 _forwardDir, float _radius)
    {
        _isGrabbingLedge = true;
        _rig.useGravity = false;
        _rig.linearVelocity = Vector3.zero;

        Vector3 _pivotToCenter = _col.bounds.center - transform.position;

        Vector3 _hangCenter = new Vector3(_wallHit.point.x, _topHit.point.y - _handHeightOffset, _wallHit.point.z) + _wallHit.normal * (_radius + 0.05f);
        transform.position = _hangCenter - _pivotToCenter;

        Vector3 _standCenter = _topHit.point + Vector3.up * (_col.bounds.extents.y + 0.05f) + _forwardDir * (_radius + 0.2f);
        _climbTargetPosition = _standCenter - _pivotToCenter;

        TriggerGrab();
    }

    private IEnumerator SubirBeiradaRoutine()
    {
        _isGrabbingLedge = false;
        _isClimbingLedge = true;

        if (_animator != null) _animator.SetBool("IsGrabbing", false);

        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < _climbDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / _climbDuration);
            transform.position = Vector3.Lerp(startPos, _climbTargetPosition, t);
            yield return null;
        }

        transform.position = _climbTargetPosition;
        _isClimbingLedge = false;
        _rig.useGravity = true;
        _ledgeCooldownTimer = 0.4f;
    }

    public void SoltarBeirada()
    {
        _isGrabbingLedge = false;
        _isClimbingLedge = false;
        _rig.useGravity = true;
        _ledgeCooldownTimer = 0.4f;
        if (_animator != null) _animator.SetBool("IsGrabbing", false);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(_col.bounds.center, Vector3.down, _col.bounds.extents.y * 1.2f, _floorLayers);
    }

    private void AtualizarVisuais2D()
    {
        if (_animator == null || _spriteRenderer == null) return;

        if (!_isGrabbingLedge && !_isClimbingLedge && _input.x != 0)
        {
            bool _indoParaEsquerda = _input.x < 0;
            _spriteRenderer.flipX = _inverterFlipSide ? !_indoParaEsquerda : _indoParaEsquerda;
        }

        _animator.SetFloat("MoveX", _lastMoveX);
        _animator.SetFloat("MoveY", _lastMoveY);
        _animator.SetBool("IsMoving", _isMoving);
        _animator.SetBool("IsGrabbing", _isGrabbingLedge);

        bool _isGrounded = IsGrounded();
        _animator.SetBool("IsGrounded", _isGrounded);
        _animator.SetFloat("VerticalVelocity", _rig.linearVelocity.y);
    }

    public void StopMovement()
    {
        _input = Vector2.zero;
        _moveInput = Vector3.zero;

        if (_rig != null)
        {
            _rig.linearVelocity = new Vector3(0f, _rig.linearVelocity.y, 0f);
        }
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
        if (_animator != null)
        {
            _animator.SetTrigger("Grab");
            _animator.SetBool("IsGrabbing", true);
        }
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

    private void OnDrawGizmosSelected()
    {
        if (_col == null) return;

        Vector3 forwardDir = Application.isPlaying ? ObterDirecaoVisual() : Vector3.right;
        float radius = Mathf.Max(_col.bounds.extents.x, _col.bounds.extents.z);
        float totalReach = radius + _ledgeReach;

        Vector3 handOrigin = _col.bounds.center + Vector3.up * _handHeightOffset;
        Vector3 headOrigin = handOrigin + Vector3.up * _headClearance;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(handOrigin, handOrigin + forwardDir * totalReach);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(headOrigin, headOrigin + forwardDir * (totalReach + 0.15f));
    }
}