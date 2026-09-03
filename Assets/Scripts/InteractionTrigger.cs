using UnityEngine;
using UnityEngine.Events;

public class InteractionTrigger : MonoBehaviour, IInteractable
{
    public UnityEvent _onInteract;

    public void Interact()
    {
        _onInteract?.Invoke();
    }
}
