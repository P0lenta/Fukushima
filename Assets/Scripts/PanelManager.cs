using System.Collections.Generic;
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
        string cena = SceneManager.GetActiveScene().name;
        string caminho = ObterCaminhoHierarquia(transform);

        var todosManagers = FindObjectsByType<PanelManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log($"[PM-DEBUG] Awake | Cena: '{cena}' | Objeto: '{caminho}' | _isMainMenu: {_isMainMenu} | Total PanelManagers na cena: {todosManagers.Length}");

        if (todosManagers.Length > 1)
        {
            foreach (var pm in todosManagers)
            {
                Debug.LogWarning($"[PM-DEBUG] AVISO DUPLICADO -> PanelManager encontrado em: '{ObterCaminhoHierarquia(pm.transform)}' (_isMainMenu={pm._isMainMenu})");
            }
        }

        Instance = this;
        GarantirReferenciasEFecharTudo("Awake");
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
        Debug.Log($"[PM-DEBUG] OnDestroy | Objeto: '{ObterCaminhoHierarquia(transform)}'");

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
        string cena = SceneManager.GetActiveScene().name;
        Debug.Log($"[PM-DEBUG] Start | Cena: '{cena}' | _canvasPause: {(_canvasPause != null ? ObterCaminhoHierarquia(_canvasPause.transform) : "NULL")} | _initialPanel: {(_initialPanel != null ? _initialPanel.name : "NULL")} | Qtd _panels: {(_panels != null ? _panels.Length : 0)}");

        _menuOpen = false;
        _isFlipping = false;
        _currentPanelActive = null;

        GarantirReferenciasEFecharTudo("Start");

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

        Debug.Log($"[PM-DEBUG] Fim do Start | Time.timeScale: {Time.timeScale} | Cursor.lockState: {Cursor.lockState} | EventSystem: {(EventSystem.current != null ? EventSystem.current.name : "NULL")}");
    }

    private void GarantirReferenciasEFecharTudo(string origem)
    {
        // Verifica todos os objetos chamados "Canvas Pause" ou "SurePanel" na cena inteira
        Transform[] todosTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int qtdCanvasPause = 0;

        foreach (Transform t in todosTransforms)
        {
            if (t.name == "Canvas Pause")
            {
                qtdCanvasPause++;
                Debug.Log($"[PM-DEBUG:{origem}] Encontrado 'Canvas Pause' #{qtdCanvasPause} em: '{ObterCaminhoHierarquia(t)}' (ativo={t.gameObject.activeSelf})");

                if (_canvasPause == null)
                {
                    _canvasPause = t.gameObject;
                }
            }
            else if (t.name == "SurePanel" && t.gameObject.activeSelf)
            {
                Debug.LogWarning($"[PM-DEBUG:{origem}] 'SurePanel' estava ATIVO em '{ObterCaminhoHierarquia(t)}'. Desativando agora.");
                t.gameObject.SetActive(false);
            }
        }

        if (qtdCanvasPause > 1)
        {
            Debug.LogError($"[PM-DEBUG:{origem}] ERRO CRÍTICO: Existem {qtdCanvasPause} objetos 'Canvas Pause' na mesma cena! Isso causa UI sobreposta.");
        }

        if (_canvasPause != null)
        {
            _canvasRectTransform = _canvasPause.GetComponent<RectTransform>();
            if (_canvasRectTransform != null)
            {
                _canvasRectTransform.DOKill();
                _canvasRectTransform.anchoredPosition = Vector2.zero;
                _canvasRectTransform.localRotation = Quaternion.identity;
            }

            if (_panels == null || _panels.Length == 0)
            {
                List<GameObject> listaPaineis = new List<GameObject>();
                foreach (Transform child in _canvasPause.transform)
                {
                    listaPaineis.Add(child.gameObject);
                }
                _panels = listaPaineis.ToArray();
                Debug.LogWarning($"[PM-DEBUG:{origem}] Array _panels estava vazio! Preenchido automaticamente com {_panels.Length} filhos.");
            }

            for (int i = 0; i < _panels.Length; i++)
            {
                if (_panels[i] != null)
                {
                    _panels[i].SetActive(false);
                }
                else
                {
                    Debug.LogWarning($"[PM-DEBUG:{origem}] _panels[{i}] está NULL no Inspector!");
                }
            }

            foreach (Transform t in _canvasPause.GetComponentsInChildren<Transform>(true))
            {
                if (t.name == "SurePanel")
                {
                    t.gameObject.SetActive(false);
                }
            }

            _canvasPause.SetActive(false);
            Debug.Log($"[PM-DEBUG:{origem}] _canvasPause ('{ObterCaminhoHierarquia(_canvasPause.transform)}') fechado. activeInHierarchy={_canvasPause.activeInHierarchy}");
        }
        else
        {
            Debug.LogError($"[PM-DEBUG:{origem}] _canvasPause é NULL e nenhum 'Canvas Pause' foi encontrado na cena!");
        }
    }

    private void Update()
    {
        if (!_isMainMenu && _pauseAction != null && _pauseAction.action.WasPressedThisFrame())
        {
            Debug.Log("[PM-DEBUG] Tecla de Pause pressionada -> chamando ToggleMenu()");
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        Debug.Log($"[PM-DEBUG] ToggleMenu() chamado | _menuOpen atual: {_menuOpen} -> novo: {!_menuOpen}");

        if (_canvasPause == null)
        {
            Debug.LogError("[PM-DEBUG] ToggleMenu abortado: _canvasPause é NULL.");
            return;
        }
        if (DialogueUIManager.Instance != null && DialogueUIManager.Instance._isDialogueActive)
        {
            Debug.Log("[PM-DEBUG] ToggleMenu bloqueado: diálogo ativo.");
            return;
        }
        if (PlayerDeath.Instance != null && PlayerDeath.Instance._isDead)
        {
            Debug.Log("[PM-DEBUG] ToggleMenu bloqueado: jogador morto.");
            return;
        }
        if (_isFlipping)
        {
            Debug.Log("[PM-DEBUG] ToggleMenu bloqueado: animação de flip em andamento.");
            return;
        }

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
        Debug.Log($"[PM-DEBUG] AbrirPainel('{(_desiredPanel != null ? _desiredPanel.name : "NULL")}')");

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
        Debug.Log($"[PM-DEBUG] IrParaCena('{_cena}') chamado.");
        if (_canvasRectTransform != null) _canvasRectTransform.DOKill();
        DOTween.KillAll();
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

    private string ObterCaminhoHierarquia(Transform alvo)
    {
        if (alvo == null) return "NULL";
        string path = alvo.name;
        while (alvo.parent != null)
        {
            alvo = alvo.parent;
            path = alvo.name + "/" + path;
        }
        return path;
    }
}