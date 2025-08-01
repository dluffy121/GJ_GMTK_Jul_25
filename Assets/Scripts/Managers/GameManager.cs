using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Player m_player;
    [SerializeField]
    GameSceneManager m_sceneManager;

    static GameManager Instance;

    Player CurrentPlayer;

    public static Action OnPlayerInstantiated;
    public static Action OnPlayerDestroyed;

    const int INDEX__MAINMENU = 0;
    const int INDEX__GAMEPLAY = 1;

    private void Awake()
    {
        Instance = this;
    }
    private void OnDestroy()
    {
        Instance = null;
    }

    public static void LoadMainMenuScene()
    {
        Instance.m_sceneManager.LoadScene(INDEX__MAINMENU);
    }
    public static void LoadGameplayScene()
    {
        Instance.m_sceneManager.LoadScene(INDEX__GAMEPLAY);
    }

    public static void LoadLevel(uint a_levelToLoad)
    {
        LevelManager.LoadLevel(a_levelToLoad);
    }

    public static void ReloadLevel()
    {
        LevelManager.ReloadLevel();
    }

    public static void LoadNextLevel()
    {
        LevelManager.LoadNextLevel();
    }

    public static void InstantiatePlayer(Transform a_transParent)
    {
        Instance.CurrentPlayer = Instantiate(Instance.m_player, a_transParent);
        OnPlayerInstantiated?.Invoke();
    }

    public static void DestroyPlayer()
    {
        Destroy(Instance.CurrentPlayer.gameObject);
        Instance.CurrentPlayer = null;
        OnPlayerDestroyed?.Invoke();
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
