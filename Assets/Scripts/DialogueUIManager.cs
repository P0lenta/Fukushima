using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;

public class DialogueUIManager : MonoBehaviour
{
    [Header("Componentes de UI")]
    public GameObject _dialoguePanel;
    public TextMeshProUGUI _dialogueText;
    public Image _documentImageDisplay;

    [Header("Velocidade do texto")]
    public float _typingSpeed = 0.02f;

    [Header("Referências")]
    public PlayerMovement _playerMovement;

    private DialogueData _currentData;
    private int _currentLineIndex = 0;
    private bool _isTyping = false;
    private bool _isDialogueActive = false;
    private int _startFrame;
    private Coroutine _typingCoroutine;

    private void OnEnable()
    {
        DialogueInteractable.OnDialogueTriggered += StartDialogue;
    }

    private void OnDisable()
    {
        DialogueInteractable.OnDialogueTriggered -= StartDialogue;
    }

    private void Start()
    {
        _dialoguePanel.SetActive(false);
        _dialogueText.text = "";
        _documentImageDisplay.gameObject.SetActive(false);
    }

    private void StartDialogue(DialogueData _data)
    {
        if (_isDialogueActive) return;

        _currentData = _data;
        _currentLineIndex = 0;
        _isDialogueActive = true;

        _startFrame = Time.frameCount;

        _dialoguePanel.SetActive(true);

        if (_playerMovement != null) _playerMovement.StopMovement();

        _typingCoroutine = StartCoroutine(TypeLine());
    }

    public void OnAdvanceDialogue(InputAction.CallbackContext context)
    {
        if (!_isDialogueActive || Time.frameCount == _startFrame   ) return;

        if (context.started)
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
            _documentImageDisplay.gameObject.SetActive(false);
        }

        foreach (char _c in _currentData._lines[_currentLineIndex]._text.ToCharArray())
        {
            _dialogueText.text += _c;
            yield return new WaitForSeconds(_typingSpeed); 
        }

        _isTyping = false;
    }
}
