using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour
{
    [Header("Configuração de Acesso")]
    [Tooltip("Qual cartão abre esta porta? (1, 2 ou 3)")]
    public int _requiredCard = 1;

    [Header("Transição de Fase (Opcional)")]
    [Tooltip("Se preenchido, interagir com a porta aberta carrega esta cena.")]
    public string _cenaDestino;

    [Header("Animação de Abertura")]
    [Tooltip("Deslocamento da porta ao abrir (ex: Y = 3 para subir)")]
    public Vector3 _posicaoAbertaOffset = new Vector3(0f, 3f, 0f);
    public float _tempoAnimacao = 1f;

    private bool _isOpen = false;

    public void TentarAbrir()
    {
        if (_isOpen)
        {
            EntrarNaPorta();
            return;
        }

        bool _hasAccess = false;

        if (PlayerInventory.Instance != null)
        {
            if (_requiredCard == 1) _hasAccess = PlayerInventory.Instance._hasCard1;
            else if (_requiredCard == 2) _hasAccess = PlayerInventory.Instance._hasCard2;
            else if (_requiredCard == 3) _hasAccess = PlayerInventory.Instance._hasCard3;
        }

        if (_hasAccess)
        {
            AbrirPorta();
        }
        else
        {
            Debug.Log($"[Porta] Trancada! Requer o cartão {_requiredCard}.");
            
            transform.DOKill();
            transform.DOPunchPosition(transform.right * 0.12f, 0.25f, 10, 1f);
        }
    }

    private void AbrirPorta()
    {
        _isOpen = true;

        transform.DOMove(transform.position + _posicaoAbertaOffset, _tempoAnimacao)
                 .SetEase(Ease.OutCubic);
    }

    private void EntrarNaPorta()
    {
        if (!string.IsNullOrEmpty(_cenaDestino))
        {
            if (SceneChanger.Instance != null)
            {
                SceneChanger.Instance.ChangeScene(_cenaDestino);
            }
            else
            {
                Debug.LogWarning("[Porta] SceneChanger não encontrado na cena!");
            }
        }
    }
}