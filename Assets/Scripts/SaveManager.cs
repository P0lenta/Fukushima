using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public PlayerData _currentData; 

    [Header("Cenas que não salvam progresso")]
    public string[] _cenasIgnoradas = { "Menu principal", "Credits", "Creditos" };

    private string _savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            
            _savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
            
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += AoCarregarCena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AoCarregarCena;
    }

    private bool EhCenaIgnorada(string nomeCena)
    {
        if (PanelManager.Instance != null && PanelManager.Instance._isMainMenu) return true;

        // Ignora cenas de cutscene/créditos/menu que não possuem Player
        if (FindFirstObjectByType<PlayerMovement>(FindObjectsInactive.Include) == null) return true;

        if (_cenasIgnoradas != null)
        {
            foreach (string ignorada in _cenasIgnoradas)
            {
                if (!string.IsNullOrEmpty(ignorada) &&
                    string.Equals(ignorada, nomeCena, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void AoCarregarCena(Scene _cena, LoadSceneMode _modo)
    {
        if (_currentData == null) _currentData = new PlayerData();
        if (EhCenaIgnorada(_cena.name)) return;

        if (_currentData._currentLevelName != _cena.name)
        {
            _currentData._currentLevelName = _cena.name;
            if (_cena.buildIndex > 0)
            {
                _currentData._playerLevel = Mathf.Max(_currentData._playerLevel, _cena.buildIndex);
            }

            _currentData._hasCheckpoint = false;
            _currentData._checkpointSceneName = "";
            _currentData._checkpointPosition = Vector3.zero;
            _currentData._checkpointCameraPosition = Vector3.zero;
            SaveGame();
        }
    }

    public void SaveGame()
    {
        if (_currentData == null) _currentData = new PlayerData();

        try
        {
            string _json = JsonUtility.ToJson(_currentData, true);
            File.WriteAllText(_savePath, _json);
        }
        catch (Exception e)
        {
            Debug.LogWarning("[SaveManager] Falha ao salvar arquivo: " + e.Message);
        }
    }

    public void LoadGame()
    {
        try
        {
            if (File.Exists(_savePath))
            {
                string _json = File.ReadAllText(_savePath);
                if (!string.IsNullOrWhiteSpace(_json))
                {
                    _currentData = JsonUtility.FromJson<PlayerData>(_json);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[SaveManager] Falha ao ler save existente, recriando: " + e.Message);
            _currentData = null;
        }

        if (_currentData == null)
        {
            ResetData();
        }
    }

    public void ResetData()
    {
        _currentData = new PlayerData(); 
        SaveGame();
    }

    public void SalvarCheckpoint(Vector3 _posicaoPlayer, Vector3 _posicaoCamera)
    {
        if (_currentData == null) _currentData = new PlayerData();

        _currentData._hasCheckpoint = true;
        _currentData._checkpointSceneName = SceneManager.GetActiveScene().name;
        _currentData._checkpointPosition = _posicaoPlayer;
        _currentData._checkpointCameraPosition = _posicaoCamera;
        SaveGame();
    }

    public bool TemCheckpointNaCenaAtual()
    {
        if (_currentData == null || !_currentData._hasCheckpoint) return false;
        return _currentData._checkpointSceneName == SceneManager.GetActiveScene().name;
    }

    public void ContinuarJogoSalvo(string _primeiroLevelPadrao)
    {
        if (_currentData == null) _currentData = new PlayerData();

        string _cenaDestino = string.IsNullOrEmpty(_currentData._currentLevelName) 
            ? _primeiroLevelPadrao 
            : _currentData._currentLevelName;

        Time.timeScale = 1f;

        if (SceneChanger.Instance != null)
        {
            SceneChanger.Instance.ChangeScene(_cenaDestino);
        }
        else
        {
            SceneManager.LoadScene(_cenaDestino);
        }
    }
}