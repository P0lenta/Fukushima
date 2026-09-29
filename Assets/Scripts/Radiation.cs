using UnityEngine;
using UnityEngine.UI;

public class RadiationSystem : MonoBehaviour
{
    [Header("Slider Radiação")]
    public RectTransform _radiationPointer;
    public bool _takingRadiation = false;

    [Header("Modificadores velocidade")]
    public float _radiationDamage = 5f;
    public float _radiationRegen = 1f;

    [Header("Modificadores ponteiro")]
    public float _safeAngle = 70f;
    public float _fuckAngle = -70f;
    private float _currentRadiation = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _takingRadiation = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // CORRIGIDO: de "Plaeyr" para "Player"
        if (other.CompareTag("Player")) 
        {
            _takingRadiation = false;
        }
    }

    private void FixedUpdate()
    {
        CountRadiation();
        UpdatePointer();
    }

    private void CountRadiation()
    {
        if (_takingRadiation)
        {
            _currentRadiation = Mathf.Clamp01(_currentRadiation + (_radiationDamage / 20) * Time.fixedDeltaTime);
        }
        else if (!_takingRadiation && _currentRadiation > 0f) 
        {
            _currentRadiation = Mathf.Clamp01(_currentRadiation - (_radiationRegen / 20) * Time.fixedDeltaTime);
        }
    }

    private void UpdatePointer()
    {
        if (_radiationPointer == null) return;

        float _targetAngle = Mathf.Lerp(_safeAngle, _fuckAngle, _currentRadiation);

        float _currentZ = _radiationPointer.eulerAngles.z;

        _radiationPointer.localRotation = Quaternion.Euler(0f, 0f, _targetAngle);
    }
}