using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance { get; private set; }

    [Header("Input (Novo Input System)")]
    public InputActionReference _pauseAction;

    [Header("Canvas de pause")]
    public GameObject _canvasPause;
    private RectTransform _canvasRectTransform;

    [Header("Painéis")]
    public GameObject[] _panels;
    public GameObject _initialPanel;

    [Header("Menu Principal ou In-game")]
    public bool _isMainMenu = false;

    private bool _menuOpen = false;
    private bool _isFlipping = false;
    private GameObject _currentPanelActive;

    private const float _duracao_animacao = 0.35f;
    private const float _duracao_flip = 0.25f;
    private const float _angulo_inclinacao = -8f;

    public bool IsMenuOpen => _menuOpen || _isFlipping;

    private void Awake()
    {
        // Sempre assume o controle dos painéis da cena atual
        Instance = this;

        if (_canvasPause != null)
        {
            _canvasRectTransform = _canvasPause.GetComponent<RectTransform>();
        }
    }

    private void OnEnable()
    {
        if (_pauseAction != null)
        {
            _pauseAction.action.Enable();
        }
    }

    private void OnDestroy()
    {
        if (_canvasRectTransform != null)
        {
            _canvasRectTransform.DOKill();
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        _menuOpen = false;
        _isFlipping = false;
        _currentPanelActive = null;

        if (_canvasRectTransform != null)
        {
            _canvasRectTransform.DOKill();
            _canvasRectTransform.anchoredPosition = Vector2.zero;
            _canvasRectTransform.localRotation = Quaternion.identity;
        }

        foreach (GameObject _panel in _panels)
        {
            if (_panel != null) _panel.SetActive(false);
        }

        if (_canvasPause != null)
        {
            _canvasPause.SetActive(false);
        }

        if (!_isMainMenu)
        {
            TravarJogo(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 1f;
        }
    }

    private void Update()
    {
        if (!_isMainMenu && _pauseAction != null && _pauseAction.action.WasPressedThisFrame())
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        if (_canvasPause == null) return;
        if (DialogueUIManager.Instance != null && DialogueUIManager.Instance._isDialogueActive) return;
        if (PlayerDeath.Instance != null && PlayerDeath.Instance._isDead) return;
        if (_isFlipping) return;

        _menuOpen = !_menuOpen;

        if (_canvasRectTransform == null)
        {
            _canvasRectTransform = _canvasPause.GetComponent<RectTransform>();
        }

        _canvasRectTransform.DOKill();
        float posicaoYEscondido = -Screen.height;

        if (_menuOpen)
        {
            if (!_isMainMenu) TravarJogo(true);
            TrocarVisibilidadeInstantanio(_initialPanel);
            _canvasPause.SetActive(true);

            _canvasRectTransform.anchoredPosition = new Vector2(0f, posicaoYEscondido);
            _canvasRectTransform.localRotation = Quaternion.Euler(0f, 0f, _angulo_inclinacao);

            _canvasRectTransform.DOAnchorPos(Vector2.zero, _duracao_animacao)
                .SetUpdate(true)
                .SetEase(Ease.OutBack)
                .SetLink(_canvasPause);

            _canvasRectTransform.DOLocalRotate(Vector3.zero, _duracao_animacao)
                .SetUpdate(true)
                .SetEase(Ease.OutCubic)
                .SetLink(_canvasPause);
        }
        else
        {
            if (!_isMainMenu) TravarJogo(false);

            _canvasRectTransform.DOAnchorPos(new Vector2(0f, posicaoYEscondido), _duracao_animacao * 0.8f)
                .SetUpdate(true)
                .SetEase(Ease.InCubic)
                .SetLink(_canvasPause);

            _canvasRectTransform.DOLocalRotate(new Vector3(0f, 0f, _angulo_inclinacao * 1.5f), _duracao_animacao * 0.8f)
                .SetUpdate(true)
                .SetEase(Ease.InCubic)
                .SetLink(_canvasPause)
                .OnComplete(() =>
                {
                    if (_canvasPause != null) _canvasPause.SetActive(false);
                });
        }
    }

    public void AbrirPainel(GameObject _desiredPanel)
    {
        if (_desiredPanel == null || _canvasPause == null || _isFlipping || _currentPanelActive == _desiredPanel) return;

        if (_canvasRectTransform == null)
        {
            _canvasRectTransform = _canvasPause.GetComponent<RectTransform>();
        }

        if (_currentPanelActive == null)
        {
            TrocarVisibilidadeInstantanio(_desiredPanel);
            return;
        }

        _isFlipping = true;
        _canvasRectTransform.DOKill();
        _canvasRectTransform.DOLocalRotate(new Vector3(0f, 90f, 0f), _duracao_flip)
            .SetUpdate(true)
            .SetEase(Ease.InBack)
            .SetLink(_canvasPause)
            .OnComplete(() =>
            {
                if (_canvasRectTransform == null) return;

                TrocarVisibilidadeInstantanio(_desiredPanel);
                _canvasRectTransform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                _canvasRectTransform.DOLocalRotate(Vector3.zero, _duracao_flip)
                    .SetUpdate(true)
                    .SetEase(Ease.OutBack)
                    .SetLink(_canvasPause)
                    .OnComplete(() => _isFlipping = false);
            });
    }

    private void TrocarVisibilidadeInstantanio(GameObject _desiredPanel)
    {
        foreach (GameObject _panel in _panels)
        {
            if (_panel != null) _panel.SetActive(_panel == _desiredPanel);
        }
        _currentPanelActive = _desiredPanel;
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }

    public void VoltarProInicio() => AbrirPainel(_initialPanel);

    public void TravarJogo(bool _travar)
    {
        Cursor.lockState = _travar ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _travar;
        Time.timeScale = _travar ? 0f : 1f;
    }

    public void IrParaCena(string _cena)
    {
        if (_canvasRectTransform != null) _canvasRectTransform.DOKill();
        Time.timeScale = 1f;

        if (SceneChanger.Instance != null)
        {
            SceneChanger.Instance.ChangeScene(_cena);
        }
        else
        {
            SceneManager.LoadScene(_cena);
        }
    }

    public void Sair()
    {
        Debug.Log("Jogo fechado");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void DeletarSave()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ResetData();
            Debug.Log("Save resetado com sucesso!");
        }
    }

    public void ContinuarJogo(string _nomePrimeiroLevel)
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ContinuarJogoSalvo(_nomePrimeiroLevel);
        }
    }
}