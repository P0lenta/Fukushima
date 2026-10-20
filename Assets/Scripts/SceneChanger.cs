using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }

    [Header("Referências de UI")]
    [SerializeField] private CanvasGroup _fadeCanvasGroup;
    [SerializeField] private GameObject _loadingAnimation;

    [Header("Configurações da Tela de Loading")]
    [SerializeField] private float _fadeDuration = 0.5f;
    [Tooltip("Tempo MÍNIMO que a tela de loading fica visível no início/transição (em segundos)")]
    [SerializeField] private float _minLoadingTime = 1.5f; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (_fadeCanvasGroup != null && _fadeCanvasGroup.alpha > 0.9f)
        {
            StartCoroutine(StartSceneWithLoading());
        }
    }

    /// <summary>
    /// </summary>
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        _fadeCanvasGroup.blocksRaycasts = true;

        yield return StartCoroutine(Fade(0f, 1f));

        if (_loadingAnimation != null)
            _loadingAnimation.SetActive(true);

        float startTime = Time.unscaledTime;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        float elapsedTime = Time.unscaledTime - startTime;
        if (elapsedTime < _minLoadingTime)
        {
            yield return new WaitForSecondsRealtime(_minLoadingTime - elapsedTime);
        }

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (_loadingAnimation != null)
            _loadingAnimation.SetActive(false);

        yield return StartCoroutine(Fade(1f, 0f));

        _fadeCanvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// </summary>
    
    private IEnumerator StartSceneWithLoading()
    {
        _fadeCanvasGroup.blocksRaycasts = true;

        if (_loadingAnimation != null)
            _loadingAnimation.SetActive(true);

        yield return new WaitForSecondsRealtime(_minLoadingTime);

        if (_loadingAnimation != null)
            _loadingAnimation.SetActive(false);

        yield return StartCoroutine(Fade(1f, 0f));

        _fadeCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator Fade(float startAlpha, float targetAlpha)
    {
        float timer = 0f;

        while (timer < _fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            _fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / _fadeDuration);
            yield return null;
        }

        _fadeCanvasGroup.alpha = targetAlpha;
    }
}