using UnityEngine;
using TMPro;

namespace HiddenResidue.UI
{
    /// <summary>
    /// LevelCompleteUI — Layar "LEVEL SELESAI" beserta total skor.
    ///
    /// Attach ke: GameObject "LevelCompletePanel" di Canvas.
    ///
    /// Struktur UI:
    ///   LevelCompletePanel
    ///   ├── TitleText          (TextMeshProUGUI — "LEVEL SELESAI!")
    ///   ├── LevelNameText      (TextMeshProUGUI — nama level, opsional)
    ///   ├── TotalScoreText     (TextMeshProUGUI — "Total Skor: 250")
    ///   ├── NextLevelButton    (Button → OnClick: LevelCompleteUI.OnNextLevelClicked)
    ///   └── MainMenuButton     (Button → OnClick: LevelCompleteUI.OnMainMenuClicked)
    /// </summary>
    public class LevelCompleteUI : MonoBehaviour
    {
        public static LevelCompleteUI Instance { get; private set; }

        [Header("Panel")]
        [SerializeField] private GameObject completePanel;

        [Header("Teks")]
        [SerializeField] private TextMeshProUGUI totalScoreText;
        [SerializeField] private TextMeshProUGUI levelNameText;

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            Instance = this;
            if (completePanel) completePanel.SetActive(false);
        }

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>Tampilkan layar level selesai. Dipanggil oleh GameManager.</summary>
        public void Show()
        {
            if (completePanel) completePanel.SetActive(true);

            int score = Core.ScoreManager.Instance?.CurrentScore ?? 0;
            if (totalScoreText) totalScoreText.text = $"Total Skor: {score}";

            string lName = Core.LevelManager.Instance != null ? "" : "";
            if (levelNameText) levelNameText.text = lName;
        }

        /// <summary>Dipanggil tombol "Level Berikutnya".</summary>
        public void OnNextLevelClicked()
        {
            Core.GameManager.Instance?.LoadNextLevel();
        }

        /// <summary>Dipanggil tombol "Menu Utama".</summary>
        public void OnMainMenuClicked()
        {
            Core.GameManager.Instance?.GoToMainMenu();
        }
    }
}
