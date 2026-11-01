using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class DeathFadeUI : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    private void OnEnable()
    {
        PlayerDeath.OnFadeOutRequested += FazerFadeOut;
    }

    private void OnDisable()
    {
        PlayerDeath.OnFadeOutRequested -= FazerFadeOut;
    }

    private void FazerFadeOut(float _duracaoFade)
    {
        _canvasGroup.blocksRaycasts = true;
        
        _canvasGroup.DOKill();
        _canvasGroup.DOFade(1f, _duracaoFade).SetUpdate(true);
    }
}