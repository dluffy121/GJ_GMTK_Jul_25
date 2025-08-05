using System;
using GJ_GMTK_Jul_2025;
using UnityEngine;

public class Vacuum : MonoBehaviour
{
    [SerializeField] float _pullStrength;

    PlayerController _playerObjRef;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        other.TryGetComponent(out _playerObjRef);
    }

    void OnTriggerStay(Collider other)
    {
        if (_playerObjRef.gameObject != other.gameObject) return;

        _playerObjRef.ApplyPullEffect(transform.position, _pullStrength);
    }

    void OnTriggerExit(Collider other)
    {
        _playerObjRef.ApplyPullEffect(transform.position, 0);
        _playerObjRef = null;
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