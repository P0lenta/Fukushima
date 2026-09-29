using UnityEngine;

public class FixerPlatform : MonoBehaviour
{
    [Header("Configurações")]
    public LayerMask _floorLayers;

    private Collider _col;
    private Rigidbody _currentPlatformRb;
    private Vector3 _lastPlatformPosition;

    public Vector3 _platformGetVelocity { get; private set; }

    private void Start() 
    {
        _col = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        ChecarPlataforma();

        if (_currentPlatformRb != null)
        {
            _platformGetVelocity = (_currentPlatformRb.position - _lastPlatformPosition) / Time.fixedDeltaTime;

            if (_platformGetVelocity.magnitude > 20f) 
                _platformGetVelocity = Vector3.zero;

            _lastPlatformPosition = _currentPlatformRb.position;
        }
        else
        {
            _platformGetVelocity = Vector3.zero;
        }
    }

    private void ChecarPlataforma()
    {
        if (Physics.Raycast(_col.bounds.center, Vector3.down, out RaycastHit _hit, _col.bounds.extents.y * 1.2f, _floorLayers, QueryTriggerInteraction.Ignore))
        {
            Rigidbody _platformRb = _hit.collider.GetComponentInParent<Rigidbody>();
            if (_platformRb != null)
            {       
                if (_currentPlatformRb != _platformRb)
                {
                    _currentPlatformRb = _platformRb;
                    _lastPlatformPosition = _platformRb.position;
                }
                return;
            }
        }
        _currentPlatformRb = null;
    }
}