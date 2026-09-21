using UnityEngine;
using UnityEngine.Events;

public class InteractionTrigger : MonoBehaviour, IInteractable
{
    [Header("Eventos de interação")]
    public UnityEvent _onInteract;

    [Header("Ícone de interação")]
    public UnityEvent _onFocus;
    public UnityEvent _onLoseFocus; 

    public void Interact()
    {
        _onInteract?.Invoke();
    }

    public void OnFocus()
    {
        Debug.Log("Raycast funcionou: Entrou em OnFocus!");
        _onFocus?.Invoke();
    }

    public void OnLoseFocus()
    {
        Debug.Log("Raycast funcionou: Entrou em OnLoseFocus!");
        _onLoseFocus?.Invoke();
    }
}
