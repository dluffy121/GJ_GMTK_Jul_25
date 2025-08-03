using System.Collections;
using UnityEngine;

public class RandomPlayAsteroids : MonoBehaviour
{
    [SerializeField]
    GameObject[] m_asteroidPairs;
    [SerializeField]
    float m_waitForTimeBeforePlay;

    private void Start()
    {
        StartCoroutine(PlayAsteroidMovement());

    }

    IEnumerator PlayAsteroidMovement()
    {
        yield return new WaitForSeconds(m_waitForTimeBeforePlay);
        while (true) 
        {
            int l_randomNo = Random.Range(0, m_asteroidPairs.Length - 1);
            m_asteroidPairs[l_randomNo].SetActive(true);
            for (int l_index = 0; l_index < m_asteroidPairs[l_randomNo].transform.childCount; l_index++)
            {
                m_asteroidPairs[l_randomNo].transform.GetChild(l_index).gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(5);
        }
    }
}
