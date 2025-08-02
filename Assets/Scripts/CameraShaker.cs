using UnityEngine;
using System.Collections;
using UnityEditor.Rendering.LookDev;

public class CameraShaker : MonoBehaviour
{
    private Vector3 originalPos;
    private Coroutine shakeRoutine;

    void Awake()
    {
        originalPos = transform.localPosition;
    }

    private void Start()
    {
        Player.OnPlayerDamaged += CameraShake;
    }

    private void OnDestroy()
    {
        Player.OnPlayerDamaged += CameraShake;
    }

    private void CameraShake()
    {
        Shake(1,10);
    }

    /// <summary>
    /// Shakes the camera for a given duration and strength.
    /// </summary>
    /// <param name="duration">How long to shake (seconds)</param>
    /// <param name="magnitude">How strong the shake is</param>
    public void Shake(float duration, float magnitude)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            yield return null;
        }

        // Reset camera to original position
        transform.localPosition = originalPos;
        shakeRoutine = null;
    }
}