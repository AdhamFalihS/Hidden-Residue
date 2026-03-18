using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HiddenResidue.UI
{
    /// <summary>
    /// HUD — Heads-Up Display utama.
    /// Menampilkan skor, progress cleaning (slider & text), dan jumlah bukti.
    /// Attach ke: GameObject "HUD" di dalam Canvas.
    /// </summary>
    public class HUD : MonoBehaviour
    {
        public static HUD Instance { get; private set; }

        [Header("Score")]
        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("Cleaning Progress")]
        [SerializeField] private Slider cleanProgressSlider;
        [SerializeField] private TextMeshProUGUI cleanCountText; // Menampilkan format "0/10"

        [Header("Evidence Progress")]
        [SerializeField] private TextMeshProUGUI evidenceCountText; // Menampilkan format "0/3"

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            // Singleton setup sederhana
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            // Berlangganan ke event dari Manager
            Core.ScoreManager.OnScoreChanged += HandleScoreChanged;
            Core.LevelManager.OnLevelProgressUpdated += HandleProgressUpdated;
        }

        private void OnDisable()
        {
            // Berhenti berlangganan saat object tidak aktif/hancur (mencegah memory leak)
            Core.ScoreManager.OnScoreChanged -= HandleScoreChanged;
            Core.LevelManager.OnLevelProgressUpdated -= HandleProgressUpdated;
        }

        private void Start()
        {
            // Inisialisasi tampilan awal saat game dimulai
            if (scoreText) scoreText.text = "Skor: 0";
            
            if (cleanProgressSlider) cleanProgressSlider.value = 0f;
            if (cleanCountText) cleanCountText.text = "Cleaning: 0/0";
            
            if (evidenceCountText) evidenceCountText.text = "Bukti: 0/0";

            // Panggil sekali di awal untuk sinkronisasi data jika ada data yang sudah terisi
            HandleProgressUpdated();
        }

        // ─── Event Handlers ───────────────────────────────────────────────────

        /// <summary>
        /// Diupdate setiap kali ScoreManager menambah skor.
        /// </summary>
        private void HandleScoreChanged(int added, int total)
        {
            if (scoreText) scoreText.text = $"Skor: {total}";
        }

        /// <summary>
        /// Diupdate setiap kali ada kemajuan di LevelManager (Cleaning atau Evidence).
        /// </summary>
        private void HandleProgressUpdated()
        {
            var lm = Core.LevelManager.Instance;
            if (lm == null) return;

            // Update Progress Slider (Visual Bar)
            if (cleanProgressSlider)
                cleanProgressSlider.value = lm.GetCleanProgress();

            // Update Cleaning Text (Format: 0/10)
            // Pastikan di LevelManager kamu punya properti CleanedCount dan TotalCleanable
            if (cleanCountText)
            {
                cleanCountText.text = $"Cleaning: {lm.CleanedCount}/{lm.TotalCleanable}";
            }

            // Update Evidence Text (Format: 0/3)
            if (evidenceCountText)
            {
                evidenceCountText.text = $"Bukti: {lm.EvidenceCount}/{lm.TotalEvidence}";
            }
        }
    }
}