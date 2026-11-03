using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Configuração de Acesso")]
    [Tooltip("Qual cartão abre esta porta? (1, 2 ou 3)")]
    public int _requiredCard = 1;

    [Header("Transição de Fase (Opcional)")]
    [Tooltip("Se preenchido, interagir com a porta aberta carrega esta cena. Se vazio, interagir a fecha.")]
    public string _cenaDestino;

    [Header("Animação (Animator)")]
    [Tooltip("Arraste o objeto que possui o componente Animator (ex: Porta_Mesh)")]
    public Animator _animator;

    private bool _isOpen = false;

    public void TentarAbrir()
    {
        if (_isOpen)
        {
            if (!string.IsNullOrEmpty(_cenaDestino))
            {
                EntrarNaPorta();
            }
            return;
        }

        bool _hasAccess = false;

        if (PlayerInventory.Instance != null)
        {
            if (_requiredCard == 1) _hasAccess = PlayerInventory.Instance._hasCard1;
            else if (_requiredCard == 2) _hasAccess = PlayerInventory.Instance._hasCard2;
            else if (_requiredCard == 3) _hasAccess = PlayerInventory.Instance._hasCard3;
            else if (_requiredCard == 0) _hasAccess = true;
        }

        if (_hasAccess)
        {
            AbrirPorta();
        }
        else
        {
            Debug.Log($"[Porta] Trancada! Requer o cartão {_requiredCard}.");
        }
    }

    private void AbrirPorta()
    {
        _isOpen = true;

        if (_animator != null)
        {
            _animator.SetTrigger("_abrirPorta");
        }
        else
        {
            Debug.LogWarning("[Porta] O Animator não foi arrastado no Inspector!");
        }
    }

    public void FecharPorta()
    {
        _isOpen = false;

        if (_animator != null)
        {
            _animator.SetTrigger("_fecharPorta");
        }
    }

    private void EntrarNaPorta()
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