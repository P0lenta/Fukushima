using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform _player;
    public bool _isParkourLevel = false;
    public float _turnSpeed;
    public float _wallCheckDistance = 3f;
    public LayerMask _wallLayer;
    
    private Vector3 _offset;
    private bool _wallIsBlocking = false;


    private void Start() 
    {
        if (_player != null) _offset = transform.position - _player.position;    
    }

    private void LateUpdate() 
    {
        if (_player == null) return;

        CheckWalls();

        Vector3 _cameraPosition;

        if (_isParkourLevel)
        {
            _cameraPosition = new Vector3
            (_player.position.x + _offset.x, _player.position.y + _offset.y, transform.position.z);
        }

        else
        {
            _cameraPosition = new Vector3
            (_player.position.x + _offset.x, transform.position.y, transform.position.z);
        }

        float _moveDirection = _cameraPosition.x - transform.position.x;

        if (_wallIsBlocking)
        {
            if ((_moveDirection > 0 && HitRightWall()) || _moveDirection < 0 && HitLeftWall()) return;
        }

        transform.position = Vector3.Lerp(transform.position, _cameraPosition, _turnSpeed * Time.deltaTime);


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
    }
}
