using UnityEngine;

public class EnemiesBase : MonoBehaviour
{
    [SerializeField]
    EEnemy m_enemyType;

    private void OnCollisionEnter(Collision other)
    {
        OnCollided(other);
    }

    protected virtual void OnCollided(Collision other)
    {
        if (!other.collider.CompareTag("Player"))
            return;
        DamagePlayer();
    }

    protected virtual void DamagePlayer()
    {
        Player.DecreasePlayerHealth(1);
    }
}
