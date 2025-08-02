using UnityEngine;

public class Shield : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.IncreasePlayerHealth(1);
            Destroy(gameObject);
        }
    }
}
