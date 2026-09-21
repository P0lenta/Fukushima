using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("HUD")]
    public GameObject _uiCard;
    public GameObject _uiFuseX;
    public GameObject _uiFuseY;
    public GameObject _uiFuseZ;

    [Header("Itens coletados")]
    public bool _hasCard = false;
    public bool _hasFuseX = false;
    public bool _hasFuseY = false;
    public bool _hasFuseZ = false;

    private void Start()
    {
        if (_uiCard != null) _uiCard.SetActive(_hasCard);
        if (_uiFuseX != null) _uiFuseX.SetActive(_hasFuseX);
        if (_uiFuseY != null) _uiFuseY.SetActive(_hasFuseY);
        if (_uiFuseZ != null) _uiFuseZ.SetActive(_hasFuseZ);
    }

    public void ColetarCartao()
    {
        _hasCard = !_hasCard;
        
        if (_uiCard != null) _uiCard.SetActive(_hasCard);
    }

    public void ColetarFusivelX()
    {
        _hasFuseX = !_hasFuseX;
        
        if (_uiFuseX != null) _uiFuseX.SetActive(_hasFuseX);
    }

    public void ColetarFusivelY()
    {
        _hasFuseY = !_hasFuseY;
        
        if (_uiFuseY != null) _uiFuseY.SetActive(_hasFuseY);
    }

    public void ColetarFusivelZ()
    {
        _hasFuseZ = !_hasFuseZ;
        
        if (_uiFuseZ != null) _uiFuseZ.SetActive(_hasFuseZ);
    }


}
