using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepManager : MonoBehaviour
{
    [Header("Configurações do Raio")]
    [Tooltip("Arraste o objeto Pai (Player) aqui para o raio sair do centro dele")]
    public Transform _raycastOrigin; 
    public float _rayDistance = 1.2f;
    public LayerMask _floorLayer;
    
    [Header("Bancos de Áudio (Sons de Passos)")]
    public AudioClip[] _stoneSounds;
    public AudioClip[] _woodSounds;
    public AudioClip[] _metalSounds;
    public AudioClip[] _waterSounds;
    public AudioClip[] _defaultSounds;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 1f;
    }

    public void PlayFootstep()
    {
        if (Physics.Raycast(_raycastOrigin.position, Vector3.down, out RaycastHit hit, _rayDistance, _floorLayer))
        {
            AudioClip clipToPlay = null;

            switch (hit.collider.tag)
            {
                case "Wood":
                    clipToPlay = GetRandomClip(_woodSounds);
                    break;
                case "Stone":
                    clipToPlay = GetRandomClip(_stoneSounds);
                    break;
                case "Metal":
                    clipToPlay = GetRandomClip(_metalSounds);
                    break;
                case "Water":
                    clipToPlay = GetRandomClip(_waterSounds);
                    break;
                default:
                    clipToPlay = GetRandomClip(_defaultSounds);
                    break;
            }

            if (clipToPlay != null)
            {
                _audioSource.pitch = Random.Range(0.85f, 1.15f);
                _audioSource.volume = Random.Range(0.8f, 1f);
                
                _audioSource.PlayOneShot(clipToPlay);
            }
        }
    }

    private AudioClip GetRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }
}