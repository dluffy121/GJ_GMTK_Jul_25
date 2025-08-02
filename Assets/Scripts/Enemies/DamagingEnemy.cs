using UnityEngine;

public class DamagingEnemy : EnemiesBase
{
    [SerializeField] float DamageValue;

    protected override void DamagePlayer()
    {
        Player.DecreasePlayerHealth(DamageValue);
        Destroy(gameObject);
    }
}