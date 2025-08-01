using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    Levels[] m_levels;

    public Transform PlayerInstantiatePos;

    Levels m_currLevel;
    int m_currentLevelIndex = 1;

    static LevelManager Instance;

    public int GetCurrentLevel() => m_currentLevelIndex;
    public int SetCurrentLevel(int a_levelNo) => m_currentLevelIndex = a_levelNo;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GameManager.OnPlayerDestroyed += OnPlayerDestroyed;
        GameManager.OnGameplaySceneLoaded += OnGameplaySceneLoaded;
    }


    private void OnDestroy()
    {
        GameManager.OnPlayerDestroyed -= OnPlayerDestroyed;
        GameManager.OnGameplaySceneLoaded -= OnGameplaySceneLoaded;
        Instance = null;
    }

    private void OnGameplaySceneLoaded()
    {
        LoadLevel();
    }

    private void InstantiateLevel(int a_loadLevel)
    {
        m_currLevel = Instantiate( m_levels[a_loadLevel-1]);
        GameManager.InstantiatePlayer(m_currLevel.playerPos);
    }
    private void LoadLevel()
    {
        InstantiateLevel(m_currentLevelIndex);
    }

    private void OnPlayerDestroyed()
    {
        LevelRetry();
    }

    public static void LevelFailed()
    {

    }

    public static void LevelRetry()
    {
        GameManager.ReloadScene();
    }

    public static void LevelCompleted()
    {
        Instance.m_currentLevelIndex++;
        GameManager.ReloadScene();
    }
}
