using GJ_GMTK_Jul_2025;
using System;
using System.Collections;
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

    [Header("SFX")]
    [SerializeField] AudioSource _as;
    [SerializeField] AudioClip _shieldUpSFX;
    [SerializeField] float _shieldUpSFXVol = 1;
    [SerializeField] AudioClip _deathSFX;
    [SerializeField] float _deathSFXVol = 1;
    [SerializeField] AudioClip _starCollectSFX;
    [SerializeField] float _starCollectSFXVol = 1;
    [SerializeField] AudioClip _reboundSFX;
    [SerializeField] float _reboundSFXVol = 1;

    static Player Instance;

    bool _isDead = false;

    public static PlayerMovementData PlayerMovData => Instance?.m_playerMovData;
    public static Rigidbody PlayerRigidbody => Instance?.m_rigidbody;
    public static Health PlayerHealth => Instance?.m_playerHealth;

    public static Action OnPlayerDamaged;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (m_playerHealth)
            m_playerHealth.OnHealthZero += OnPlayerDied;
        LevelManager.OnStarCollect += OnStarCollected;
    }

    private void OnDestroy()
    {
        LevelManager.OnStarCollect -= OnStarCollected;
        if (m_playerHealth)
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
        if (_isDead) return;
        m_playerCollider.enabled = false;
        m_controller.StopInputs();
        _deathAnim.gameObject.SetActive(true);
        _deathAnim.Play("Death");
        _as.PlayOneShot(_deathSFX, _deathSFXVol);
        GameManager.DestroyPlayer();
    }

    public static void IncreasePlayerHealth(float a_incHealth)
    {
        Instance.m_playerHealth.IncreaseHealth(a_incHealth);
        if (Instance.m_playerHealth.GetHealthVal() > 1)
        {
            Instance._shieldUp.gameObject.SetActive(true);
            Instance._shieldUp.Play("ShieldUp");
            Instance._as.PlayOneShot(Instance._shieldUpSFX, Instance._shieldUpSFXVol);
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

    private void OnStarCollected()
    {
        _as.PlayOneShot(_starCollectSFX, _starCollectSFXVol);
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

    internal void Teleport(Transform target, float forwardOffset)
    {
        m_controller.Teleport(target, forwardOffset);
    }

    internal void Rebound()
    {
        _as.PlayOneShot(_reboundSFX, _reboundSFXVol);
        m_controller.Rebound();
    }

    #endregion
}
