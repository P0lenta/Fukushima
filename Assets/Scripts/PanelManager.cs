using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PanelManager : MonoBehaviour
{
    [Header("Canvas de pause")]
    public GameObject _canvasPause;
    private RectTransform _canvasRectTransform;

    [Header("Painéis")]
    public GameObject[] _panels;
    public GameObject _initialPanel;

    [Header("Menu Principal ou In-game")]
    public bool _isMainMenu = false;

    private bool _menuOpen = false;

    private const float _duracao_animacao = 0.35f;
    private const float _angulo_inclinacao = -8f;

    private void Awake()
    {
        if (_canvasPause != null)
        {
            _canvasRectTransform = _canvasPause.GetComponent<RectTransform>();
        }
    }

    private void Start() 
    {
        if (_canvasPause != null) _canvasPause.SetActive(false);

        foreach (GameObject _panel in _panels)
        {
            if (_panel != null) _panel.SetActive(false);
        }

        _menuOpen = false;  

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

    public void ToggleMenu()
    {
        _menuOpen = !_menuOpen;

        if (_canvasRectTransform == null && _canvasPause != null)
            _canvasRectTransform = _canvasPause.GetComponent<RectTransform>();

        // Mata qualquer animação anterior para impedir o glitch de posição
        _canvasRectTransform.DOKill();

        // Calcula a posição fora da tela dinamicamente com base na resolução atual
        float posicaoYEscondido = -Screen.height;

        if (_menuOpen)
        {
            if (!_isMainMenu) TravarJogo(true);
            AbrirPainel(_initialPanel);

            _canvasPause.SetActive(true);

            // 1. Início: Fora da tela (embaixo) + inclinação de crachá
            _canvasRectTransform.anchoredPosition = new Vector2(0f, posicaoYEscondido);
            _canvasRectTransform.localRotation = Quaternion.Euler(0f, 0f, _angulo_inclinacao);

            // 2. Animação de subir (Puxando do bolso com amortecimento)
            _canvasRectTransform.DOAnchorPos(Vector2.zero, _duracao_animacao)
                .SetUpdate(true)
                .SetEase(Ease.OutBack);

            _canvasRectTransform.DOLocalRotate(Vector3.zero, _duracao_animacao)
                .SetUpdate(true)
                .SetEase(Ease.OutCubic);
        }
        else
        {
            if (!_isMainMenu) TravarJogo(false);

            // 3. Animação de guardar de volta no bolso
            _canvasRectTransform.DOAnchorPos(new Vector2(0f, posicaoYEscondido), _duracao_animacao * 0.8f)
                .SetUpdate(true)
                .SetEase(Ease.InCubic);

            _canvasRectTransform.DOLocalRotate(new Vector3(0f, 0f, _angulo_inclinacao * 1.5f), _duracao_animacao * 0.8f)
                .SetUpdate(true)
                .SetEase(Ease.InCubic)
                .OnComplete(() =>
                {
                    _canvasPause.SetActive(false);
                });
        }
    }

    public void AbrirPainel(GameObject _desiredPanel)
    {
        foreach (GameObject _panel in _panels)
        {
            _panel.SetActive(_panel == _desiredPanel);
        }

        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }

    public void VoltarProInicio()
    {
        AbrirPainel(_initialPanel);
    }

    public void TravarJogo(bool _travar)
    {
        Cursor.lockState = _travar ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _travar;

        Time.timeScale = _travar ? 0f : 1f;
    }

    public void IrParaCena(string _cena)
    {
        Time.timeScale = 1f;
        SceneChanger.Instance.ChangeScene(_cena);
    }
}
