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
    GameObject m_gameObject;

    private void OnEnable()
    {
        StartCoroutine(MoveElement());
    }

    public IEnumerator MoveElement()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            rect.anchoredPosition = Vector2.Lerp(m_startPos, m_endPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
        rect.anchoredPosition = m_endPos;
        GameObject l_asteroidblast = Instantiate(m_gameObject, transform.parent);
        ((RectTransform)l_asteroidblast.transform).anchoredPosition = rect.anchoredPosition;
        gameObject.SetActive(false);
        Destroy(l_asteroidblast, 1);
    }
}