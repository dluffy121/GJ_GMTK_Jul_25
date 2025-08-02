using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public int damage = 1;

    private void Start()
    {
        Destroy(gameObject, lifeTime); // auto destroy after some time
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Example: call player health
            Player.DecreasePlayerHealth(damage);
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy")) // avoid hitting self
        {
            Destroy(gameObject);
        }
    }
}
