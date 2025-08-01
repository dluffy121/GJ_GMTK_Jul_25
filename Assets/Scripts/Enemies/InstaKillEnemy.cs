using UnityEngine;

public class InstaKillEnemy : EnemiesBase
{
    protected override void DamagePlayer()
    {
        Player.DecreasePlayerHealth(Player.PlayerHealth.GetHealthVal());
    }
}
