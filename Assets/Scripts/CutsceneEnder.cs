using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneEnder : MonoBehaviour
{
    [Header("Cena Destino")]
    [Tooltip("Nome da cena que vai carregar quando a cutscene acabar.")]
    public string _proximaCena;

    public void FinalizarCutscene()
    {
        if (SceneChanger.Instance != null)
        {
            SceneChanger.Instance.ChangeScene(_proximaCena);
        }
        else
        {
            Debug.LogWarning("[CutsceneEnder] SceneChanger não encontrado. Carregando cena bruscamente.");
            SceneManager.LoadScene(_proximaCena);
        }
    }
}