using System.Collections.Generic;
using UnityEngine;

public class KeyHolder : MonoBehaviour
{
    public static KeyHolder Instance { get; private set; }

    private HashSet<string> _keys = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
          Destroy(gameObject);
          return;  
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddKey(string _key)
    {
        if (!_keys.Contains(_key)) _keys.Add(_key);
    }

    public void RemoveKey(string _key)
    {
        if (_keys.Contains(_key)) _keys.Remove(_key);
    }

    public bool HasKey(string _key) 
    {
        return _keys.Contains(_key);
    }

    public void ResetAllKeys() 
    {
        _keys.Clear();
    }

}
