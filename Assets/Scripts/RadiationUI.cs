using UnityEngine;
using UnityEngine.UI;

public class RadiationUI : MonoBehaviour
{
    [Header("Slider Radiação (Ponteiro)")]
    public RectTransform _radiationPointer;
    public float _safeAngle = 70f;
    public float _fuckAngle = -70f;

    [Header("Borda de Aviso (Vinheta Amarela)")]
    public Image _yellowBorderImage;
    [Range(0f, 1f)]
    public float _maxBorderAlpha = 0.85f;

    [Header("Efeito de Ruído e Estática")]
    public RawImage _staticNoiseImage;
    [Range(0f, 1f)]
    public float _maxStaticAlpha = 0.35f;
    [Tooltip("Tamanho da repetição do ruído na tela (valores maiores = grãos menores)")]
    public float _noiseTiling = 4f;
    [Tooltip("Taxa de atualização da estática em FPS (24 a 30 dá um aspecto analógico)")]
    public float _staticUpdateFPS = 24f;

    [Header("Áudio de Interferência / Geiger")]
    public AudioSource _radiationAudioSource;
    [Range(0f, 1f)]
    public float _maxAudioVolume = 1f;
    public float _minAudioPitch = 0.9f;
    public float _maxAudioPitch = 1.35f;

    [Header("Ajuste Fino")]
    [Tooltip("A partir de quanto de radiação (0 a 1) os efeitos visuais começam a aparecer")]
    [Range(0f, 0.5f)]
    public float _effectStartThreshold = 0.05f;

    private float _currentIntensity = 0f;
    private float _staticTimer = 0f;

    private void Awake()
    {
        if (_staticNoiseImage != null && _staticNoiseImage.texture == null)
        {
            _staticNoiseImage.texture = GerarTexturaRuido(128, 128);
        }
    }

    private void OnEnable()
    {
        RadiationSystem.OnRadiationChanged += AtualizarFeedbackRadiacao;
    }

    private void OnDisable()
    {
        RadiationSystem.OnRadiationChanged -= AtualizarFeedbackRadiacao;
        if (_radiationAudioSource != null) _radiationAudioSource.Stop();
    }

    private void Start()
    {
        if (_radiationAudioSource != null)
        {
            _radiationAudioSource.loop = true;
            _radiationAudioSource.playOnAwake = false;
            _radiationAudioSource.volume = 0f;
        }

        AtualizarFeedbackRadiacao(0f);
    }

    private void Update()
    {
        if (_staticNoiseImage != null && _currentIntensity > 0f)
        {
            _staticTimer += Time.deltaTime;
            float interval = 1f / Mathf.Max(1f, _staticUpdateFPS);

            if (_staticTimer >= interval)
            {
                _staticTimer = 0f;
                _staticNoiseImage.uvRect = new Rect(Random.value, Random.value, _noiseTiling, _noiseTiling);
            }
        }
    }

    private void AtualizarFeedbackRadiacao(float _currentRadiation)
    {
        if (_radiationPointer != null)
        {
            float _targetAngle = Mathf.Lerp(_safeAngle, _fuckAngle, _currentRadiation);
            _radiationPointer.localRotation = Quaternion.Euler(0f, 0f, _targetAngle);
        }

        _currentIntensity = Mathf.InverseLerp(_effectStartThreshold, 1f, _currentRadiation);

        if (_yellowBorderImage != null)
        {
            Color c = _yellowBorderImage.color;
            c.a = Mathf.Lerp(0f, _maxBorderAlpha, _currentIntensity);
            _yellowBorderImage.color = c;
        }

        if (_staticNoiseImage != null)
        {
            Color c = _staticNoiseImage.color;
            c.a = Mathf.Lerp(0f, _maxStaticAlpha, _currentIntensity);
            _staticNoiseImage.color = c;
        }

        if (_radiationAudioSource != null)
        {
            if (_currentIntensity > 0f)
            {
                if (!_radiationAudioSource.isPlaying) _radiationAudioSource.Play();
                _radiationAudioSource.volume = Mathf.Lerp(0f, _maxAudioVolume, _currentIntensity);
                _radiationAudioSource.pitch = Mathf.Lerp(_minAudioPitch, _maxAudioPitch, _currentIntensity);
            }
            else if (_radiationAudioSource.isPlaying)
            {
                _radiationAudioSource.Stop();
            }
        }
    }

    private Texture2D GerarTexturaRuido(int width, int height)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Repeat;
        tex.filterMode = FilterMode.Point;

        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            float v = Random.value;
            pixels[i] = new Color(v, v, v, 1f);
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }
}