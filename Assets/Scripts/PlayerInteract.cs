using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Configurações de mira")]
    public float _interactRange = 2f;
    public LayerMask _interactLayer;

    public Transform rayOrigin;

    private IInteractable _currentInteractable;

    void Update()
    {
        CheckInteractable();
    }

    void CheckInteractable()
    {
        Vector3 _direction = transform.forward;

        Debug.DrawRay(rayOrigin.position, _direction * _interactRange, Color.red);

        if (Physics.Raycast(rayOrigin.position, _direction, out RaycastHit _hit, _interactRange, _interactLayer))
        {
            if (_hit.collider.TryGetComponent<IInteractable>(out IInteractable _interactableObject))
            {
                _currentInteractable = _interactableObject;
                Debug.DrawRay(rayOrigin.position, _direction * _interactRange, Color.green);
                return;
            }
        }

        _currentInteractable = null;


    }


}
