using UnityEngine;

namespace GJ_GMTK_Jul_2025
{
    public class UIMainMenu : MonoBehaviour
    {
        public void OnBtnClicked_Start()
        {
            GameManager.LoadGameplaySceneWithLastUnlockedLevel();
        }
        public void OnBtnClicked_Levels()
        {
            GameManager.ShowLevelSelectionUI();
            GameManager.HideMainMenuUI();
        }
        
    }
}