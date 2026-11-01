using UnityEngine;
using UnityEngine.UI;

public class UI_Audio : MonoBehaviour
{
    [Header("Sliders")]
    public Slider _musicSlider;
    public Slider _sfxSlider;

    private void OnEnable()
    {
        if (MusicManager.Instance != null)
        {
            _musicSlider.SetValueWithoutNotify(MusicManager.Instance._currentMusicVolume);
            _sfxSlider.SetValueWithoutNotify(MusicManager.Instance._currentSFXVolume);
        }
    }

    public void OnMusicSliderChanged (float _value)
    {
        if (MusicManager.Instance != null) MusicManager.Instance.SetMusicVolume(_value);
    }

    public void OnSFXSliderChanged (float _value)
    {
        if (MusicManager.Instance != null) MusicManager.Instance.SetSFXVolume(_value);
    }

}
