using UnityEngine;
using System;

public class RadiationSystem : MonoBehaviour
{
    public static event Action<float> OnRadiationChanged;

    public bool _takingRadiation = false;

    [Header("Modificadores velocidade")]
    public float _radiationDamage = 5f;
    public float _radiationRegen = 1f;

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
        if (other.CompareTag("Player")) 
        {
            _takingRadiation = false;
        }
    }

    private void FixedUpdate()
    {
        CountRadiation();
    }

    private void CountRadiation()
    {
        if (PlayerDeath.Instance != null && PlayerDeath.Instance._isDead) return;

        bool _valueHasChanged = false;

        if (_takingRadiation)
        {
            _currentRadiation = Mathf.Clamp01(_currentRadiation + (_radiationDamage / 20) * Time.fixedDeltaTime);
            _valueHasChanged = true;

            if (_currentRadiation >= 1f)
            {
                PlayerDeath.Instance.Morrer("Radiacao");
            }
        }
        else if (!_takingRadiation && _currentRadiation > 0f) 
        {
            _currentRadiation = Mathf.Clamp01(_currentRadiation - (_radiationRegen / 20) * Time.fixedDeltaTime);
            _valueHasChanged = true;
        }

        if (_valueHasChanged)
        {
            OnRadiationChanged?.Invoke(_currentRadiation);
        }
    }
}