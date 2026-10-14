using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Mixer")]
    public AudioMixer _masterMixer;

    private const float _defaultVolume = 0.5f;

    public float _currentMusicVolume { get; private set; }
    public float _currentSFXVolume { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAudioSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ApplyMusicVolume(_currentMusicVolume);
        ApplySFXVolume(_currentSFXVolume);
    }

    private void LoadAudioSettings()
    {
        _currentMusicVolume = PlayerPrefs.GetFloat("VolumeMusic", _defaultVolume);
        _currentSFXVolume = PlayerPrefs.GetFloat("VolumeSFX", _defaultVolume);
    }

    public void SetMusicVolume(float _volume)
    {
        _currentMusicVolume = _volume;
        ApplyMusicVolume(_volume);
        PlayerPrefs.SetFloat("VolumeMusic", _volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float _volume)
    {
        _currentSFXVolume = _volume;
        ApplySFXVolume(_volume);
        PlayerPrefs.SetFloat("VolumeSFX", _volume);
        PlayerPrefs.Save();
    }

    private void ApplyMusicVolume(float _linearVolume)
    {
        float dB = Mathf.Log10(Mathf.Max(_linearVolume, 0.0001f)) * 20f;
        _masterMixer.SetFloat("VolumeMusic", dB);
    }

    private void ApplySFXVolume(float _linearVolume)
    {
        float dB = Mathf.Log10(Mathf.Max(_linearVolume, 0.0001f)) * 20f;
        _masterMixer.SetFloat("VolumeSFX", dB);
    }
}