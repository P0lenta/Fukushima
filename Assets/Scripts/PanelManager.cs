using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [Header("Canvas de pause")]
    public GameObject _canvasPause;

    [Header("Painéis")]
    public GameObject[] _panels;
    public GameObject _initialPanel;

    private bool _menuOpen = false;

    private void Start() 
    {
        if (_canvasPause != null) _canvasPause.SetActive(false);
    }

    public void ToggleMenu()
    {
        _menuOpen = !_menuOpen;
        _canvasPause.SetActive(_initialPanel);

        if (_menuOpen)
        {
            AbrirPainel(_initialPanel);
            TravarJogo(true);
        }
            else
            {
                TravarJogo(false);
            }
    }
    public void AbrirPainel(GameObject _desiredPanel)
    {
        foreach (GameObject _panel in _panels)
        {
            _panel.SetActive(_panel == _desiredPanel);
        }
    }

    public void TravarJogo(bool _travar)
    {
        Cursor.lockState = _travar ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _travar;

        Time.timeScale = _travar ? 0f : 1f;
    }
}
