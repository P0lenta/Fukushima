using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Configuração de Acesso")]
    [Tooltip("Qual cartão abre esta porta? (1, 2 ou 3)")]
    public int _requiredCard = 1;

    [Header("Feedback J1")]
    public MeshRenderer _doorRenderer;
    public Material _materialAberta;

    private bool _isOpen = false;

    public void TentarAbrir()
    {
        if (_isOpen) return;

        bool _hasAccess = false;

        if (_requiredCard == 1) _hasAccess = PlayerInventory.Instance._hasCard1;
        else if (_requiredCard == 2) _hasAccess = PlayerInventory.Instance._hasCard2;
        else if (_requiredCard == 3) _hasAccess = PlayerInventory.Instance._hasCard3;

        if (_hasAccess)
        {   
            AbrirPorta();
        }
        else 
        {
            Debug.Log($"A porta está trancada. Requer o cartão {_requiredCard}.");
        }
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