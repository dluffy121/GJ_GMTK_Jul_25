using UnityEngine;

public class Star : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.OnStarCollect?.Invoke();
            Destroy(gameObject);
        }
    }
}
