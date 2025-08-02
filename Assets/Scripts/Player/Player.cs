using GJ_GMTK_Jul_2025;
using Mono.Cecil;
using System;
using System.Collections;
using Unity.VisualScripting;
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
    [SerializeField]
    BoxCollider m_playerCollider;
    [SerializeField]
    float m_timeToMakePlayerBeAttacked;

    [Header("Animations")]
    [SerializeField] Animator _deathAnim;
    [SerializeField] Animator _shieldUp;

    static Player Instance;

    public static PlayerMovementData PlayerMovData => Instance.m_playerMovData;
    public static Rigidbody PlayerRigidbody => Instance.m_rigidbody;
    public static Health PlayerHealth => Instance.m_playerHealth;

    public static Action OnPlayerDamaged;

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

    IEnumerator TurnOffPlayerCOllider()
    {
        m_playerCollider.enabled = false;
        yield return new WaitForSeconds(m_timeToMakePlayerBeAttacked);
        m_playerCollider.enabled = true;
    }

    private void OnPlayerDied()
    {
        GameManager.DestroyPlayer();
        m_controller.StopInputs();
        _deathAnim.gameObject.SetActive(true);
        _deathAnim.Play("Death");
    }

    public static void IncreasePlayerHealth(float a_incHealth)
    {
        Instance.m_playerHealth.IncreaseHealth(a_incHealth);
        if (Instance.m_playerHealth.GetHealthVal() == 2)
        {
            Instance._shieldUp.gameObject.SetActive(true);
            Instance._shieldUp.Play("ShieldUp");
        }
    }
    public static void IncreasePlayerHealthMultiplier(float a_incHealthMultiplier)
    {
        Instance.m_playerHealth.IncreaseHealthMultiplier(a_incHealthMultiplier);
    }
    public static void DecreasePlayerHealth(float a_decHealth)
    {
        Instance.m_playerHealth.DecreaseHealth(a_decHealth);
        if (Instance.m_playerHealth.GetHealthVal() >= 1)
        {
            Instance._shieldUp.StopPlayback();
            Instance._shieldUp.gameObject.SetActive(false);
        }
        OnPlayerDamaged?.Invoke();
        if (Instance.m_playerHealth.GetHealthVal() > 0)
            Instance.StartCoroutine(Instance.TurnOffPlayerCOllider());
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
        GameManager.PlayerOutOfBounds();
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

    internal void Rebound()
    {
        m_controller.Rebound();
    }

    #endregion
}
