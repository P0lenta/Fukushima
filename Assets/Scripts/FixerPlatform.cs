using System.Collections.Generic;
using UnityEngine;

public class FixerPlatform : MonoBehaviour
{


    private void OnTriggerEnter(Collider _other)
    {
        Transform _playerTransform = ObterTransformPlayer(_other);

        if(_playerTransform != null)
        {
            _playerTransform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        Transform _playerTransform = ObterTransformPlayer(_other);

        if(_playerTransform != null)
        {
            _playerTransform.SetParent(null);
        }
    }

    private Transform ObterTransformPlayer(Collider _col)
    {
        if (_col.CompareTag("Player")) return _col.transform;
        
        Transform _parent = _col.transform.GetComponentInParent<Transform>();

        if (_parent != null && _parent.CompareTag("Player")) return _parent;
        
        return null;
    }


}
