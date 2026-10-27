using UnityEngine;

public class DialogueConditionSetter : MonoBehaviour
{
    [Header("Chave de diálogo")]
    public string _conditionKey;

    public void UnlockCondition()
    {
        if (!SaveManager.Instance._currentData._unlockedConditions.Contains(_conditionKey))
        {
            SaveManager.Instance._currentData._unlockedConditions.Add(_conditionKey);
            
            SaveManager.Instance.SaveGame(); 

            Debug.Log($"Condição '{_conditionKey}' foi ativada e salva no JSON!");
        }
    }
}