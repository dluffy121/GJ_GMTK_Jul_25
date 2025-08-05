using UnityEngine;

public class Fan : MonoBehaviour
{
    [SerializeField] float _pushStrength;

    Player _playerObjRef;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        other.TryGetComponent(out _playerObjRef);
    }

    void OnTriggerStay(Collider other)
    {
        if (_playerObjRef.gameObject != other.gameObject) return;

        _playerObjRef.ApplyPushEffect(transform.forward, _pushStrength);
    }

    void OnTriggerExit(Collider other)
    {
        _playerObjRef.ApplyPushEffect(Vector3.zero, -1);
        _playerObjRef = null;
    }

#if UNITY_EDITOR

    BoxCollider _collider;

    void OnDrawGizmos()
    {
        _collider ??= GetComponent<BoxCollider>();
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(_collider.center, _collider.size);
        Gizmos.DrawLine(_collider.center + Vector3.back, _collider.center + Vector3.forward);
        Gizmos.DrawSphere(_collider.center + Vector3.forward, 0.1f);
        Gizmos.matrix = Matrix4x4.identity;
    }

#endif
}