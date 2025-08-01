using UnityEngine;

public class Star : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if(other.transform.CompareTag("Player"))
        {
            LevelManager.OnStarCollect?.Invoke();
            Destroy(gameObject);
        }
    }
}
