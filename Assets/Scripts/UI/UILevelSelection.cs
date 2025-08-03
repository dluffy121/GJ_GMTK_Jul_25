using System;
using UnityEngine;
using UnityEngine.UI;

namespace GJ_GMTK_Jul_2025
{
    public class UILevelSelection : MonoBehaviour
    {
        [SerializeField]
        LevelsUI[] m_goLevels;

        private void Start()
        {
            for (int index = 0; index < m_goLevels.Length; index++)
            {
                if (PlayerPrefs.HasKey(LevelManager.UNLOCKED_LEVEL))
                {
                    if (index <= PlayerPrefs.GetInt(LevelManager.UNLOCKED_LEVEL))
                    {
                        m_goLevels[index].m_button.interactable = true;
                        m_goLevels[index].m_image.SetActive(false);
                    }
                }
            }
        }

        GameObject levelText;
        GameObject player;

        void OnEnable()
        {
            levelText ??= GameObject.Find("LevelText");
            player ??= GameObject.Find("Player MainMenu");

            levelText.SetActive(false);
            player.SetActive(false);
        }

        void OnDisable()
        {
            if(levelText)
                levelText.SetActive(true);
            if(player)
                player.SetActive(true);
        }

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

    [Serializable]
    public struct LevelsUI
    {
        public Button m_button;
        public GameObject m_image;
    }
}