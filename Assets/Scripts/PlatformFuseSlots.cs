using UnityEngine;

public class PlatformFuseSlots : MonoBehaviour
{
    public enum FuseType { Nenhum, X, Y, Z }

    [Header("Estado da plataforma")]
    public FuseType _fusivelAtual = FuseType.Nenhum;

    [Header("Referências")]
    public FuseSelectionUI _selectionUI;
    public PlayerInventory _playerInventory;

    [Header("Waypoints")]
    public WaypointPlatform _waypointScript;

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
            _selectionUI.AbrirMenuFusivel(this);
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
        if (_fusivelAtual == FuseType.X) _playerInventory.ColetarFusivelX();
        else if (_fusivelAtual == FuseType.Y) _playerInventory.ColetarFusivelY();
        else if (_fusivelAtual == FuseType.Z) _playerInventory.ColetarFusivelZ();
        
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
