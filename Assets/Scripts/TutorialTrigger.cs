using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public enum TipoTutorial { Movimento, Pulo }
    
    [Header("Qual tutorial este gatilho ativa?")]
    public TipoTutorial _tipo;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_tipo == TipoTutorial.Movimento)
            {
                TutorialManager.Instance.IniciarTutorialMovimento();
            }
            else if (_tipo == TipoTutorial.Pulo)
            {
                TutorialManager.Instance.IniciarTutorialPulo();
            }
            
            Destroy(gameObject); 
        }
    }
}