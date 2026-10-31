using UnityEngine;
using DG.Tweening;

public class FuseItem : MonoBehaviour
{
    public enum FuseType { X, Y, Z }

    [Header("Configuração do Fusível")]
    [Tooltip("Qual é o tipo deste fusível? (Altere para ver a cor mudar na hora)")]
    public FuseType _fuseType = FuseType.X;

    [Header("Referências Visuais")]
    [Tooltip("Arraste o objeto filho que contém a malha 3D. Se deixar vazio, pega o 1º filho automaticamente para proteger o Root.")]
    public Transform _meshTransform;
    public MeshRenderer _fuseRenderer;
    public Material _materialX;
    public Material _materialY;
    public Material _materialZ;

    [Header("Animação Estilo RPG")]
    public bool _animarItem = true;
    public float _floatHeight = 0.2f;
    public float _floatDuration = 1.2f;
    public float _rotationSpeed = 60f;

    private Vector3 _initialLocalPosition;

    private void OnValidate()
    {
        AtualizarMaterialVisivel();
    }

    private void Start()
    {
        AtualizarMaterialVisivel();

        if (_animarItem)
        {
            IniciarAnimacaoRPG();
        }
    }

    private void AtualizarMaterialVisivel()
    {
        if (_fuseRenderer == null) return;

        switch (_fuseType)
        {
            case FuseType.X:
                if (_materialX != null) _fuseRenderer.sharedMaterial = _materialX;
                break;
            case FuseType.Y:
                if (_materialY != null) _fuseRenderer.sharedMaterial = _materialY;
                break;
            case FuseType.Z:
                if (_materialZ != null) _fuseRenderer.sharedMaterial = _materialZ;
                break;
        }
    }

    private void IniciarAnimacaoRPG()
    {
        if (_meshTransform == null)
        {
            if (transform.childCount > 0)
                _meshTransform = transform.GetChild(0);
            else
                _meshTransform = transform;
        }

        _initialLocalPosition = _meshTransform.localPosition;

        _meshTransform.DOLocalMoveY(_initialLocalPosition.y + _floatHeight, _floatDuration)
                      .SetLoops(-1, LoopType.Yoyo)
                      .SetEase(Ease.InOutSine);

        float _tempoPorVolta = 360f / _rotationSpeed;
        _meshTransform.DOLocalRotate(new Vector3(0f, 360f, 0f), _tempoPorVolta, RotateMode.FastBeyond360)
                      .SetLoops(-1, LoopType.Restart)
                      .SetEase(Ease.Linear);
    }


    public void Coletar()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.ColetarFusivel(_fuseType.ToString());
            Debug.Log($"[Fusível] Fusível {_fuseType} coletado!");
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("[Fusível] PlayerInventory não encontrado na cena!");
        }
    }

    private void OnDestroy()
    {
        if (_meshTransform != null)
        {
            _meshTransform.DOKill();
        }
    }
}