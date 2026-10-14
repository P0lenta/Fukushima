using UnityEngine;
using UnityEngine.Events;

public class ConditionalEventTrigger : MonoBehaviour
{
    [Header("Chave de validação")]
    public string _conditionKey;

    [Header("Evento com a condição (Destrancado)")]
    public UnityEvent _onConditionTrue;

    [Header("Evento sem a condição (Trancado)")]
    public UnityEvent _onConditionFalse;

    public void ExecuteCondition()
    {
        bool _hasCondition = PlayerPrefs.GetInt(_conditionKey, 0) == 1;

        if (_hasCondition)
        {
            _onConditionTrue?.Invoke();
        }
        else
        {
            _onConditionFalse?.Invoke();
        }
    }
}
