using GJ_GMTK_Jul_2025;
using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Serialize Fields
    [SerializeField]
    private Transform m_parentCanvas;
    [SerializeField]
    private GameObject m_UIMainMenu;
    [SerializeField]
    private GameObject m_UILevelSelection;
    [SerializeField]
    private GameObject m_UIWin;
    [SerializeField]
    private GameObject m_UILoss;
    #endregion

    #region Private Fields
    private GameObject UIMainMenu;
    private GameObject UILevelSelection;
    private GameObject UIWin;
    private GameObject UILoss;
    #endregion

    #region Unity Methods
    private void Start()
    {
        GameManager.OnMainMenuSceneLoaded += OnMainMenuSceneLoaded;
        GameManager.OnMainMenuSceneUnloaded += OnMainMenuSceneUnloaded;
        GameManager.OnGameplaySceneLoaded += OnGameplaySceneLoaded;
        GameManager.OnGameplaySceneUnloaded += OnGameplaySceneLoaded;
    }
    private void OnDestroy()
    {
        GameManager.OnMainMenuSceneLoaded -= OnMainMenuSceneLoaded;
        GameManager.OnMainMenuSceneUnloaded -= OnMainMenuSceneUnloaded;
        GameManager.OnGameplaySceneLoaded -= OnGameplaySceneLoaded;
        GameManager.OnGameplaySceneUnloaded -= OnGameplaySceneUnloaded;
    }
    #endregion

    #region Private Methods
    private void OnGameplaySceneLoaded()
    {
        if (!UIWin)
        {
            UIWin = Instantiate(m_UIWin, m_parentCanvas);
            UILoss = Instantiate(m_UILoss, m_parentCanvas);
        }
    }
    private void OnGameplaySceneUnloaded()
    {
        Destroy(UIWin);
        Destroy(UILoss);
    }

    private void OnMainMenuSceneLoaded()
    {
        UIMainMenu = Instantiate(m_UIMainMenu, m_parentCanvas);
        UILevelSelection = Instantiate(m_UILevelSelection, m_parentCanvas);
    }
    private void OnMainMenuSceneUnloaded()
    {
        Destroy(UIMainMenu);
        Destroy(UILevelSelection);
    }
    #endregion

    #region Public Methods
    public void ShowMainMenuUI()
    {
        UIMainMenu.SetActive(true);
    }
    public void HideMainMenuUI()
    {
        UIMainMenu.SetActive(false);
    }
    public void ShowLevelSelectionUI()
    {
        UILevelSelection.SetActive(true);
    }
    public void HideLevelSelectionUI()
    {
        UILevelSelection.SetActive(false);
    }

    public void ShowWinUI()
    {
        UIWin.SetActive(true);
    }
    public void HideWinUI()
    {
        UIWin.SetActive(false);
    }
    public void ShowLossUI()
    {
        UILoss.SetActive(true);
    }
    public void HideLossUI()
    {
        UILoss.SetActive(false);
    }
    #endregion
}
