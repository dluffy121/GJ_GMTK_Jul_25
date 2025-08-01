using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform PlayerInstantiatePos;

    uint m_currentLevel;

    static LevelManager Instance;

    public uint GetCurrentLevel() => m_currentLevel;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GameManager.InstantiatePlayer(PlayerInstantiatePos);
        GameManager.OnPlayerDestroyed += OnPlayerDestroyed;
    }
    private void OnDestroy()
    {
        GameManager.OnPlayerDestroyed -= OnPlayerDestroyed;
        Instance = null;
    }

    private void OnPlayerDestroyed()
    {
        LevelFailed();
    }

    public static void LoadNextLevel()
    {
        Instance.m_currentLevel++;
    }

    public static void ReloadLevel()
    {

    }

    public static void LoadLevel(uint a_loadLevel)
    {

    }

    public static void LevelFailed()
    {

    }

    public static void LevelRetry()
    {
        ReloadLevel();
    }

    public static void LevelCompleted()
    {
        LoadNextLevel();
    }
}
