using UnityEngine;

public class InstaKillEnemy : EnemiesBase
{
    [SerializeField] AudioSource _as;
    [SerializeField] AudioClip _hitSFX;
    [SerializeField] float _hitSFXVol = 1;

    protected override void DamagePlayer()
    {
        _as.PlayOneShot(_hitSFX, _hitSFXVol);

        Player.DecreasePlayerHealth(1);
        Destroy(gameObject);
    }
}
