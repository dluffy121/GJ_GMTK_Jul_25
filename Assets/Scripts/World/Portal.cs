using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] Portal _out;
    [SerializeField] float _cooldown = 1;

    Player _playerRef;
    float _timer = 0;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!other.TryGetComponent(out _playerRef))
            return;

        if (_timer > 0) return;

        _playerRef.Teleport(_out.transform);
        _timer = _cooldown;
        _out._timer = _cooldown;
    }

    void Update()
    {
        _timer -= Time.deltaTime;
    }

#if UNITY_EDITOR

    CapsuleCollider _capsuleCollider;

    void OnDrawGizmos()
    {
        _capsuleCollider ??= GetComponent<CapsuleCollider>();
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireSphere(_capsuleCollider.center, _capsuleCollider.radius);
        Gizmos.matrix = Matrix4x4.identity;
    }

#endif
}