using System.Collections;
using UnityEngine;

public class EnemyLaserBeam : MonoBehaviour
{
    [SerializeField]
    Transform m_enemyObject;
    public Transform firePoint;         // Starting point of laser
    public Transform beamTransform;     // Empty GameObject with SpriteRenderer
    public SpriteRenderer beamSprite;   // The laser sprite
    public float maxDistance = 50f;
    public int damagePerSecond = 10;
    public LayerMask hitMask;
    [SerializeField]
    GameObject m_aboutToLaunch;

    [Header("Firing Pattern")]
    public float fireDuration = 4f;
    public float restDuration = 1f;

    private float timer;
    private bool isFiring;
    private float firingTime;

    private Vector3 initialScale;
    private Quaternion initialRot;

    public float amplitude = 1f;       // Max height deviation
    public float speed = 2f;           // Speed of pendulum swing
    public float moveDuration = 3f;    // How long the pendulum effect lasts

    int setEnemyPower = 0;

    void Start()
    {
        if (beamSprite != null)
            beamSprite.enabled = false;

        if (beamTransform == null)
            beamTransform = beamSprite.transform;

        initialRot = transform.rotation;
        initialScale = beamTransform.localScale;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (isFiring && timer > fireDuration)
        {
            isFiring = false;
            timer = 0f;
            beamSprite.enabled = false;
        }
        else if (!isFiring && timer > restDuration)
        {
            isFiring = true;
            timer = 0f;
            beamSprite.enabled = true;
            setEnemyPower = Random.Range(0, 2);
        }

        if (isFiring)
        {
            if (m_aboutToLaunch.activeSelf)
                m_aboutToLaunch.SetActive(false);
            FireLaser();
            //if(setEnemyPower == 1)
            //    StartCoroutine(DoPendulum());
        }
        else
        {
            if (!m_aboutToLaunch.activeSelf)
                m_aboutToLaunch.SetActive(true);
        }
    }

    void PendulumRotation()
    {
        if (timer < moveDuration)
        {
            float yOffset = Mathf.Sin(Time.time * speed) * amplitude;
            transform.rotation = initialRot * Quaternion.Euler(0, yOffset, 0);
        }
    }
    IEnumerator DoPendulum()
    {
        Quaternion initialRot = m_enemyObject.localRotation;

        float l_timer = 0f;
        while (l_timer < moveDuration)
        {
            float normalized = timer / moveDuration;
            float angle = Mathf.Sin(normalized * Mathf.PI) * amplitude;
            m_enemyObject.localRotation = initialRot * Quaternion.Euler(0f, angle, 0f);

            timer += Time.deltaTime;
            yield return null;
        }

        // ensure exact reset
        m_enemyObject.localRotation = initialRot;
    }

    void FireLaser()
    {
        Vector3 start = firePoint.position;
        Vector3 direction = firePoint.forward; // laser direction = +X axis

        float distance = maxDistance;
        if (Physics.Raycast(start, direction, out RaycastHit hitInfo, maxDistance, hitMask))
        {
            distance = hitInfo.distance;

            if (hitInfo.collider.CompareTag("Player"))
            {
                Player.DecreasePlayerHealth(1);
            }
        }
        float t = Mathf.Clamp01(timer / 2f);
        float currentLength = Mathf.Lerp(0, distance, t);

        // Apply scaling
        beamTransform.localScale = new Vector3(currentLength, initialScale.y, initialScale.z);
    }
}