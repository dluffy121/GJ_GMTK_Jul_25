using UnityEngine;

public class OnMMAsteroidCollide : MonoBehaviour
{
    [SerializeField] GameObject m_gameObject;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Asteroid"))
        {
            Instantiate(m_gameObject, transform.position, transform.rotation);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
