using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class TriggerEvent : MonoBehaviour
{
    [Header("Debug (Testes)")]
    [Tooltip("Se marcado, ignora o JSON e a memória de morte, rodando o gatilho sempre (Apenas no Editor do Unity)")]
    public bool _forcarModoTeste = false;

    [Header("Filtros de Colisão")]
    public string _targetTag = "Player";
    public bool _triggerOnlyOnce = true;
    [Tooltip("Se marcado, o gatilho some se o player morrer e a fase recarregar (ideal para câmeras de puzzle, sem precisar criar chave no JSON).")]
    public bool _naoRepetirAoMorrer = false;

    [Header("Ações de Limpeza (Para Cutscenes)")]
    [Tooltip("Se marcado, desativa o GameObject inteiro após ativar. Excelente para não deixar lixo na cena.")]
    public bool _disableGameObjectAfter = false;

    [Header("Persistência Permanente (SaveManager JSON)")]
    [Tooltip("Opcional: salva permanentemente no JSON (use para diálogos únicos, chaves ou progresso definitivo).")]
    public string _saveConditionKey = "";

    [Header("Proteção de Spawn")]
    [Tooltip("Tempo aguardando após a cena carregar antes de aceitar colisões.")]
    public float _spawnDelay = 0.5f;

    [Header("Eventos Inspector")]
    public UnityEvent _onTriggerEnter;
    public UnityEvent _onTriggerExit;

    private bool _hasTriggered = false;
    private bool _canTrigger = false;
    private Collider[] _meusColisores;

    private static readonly HashSet<string> _triggeredInCurrentScene = new HashSet<string>();
    private static string _lastGameplayScene = "";

    private IEnumerator Start()
    {
        _meusColisores = GetComponentsInChildren<Collider>(true);

        string cenaAtual = SceneManager.GetActiveScene().name;
        bool isMenu = PanelManager.Instance != null && PanelManager.Instance._isMainMenu;

        if (isMenu)
        {
            _triggeredInCurrentScene.Clear();
            _lastGameplayScene = "";
        }
        else if (_lastGameplayScene != cenaAtual)
        {
            _triggeredInCurrentScene.Clear();
            _lastGameplayScene = cenaAtual;
        }

        bool jaPassou = false;

        if (_naoRepetirAoMorrer && _triggeredInCurrentScene.Contains(ObterAutoID()))
        {
            jaPassou = true;
        }

        if (!jaPassou && !string.IsNullOrEmpty(_saveConditionKey) && SaveManager.Instance != null)
        {
            jaPassou = SaveManager.Instance._currentData._unlockedConditions.Contains(_saveConditionKey);
        }

#if UNITY_EDITOR
        if (_forcarModoTeste) jaPassou = false;
#endif

        if (jaPassou)
        {
            DesativarGatilhoPermanentemente();
            yield break;
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

            if (_naoRepetirAoMorrer)
            {
                _triggeredInCurrentScene.Add(ObterAutoID());
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

    private string ObterAutoID()
    {
        string cena = SceneManager.GetActiveScene().name;
        return $"{cena}_{gameObject.name}_{transform.position.x:F1}_{transform.position.y:F1}_{transform.position.z:F1}";
    }

    private void DesativarGatilhoPermanentemente()
    {
        if (_meusColisores != null)
        {
            foreach (Collider col in _meusColisores)
            {
                if (col != null) col.enabled = false;
            }
        }

        if (_disableGameObjectAfter)
        {
            gameObject.SetActive(false);
        }
    }

    [ContextMenu("Resetar este evento no Save")]
    public void ResetarEventoNoSave()
    {
        _triggeredInCurrentScene.Remove(ObterAutoID());

        if (string.IsNullOrEmpty(_saveConditionKey))
        {
            Debug.Log("Memória temporária deste gatilho limpa.");
            return;
        }

        if (SaveManager.Instance != null && SaveManager.Instance._currentData != null)
        {
            if (SaveManager.Instance._currentData._unlockedConditions.Contains(_saveConditionKey))
            {
                SaveManager.Instance._currentData._unlockedConditions.Remove(_saveConditionKey);
                SaveManager.Instance.SaveGame();
                Debug.Log($"<color=green>Sucesso!</color> A chave '{_saveConditionKey}' foi apagada do save. Dê play para testar de novo.");
            }
            else
            {
                Debug.Log("O save já não possui esse evento registrado.");
            }
        }
        else
        {
            Debug.LogWarning("Dê play no jogo pelo menos uma vez na cena para carregar o SaveManager antes de usar este botão.");
        }
    }
}