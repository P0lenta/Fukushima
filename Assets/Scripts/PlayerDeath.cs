using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerDeath : MonoBehaviour
{
    public static PlayerDeath Instance { get; private set; }

    public static event Action OnPlayerDied;
    public static event Action<float> OnFadeOutRequested;

    [Header("Animações e Tempos")]
    public Animator _playerAnimator;
    public float _tempoAteReset = 2.5f; 
    public float _fadeDuration = 1.5f;

    [Header("Limite de Queda do Mundo")]
    [Tooltip("Se a posição Y do Player for menor ou igual a esse valor, a morte por queda é disparada automaticamente.")]
    public float _killYThreshold = -10f;

    [Header("Morte na Água")]
    [Tooltip("Tempo contínuo em contato com a layer 'Water' para morrer.")]
    public float _tempoParaMorrerNaAgua = 0.5f;

    public bool _isDead { get; private set; } = false;

    private Rigidbody _rig;
    private Collider _col;
    private LayerMask _waterMask;
    private float _waterContactTimer = 0f;
    private bool _isInWater = false;
    private Vector3 _lastPosition;
    private readonly HashSet<Collider> _activeWaterColliders = new HashSet<Collider>();

    private void Awake()
    {
        Instance = this;
        _waterMask = LayerMask.GetMask("Water");
        _rig = GetComponentInParent<Rigidbody>();
        _col = GetComponentInParent<Collider>();
        if (_col == null) _col = GetComponentInChildren<Collider>();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        if (PlayerMovement.Instance != null)
        {
            if (_rig == null) _rig = PlayerMovement.Instance._rig;
            if (_col == null) _col = PlayerMovement.Instance._col;
        }

        _lastPosition = _rig != null ? _rig.position : transform.position;

        if (_waterMask == 0)
        {
            Debug.LogWarning("[PlayerDeath] A layer 'Water' não foi encontrada. Crie-a em Edit > Project Settings > Tags and Layers.");
        }
    }

    private void FixedUpdate()
    {
        Vector3 posAtual = _rig != null ? _rig.position : transform.position;

        if (_isDead) return;

        if (PlayerMovement.Instance != null && PlayerMovement.Instance._isCheatModeActive)
        {
            _lastPosition = posAtual;
            return;
        }

        if (posAtual.y <= _killYThreshold)
        {
            Morrer("Queda");
            return;
        }

        VerificarContatoComAgua(posAtual);
        _lastPosition = _rig != null ? _rig.position : transform.position;
    }

    private void VerificarContatoComAgua(Vector3 posAtual)
    {
        _activeWaterColliders.RemoveWhere(c => c == null || !c.enabled || !c.gameObject.activeInHierarchy);

        bool tocouAgua = _activeWaterColliders.Count > 0;

        // 1. Checa sobreposição direta na caixa do colisor do Player
        if (!tocouAgua && _waterMask != 0)
        {
            Vector3 center = _col != null ? _col.bounds.center : posAtual;
            Vector3 halfExtents = _col != null ? _col.bounds.extents : new Vector3(0.3f, 0.5f, 0.3f);

            tocouAgua = Physics.CheckBox(
                center,
                halfExtents,
                Quaternion.identity,
                _waterMask,
                QueryTriggerInteraction.Collide
            );
        }

        // 2. Anti-tunneling: se caiu rápido e atravessou um plano fino de água entre dois frames, puxa de volta para a superfície
        if (!tocouAgua && _waterMask != 0 && _lastPosition != posAtual)
        {
            if (Physics.Linecast(_lastPosition, posAtual, out RaycastHit hit, _waterMask, QueryTriggerInteraction.Collide))
            {
                tocouAgua = true;
                if (_rig != null)
                {
                    _rig.position = new Vector3(posAtual.x, hit.point.y, posAtual.z);
                }
                else
                {
                    transform.position = new Vector3(posAtual.x, hit.point.y, posAtual.z);
                }
            }
        }

        if (tocouAgua)
        {
            if (!_isInWater)
            {
                _isInWater = true;
                CongelarNaAgua();
            }

            _waterContactTimer += Time.fixedDeltaTime;

            if (_waterContactTimer >= _tempoParaMorrerNaAgua)
            {
                Morrer("Agua");
            }
        }
        else
        {
            if (_isInWater)
            {
                _isInWater = false;
                DescongelarDaAgua();
            }

            _waterContactTimer = 0f;
        }
    }

    private bool PertenceALayerWater(GameObject obj)
    {
        if (_waterMask == 0 || obj == null) return false;

        Transform atual = obj.transform;
        while (atual != null)
        {
            if ((_waterMask.value & (1 << atual.gameObject.layer)) != 0)
                return true;
            atual = atual.parent;
        }
        return false;
    }

    private void CongelarNaAgua()
    {
        if (_rig == null) return;

        _rig.linearVelocity = Vector3.zero;
        _rig.angularVelocity = Vector3.zero;
        _rig.useGravity = false;
        _rig.isKinematic = true;
    }

    private void DescongelarDaAgua()
    {
        if (_rig == null || _isDead) return;

        _rig.isKinematic = false;
        _rig.useGravity = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PertenceALayerWater(other.gameObject))
        {
            _activeWaterColliders.Add(other);

            bool emCheat = PlayerMovement.Instance != null && PlayerMovement.Instance._isCheatModeActive;
            if (!_isDead && !emCheat)
            {
                _isInWater = true;
                CongelarNaAgua();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (PertenceALayerWater(other.gameObject))
        {
            _activeWaterColliders.Remove(other);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (PertenceALayerWater(collision.gameObject))
        {
            _activeWaterColliders.Add(collision.collider);

            bool emCheat = PlayerMovement.Instance != null && PlayerMovement.Instance._isCheatModeActive;
            if (!_isDead && !emCheat)
            {
                _isInWater = true;
                CongelarNaAgua();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (PertenceALayerWater(collision.gameObject))
        {
            _activeWaterColliders.Remove(collision.collider);
        }
    }

    public void Morrer(string _causaDaMorte)
    {
        if (_isDead) return;
        _isDead = true;

        if (_causaDaMorte == "Agua")
        {
            CongelarNaAgua();
        }

        OnPlayerDied?.Invoke();

        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.TriggerDeath();
        }
        else if (_playerAnimator != null)
        {
            _playerAnimator.SetTrigger("Death");
        }

        StartCoroutine(RotinaDeMorte());
    }

    private IEnumerator RotinaDeMorte()
    {
        yield return new WaitForSeconds(_tempoAteReset);

        OnFadeOutRequested?.Invoke(_fadeDuration);

        yield return new WaitForSecondsRealtime(_fadeDuration + 0.2f);

        Time.timeScale = 1f;

        string _cenaAtual = SceneManager.GetActiveScene().name;

        if (SceneChanger.Instance != null)
        {
            SceneChanger.Instance.ChangeScene(_cenaAtual);
        }
        else
        {
            SceneManager.LoadScene(_cenaAtual);
        }
    }

    public void AtivarCheckpoint(Transform _pontoDeRespawn)
    {
        if (_isDead || _pontoDeRespawn == null) return;

        if (SaveManager.Instance != null)
        {
            Camera _cam = Camera.main != null ? Camera.main : FindFirstObjectByType<Camera>();
            Vector3 _posicaoCamera = _cam != null ? _cam.transform.position : Vector3.zero;

            SaveManager.Instance.SalvarCheckpoint(_pontoDeRespawn.position, _posicaoCamera);
        }
    }
}