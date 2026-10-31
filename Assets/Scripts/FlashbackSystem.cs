using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class FlashbackSystem : MonoBehaviour
{
    public static FlashbackSystem Instance { get; private set; }

    [Header("UI do Flashback (Fade Branco)")]
    [Tooltip("CanvasGroup do painel branco no Canvas")]
    public CanvasGroup _whiteFadeGroup;
    public float _duracaoFadeBranco = 1.2f;

    private static string _cenaOrigem;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        if (_whiteFadeGroup != null)
        {
            _whiteFadeGroup.alpha = 0f;
            _whiteFadeGroup.blocksRaycasts = false;
            _whiteFadeGroup.interactable = false;
        }
    }

    public void IrParaPassado(string _nomeCenaPassado)
    {
        _cenaOrigem = SceneManager.GetActiveScene().name;
        ExecutarSequenciaFlashback(_nomeCenaPassado);
    }

    public void VoltarParaPresente()
    {
        if (!string.IsNullOrEmpty(_cenaOrigem))
        {
            ExecutarSequenciaFlashback(_cenaOrigem);
        }
        else
        {
            Debug.LogWarning("[Flashback] Nenhuma cena de origem foi registrada!");
        }
    }

    private void ExecutarSequenciaFlashback(string _cenaDestino)
    {
        if (_whiteFadeGroup == null)
        {
            ChamarSceneChanger(_cenaDestino);
            return;
        }

        _whiteFadeGroup.blocksRaycasts = true;
        _whiteFadeGroup.DOKill();

        _whiteFadeGroup.DOFade(1f, _duracaoFadeBranco)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                ChamarSceneChanger(_cenaDestino);
            });
    }

    private void ChamarSceneChanger(string _cenaDestino)
    {
        if (SceneChanger.Instance != null)
        {
            SceneChanger.Instance.ChangeScene(_cenaDestino);
        }
        else
        {
            SceneManager.LoadScene(_cenaDestino);
        }
    }
}