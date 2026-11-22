using System.Collections;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [Header("Seguir Jogador")]
    public Transform _player;
    public bool _isParkourLevel = false;
    public float _turnSpeed = 5f;
    public float _wallCheckDistance = 3f;
    public LayerMask _wallLayer;

    [Header("Preview de Puzzle (Caminho da Câmera)")]
    [Tooltip("Se marcado, a câmera percorre o caminho automaticamente assim que o level começa.")]
    public bool _playOnStart = false;
    [Tooltip("Arraste objetos vazios (Empty GameObjects) na ordem em que a câmera deve visitá-los.")]
    public Transform[] _waypoints;
    [Tooltip("Velocidade de deslocamento da câmera durante a apresentação do puzzle.")]
    public float _panSpeed = 7f;
    [Tooltip("Tempo (em segundos) que a câmera para em cada ponto antes de ir para o próximo.")]
    public float _waitAtPoint = 0.8f;

    private Vector3 _offset;
    private Vector3 _initialPosition;
    private bool _wallIsBlocking = false;
    private bool _isPlayingCutscene = false;

    private bool _respawnouNoCheckpoint = false;

    private void Awake()
    {
        if (_player != null)
        {
            _offset = transform.position - _player.position;
        }
    }

    private void Start() 
    {
        if (SaveManager.Instance != null && SaveManager.Instance.TemCheckpointNaCenaAtual())
        {
            transform.position = SaveManager.Instance._currentData._checkpointCameraPosition;
            _respawnouNoCheckpoint = true;
        }

        _initialPosition = transform.position;

        if (_playOnStart && !_respawnouNoCheckpoint && _waypoints != null && _waypoints.Length > 0)
        {
            IniciarCaminhoCamera();
        }
    }

    private void LateUpdate() 
    {
        if (_player == null || _isPlayingCutscene) return;

        CheckWalls();

        Vector3 _cameraPosition = CalcularPosicaoSeguirPlayer();
        float _moveDirection = _cameraPosition.x - transform.position.x;

        if (_wallIsBlocking)
        {
            if ((_moveDirection > 0 && HitRightWall()) || (_moveDirection < 0 && HitLeftWall())) return;
        }

        transform.position = Vector3.Lerp(transform.position, _cameraPosition, _turnSpeed * Time.deltaTime);
    }

    private Vector3 CalcularPosicaoSeguirPlayer()
    {
        if (_isParkourLevel)
        {
            return new Vector3(
                _player.position.x + _offset.x,
                _player.position.y + _offset.y,
                _player.position.z + _offset.z
            );
        }
        else
        {
            return new Vector3(
                _player.position.x + _offset.x,
                _initialPosition.y,
                _initialPosition.z
            );
        }
    }

    public void IniciarCaminhoCamera()
    {
        IniciarCaminhoCustomizado(_waypoints);
    }

    public void IniciarCaminhoCustomizado(Transform[] customWaypoints)
    {
        if (_isPlayingCutscene || customWaypoints == null || customWaypoints.Length == 0) return;
        StartCoroutine(PercorrerCaminhoRoutine(customWaypoints));
    }

    private IEnumerator PercorrerCaminhoRoutine(Transform[] pontos)
    {
        _isPlayingCutscene = true;

        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.TravarControles();
        }

        foreach (Transform ponto in pontos)
        {
            if (ponto == null) continue;

            yield return StartCoroutine(MoverSuavementePara(ponto.position, _panSpeed));

            if (_waitAtPoint > 0f)
            {
                yield return new WaitForSeconds(_waitAtPoint);
            }
        }

        if (_player != null)
        {
            Vector3 posicaoDeRetorno = CalcularPosicaoSeguirPlayer();
            yield return StartCoroutine(MoverSuavementePara(posicaoDeRetorno, _panSpeed * 1.15f));
            transform.position = posicaoDeRetorno;
        }

        _isPlayingCutscene = false;

        if (PlayerMovement.Instance != null && (PlayerDeath.Instance == null || !PlayerDeath.Instance._isDead))
        {
            PlayerMovement.Instance.DestravarControles();
        }
    }

    private IEnumerator MoverSuavementePara(Vector3 destino, float velocidadeBase)
    {
        while (Vector3.Distance(transform.position, destino) > 0.03f)
        {
            float distancia = Vector3.Distance(transform.position, destino);
            float fatorSuavizacao = Mathf.Clamp(distancia / 2f, 0.25f, 1f);
            float passo = velocidadeBase * fatorSuavizacao * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, destino, passo);
            yield return null;
        }

        transform.position = destino;
    }

    private void CheckWalls()
    {
        _wallIsBlocking = HitLeftWall() || HitRightWall();
    }

    bool HitRightWall()
    {
        return Physics.Raycast(transform.position, Vector3.right, _wallCheckDistance, _wallLayer);
    }

    bool HitLeftWall()
    {
        return Physics.Raycast(transform.position, Vector3.left, _wallCheckDistance, _wallLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.left * _wallCheckDistance);
        Gizmos.DrawRay(transform.position, Vector3.right * _wallCheckDistance);

        if (_waypoints == null || _waypoints.Length == 0) return;

        Vector3 pontoAnterior = Application.isPlaying ? _initialPosition : transform.position;

        for (int i = 0; i < _waypoints.Length; i++)
        {
            if (_waypoints[i] == null) continue;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pontoAnterior, _waypoints[i].position);
            Gizmos.DrawWireSphere(_waypoints[i].position, 0.35f);

            pontoAnterior = _waypoints[i].position;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pontoAnterior, Application.isPlaying ? _initialPosition : transform.position);
    }
}