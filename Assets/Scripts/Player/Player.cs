using GJ_GMTK_Jul_2025;
using Mono.Cecil;
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    PlayerController m_controller;
    [SerializeField]
    Health m_playerHealth;
    [SerializeField]
    PlayerMovementData m_playerMovData;
    [SerializeField]
    Rigidbody m_rigidbody;

    static Player Instance;

    public static PlayerMovementData PlayerMovData => Instance.m_playerMovData;
    public static Rigidbody PlayerRigidbody => Instance.m_rigidbody;
    public static Health PlayerHealth => Instance.m_playerHealth;


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        m_playerHealth.OnHealthZero += OnPlayerDied;
    }
    private void OnDestroy()
    {
        m_playerHealth.OnHealthZero -= OnPlayerDied;
        Instance = null;
    }


    private void OnPlayerDied()
    {
        GameManager.DestroyPlayer();
    }


    public static void IncreasePlayerHealth(float a_incHealth)
    {
        Instance.m_playerHealth.IncreaseHealth(a_incHealth);
    }
    public static void IncreasePlayerHealthMultiplier(float a_incHealthMultiplier)
    {
        Instance.m_playerHealth.IncreaseHealthMultiplier(a_incHealthMultiplier);
    }
    public static void DecreasePlayerHealth(float a_decHealth)
    {
        Instance.m_playerHealth.DecreaseHealth(a_decHealth);
    }
    public static void IncreasePlayerSpeed(float a_incSpeed)
    {

    }
    public static void DecreasePlayerSpeed(float a_decSpeed)
    {

    }
    public static void DecreaseRadius(float a_decRadius)
    {

    }
    public static void IncreaseRadius(float a_incRadius)
    {

    }

    public static void OnPickupPowerup(EPowerup a_powerupType, float a_value)
    {
        switch (a_powerupType)
        {
            case EPowerup.IncreaseSpeed:
                break;
            case EPowerup.DecreaseRadius:
                break;
            case EPowerup.IncreaseHealth:
                IncreasePlayerHealth(a_value);
                break;
            case EPowerup.IncreaseHealthMul:
                IncreasePlayerHealthMultiplier(a_value);
                break;
            default:
                break;
        }
    }

    public static void OnPickupDebuff(EDubuff a_debuffType, float a_value)
    {
        switch (a_debuffType)
        {
            case EDubuff.DecreaseSpeed:
                break;
            case EDubuff.IncreaseRadius:
                break;
            case EDubuff.DecreaseHealth:
                DecreasePlayerHealth(a_value);
                break;
            default:
                break;
        }
    }

    #region World Interactions

    internal void StopInputs()
    {
        m_controller.StopInputs();
    }

    internal void ApplyPullEffect(Vector3 position, float pullStrength)
    {
        m_controller.ApplyPullEffect(position, pullStrength);
    }

    internal void ApplyPushEffect(Vector3 direction, float pushStrength)
    {
        m_controller.ApplyPushEffect(direction, pushStrength);
    }

    internal void Teleport(Transform target)
    {
        m_controller.Teleport(target);
    }

    #endregion
}
