using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("HUD Cartões")]
    public GameObject _uiCard1;
    public GameObject _uiCard2;
    public GameObject _uiCard3;

    [Header("HUD Fusíveis")]
    public GameObject _uiFuseX;
    public GameObject _uiFuseY;
    public GameObject _uiFuseZ;

    private void OnEnable()
    {
        PlayerInventory.OnInventoryChanged += AtualizarHUD;
    }

    private void OnDisable()
    {
        PlayerInventory.OnInventoryChanged -= AtualizarHUD;
    }

    private void Start()
    {
        AtualizarHUD(); 
    }

    private void AtualizarHUD()
    {
        if (PlayerInventory.Instance == null) return;

        if (_uiCard1 != null) _uiCard1.SetActive(PlayerInventory.Instance._hasCard1);
        if (_uiCard2 != null) _uiCard2.SetActive(PlayerInventory.Instance._hasCard2);
        if (_uiCard3 != null) _uiCard3.SetActive(PlayerInventory.Instance._hasCard3);

        if (_uiFuseX != null) _uiFuseX.SetActive(PlayerInventory.Instance._hasFuseX);
        if (_uiFuseY != null) _uiFuseY.SetActive(PlayerInventory.Instance._hasFuseY);
        if (_uiFuseZ != null) _uiFuseZ.SetActive(PlayerInventory.Instance._hasFuseZ);
    }
}