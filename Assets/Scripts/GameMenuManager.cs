using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameMenuManager : MonoBehaviour
{
    [Header("Referências")]
    public PanelManager _panelManager;

    [Header("Cenas")]
    public string _mainMenuScene = "Menu Principal";

    public void OnTogglePause(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (_panelManager!= null) _panelManager.ToggleMenu();
    }

    public void VoltarMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(_mainMenuScene);
    }
}
