using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class TriggerEvent : MonoBehaviour
{
    [Header("Filtros de Colisão")]
    public string _targetTag = "Player";
    public bool _triggerOnlyOnce = true;

    [Header("Ações de Limpeza (Para Cutscenes)")]
    [Tooltip("Se marcado, desativa o GameObject inteiro após ativar. Excelente para não deixar lixo na cena.")]
    public bool _disableGameObjectAfter = false;

    [Header("Persistência com SaveManager")]
    [Tooltip("Se preenchido, salva no JSON. Ao recarregar a cena, o gatilho nem chegará a existir.")]
    public string _saveConditionKey = "";

    [Header("Proteção de Spawn")]
    [Tooltip("Tempo aguardando após a cena carregar antes de aceitar colisões.")]
    public float _spawnDelay = 0.5f;

    [Header("Eventos Inspector")]
    public UnityEvent _onTriggerEnter;
    public UnityEvent _onTriggerExit;

    // --- Variáveis de Controle ---
    private bool _hasTriggered = false;
    private bool _canTrigger = false;
    private Collider _meuColisor;

    private IEnumerator Start()
    {
        _meuColisor = GetComponent<Collider>();

        if (!string.IsNullOrEmpty(_saveConditionKey) && SaveManager.Instance != null)
        {
            if (SaveManager.Instance._currentData._unlockedConditions.Contains(_saveConditionKey))
            {
                DesativarGatilhoPermanentemente();
                yield break; 
            }
        }

        _canTrigger = false;
        yield return new WaitForSeconds(_spawnDelay);
        _canTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_canTrigger || _hasTriggered) return;

        if (string.IsNullOrEmpty(_targetTag) || other.CompareTag(_targetTag))
        {
            if (_triggerOnlyOnce) 
            {
                _hasTriggered = true; 
            }

            if (!string.IsNullOrEmpty(_saveConditionKey) && SaveManager.Instance != null)
            {
                if (!SaveManager.Instance._currentData._unlockedConditions.Contains(_saveConditionKey))
                {
                    SaveManager.Instance._currentData._unlockedConditions.Add(_saveConditionKey);
                    SaveManager.Instance.SaveGame();
                }
            }

            _onTriggerEnter?.Invoke();

            if (_triggerOnlyOnce)
            {
                DesativarGatilhoPermanentemente();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_canTrigger || _hasTriggered) return;

        if (string.IsNullOrEmpty(_targetTag) || other.CompareTag(_targetTag))
        {
            _onTriggerExit?.Invoke();
        }
    }

    private void DesativarGatilhoPermanentemente()
    {
        if (_meuColisor != null)
        {
            _meuColisor.enabled = false;
        }

        // Desliga o objeto inteiro da cena
        if (_disableGameObjectAfter)
        {
            gameObject.SetActive(false);
        }
    }
}