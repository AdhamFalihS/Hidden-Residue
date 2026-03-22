using UnityEngine;
using TMPro;

namespace HiddenResidue.UI
{
    /// <summary>
    /// LevelCompleteUI — Panel "Level Selesai".
    /// Dipanggil oleh GameManager.TriggerLevelComplete().
    ///
    /// Attach ke: GameObject "LevelCompleteUI" di Canvas (per-scene, bukan DontDestroyOnLoad).
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
            // Guard: jika sudah ada instance lain, hancurkan duplikat
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            // JANGAN DontDestroyOnLoad — UI ini spesifik per scene

            if (completePanel == null)
                Debug.LogError("[LevelCompleteUI] completePanel belum di-assign di Inspector!");

            if (completePanel) completePanel.SetActive(false);
        }

        private void OnDestroy()
        {
            // Bersihkan Instance saat scene di-unload agar tidak null reference
            if (Instance == this) Instance = null;
        }

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>Tampilkan panel level selesai. Dipanggil oleh GameManager.</summary>
        public void Show()
        {
            if (completePanel == null)
            {
                Debug.LogError("[LevelCompleteUI] completePanel NULL! Assign di Inspector.");
                return;
            }

            completePanel.SetActive(true);

            // Tampilkan skor akhir
            int score = Core.ScoreManager.Instance?.CurrentScore ?? 0;
            if (totalScoreText) totalScoreText.text = $"Total Skor: {score}";

            // Tampilkan nama level (ambil dari LevelManager jika ada)
            // LevelManager menyimpan levelName sebagai field private, expose via property jika perlu
            if (levelNameText) levelNameText.text = "";

            Debug.Log($"[LevelCompleteUI] Panel ditampilkan. Skor: {score}");
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