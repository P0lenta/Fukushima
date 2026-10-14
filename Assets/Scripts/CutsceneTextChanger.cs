using System.Collections;
using UnityEngine;
using TMPro;

public class CutsceneTypewriter : MonoBehaviour
{
    [Header("Referências de UI")]
    public TextMeshProUGUI _textComponent;

    [Tooltip("Velocidade")]
    public float _typingSpeed = 0.04f;

    private Coroutine _typewriterCoroutine;

    public void MudarTextoComEfeito(string _novaFrase)
    {
        if (_textComponent == null) return;

        if (_typewriterCoroutine != null)
        {
            StopCoroutine(_typewriterCoroutine);
        }

        _typewriterCoroutine = StartCoroutine(TypeTextRoutine(_novaFrase));
    }

    private IEnumerator TypeTextRoutine(string _targetText)
    {
        _textComponent.text = ""; 

        foreach (char _letter in _targetText.ToCharArray())
        {
            _textComponent.text += _letter;
            yield return new WaitForSecondsRealtime(_typingSpeed);
        }
    }
}