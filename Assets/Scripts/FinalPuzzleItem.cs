using UnityEngine;
using UnityEngine.Events;

public class FinalPuzzleItem : MonoBehaviour
{
    [Header("Feedback Visual / Animação (Opcional)")]
    public Animator _animator;
    public string _triggerAnimacao = "Ativar";

    [Header("Diálogo ao Interagir (Opcional)")]
    public DialogueData _dialogoPrimeiraVez;
    public DialogueData _dialogoJaInteragido;

    [Header("Eventos (Opcional)")]
    public UnityEvent _onPrimeiraInteracao;
    public UnityEvent _onQualquerInteracao;

    private bool _foiInteragido = false;
    private DialogueInteractable _dialogueInteractable;

    public bool FoiInteragido => _foiInteragido;

    private void Awake()
    {
        _dialogueInteractable = GetComponent<DialogueInteractable>();
    }

    public void Interagir()
    {
        _onQualquerInteracao?.Invoke();

        if (!_foiInteragido)
        {
            _foiInteragido = true;

            if (_animator != null && !string.IsNullOrEmpty(_triggerAnimacao))
            {
                _animator.SetTrigger(_triggerAnimacao);
            }

            _onPrimeiraInteracao?.Invoke();

            if (_dialogueInteractable != null && _dialogoPrimeiraVez != null)
            {
                _dialogueInteractable._currentDialogue = _dialogoPrimeiraVez;
                _dialogueInteractable.TriggerDialogue();
            }
        }
        else
        {
            DialogueData dialogo = _dialogoJaInteragido != null ? _dialogoJaInteragido : _dialogoPrimeiraVez;
            if (_dialogueInteractable != null && dialogo != null)
            {
                _dialogueInteractable._currentDialogue = dialogo;
                _dialogueInteractable.TriggerDialogue();
            }
        }
    }
}