using UnityEngine;
using System;

public class DialogueInteractable : MonoBehaviour
{
    public static event Action<DialogueData> OnDialogueTriggered;

    [Header("Arquivo do diálogo")]
    public DialogueData _currentDialogue;

    public void TriggerDialogue()
    {
        if (Time.timeScale == 0f) return;
        if (PanelManager.Instance != null && PanelManager.Instance.IsMenuOpen) return;
        if (DialogueUIManager.Instance != null && DialogueUIManager.Instance._isDialogueActive) return;
        if (_currentDialogue != null && _currentDialogue._lines.Length > 0)
        {
            OnDialogueTriggered?.Invoke(_currentDialogue);
        }
    }
}