using System;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public GameObject laserPrefab;
    public Transform firePoint;
    public float fireRate = 1f;

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
        Destroy(gameObject);
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
        Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
    }
}