using UnityEngine;

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
    public float _initialMoveSpeed;

    private enum _eixo { None, X, Y, Z }

    private _eixo _eixoActual = _eixo.None;
    private _eixo _eixoDesired = _eixo.None;

    private Transform[] _waypointCurrent;
    private int _currentTarget = 0;

    private void Start() 
    {
        _initialMoveSpeed = _moveSpeed;
        UpdateCurrentDirection();
    }

    private void Update() 
    {
        UpdateCurrentDirection();
        MovePlatform();
    }

    private void UpdateCurrentDirection()
    {
        if (_moveToX) _eixoDesired = _eixo.X;
        else if (_moveToY) _eixoDesired = _eixo.Y;
        else if (_moveToZ) _eixoDesired = _eixo.Z;
        else _eixoDesired = _eixo.None;

        if (_moveToX) {_moveToY = false; _moveToZ = false;}
        if (_moveToY) {_moveToX = false; _moveToZ = false;}
        if (_moveToZ) {_moveToX = false; _moveToY = false;}
    }

    private void MovePlatform()
    {
        if (_eixoActual == _eixo.None && _eixoDesired != _eixo.None)
        {
            StartCycle(_eixoDesired);
        }

            if (_eixoActual != _eixo.None && _waypointCurrent != null && _waypointCurrent.Length >= 2)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, _waypointCurrent[_currentTarget].position, _moveSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, _waypointCurrent[_currentTarget].position) <= 0.01f)
                {
                    if (_currentTarget == 1)
                    {
                        _currentTarget = 0;
                    }
                    else if (_currentTarget == 0)
                    {
                        if (_eixoDesired != _eixoActual)
                        {
                            _eixoActual = _eixo.None;
                        }
                        else
                        {
                            StartCycle(_eixoDesired);
                        }
                    }
                    else
                    {
                        _currentTarget = 1;
                    }
                }        
            }
        
    }

    private void StartCycle(_eixo _newEixo)
    {
        _eixoActual = _newEixo;

        if (_eixoActual == _eixo.X) _waypointCurrent = _waypoipointX;
        else if (_eixoActual == _eixo.Y) _waypointCurrent = _waypoipointY;
        else if (_eixoActual == _eixo.Z) _waypointCurrent = _waypoipointZ;

        _currentTarget = 1;
    }

}
