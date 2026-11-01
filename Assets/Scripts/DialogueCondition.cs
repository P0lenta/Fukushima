using UnityEngine;

[RequireComponent(typeof(DialogueInteractable))]

public class DialogueCondition : MonoBehaviour
{
    [Header("Nova fala condicional")]
    public DialogueData _conditionalDialogue;
    public string _conditionKey;

    private DialogueInteractable _dialogueComponent;
    private DialogueData _defaultDialogue;

    private void Awake()
    {
        _dialogueComponent = GetComponent<DialogueInteractable>();
        _defaultDialogue = _dialogueComponent._currentDialogue;
    }

    public void CheckCondition()
    {
        if (PlayerPrefs.GetInt(_conditionKey, 0) == 1)
        {
            _dialogueComponent._currentDialogue = _conditionalDialogue;
        }
        else
        {
            _dialogueComponent._currentDialogue = _defaultDialogue;
        }
    }

    public void MarkCondition()
    {
        PlayerPrefs.SetInt(_conditionKey, 1);

        _dialogueComponent._currentDialogue = _conditionalDialogue;
    }
}
