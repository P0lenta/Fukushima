using UnityEngine;

public class DialogueConditionSetter : MonoBehaviour
{
    [Header("Chave de diálogo")]
    public string _conditionKey;

    public void UnlockCondition()
    {
        PlayerPrefs.SetInt(_conditionKey, 1);
        PlayerPrefs.Save();

        Debug.Log($"Condição '{_conditionKey}' foi ativada!");
    }
}
