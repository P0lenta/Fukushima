using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
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

    public bool _isDead { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Update()
    {
        if (!_isDead && transform.position.y <= _killYThreshold)
        {
            Morrer("Queda");
        }
    }

    public void Morrer(string _causaDaMorte)
    {
        if (_isDead) return;
        _isDead = true;

        OnPlayerDied?.Invoke();

        /*if (_playerAnimator != null)
        {
            if (_causaDaMorte == "Queda") _playerAnimator.SetTrigger("DieFall");
            else if (_causaDaMorte == "Radiacao") _playerAnimator.SetTrigger("DieRadiation");
        }*/

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
}