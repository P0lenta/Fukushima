using UnityEngine;
using System;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    public static event Action OnInventoryChanged;

    [Header("Cartões de Acesso")]
    public bool _hasCard1 = false;
    public bool _hasCard2 = false;
    public bool _hasCard3 = false;

    [Header("Fusíveis")]
    public bool _hasFuseX = false;
    public bool _hasFuseY = false;
    public bool _hasFuseZ = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        OnInventoryChanged?.Invoke();
    }

    public void ColetarCartao(int _numeroDoCartao)
    {
        if (_numeroDoCartao == 1) _hasCard1 = true;
        else if (_numeroDoCartao == 2) _hasCard2 = true;
        else if (_numeroDoCartao == 3) _hasCard3 = true;

        OnInventoryChanged?.Invoke(); 
    }

    public void ColetarFusivel(string _tipoFusivel)
    {
        if (_tipoFusivel == "X") _hasFuseX = true;
        else if (_tipoFusivel == "Y") _hasFuseY = true;
        else if (_tipoFusivel == "Z") _hasFuseZ = true;

        OnInventoryChanged?.Invoke();
    }

    public void ConsumirFusivel(string _tipoFusivel)
    {
        if (_tipoFusivel == "X") _hasFuseX = false;
        else if (_tipoFusivel == "Y") _hasFuseY = false;
        else if (_tipoFusivel == "Z") _hasFuseZ = false;

        OnInventoryChanged?.Invoke();
    }
}