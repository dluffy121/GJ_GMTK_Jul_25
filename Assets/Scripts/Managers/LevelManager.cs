using System;
using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    Levels[] m_levels;
    [SerializeField]
    float m_timeBeforeReloading;

    Levels m_currLevel;
    int m_currentLevelIndex = 0;

    static LevelManager Instance;

    public static Action OnStarCollect;

    public static int GetCurrentLevel() => Instance.m_currentLevelIndex;
    public static void SetCurrentLevelIndex(int a_levelNo) => Instance.m_currentLevelIndex = a_levelNo;
    public static bool IsLastLevel() => (Instance.m_currentLevelIndex == Instance.m_levels.Length - 1);
    public static void SetCurrentLevelAsLastUnlocked()
    {
        if (PlayerPrefs.HasKey(UNLOCKED_LEVEL))
        {
            Instance.m_currentLevelIndex = PlayerPrefs.GetInt(UNLOCKED_LEVEL);
            if (Instance.m_currentLevelIndex + 1 > Instance.m_levels.Length)
                Instance.m_currentLevelIndex = 0;
        }
        else
            Instance.m_currentLevelIndex = 0;
    }

    public const string UNLOCKED_LEVEL = "UnlockedLevel";

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GameManager.OnPlayerDestroyed += OnPlayerDestroyed;
        GameManager.OnPlayerOutOfBounds += OnPlayerOutOfBounds;
        GameManager.OnGameplaySceneLoaded += OnGameplaySceneLoaded;
        OnStarCollect += OnStarCollected;
    }


    private void OnDestroy()
    {
        GameManager.OnPlayerDestroyed -= OnPlayerDestroyed;
        GameManager.OnPlayerOutOfBounds -= OnPlayerOutOfBounds;
        GameManager.OnGameplaySceneLoaded -= OnGameplaySceneLoaded;
        OnStarCollect -= OnStarCollected;
        Instance = null;
    }

    private void OnStarCollected()
    {
        m_currLevel.IncrementStarsCollected();
    }

    private void OnGameplaySceneLoaded()
    {
        LoadLevel();
    }

    private void InstantiateLevel(int a_loadLevel)
    {
        m_currLevel = Instantiate(m_levels[a_loadLevel]);
        if (m_currLevel._UIbossLevel)
        {
            m_currLevel._UIbossLevel.gameObject.SetActive(true);
            StartCoroutine(LerpNHide(m_currLevel._UIbossLevel, m_currLevel._UIFadeDuration));
        }
        GameManager.InstantiatePlayer(m_currLevel.playerPos);
    }

    private IEnumerator LerpNHide(CanvasGroup uIbossLevel, float fadeTime)
    {
        uIbossLevel.alpha = 1;
        float totalTime = fadeTime;
        while (fadeTime > 0)
        {
            uIbossLevel.alpha = Mathf.Lerp(1, 0, fadeTime / totalTime);
            fadeTime -= Time.deltaTime;

            yield return null;
        }

        uIbossLevel.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.05f);

        uIbossLevel.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        uIbossLevel.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);

        uIbossLevel.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.05f);

        uIbossLevel.gameObject.SetActive(false);
    }

    private void LoadLevel()
    {
        InstantiateLevel(m_currentLevelIndex);
    }

    private void OnPlayerDestroyed()
    {
        StartCoroutine(WaitToRetryLevel());
    }
    private void OnPlayerOutOfBounds()
    {
        StartCoroutine(WaitAndDestroyPlayer());
    }

    private void LevelRetry()
    {
        GameManager.ReloadScene();
    }

    IEnumerator WaitAndDestroyPlayer()
    {
        yield return StartCoroutine(WaitToRetryLevel());
        GameManager.DestroyPlayerWithoutCallBack();
    }

    IEnumerator WaitToRetryLevel()
    {
        GameManager.ShowLossUI();
        yield return new WaitForSeconds(m_timeBeforeReloading);
        GameManager.HideLossUI();
        LevelRetry();
    }
    IEnumerator WaitToCompleteLevel()
    {
        GameManager.ShowWinUI();
        Player.PlayerCollider.enabled = false;
        yield return new WaitForSeconds(m_timeBeforeReloading);
        GameManager.HideWinUI();
        if (m_currentLevelIndex < m_levels.Length)
            GameManager.ReloadScene();
        else
            GameManager.LoadMainMenuScene();
    }

    public static void LevelCompletedWithoutTimeWait()
    {
        Instance.m_currentLevelIndex++;
        GameManager.DestroyPlayerWithoutCallBack();
        if (!PlayerPrefs.HasKey(UNLOCKED_LEVEL) || PlayerPrefs.GetInt(UNLOCKED_LEVEL) < Instance.m_currentLevelIndex)
            PlayerPrefs.SetInt(UNLOCKED_LEVEL, Instance.m_currentLevelIndex);
        if (Instance.m_currentLevelIndex < Instance.m_levels.Length)
            GameManager.ReloadScene();
        else
            GameManager.LoadMainMenuScene();
    }

    public static void LevelCompleted()
    {
        Instance.m_currentLevelIndex++;
        Instance.m_currLevel.Complete();
        GameManager.DestroyPlayerWithoutCallBack();
        if (!PlayerPrefs.HasKey(UNLOCKED_LEVEL) || PlayerPrefs.GetInt(UNLOCKED_LEVEL) < Instance.m_currentLevelIndex)
            PlayerPrefs.SetInt(UNLOCKED_LEVEL, Instance.m_currentLevelIndex);
        Instance.StartCoroutine(Instance.WaitToCompleteLevel());
    }

    public static void LevelFailed()
    {

    }
}
