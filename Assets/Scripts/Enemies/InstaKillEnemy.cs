using UnityEngine;

public class InstaKillEnemy : EnemiesBase
{
    protected override void DamagePlayer()
    {
        Player.DecreasePlayerHealth(1);
        Destroy(gameObject);
    }
}
