using UnityEngine;

public class Shield : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.IncreasePlayerHealth(Player.PlayerHealth.GetHealthVal());
            Destroy(gameObject);
        }
    }
}
