    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.InputSystem;

    public class FuseSelectionUI : MonoBehaviour
    {
        [Header("Referências")]
        public PlayerInventory _inventory;
        public GameObject _selectorImage;

        [Header("Posições")]
        public RectTransform _slotX;
        public RectTransform _slotY;
        public RectTransform _slotZ;

        [Header("Eventos de Trava")]
        public UnityEvent _onOpenMenu;
        public UnityEvent _onCloseMenu;

        private bool _isMenuOpen = false;
        private bool _canConfirm = false;
        private PlatformFuseSlots _currentPlatform;

        private List<int> _availableIndexes = new List<int>();
        private int _currentSelectionPointer = 0;

        private void Start()
        {
            if (_selectorImage != null) _selectorImage.SetActive(false);
            
            _canConfirm = false;
        }

        public void AbrirMenuFusivel(PlatformFuseSlots _platform)
        {
            if (_isMenuOpen) return;
            
            _currentPlatform = _platform;
            _availableIndexes.Clear();

            if (_inventory._hasFuseX) _availableIndexes.Add(0);
            if (_inventory._hasFuseY) _availableIndexes.Add(1);
            if (_inventory._hasFuseZ) _availableIndexes.Add(2);

            if (_availableIndexes.Count == 0)
            {
                Debug.Log("Jogador não possui fusivel");
                return;
            }

            _isMenuOpen = true;
            _canConfirm = false;
            _currentSelectionPointer = 0;

            if (_selectorImage != null) _selectorImage.SetActive(true);

            AtualizarSeletor();
            _onOpenMenu?.Invoke();

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

            if (_selectorImage != null) _selectorImage.SetActive(false);

            _onCloseMenu?.Invoke();
        }

        public void NavegarFusivel(InputAction.CallbackContext context)
        {
            if (!_isMenuOpen || _availableIndexes.Count <= 1) return;

            if (context.started)
            {

            }

            if (context.started || context.performed)
            {
                Vector2 _inputDir = context.ReadValue<Vector2>();
                
                if (_inputDir.x < -0.1f)
                {
                    _currentSelectionPointer = (_currentSelectionPointer + 1) % _availableIndexes.Count;
                    AtualizarSeletor();
                }
                else if (_inputDir.x > 0.1f)
                {   
                    _currentSelectionPointer--;
                    if (_currentSelectionPointer < 0) _currentSelectionPointer = _availableIndexes.Count -1;
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
            if (_selectorImage == null || _availableIndexes.Count == 0) return;

            int _realFuseIndex = _availableIndexes[_currentSelectionPointer];
            RectTransform _targetSlot = PegarPosicaoSlot(_realFuseIndex);

            if (_targetSlot != null)
            {
                Canvas.ForceUpdateCanvases();
                _selectorImage.transform.position = _targetSlot.position;
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
            int _selectedFuse = _availableIndexes[_currentSelectionPointer];

            if (_selectedFuse == 0 && _inventory._hasFuseX)
            {
                _inventory.ColetarFusivelX();
                _currentPlatform.ReceberFusivel(PlatformFuseSlots.FuseType.X);
                FecharMenuFusivel();
            }
            else if (_selectedFuse == 1 && _inventory._hasFuseY)
            {
                _inventory.ColetarFusivelY();
                _currentPlatform.ReceberFusivel(PlatformFuseSlots.FuseType.Y);
                FecharMenuFusivel();
            }
            else if (_selectedFuse == 2 && _inventory._hasFuseZ)
            {
                _inventory.ColetarFusivelZ();
                _currentPlatform.ReceberFusivel(PlatformFuseSlots.FuseType.Z);
                FecharMenuFusivel();
            }
        }

    }
