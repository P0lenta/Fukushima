using UnityEngine;

[System.Serializable] 
public class PlayerData
{
    [Header("Configurações")]
    public float _musicVolume = 0.5f;
    public float _sfxVolume = 0.5f;

    [Header("Progresso do Jogador")]
    public int _playerLevel = 1;

    [Header("Inventário / Condicionais")]
    public bool _isTutorialCompleted = false;

    [Header("Condições e Diálogos")]
    public System.Collections.Generic.List<string> _unlockedConditions = new System.Collections.Generic.List<string>();
}