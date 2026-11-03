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
        if (SaveManager.Instance._currentData._unlockedConditions.Contains(_conditionKey))
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
        if (!SaveManager.Instance._currentData._unlockedConditions.Contains(_conditionKey))
        {
            SaveManager.Instance._currentData._unlockedConditions.Add(_conditionKey);
            SaveManager.Instance.SaveGame();
        }

        _dialogueComponent._currentDialogue = _conditionalDialogue;
    }
}