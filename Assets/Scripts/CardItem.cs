using UnityEngine;

public class CardItem : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Qual é o número deste cartão? (1, 2 ou 3)")]
    public int _cardNumber = 1;

    public void Coletar()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.ColetarCartao(_cardNumber);
            
            Debug.Log($"[Cartão] Cartão {_cardNumber} coletado!");
        }
        else
        {
            Debug.LogWarning("[Cartão] PlayerInventory não encontrado na cena!");
        }
    }
}