using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaypointPlatform : MonoBehaviour
{
    [Header("Controle de fusíveis")]
    public bool _moveToX;
    public bool _moveToY;
    public bool _moveToZ;

    [Header("Waypoints")]
    public Transform[] _waypoipointX;
    public Transform[] _waypoipointY;
    public Transform[] _waypoipointZ;

    [Header("Configurações")]
    public float _moveSpeed = 5f;

    private Rigidbody _rb;
    private Vector3 _posicaoOriginal;
    private Transform[] _rotaAtual;
    private Transform[] _rotaAlvo;
    private int _indiceAtual = 0;
    private bool _retornandoParaBase = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        _rb.isKinematic = true; 
        
        _posicaoOriginal = _rb.position;
    }

    private void FixedUpdate()
    {
        ChecarMudancaDeRota();
        ExecutarMovimento();
    }

    private void ChecarMudancaDeRota()
    {
        Transform[] _novaRota = null;

        if (_moveToX) _novaRota = _waypoipointX;
        else if (_moveToY) _novaRota = _waypoipointY;
        else if (_moveToZ) _novaRota = _waypoipointZ;

        if (_rotaAlvo != _novaRota)
        {
            _rotaAlvo = _novaRota;
            _retornandoParaBase = true;
        }
    }

    private void ExecutarMovimento()
    {
        if (_retornandoParaBase)
        {
            Vector3 _novaPos = Vector3.MoveTowards(_rb.position, _posicaoOriginal, _moveSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(_novaPos);

            if (Vector3.Distance(_rb.position, _posicaoOriginal) <= 0.01f)
            {
                _retornandoParaBase = false;
                _rotaAtual = _rotaAlvo;
                _indiceAtual = 0;
            }
        }
        else if (_rotaAtual != null && _rotaAtual.Length > 0)
        {
            Transform _alvo = _rotaAtual[_indiceAtual];

            if (_alvo != null)
            {
                Vector3 _novaPos = Vector3.MoveTowards(_rb.position, _alvo.position, _moveSpeed * Time.fixedDeltaTime);
                _rb.MovePosition(_novaPos);

                if (Vector3.Distance(_rb.position, _alvo.position) <= 0.01f)
                {
                    _indiceAtual++;
                    if (_indiceAtual >= _rotaAtual.Length)
                    {
                        _indiceAtual = 0;
                    }
                }
            }
        }
    }
}