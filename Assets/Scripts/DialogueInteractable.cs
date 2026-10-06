using UnityEngine;
using System;

public class DialogueInteractable : MonoBehaviour
{
    public static event Action<DialogueData> OnDialogueTriggered;

    [Header("Arquivo do diálogo")]
    public DialogueData _currentDialogue;

    public void TriggerDialogue()
    {
        if (_currentDialogue != null && _currentDialogue._lines.Length > 0)
        {
            OnDialogueTriggered?.Invoke(_currentDialogue);
        }
    }
}