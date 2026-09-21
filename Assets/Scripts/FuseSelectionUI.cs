using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FuseSelectionUI : MonoBehaviour
{
    [Header("Referências")]
    public PlayerInventory _inventory;
    public GameObject _selectorImage;
    public RectTransform[] _fuseSlots;

    [Header("Eventos de Trava")]
    public UnityEvent _onOpenMenu;
    public UnityEvent _onCloseMenu;

    private bool _isMenuOpen = false;
    private bool _canConfirm = false;
    private int _selectedIndex = 0;
    private PlatformFuseSlots _currentPlatform;

    private void Start()
    {
        if (_selectorImage != null) _selectorImage.SetActive(false);
        
        _canConfirm = false;
    }

    public void AbrirMenuFusivel(PlatformFuseSlots _platform)
    {
        _currentPlatform = _platform;
        _isMenuOpen = true;
        _selectorImage.SetActive(true);
        _selectedIndex = 0;

        AtualizarSeletor();
        _onOpenMenu?.Invoke();

        StartCoroutine(LiberarConfirmacao());
    }

    private IEnumerator LiberarConfirmacao()
    {
        yield return new WaitForEndOfFrame();
        _canConfirm = true;
    }

    public void FecharMenuFusivel()
    {
        _isMenuOpen = false;
        _selectorImage.SetActive(false);
        _currentPlatform = null;
        _canConfirm = false;

        _onCloseMenu?.Invoke();
    }

    public void NavegarFusivel(InputAction.CallbackContext context)
    {
        if (!_isMenuOpen || (!context.started && !context.performed)) return;

        Vector2 _inputDir = context.ReadValue<Vector2>();


        if (context.started || context.performed)
        {
            if (_inputDir.x > 0.5f)
        {
            _selectedIndex = (_selectedIndex + 1) % 3;
            AtualizarSeletor();
        }
        else if (_inputDir.x < -0.5f)
        {
            _selectedIndex--;
            if (_selectedIndex < 0) _selectedIndex = 2;
            AtualizarSeletor();
        }
        
        }
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (!_isMenuOpen || !_canConfirm || !context.started) return;

        TentarPegarFusivel();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!_isMenuOpen || !context.started) return;

        FecharMenuFusivel();
    }

    private void AtualizarSeletor()
    {
        if (_fuseSlots.Length > _selectedIndex && _fuseSlots[_selectedIndex] != null)
        {
            _selectorImage.transform.position = _fuseSlots[_selectedIndex].position;
        }
    }

    private void TentarPegarFusivel()
    {
        if (_selectedIndex == 0 && _inventory._hasFuseX)
        {
            _inventory.ColetarFusivelX();
            _currentPlatform.ReceberFusivel(PlatformFuseSlots.FuseType.X);
            FecharMenuFusivel();
        }
        else if (_selectedIndex == 1 && _inventory._hasFuseY)
        {
            _inventory.ColetarFusivelY();
            _currentPlatform.ReceberFusivel(PlatformFuseSlots.FuseType.Y);
            FecharMenuFusivel();
        }
        else if (_selectedIndex == 2 && _inventory._hasFuseZ)
        {
            _inventory.ColetarFusivelZ();
            _currentPlatform.ReceberFusivel(PlatformFuseSlots.FuseType.Z);
            FecharMenuFusivel();
        }
    }

}
