using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("Configurações de mira")]
    public float _interactRange = 2f;
    public LayerMask _interactLayer;

    public Transform rayOrigin;

    private IInteractable _currentInteractable;

    void Update()
    {
        ChecarInteracao();
    }

    void ChecarInteracao()
    {
        Vector3 _direction = transform.forward;

        Debug.DrawRay(rayOrigin.position, _direction * _interactRange, Color.red);

        if (Physics.Raycast(rayOrigin.position, _direction, out RaycastHit _hit, _interactRange, _interactLayer))
        {
            if (_hit.collider.TryGetComponent<IInteractable>(out IInteractable _interactableObject))
            {
                Debug.DrawRay(rayOrigin.position, _direction * _interactRange, Color.green);
                
                if (_currentInteractable != _interactableObject)
                {
                    if (_currentInteractable != null) _currentInteractable.OnLoseFocus();

                    _currentInteractable = _interactableObject;
                    _currentInteractable.OnFocus();
                }
                return;
            }
        }

        if (_currentInteractable != null)
        {
            _currentInteractable.OnLoseFocus();
            _currentInteractable = null;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (_currentInteractable != null) _currentInteractable.Interact();
    }
    

}
