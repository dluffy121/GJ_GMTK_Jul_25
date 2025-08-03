using UnityEngine;

namespace GJ_GMTK_Jul_2025
{
    public class UIMainMenu : MonoBehaviour
    {
        [SerializeField] int _musicIndex = 0;

        void Start()
        {
            GameManager.StartMusic(_musicIndex);
        }

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