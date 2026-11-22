using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public PlayerData _currentData; 

    private string _savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            _savePath = Application.persistentDataPath + "/savegame.json";
            
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

    private void AoCarregarCena(Scene _cena, LoadSceneMode _modo)
    {
        if (PanelManager.Instance != null && PanelManager.Instance._isMainMenu) return;

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
        string _json = JsonUtility.ToJson(_currentData, true);
        File.WriteAllText(_savePath, _json);
        
        Debug.Log("Jogo salvo com sucesso em: " + _savePath);
    }

    public void LoadGame()
    {
        if (File.Exists(_savePath))
        {
            string _json = File.ReadAllText(_savePath);
            _currentData = JsonUtility.FromJson<PlayerData>(_json);
        }
        else
        {
            Debug.Log("Nenhum save encontrado. Criando um novo...");
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