using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HiddenResidue.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject levelSelectPanel;

        [Header("Level Select")]
        [Tooltip("Drag semua LevelButton ke sini secara berurutan (index 0 = Level 1)")]
        [SerializeField] private Button[]            levelButtons;
        [SerializeField] private TextMeshProUGUI[]   levelButtonTexts;  
        [SerializeField] private GameObject[]        lockIcons;        

        [Header("Scene Build Index")]
        [Tooltip("Build index scene level pertama. Biasanya 1 (0 = MainMenu).")]
        [SerializeField] private int firstLevelBuildIndex = 1;

        private void Start()
        {
            if (mainPanel        != null) mainPanel.SetActive(true);
            if (levelSelectPanel != null) levelSelectPanel.SetActive(false);

            Core.AudioManager.Instance?.PlayBGM(Core.AudioManager.BGM.MainMenu);

            SetupLevelButtons();
        }



        public void OnPlayClicked()
        {
            Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.ButtonClick);
            if (mainPanel        != null) mainPanel.SetActive(false);
            if (levelSelectPanel != null) levelSelectPanel.SetActive(true);
            SetupLevelButtons(); 
        }

        public void OnOptionsClicked()
        {
            Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.ButtonClick);
            SettingsUI.Instance?.OpenSettings();
        }

        public void OnExitClicked()
        {
            Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.ButtonClick);
            Debug.Log("[MainMenu] Exit game.");
            Application.Quit();


#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public void OnBackToMainClicked()
        {
            Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.ButtonClick);
            if (mainPanel        != null) mainPanel.SetActive(true);
            if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        }


        private void SetupLevelButtons()
        {
            if (levelButtons == null) return;

            var progress = Core.LevelProgressManager.Instance;

            for (int i = 0; i < levelButtons.Length; i++)
            {
                if (levelButtons[i] == null) continue;

                int buildIndex  = firstLevelBuildIndex + i;
                bool isUnlocked = progress == null || progress.IsLevelUnlocked(buildIndex);

                levelButtons[i].interactable = isUnlocked;

                if (lockIcons != null && i < lockIcons.Length && lockIcons[i] != null)
                    lockIcons[i].SetActive(!isUnlocked);

                if (levelButtonTexts != null && i < levelButtonTexts.Length && levelButtonTexts[i] != null)
                    levelButtonTexts[i].text = isUnlocked ? $"{i + 1}" : $" {i + 1}";

                int capturedIndex = buildIndex;
                levelButtons[i].onClick.RemoveAllListeners();
                levelButtons[i].onClick.AddListener(() => LoadLevel(capturedIndex));
            }
        }

        private void LoadLevel(int buildIndex)
        {
            Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.ButtonClick);

            Core.ScoreManager.Instance?.ResetScore();

            Core.SceneLoader.Instance?.LoadScene(buildIndex);
        }
    }
}
