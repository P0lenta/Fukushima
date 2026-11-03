using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;

public class DialogueUIManager : MonoBehaviour
{
    public static DialogueUIManager Instance { get; private set; }

    [Header("Componentes de UI")]
    public GameObject _dialoguePanel;
    public TextMeshProUGUI _dialogueText;
    public Image _documentImageDisplay;

    [Header("Velocidade do texto")]
    public float _typingSpeed = 0.02f;

    [Header("Input (Novo Input System)")]
    [Tooltip("Arraste a AÇÃO (ex: Interact/Submit) do seu arquivo .inputactions aqui")]
    public InputActionReference _advanceAction;

    private DialogueData _currentData;
    private int _currentLineIndex = 0;
    private bool _isTyping = false;
    private bool _isDialogueActive = false;
    private int _startFrame;
    private Coroutine _typingCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnEnable()
    {
        DialogueInteractable.OnDialogueTriggered += StartDialogue;
        
        if (_advanceAction != null) _advanceAction.action.Enable(); 
    }

    private void OnDisable()
    {
        DialogueInteractable.OnDialogueTriggered -= StartDialogue;
    }

    private void Start()
    {
        _dialoguePanel.SetActive(false);
        _dialogueText.text = "";
        if (_documentImageDisplay != null) _documentImageDisplay.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_isDialogueActive || Time.frameCount == _startFrame) return;

        if (_advanceAction != null && _advanceAction.action.WasPressedThisFrame())
        {
            AvancarOuCompletarTexto();
        }
    }

    private void AvancarOuCompletarTexto()
    {
        if (_isTyping)
        {
            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
            
            _dialogueText.text = _currentData._lines[_currentLineIndex]._text;
            _isTyping = false;
        }
        else
        {
            NextLine();
        }
    }

    private void StartDialogue(DialogueData _data)
    {
        if (_isDialogueActive) return;

        _currentData = _data;
        _currentLineIndex = 0;
        _isDialogueActive = true;
        _startFrame = Time.frameCount;

        _dialoguePanel.SetActive(true);

        if (PlayerMovement.Instance != null) 
        {
            PlayerMovement.Instance.enabled = false; 
        }

        _typingCoroutine = StartCoroutine(TypeLine());
    }

    private void NextLine()
    {
        if (_currentLineIndex < _currentData._lines.Length - 1)
        {
            _currentLineIndex++;
            _typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        _isDialogueActive = false;
        _dialoguePanel.SetActive(false);
        _dialogueText.text = "";

        if (_documentImageDisplay != null)
            _documentImageDisplay.gameObject.SetActive(false);

        if (PlayerMovement.Instance != null) 
        {
            PlayerMovement.Instance.enabled = true;
        }
    }

    private IEnumerator TypeLine()
    {
        _isTyping = true;
        _dialogueText.text = "";

        Sprite _currentImage = _currentData._lines[_currentLineIndex]._documentImage;
        if (_currentImage != null)
        {
            _documentImageDisplay.sprite = _currentImage;
            _documentImageDisplay.gameObject.SetActive(true);
        }
        else
        {
            if (_documentImageDisplay != null) _documentImageDisplay.gameObject.SetActive(false);
        }

        foreach (char _c in _currentData._lines[_currentLineIndex]._text.ToCharArray())
        {
            _dialogueText.text += _c;
            yield return new WaitForSeconds(_typingSpeed); 
        }

        _isTyping = false;
    }
}