using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class HoldToSkipCutscene : MonoBehaviour
{
    [Header("Input (Novo Input System)")]
    [Tooltip("Arraste aqui a Action de pular (ex: Skip ou Interact) do seu arquivo .inputactions.")]
    public InputActionReference _skipAction;

    [Header("Configurações de Tempo")]
    [Tooltip("Tempo (em segundos) que o jogador precisa segurar o botão para pular.")]
    public float _holdTime = 1f;

    [Header("Feedback Visual (UI)")]
    [Tooltip("Imagem configurada como Image Type: Filled para mostrar o carregamento.")]
    public Image _progressImage;

    [Header("Evento ao Completar")]
    public UnityEvent _onSkipCompleted;

    private float _currentTimer = 0f;
    private bool _isHoldingViaEvent = false;
    private bool _hasSkipped = false;

    private void Awake()
    {
        if (_progressImage != null)
        {
            _progressImage.fillAmount = 0f;
        }
    }

    private void OnEnable()
    {
        if (_skipAction != null)
        {
            _skipAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (_skipAction != null)
        {
            _skipAction.action.Disable();
        }
    }

    public void OnSkip(InputAction.CallbackContext context)
    {
        if (_hasSkipped) return;

        if (context.started || context.performed)
        {
            _isHoldingViaEvent = true;
        }
        else if (context.canceled)
        {
            _isHoldingViaEvent = false;
        }
    }

    private void Update()
    {
        if (_hasSkipped) return;

        bool isHolding = (_skipAction != null && _skipAction.action.IsPressed()) || _isHoldingViaEvent;

        if (isHolding)
        {
            _currentTimer += Time.unscaledDeltaTime;
        }
        else
        {
            _currentTimer = Mathf.MoveTowards(_currentTimer, 0f, Time.unscaledDeltaTime * 2.5f);
        }

        if (_progressImage != null)
        {
            _progressImage.fillAmount = Mathf.Clamp01(_currentTimer / _holdTime);
        }

        if (_currentTimer >= _holdTime)
        {
            _hasSkipped = true;
            _onSkipCompleted?.Invoke();
        }
    }
}