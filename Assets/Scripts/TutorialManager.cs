using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Painéis de Tutorial")]
    public CanvasGroup _moveTutorialPanel;
    public CanvasGroup _jumpTutorialPanel;

    [Header("Feedbacks Visuais")]
    [Tooltip("Imagem com Image Type em 'Filled'")]
    public Image _moveProgressFill; 
    
    [Tooltip("Texto para mostrar a contagem, ex: 0/3")]
    public TextMeshProUGUI _jumpCounterText;

    private bool _isMoveTutorialActive = false;
    private bool _isJumpTutorialActive = false;

    private float _moveTimer = 0f;
    private int _jumpCount = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (_moveTutorialPanel != null) { _moveTutorialPanel.alpha = 0; _moveTutorialPanel.gameObject.SetActive(false); }
        if (_jumpTutorialPanel != null) { _jumpTutorialPanel.alpha = 0; _jumpTutorialPanel.gameObject.SetActive(false); }
    }

    private void OnEnable() => PlayerMovement.OnPlayerJumped += RegistrarPulo;
    private void OnDisable() => PlayerMovement.OnPlayerJumped -= RegistrarPulo;

    private void Update()
    {
        if (_isMoveTutorialActive && PlayerMovement.Instance != null)
        {
            if (PlayerMovement.Instance._isMoving)
            {
                _moveTimer += Time.deltaTime;

                if (_moveProgressFill != null)
                {
                    _moveProgressFill.fillAmount = _moveTimer / 2f;
                }

                if (_moveTimer >= 2f)
                {
                    ConcluirTutorial("Tut_Move", _moveTutorialPanel);
                    _isMoveTutorialActive = false;
                }
            }
            /*else
            {
                if (_moveTimer > 0f)
                {
                    _moveTimer = 0f;
                    if (_moveProgressFill != null) _moveProgressFill.DOFillAmount(0f, 0.3f);
                }
            }*/
        }
    }

    public void IniciarTutorialMovimento()
    {
        if (SaveManager.Instance._currentData._unlockedConditions.Contains("Tut_Move")) return;
        
        _isMoveTutorialActive = true;
        _moveTimer = 0f;

        if (_moveProgressFill != null) _moveProgressFill.fillAmount = 0f;

        MostrarPainel(_moveTutorialPanel);
    }

    public void IniciarTutorialPulo()
    {
        if (SaveManager.Instance._currentData._unlockedConditions.Contains("Tut_Jump")) return;

        _isJumpTutorialActive = true;
        _jumpCount = 0;

        if (_jumpCounterText != null) _jumpCounterText.text = "0";

        MostrarPainel(_jumpTutorialPanel);
    }

    private void RegistrarPulo()
    {
        if (_isJumpTutorialActive)
        {
            _jumpCount++;

            if (_jumpCounterText != null)
            {
                _jumpCounterText.text = $"{_jumpCount}";

                _jumpCounterText.transform.DOKill();
                _jumpCounterText.transform.localScale = Vector3.one;
                _jumpCounterText.transform.DOPunchScale(new Vector3(0.4f, 0.4f, 0f), 0.25f, 5, 1f).SetUpdate(true);
            }

            if (_jumpCount >= 3)
            {
                ConcluirTutorial("Tut_Jump", _jumpTutorialPanel);
                _isJumpTutorialActive = false;
            }
        }
    }

    private void MostrarPainel(CanvasGroup _panel)
    {
        if (_panel == null) return;
        _panel.gameObject.SetActive(true);
        _panel.DOFade(1f, 0.5f).SetUpdate(true);
    }

    private void ConcluirTutorial(string _saveKey, CanvasGroup _panel)
    {
        if (!SaveManager.Instance._currentData._unlockedConditions.Contains(_saveKey))
        {
            SaveManager.Instance._currentData._unlockedConditions.Add(_saveKey);
            SaveManager.Instance.SaveGame();
        }

        if (_panel != null)
        {
            _panel.DOFade(0f, 0.5f).SetUpdate(true).OnComplete(() => _panel.gameObject.SetActive(false));
        }
    }
}