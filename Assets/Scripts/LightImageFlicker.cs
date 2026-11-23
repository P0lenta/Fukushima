using UnityEngine;
using UnityEngine.UI;

public class LightImageFlicker : MonoBehaviour
{
    private Image _image;
    private float _timer;

    [Range(0f, 1f)] public float _minAlpha = 0f;
    [Range(0f, 1f)] public float _maxAlpha = 0.25f;
    public float _flickerSpeed = 0.001f;

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _flickerSpeed)
        {
            _timer = 0f;

            Color _corAtual = _image.color;
            _corAtual.a = Random.Range(_minAlpha, _maxAlpha);
            _image.color = _corAtual;
        }
    }
}