using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    Player m_player;
    [SerializeField]
    GameSceneManager m_sceneManager;
    [SerializeField]
    UIManager m_uiManager;
    [SerializeField] AudioSource _sfxSource;
    [SerializeField] AudioClip[] _btnClickSFX;

    static GameManager Instance;

    Player CurrentPlayer;

    public static Action OnPlayerInstantiated;
    public static Action OnPlayerDestroyed;
    public static Action OnPlayerOutOfBounds;
    public static Action OnMainMenuSceneLoaded;
    public static Action OnMainMenuSceneUnloaded;
    public static Action OnGameplaySceneLoaded;
    public static Action OnGameplaySceneUnloaded;

    const int INDEX__MAINMENU = 1;
    const int INDEX__GAMEPLAY = 2;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Application.targetFrameRate = -1;
        LoadMainMenuScene();
    }
    private void OnDestroy()
    {
        Instance = null;
    }

    #region Scene Changes
    public static void LoadMainMenuScene()
    {
        Instance.m_sceneManager.LoadScene(INDEX__MAINMENU);
    }
    public static void LoadGameplayScene()
    {
        Instance.m_sceneManager.LoadScene(INDEX__GAMEPLAY);
    }

    public static void LoadGameplaySceneWithLevel(int a_levelNo)
    {
        LevelManager.SetCurrentLevelIndex(a_levelNo);
        Instance.m_sceneManager.LoadScene(INDEX__GAMEPLAY);
    }

    public static void LoadGameplaySceneWithLastUnlockedLevel()
    {
        LevelManager.SetCurrentLevelAsLastUnlocked();
        Instance.m_sceneManager.LoadScene(INDEX__GAMEPLAY);
    }

    public static void ReloadScene()
    {
        if (Instance.m_sceneManager.IsSceneReLoading) return;
        Instance.m_sceneManager.ReloadScene();
    }
    #endregion

    #region Player Related Methods
    public static void InstantiatePlayer(Transform a_transParent)
    {
        Instance.CurrentPlayer = Instantiate(Instance.m_player, a_transParent);
        OnPlayerInstantiated?.Invoke();
    }

    public static void PlayerOutOfBounds()
    {
        OnPlayerOutOfBounds?.Invoke();
    }

    public static void DestroyPlayer()
    {
        DestroyPlayerWithoutCallBack();
        OnPlayerDestroyed?.Invoke();
    }

    public static void DestroyPlayerWithoutCallBack()
    {
        // Destroy(Instance.CurrentPlayer.gameObject);
        Instance.CurrentPlayer = null;
    }
    #endregion

    #region UIRelated
    public static void ShowWinUI()
    {
        Instance.m_uiManager.ShowWinUI();
    }
    public static void HideWinUI()
    {
        Instance.m_uiManager.HideWinUI();
    }
    public static void ShowLossUI()
    {
        Instance.m_uiManager.ShowLossUI();
    }
    public static void HideLossUI()
    {
        Instance.m_uiManager.HideLossUI();
    }
    public static void ShowMainMenuUI()
    {
        Instance.m_uiManager.ShowMainMenuUI();
    }
    public static void HideMainMenuUI()
    {
        Instance.m_uiManager.HideMainMenuUI();
    }
    public static void ShowLevelSelectionUI()
    {
        Instance.m_uiManager.ShowLevelSelectionUI();
    }
    public static void HideLevelSelectionUI()
    {
        Instance.m_uiManager.HideLevelSelectionUI();
    }
    #endregion

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #region Sound

    [SerializeField] AudioClip[] m_clips;
    [SerializeField] float[] m_volumes;

    private AudioSource m_currentAS;
    private int m_currentClip = -1;

    public static void StartMusic(int index)
    {
        Instance.Int_StartMusic(index);
    }

    public static void PlayBtnClickSFX()
    {
        Instance._sfxSource.PlayOneShot(Instance._btnClickSFX[UnityEngine.Random.Range(0, Instance._btnClickSFX.Length)]);
    }

    private void Int_StartMusic(int index)
    {
        m_currentAS ??= GetComponent<AudioSource>();

        if (m_currentClip != index)
        {
            m_currentAS.Stop();
            m_currentAS.clip = m_clips[index];
            m_currentAS.Play();
        }

        m_currentAS.volume = m_volumes[index];
        m_currentClip = index;
    }

    #endregion
}
