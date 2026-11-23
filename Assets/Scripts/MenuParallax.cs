using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(RectTransform))]
public class MenuParallax : MonoBehaviour
{
    [Header("Movimento do Parallax")]
    [Tooltip("Distância máxima (em pixels) que o fundo se desloca. Para 1920x1080, use entre 40 e 60.")]
    public float _maxOffset = 45f;
    [Tooltip("Suavidade com que o fundo acompanha o mouse.")]
    public float _suavizacao = 5f;
    [Tooltip("Se marcado, o fundo move na direção oposta ao mouse (sensação de profundidade).")]
    public bool _inverterDirecao = true;

    [Header("Travas de Abertura")]
    [Tooltip("Tempo de espera ao carregar/abrir a tela antes de liberar o movimento.")]
    public float _delayAoAbrir = 0.6f;
    [Tooltip("Opcional: arraste APENAS o painel que abre por cima (ex: CanvasPause). Não arraste o Canvas pai.")]
    public GameObject _painelBloqueador;

    private RectTransform _rectTransform;
    private Vector2 _posicaoInicial;
    private float _timerAbertura;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _posicaoInicial = _rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        _timerAbertura = 0f;
        if (_rectTransform != null)
        {
            _rectTransform.anchoredPosition = _posicaoInicial;
        }
    }

    private void LateUpdate()
    {
        _timerAbertura += Time.unscaledDeltaTime;

        if (DevePausarMovimento())
        {
            _rectTransform.anchoredPosition = Vector2.Lerp(
                _rectTransform.anchoredPosition,
                _posicaoInicial,
                _suavizacao * Time.unscaledDeltaTime
            );
            return;
        }

        if (!TentarObterPosicaoMouse(out Vector2 mousePos)) return;

        float normX = Mathf.Clamp((mousePos.x / Screen.width) * 2f - 1f, -1f, 1f);
        float normY = Mathf.Clamp((mousePos.y / Screen.height) * 2f - 1f, -1f, 1f);

        float direcao = _inverterDirecao ? -1f : 1f;
        Vector2 posicaoAlvo = _posicaoInicial + new Vector2(normX, normY) * (_maxOffset * direcao);

        _rectTransform.anchoredPosition = Vector2.Lerp(
            _rectTransform.anchoredPosition,
            posicaoAlvo,
            _suavizacao * Time.unscaledDeltaTime
        );
    }

    private bool TentarObterPosicaoMouse(out Vector2 mousePos)
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
        {
            mousePos = Mouse.current.position.ReadValue();
            return true;
        }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
        mousePos = Input.mousePosition;
        return true;
#else
        mousePos = Vector2.zero;
        return false;
#endif
    }

    private bool DevePausarMovimento()
    {
        if (_timerAbertura < _delayAoAbrir) return true;

        if (PanelManager.Instance != null && PanelManager.Instance.IsMenuOpen) return true;

        if (_painelBloqueador != null &&
            _painelBloqueador != gameObject &&
            !transform.IsChildOf(_painelBloqueador.transform) &&
            _painelBloqueador.activeInHierarchy)
        {
            return true;
        }

        return false;
    }
}