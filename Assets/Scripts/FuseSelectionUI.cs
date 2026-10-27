using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DG.Tweening;

public class FuseSelectionUI : MonoBehaviour
{
    [Header("Referências Internas do Prefab")]
    public RectTransform _selectorImage;
    public RectTransform _slotX;
    public RectTransform _slotY;
    public RectTransform _slotZ;

    [Header("Inputs (Arrastar as Actions do arquivo .inputactions)")]
    public InputActionProperty _navigateInput;
    public InputActionProperty _confirmInput;
    public InputActionProperty _cancelInput;

    public static event Action OnMenuOpened;
    public static event Action OnMenuClosed;

    private bool _isMenuOpen = false;
    private bool _canConfirm = false;
    private PlatformFuseSlots _currentPlatform;

    private List<int> _availableIndexes = new List<int>();
    private int _currentSelectionPointer = 0;

    private void OnEnable()
    {
        PlatformFuseSlots.OnAbrirMenuFusivel += AbrirMenuFusivel;

        _navigateInput.action.Enable();
        _confirmInput.action.Enable();
        _cancelInput.action.Enable();

        _navigateInput.action.performed += NavegarFusivel;
        _confirmInput.action.started += OnConfirm;
        _cancelInput.action.started += OnCancel;
    }

    private void OnDisable()
    {
        PlatformFuseSlots.OnAbrirMenuFusivel -= AbrirMenuFusivel;

        _navigateInput.action.performed -= NavegarFusivel;
        _confirmInput.action.started -= OnConfirm;
        _cancelInput.action.started -= OnCancel;

        _navigateInput.action.Disable();
        _confirmInput.action.Disable();
        _cancelInput.action.Disable();
    }

    private void Start()
    {
        if (_selectorImage != null) _selectorImage.gameObject.SetActive(false);
        _canConfirm = false;
    }

    public void AbrirMenuFusivel(PlatformFuseSlots _platform)
    {
        if (_isMenuOpen) return;
        if (PlayerInventory.Instance == null) return;

        _currentPlatform = _platform;
        _availableIndexes.Clear();

        if (_slotX != null) _slotX.gameObject.SetActive(PlayerInventory.Instance._hasFuseX);
        if (_slotY != null) _slotY.gameObject.SetActive(PlayerInventory.Instance._hasFuseY);
        if (_slotZ != null) _slotZ.gameObject.SetActive(PlayerInventory.Instance._hasFuseZ);

        if (PlayerInventory.Instance._hasFuseX) _availableIndexes.Add(0);
        if (PlayerInventory.Instance._hasFuseY) _availableIndexes.Add(1);
        if (PlayerInventory.Instance._hasFuseZ) _availableIndexes.Add(2);

        if (_availableIndexes.Count == 0) return;

        _isMenuOpen = true;
        _canConfirm = false;
        _currentSelectionPointer = 0;

        if (_selectorImage != null) _selectorImage.gameObject.SetActive(true);

        AtualizarSeletor(instantanio: true);

        OnMenuOpened?.Invoke();

        StopAllCoroutines();
        StartCoroutine(LiberarConfirmacao());
    }

    private IEnumerator LiberarConfirmacao()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        _canConfirm = true;
    }

    public void FecharMenuFusivel()
    {
        _isMenuOpen = false;
        _currentPlatform = null;
        _canConfirm = false;

        if (_selectorImage != null)
        {
            _selectorImage.DOKill();
            _selectorImage.gameObject.SetActive(false);
        }

        OnMenuClosed?.Invoke();
    }

    private void NavegarFusivel(InputAction.CallbackContext context)
    {
        if (!_isMenuOpen || _availableIndexes.Count <= 1) return;

        Vector2 _inputDir = context.ReadValue<Vector2>();

        if (_inputDir.x > 0.3f)
        {
            _currentSelectionPointer = (_currentSelectionPointer + 1) % _availableIndexes.Count;
            AtualizarSeletor(instantanio: false);
        }
        else if (_inputDir.x < -0.3f)
        {
            _currentSelectionPointer--;
            if (_currentSelectionPointer < 0) _currentSelectionPointer = _availableIndexes.Count - 1;
            AtualizarSeletor(instantanio: false);
        }
    }

    private void OnConfirm(InputAction.CallbackContext context)
    {
        if (!_isMenuOpen || !_canConfirm) return;
        TentarPegarFusivel();
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (!_isMenuOpen) return;
        FecharMenuFusivel();
    }

    private void AtualizarSeletor(bool instantanio = false)
    {
        if (_selectorImage == null || _availableIndexes.Count == 0) return;

        int _realFuseIndex = _availableIndexes[_currentSelectionPointer];
        RectTransform _targetSlot = PegarPosicaoSlot(_realFuseIndex);

        if (_targetSlot != null)
        {
            _selectorImage.DOKill();

            if (instantanio)
            {
                _selectorImage.position = _targetSlot.position;
            }
            else
            {
                _selectorImage.DOMove(_targetSlot.position, 0.15f).SetUpdate(true).SetEase(Ease.OutCubic);
            }
        }
    }

    private RectTransform PegarPosicaoSlot(int _index)
    {
        switch (_index)
        {
            case 0: return _slotX;
            case 1: return _slotY;
            case 2: return _slotZ;
            default: return null;
        }
    }

    private void TentarPegarFusivel()
    {
        if (_currentPlatform == null || _availableIndexes.Count == 0) return;

        int _selectedFuse = _availableIndexes[_currentSelectionPointer];

        if (_selectedFuse == 0 && PlayerInventory.Instance._hasFuseX)
        {
            PlayerInventory.Instance.ConsumirFusivel("X");
            _currentPlatform.ReceberFusivel(PlatformFuseSlots.FuseType.X);
            FecharMenuFusivel();
        }
        else if (_selectedFuse == 1 && PlayerInventory.Instance._hasFuseY)
        {
            PlayerInventory.Instance.ConsumirFusivel("Y");
            _currentPlatform.ReceberFusivel(PlatformFuseSlots.FuseType.Y);
            FecharMenuFusivel();
        }
        else if (_selectedFuse == 2 && PlayerInventory.Instance._hasFuseZ)
        {
            PlayerInventory.Instance.ConsumirFusivel("Z");
            _currentPlatform.ReceberFusivel(PlatformFuseSlots.FuseType.Z);
            FecharMenuFusivel();
        }
    }
}