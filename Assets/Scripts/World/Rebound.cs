using UnityEngine;

public class Rebound : MonoBehaviour
{
    [SerializeField] float _cooldown = 1;

    Player _playerRef;
    float _timer = 0;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!other.TryGetComponent(out _playerRef))
            return;

        if (_timer > 0) return;

        _playerRef.Rebound();
    }

    void Update()
    {
        _timer -= Time.deltaTime;
    }

#if UNITY_EDITOR

    Collider _collider;

    void OnDrawGizmos()
    {
        _collider ??= GetComponent<Collider>();
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        if (_collider is CapsuleCollider cpCollider)
            Gizmos.DrawWireSphere(cpCollider.center, cpCollider.radius);
        else if (_collider is BoxCollider bxCollider)
            Gizmos.DrawWireCube(bxCollider.center, bxCollider.size);
        Gizmos.matrix = Matrix4x4.identity;
    }

#endif
}