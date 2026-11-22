using UnityEngine;
using System.Collections.Generic;

[System.Serializable] 
public class PlayerData
{
    [Header("Configurações")]
    public float _musicVolume = 0.5f;
    public float _sfxVolume = 0.5f;

    [Header("Progresso do Jogador")]
    public int _playerLevel = 1;
    public string _currentLevelName = "";

    [Header("Checkpoint Atual")]
    public bool _hasCheckpoint = false;
    public string _checkpointSceneName = "";
    public Vector3 _checkpointPosition = Vector3.zero;
    public Vector3 _checkpointCameraPosition = Vector3.zero;

    [Header("Inventário / Condicionais")]
    public bool _isTutorialCompleted = false;

    [Header("Condições e Diálogos")]
    public List<string> _unlockedConditions = new List<string>();
}