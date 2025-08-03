using System;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public GameObject laserPrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    [SerializeField] AudioSource _as;
    [SerializeField] AudioClip _hitSFX;
    [SerializeField] float _hitSFXVol = 1;

    private float nextFireTime;

    private void Start()
    {
        GameManager.OnPlayerDestroyed += OnPlayerDied;
    }
    private void OnDestroy()
    {
        GameManager.OnPlayerDestroyed += OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        //if(gameObject != null)
        //    Destroy(gameObject);
    }

    private void Update()
    {
        if (Time.time >= nextFireTime)
        {
            ShootLaser();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void ShootLaser()
    {
        _as.PlayOneShot(_hitSFX, _hitSFXVol);
        Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
    }
}