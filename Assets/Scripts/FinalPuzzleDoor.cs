using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;

[RequireComponent(typeof(DialogueInteractable))]
public class FinalPuzzleDoor : MonoBehaviour
{
    [Header("Objetos Necessários para Liberar")]
    [Tooltip("Arraste aqui todos os objetos com o script FinalPuzzleItem que o jogador precisa interagir.")]
    public List<FinalPuzzleItem> _objetosNecessarios = new List<FinalPuzzleItem>();

    [Header("Diálogos")]
    [Tooltip("Fala quando ainda falta interagir com algum objeto.")]
    public DialogueData _dialogoIncompleto;
    [Tooltip("Fala quando TODOS os objetos já foram interagidos.")]
    public DialogueData _dialogoCompleto;

    [Header("Evento e Cena Final")]
    [Tooltip("Evento disparado após o diálogo final terminar (ex: tocar Timeline final, abrir porta, etc.).")]
    public UnityEvent _onPuzzleCompleto;
    [Tooltip("Nome da cena final ou créditos para carregar após o diálogo (deixe vazio se for acionar pelo evento acima).")]
    public string _cenaFinal;

    [Header("Animação & Física (Opcional)")]
    public Animator _animator;
    public Collider _meshCollider;

    private bool _jaCompletou = false;
    private bool _esperandoDialogoFinal = false;
    private DialogueInteractable _dialogueInteractable;

    private void Awake()
    {
        _dialogueInteractable = GetComponent<DialogueInteractable>();
    }

    private void Start()
    {
        if (_meshCollider == null) _meshCollider = GetComponent<Collider>();
    }

    private void OnDisable()
    {
        DialogueUIManager._onDialogueEnded -= AoTerminarDialogoFinal;
    }

    public void TentarFinalizar()
    {
        if (_esperandoDialogoFinal || _jaCompletou) return;

        if (TodosObjetosInteragidos())
        {
            IniciarFinalizacao();
        }
        else
        {
            if (_dialogueInteractable != null && _dialogoIncompleto != null)
            {
                _dialogueInteractable._currentDialogue = _dialogoIncompleto;
                _dialogueInteractable.TriggerDialogue();
            }
        }
    }

    public bool TodosObjetosInteragidos()
    {
        if (_objetosNecessarios == null || _objetosNecessarios.Count == 0) return true;

        foreach (FinalPuzzleItem item in _objetosNecessarios)
        {
            if (item != null && !item.FoiInteragido)
            {
                return false;
            }
        }

        return true;
    }

    private void IniciarFinalizacao()
    {
        _jaCompletou = true;

        if (_dialogueInteractable != null && _dialogoCompleto != null)
        {
            _dialogueInteractable._currentDialogue = _dialogoCompleto;

            _esperandoDialogoFinal = true;
            DialogueUIManager._onDialogueEnded += AoTerminarDialogoFinal;

            _dialogueInteractable.TriggerDialogue();
        }
        else
        {
            ExecutarEventoFinal();
        }
    }

    private void AoTerminarDialogoFinal()
    {
        if (!_esperandoDialogoFinal) return;

        _esperandoDialogoFinal = false;
        DialogueUIManager._onDialogueEnded -= AoTerminarDialogoFinal;

        ExecutarEventoFinal();
    }

    private void ExecutarEventoFinal()
    {
        if (_animator != null) _animator.SetTrigger("_abrirPorta");
        if (_meshCollider != null) _meshCollider.enabled = false;

        _onPuzzleCompleto?.Invoke();

        if (!string.IsNullOrEmpty(_cenaFinal))
        {
            DOTween.KillAll();
            Time.timeScale = 1f;

            if (SceneChanger.Instance != null)
            {
                SceneChanger.Instance.ChangeScene(_cenaFinal);
            }
            else
            {
                SceneManager.LoadScene(_cenaFinal);
            }
        }
    }
}