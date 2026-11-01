using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TriggerEvent : MonoBehaviour
{
    [Header("Filtros de Colisão")]
    public string _targetTag = "Player";
    public bool _triggerOnlyOnce = true;

    [Header("Persistência com SaveManager (Opcional)")]
    [Tooltip("Se preenchido, salva a condição no JSON para nunca mais disparar mesmo recarregando a cena.")]
    public string _saveConditionKey = "";

    [Header("Proteção de Spawn")]
    [Tooltip("Tempo em segundos que aguarda ao carregar a cena antes de aceitar colisões.")]
    public float _spawnDelay = 0.5f;

    [Header("Eventos Inspector")]
    public UnityEvent _onTriggerEnter;
    public UnityEvent _onTriggerExit;

    private bool _hasTriggered = false;
    private bool _canTrigger = false;

    private IEnumerator Start()
    {
        if (!string.IsNullOrEmpty(_saveConditionKey) && SaveManager.Instance != null)
        {
            if (SaveManager.Instance._currentData._unlockedConditions.Contains(_saveConditionKey))
            {
                gameObject.SetActive(false);
                yield break;
            }
        }

        _canTrigger = false;
        yield return new WaitForSeconds(_spawnDelay);
        _canTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_canTrigger || (_hasTriggered && _triggerOnlyOnce)) return;

        if (string.IsNullOrEmpty(_targetTag) || other.CompareTag(_targetTag))
        {
            _hasTriggered = true;

            if (!string.IsNullOrEmpty(_saveConditionKey) && SaveManager.Instance != null)
            {
                if (!SaveManager.Instance._currentData._unlockedConditions.Contains(_saveConditionKey))
                {
                    SaveManager.Instance._currentData._unlockedConditions.Add(_saveConditionKey);
                    SaveManager.Instance.SaveGame();
                }
            }

            _onTriggerEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_canTrigger) return;

        if (string.IsNullOrEmpty(_targetTag) || other.CompareTag(_targetTag))
        {
            _onTriggerExit?.Invoke();
        }
    }
}