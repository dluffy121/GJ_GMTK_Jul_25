using UnityEngine;

namespace GJ_GMTK_Jul_2025
{
    public class UILevelSelection : MonoBehaviour
    {
        public void OnBtnClicked_Level(int a_level)
        {
            GameManager.PlayBtnClickSFX();
            GameManager.LoadGameplaySceneWithLevel(a_level);
        }
        public void OnBtnClicked_Back()
        {
            GameManager.PlayBtnClickSFX();
            GameManager.HideLevelSelectionUI();
            GameManager.ShowMainMenuUI();
        }

    }
}