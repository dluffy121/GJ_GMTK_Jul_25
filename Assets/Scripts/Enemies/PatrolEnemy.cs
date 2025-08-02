using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    [SerializeField] float _distance;

    Vector3 _targetPoint;

    void Update()
    {
        Vector3.MoveTowards(transform.position, _targetPoint, Time.deltaTime);
    }

#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.matrix = Matrix4x4.identity;
    }

#endif
}