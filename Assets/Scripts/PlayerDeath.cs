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

    private int _waterLayer;
    private float _waterContactTimer = 0f;
    private readonly HashSet<Collider> _activeWaterColliders = new HashSet<Collider>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        _waterLayer = LayerMask.NameToLayer("Water");
    }

    private void Start()
    {
        if (_waterLayer == -1)
        {
            Debug.LogWarning("[PlayerDeath] A layer 'Water' não foi encontrada. Crie-a em Edit > Project Settings > Tags and Layers.");
        }
    }

    private void Update()
    {
        if (_isDead) return;

        if (transform.position.y <= _killYThreshold)
        {
            Morrer("Queda");
            return;
        }

        VerificarContatoComAgua();
    }

    private void VerificarContatoComAgua()
    {
        _activeWaterColliders.RemoveWhere(c => c == null || !c.enabled || !c.gameObject.activeInHierarchy);

        if (_activeWaterColliders.Count > 0)
        {
            _waterContactTimer += Time.deltaTime;

            if (_waterContactTimer >= _tempoParaMorrerNaAgua)
            {
                Morrer("Agua");
            }
        }
        else
        {
            _waterContactTimer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_waterLayer != -1 && other.gameObject.layer == _waterLayer)
        {
            _activeWaterColliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_waterLayer != -1 && other.gameObject.layer == _waterLayer)
        {
            _activeWaterColliders.Remove(other);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_waterLayer != -1 && collision.gameObject.layer == _waterLayer)
        {
            _activeWaterColliders.Add(collision.collider);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_waterLayer != -1 && collision.gameObject.layer == _waterLayer)
        {
            _activeWaterColliders.Remove(collision.collider);
        }
    }

    public void Morrer(string _causaDaMorte)
    {
        if (_isDead) return;
        _isDead = true;

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