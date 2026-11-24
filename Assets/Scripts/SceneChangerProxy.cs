using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerProxy : MonoBehaviour
{
    public void CarregarCena(string nomeDaCena)
    {
        if (string.IsNullOrEmpty(nomeDaCena))
        {
            Debug.LogWarning("[SceneChangerProxy] Nome da cena não foi preenchido!");
            return;
        }

        Time.timeScale = 1f;

        if (SceneChanger.Instance != null)
        {
            SceneChanger.Instance.ChangeScene(nomeDaCena);
        }
        else
        {
            // Fallback caso você dê Play direto na cena de créditos pelo Editor
            SceneManager.LoadScene(nomeDaCena);
        }
    }
}