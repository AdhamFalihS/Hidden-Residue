using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HiddenResidue.UI
{
    /// <summary>
    /// MainMenuUI — Controller untuk scene Main Menu.
    /// Tombol: Play → LevelSelect panel, Options → Settings panel, Exit → keluar game.
    ///
    /// Attach ke: GameObject "MainMenuUI" di Canvas.
    ///
    /// Struktur UI:
    ///   Canvas
    ///   └── MainMenuUI  ← script di sini
    ///       ├── MainPanel
    ///       │   ├── PlayButton
    ///       │   ├── OptionsButton
    ///       │   └── ExitButton
    ///       └── LevelSelectPanel
    ///           └── LevelGrid
    ///               ├── LevelButton_1
    ///               ├── LevelButton_2
    ///               └── LevelButton_3 (dst)
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject levelSelectPanel;

        [Header("Level Select")]
        [Tooltip("Drag semua LevelButton ke sini secara berurutan (index 0 = Level 1)")]
        [SerializeField] private Button[]            levelButtons;
        [SerializeField] private TextMeshProUGUI[]   levelButtonTexts;  // opsional, untuk label
        [SerializeField] private GameObject[]        lockIcons;          // opsional, ikon gembok

        [Header("Scene Build Index")]
        [Tooltip("Build index scene level pertama. Biasanya 1 (0 = MainMenu).")]
        [SerializeField] private int firstLevelBuildIndex = 1;

        private void Start()
        {
            if (mainPanel        != null) mainPanel.SetActive(true);
            if (levelSelectPanel != null) levelSelectPanel.SetActive(false);

            // Play BGM Main Menu
            Core.AudioManager.Instance?.PlayBGM(Core.AudioManager.BGM.MainMenu);

            SetupLevelButtons();
        }

        // ─── Tombol Main Menu ─────────────────────────────────────────────────

        public void OnPlayClicked()
        {
            Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.ButtonClick);
            if (mainPanel        != null) mainPanel.SetActive(false);
            if (levelSelectPanel != null) levelSelectPanel.SetActive(true);
            SetupLevelButtons(); // Refresh unlock status setiap kali dibuka
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

            // Di Editor, hentikan Play Mode
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

        // ─── Level Select ─────────────────────────────────────────────────────

        private void SetupLevelButtons()
        {
            if (levelButtons == null) return;

            var progress = Core.LevelProgressManager.Instance;

            for (int i = 0; i < levelButtons.Length; i++)
            {
                if (levelButtons[i] == null) continue;

                int buildIndex  = firstLevelBuildIndex + i;
                bool isUnlocked = progress == null || progress.IsLevelUnlocked(buildIndex);

                // Aktif/nonaktif tombol
                levelButtons[i].interactable = isUnlocked;

                // Ikon gembok
                if (lockIcons != null && i < lockIcons.Length && lockIcons[i] != null)
                    lockIcons[i].SetActive(!isUnlocked);

                // Label tombol (opsional)
                if (levelButtonTexts != null && i < levelButtonTexts.Length && levelButtonTexts[i] != null)
                    levelButtonTexts[i].text = isUnlocked ? $"{i + 1}" : $" {i + 1}";

                // Pasang listener — capture i dengan variabel lokal
                int capturedIndex = buildIndex;
                levelButtons[i].onClick.RemoveAllListeners();
                levelButtons[i].onClick.AddListener(() => LoadLevel(capturedIndex));
            }
        }

        private void LoadLevel(int buildIndex)
        {
            Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.ButtonClick);

            // Reset score sebelum mulai level baru dari menu
            Core.ScoreManager.Instance?.ResetScore();

            Core.SceneLoader.Instance?.LoadScene(buildIndex);
        }
    }
}
