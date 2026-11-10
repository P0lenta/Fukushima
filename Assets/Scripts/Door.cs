using UnityEngine;
using System;

[RequireComponent(typeof(DialogueInteractable))]
public class Door : MonoBehaviour
{
    [Header("Configuração de Acesso")]
    [Tooltip("Qual cartão abre esta porta? (1, 2 ou 3). 0 = Sem tranca.")]
    public int _requiredCard = 1;

    [Header("Transição de Fase")]
    public string _cenaDestino;

    [Header("Diálogos da Porta")]
    [Tooltip("Fala quando não tem o cartão e tenta abrir")]
    public DialogueData _dialogoTrancada;
    [Tooltip("Fala antes de entrar na porta (Sucesso)")]
    public DialogueData _dialogoAberta;

    [Header("Animação & Física")]
    public Animator _animator;
    public Collider _meshCollider;

    private bool _isOpen = false;
    private bool _esperandoDialogoDeSucesso = false;

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
        DialogueUIManager._onDialogueEnded -= AoTerminarDialogoAberta;
    }

    public void TentarAbrir()
    {
        if (_esperandoDialogoDeSucesso) return;

        if (_isOpen)
        {
            if (!string.IsNullOrEmpty(_cenaDestino)) EntrarNaPorta();
            return;
        }

        bool _hasAccess = false;
        if (PlayerInventory.Instance != null)
        {
            if (_requiredCard == 1) _hasAccess = PlayerInventory.Instance._hasCard1;
            else if (_requiredCard == 2) _hasAccess = PlayerInventory.Instance._hasCard2;
            else if (_requiredCard == 3) _hasAccess = PlayerInventory.Instance._hasCard3;
            else if (_requiredCard == 0) _hasAccess = true;
        }

        if (_hasAccess)
        {
            AbrirPorta();
        }
        else
        {
            Debug.Log($"[Porta] Trancada! Requer o cartão {_requiredCard}.");

            if (_dialogueInteractable != null && _dialogoTrancada != null)
            {
                _dialogueInteractable._currentDialogue = _dialogoTrancada;
                _dialogueInteractable.TriggerDialogue();
            }
        }
    }

    private void AbrirPorta()
    {
        if (_dialogueInteractable != null && _dialogoAberta != null)
        {
            _dialogueInteractable._currentDialogue = _dialogoAberta;

            _esperandoDialogoDeSucesso = true;
            DialogueUIManager._onDialogueEnded += AoTerminarDialogoAberta;

            _dialogueInteractable.TriggerDialogue();
        }
        else
        {
            ExecutarAberturaDaPorta();

            if (!string.IsNullOrEmpty(_cenaDestino)) EntrarNaPorta();
        }
    }

    private void AoTerminarDialogoAberta()
    {
        if (!_esperandoDialogoDeSucesso) return;

        _esperandoDialogoDeSucesso = false;
        DialogueUIManager._onDialogueEnded -= AoTerminarDialogoAberta;

        ExecutarAberturaDaPorta();

        if (!string.IsNullOrEmpty(_cenaDestino))
        {
            EntrarNaPorta();
        }
    }

    private void ExecutarAberturaDaPorta()
    {
        _isOpen = true;

        if (_animator != null) _animator.SetTrigger("_abrirPorta");
        if (_meshCollider != null) _meshCollider.enabled = false;
    }

    public void FecharPorta()
    {
        _isOpen = false;
        if (_animator != null) _animator.SetTrigger("_fecharPorta");
        if (_meshCollider != null) _meshCollider.enabled = true;
    }

    private void EntrarNaPorta()
    {
        if (SceneChanger.Instance != null) SceneChanger.Instance.ChangeScene(_cenaDestino);
    }
}