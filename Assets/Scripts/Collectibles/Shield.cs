using UnityEngine;

public class Shield : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Player.IncreasePlayerHealth(Player.PlayerHealth.GetHealthVal());
            Destroy(gameObject);
        }
    }
}
