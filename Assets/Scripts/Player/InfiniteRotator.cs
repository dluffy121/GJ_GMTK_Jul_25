
using UnityEngine;

public class InfiniteRotator : MonoBehaviour
{
    [Header("Loop Settings")]
    public float radius = 0.3f;            // Radius of each circular loop
    public float loopDuration = 4f;      // Seconds per full 360° circle
    public float loopOffset = 1.4142f;   // X‑axis offset: ≈√2→ figure‑8 touch point at origin

    Vector3 _center;
    int _side = +1;           // +1 = loop on +X side; –1 = loop on –X side
    float _angle = 0f;        // cumulative degrees spun in current loop (0…360)
    float _angularSpeed;
    Vector3 _lastPos;

    void Start()
    {
        _angularSpeed = 360f / loopDuration;
        _side = 1;
        _center = new Vector3(_side * loopOffset, 0f, 0f);

        _lastPos = transform.position;
    }

    void Update()
    {
        float delta = _angularSpeed * Time.deltaTime;
        _angle += delta;

        float angle = delta * _side;
        transform.RotateAround(_center, Vector3.up, angle);

        if (_angle >= 360f)
        {
            Debug.LogError(_angle);
            _angle = 0;
            transform.position = _lastPos;
            _side *= -1;
            _center = new Vector3(_side * loopOffset, 0f, 0f);
        }
    }
}
