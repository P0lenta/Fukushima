using UnityEngine;

public class RadiationUI : MonoBehaviour
{
    [Header("Slider Radiação")]
    public RectTransform _radiationPointer;

    [Header("Modificadores ponteiro")]
    public float _safeAngle = 70f;
    public float _fuckAngle = -70f;

    private void OnEnable()
    {
        RadiationSystem.OnRadiationChanged += AtualizarPonteiro;
    }

    private void OnDisable()
    {
        RadiationSystem.OnRadiationChanged -= AtualizarPonteiro;
    }

    private void Start()
    {
        AtualizarPonteiro(0f);
    }

    private void AtualizarPonteiro(float _currentRadiation)
    {
        if (_radiationPointer == null) return;

        float _targetAngle = Mathf.Lerp(_safeAngle, _fuckAngle, _currentRadiation);
        _radiationPointer.localRotation = Quaternion.Euler(0f, 0f, _targetAngle);
    }
}