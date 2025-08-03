using UnityEngine;

public class Levels : MonoBehaviour
{
    [SerializeField]
    private Transform m_playerPos;
    [SerializeField]
    int m_starsToCompleteLevel;
    [SerializeField]
    public CanvasGroup _UIbossLevel;
    [SerializeField]
    public float _UIFadeDuration;

    public Transform playerPos => m_playerPos;
    public int StarsToCompleteLevel => m_starsToCompleteLevel;

    private int StarsCollected;

    public int GetStarsCollected() => StarsCollected;

    public void IncrementStarsCollected()
    {
        StarsCollected++;
        if (m_starsToCompleteLevel <= StarsCollected)
        {
            LevelManager.LevelCompleted();
        }
    }

}
