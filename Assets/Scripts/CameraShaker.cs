using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{
    public float shakeDuration = 0.5f; // How long the shake lasts
    public float shakeMagnitude = 0.1f; // How strong the shake is
    public float dampingSpeed = 1.0f; // How quickly the shake fades out

    private Vector3 originalPos;
    private bool isShaking = false;

    void Awake()
    {
        originalPos = transform.localPosition;
        Player.OnPlayerDamaged += TriggerShake;
    }

    private void OnDestroy()
    {
        Player.OnPlayerDamaged -= TriggerShake;
    }

    public void TriggerShake()
    {
        if (!isShaking)
        {
            StartCoroutine(Shake());
        }
    }

    private IEnumerator Shake()
    {
        isShaking = true;
        float currentShakeDuration = shakeDuration;

        while (currentShakeDuration > 0)
        {
            // Calculate a random offset within a unit sphere, scaled by magnitude
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;

            // Apply the offset only to X and Y for a typical 2D orthographic shake
            transform.localPosition = originalPos + new Vector3(randomOffset.x, randomOffset.y, 0f);

            // Reduce shake magnitude over time for damping
            shakeMagnitude = Mathf.Lerp(shakeMagnitude, 0, Time.deltaTime * dampingSpeed);

            currentShakeDuration -= Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Reset camera position and magnitude after shake
        transform.localPosition = originalPos;
        isShaking = false;
    }
}