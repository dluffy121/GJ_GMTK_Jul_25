using UnityEngine;

public class WorldBorder : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Player player))
            return;

        player.StopInputs();
    }

#if UNITY_EDITOR

    BoxCollider _boxCollider;

    void OnDrawGizmos()
    {
        _boxCollider ??= GetComponent<BoxCollider>();
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(_boxCollider.center, _boxCollider.size);
        Gizmos.matrix = Matrix4x4.identity;
    }

#endif
}
