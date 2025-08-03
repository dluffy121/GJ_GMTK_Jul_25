using UnityEngine;
using System.Collections;

public class Move2DElement : MonoBehaviour
{
    [SerializeField]
    RectTransform rect;
    [SerializeField]
    Vector2 m_startPos;
    [SerializeField]
    Vector2 m_endPos;
    [SerializeField]
    float duration;
    [SerializeField]
    float m_waitForTimeBeforePlay;

    private void Start()
    {
        StartCoroutine(MoveElement());
    }

    public IEnumerator MoveElement()
    {
        yield return new WaitForSeconds(m_waitForTimeBeforePlay);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            rect.anchoredPosition = Vector2.Lerp(m_startPos, m_endPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
        rect.anchoredPosition = m_endPos;
        Destroy(gameObject);
    }
}