using UnityEngine;
using System;

public class PlatformFuseSlots : MonoBehaviour
{
    public enum FuseType { Nenhum, X, Y, Z }

    [Header("Estado da plataforma")]
    public FuseType _fusivelAtual = FuseType.Nenhum;

    [Header("Waypoints")]
    public WaypointPlatform _waypointScript;

    public static event Action<PlatformFuseSlots> OnAbrirMenuFusivel;

    private float _nextInteractionTime = 0f;

    private void Start()
    {
        AtualizarEstadoPlataforma();
    }

    public void InteragirComPlataforma()
    {
        if (Time.unscaledTime < _nextInteractionTime) return;
        
        if (_fusivelAtual == FuseType.Nenhum)
        {
            OnAbrirMenuFusivel?.Invoke(this);
        }
        else
        {
            RetornarFusivelAoPlayer();
            _nextInteractionTime = Time.unscaledTime + 0.3f;
        }
    }

    public void ReceberFusivel(FuseType _type)
    {
        _fusivelAtual = _type;
        _nextInteractionTime = Time.unscaledTime + 0.3f;

        AtualizarEstadoPlataforma();
    }

    private void RetornarFusivelAoPlayer()
    {
        if (PlayerInventory.Instance == null)
        {
            Debug.LogError("PlayerInventory.Instance não encontrado na cena!");
            return;
        }

        if (_fusivelAtual == FuseType.X) PlayerInventory.Instance.ColetarFusivel("X");
        else if (_fusivelAtual == FuseType.Y) PlayerInventory.Instance.ColetarFusivel("Y");
        else if (_fusivelAtual == FuseType.Z) PlayerInventory.Instance.ColetarFusivel("Z");
        
        _fusivelAtual = FuseType.Nenhum;

        AtualizarEstadoPlataforma();
    }

    private void AtualizarEstadoPlataforma()
    {
        if (_waypointScript == null) return;

        _waypointScript._moveToX = (_fusivelAtual == FuseType.X);
        _waypointScript._moveToY = (_fusivelAtual == FuseType.Y);
        _waypointScript._moveToZ = (_fusivelAtual == FuseType.Z);
    }
}