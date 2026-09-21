using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Informações de cena")]
    public string _sceneName;

    [Header("Painéis de menu")]
    public PanelManager _panelManager;
    public GameObject _painelConfig;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 1f;
    }

    public void Jogar()
    {
        SceneManager.LoadScene(_sceneName);
    }

    public void Config()
    {
        if (_panelManager != null && _painelConfig != null) _panelManager.AbrirPainel(_painelConfig);
    }

    public void Sair()
    {
        Debug.Log("Jogo fechado");
        
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}
