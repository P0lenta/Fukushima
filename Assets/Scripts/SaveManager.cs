using UnityEngine;
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
}