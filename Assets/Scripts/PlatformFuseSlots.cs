using UnityEngine;

public class PlatformFuseSlots : MonoBehaviour
{
    public enum FuseType { Nenhum, X, Y, Z }

    [Header("Estado da plataforma")]
    public FuseType _fusivelAtual = FuseType.Nenhum;

    [Header("Referências")]
    public FuseSelectionUI _selectionUI;
    public PlayerInventory _playerInventory;

    public void InteragirComPlataforma()
    {
        if (_fusivelAtual == FuseType.Nenhum)
        {
            _selectionUI.AbrirMenuFusivel(this);
        }
        else
        {
            RetornarFusivelAoPlayer();
        }
    }

    public void ReceberFusivel(FuseType _type)
    {
        _fusivelAtual = _type;
    }

    private void RetornarFusivelAoPlayer()
    {
        if (_fusivelAtual == FuseType.X) _playerInventory.ColetarFusivelX();
        else if (_fusivelAtual == FuseType.Y) _playerInventory.ColetarFusivelY();
        else if (_fusivelAtual == FuseType.Z) _playerInventory.ColetarFusivelZ();
        
    }
}
