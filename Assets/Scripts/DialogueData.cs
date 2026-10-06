using UnityEngine;

[System.Serializable]
public struct DialogueLine
{
    [TextArea (3, 5)]
    public string _text;

    [Tooltip("Imagem")]
    public Sprite _documentImage;
}

[CreateAssetMenu(fileName = "Novo Dialogo", menuName = "Sistema de Dialogo/Novo Dialogo")]
public class DialogueData : ScriptableObject
{
    [Header("Linhas de Texto do Diálogo")]
    public DialogueLine[] _lines;
}
