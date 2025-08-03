using UnityEngine;

public class DamagingEnemy : EnemiesBase
{
    [SerializeField] float DamageValue;

    [SerializeField] AudioClip _hitSFX;
    [SerializeField] float _hitSFXVol = 1;

    protected override void DamagePlayer()
    {
        PlaySoundNDestroy();
        Player.DecreasePlayerHealth(DamageValue);
        Destroy(gameObject);
    }

    private void PlaySoundNDestroy()
    {
        GameObject _asGO = new();
        AudioSource _as = _asGO.AddComponent<AudioSource>();
        _as.PlayOneShot(_hitSFX, _hitSFXVol);
        Destroy(_asGO, 5);
    }
}