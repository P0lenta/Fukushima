using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Feedback J1")]
    public MeshRenderer _doorRenderer;
    public Material _materialAberta;

    private bool _isOpen = false;

    public void TentarAbrir(PlayerInventory _playerInventory)
    {
        if (_isOpen) return;

        if (_playerInventory != null && _playerInventory._hasCard)
        {
            _playerInventory.ColetarCartao();
            AbrirPorta();
        }
        else Debug.Log("A porta está trancada");
    }

    private void AbrirPorta()
    {
        _isOpen = true;

        if (_doorRenderer != null && _materialAberta != null)
        {
            _doorRenderer.material = _materialAberta;
        }
    }
}
