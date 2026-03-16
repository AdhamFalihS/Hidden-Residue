using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HiddenResidue.UI
{
    /// <summary>
    /// HUD — Heads-Up Display utama.
    /// Menampilkan skor, progress cleaning, dan jumlah bukti.
    /// Attach ke: GameObject "HUD" di dalam Canvas.
    /// </summary>
    public class HUD : MonoBehaviour
    {
        public static HUD Instance { get; private set; }

        [Header("Score")]
        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("Progress")]
        [SerializeField] private Slider          cleanProgressSlider;
        [SerializeField] private TextMeshProUGUI evidenceCountText;

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            Core.ScoreManager.OnScoreChanged         += HandleScoreChanged;
            Core.LevelManager.OnLevelProgressUpdated += HandleProgressUpdated;
        }

        private void OnDisable()
        {
            Core.ScoreManager.OnScoreChanged         -= HandleScoreChanged;
            Core.LevelManager.OnLevelProgressUpdated -= HandleProgressUpdated;
        }

        private void Start()
        {
            // Inisialisasi tampilan awal
            if (scoreText)         scoreText.text         = "Skor: 0";
            if (cleanProgressSlider) cleanProgressSlider.value = 0f;
            if (evidenceCountText) evidenceCountText.text  = "Bukti: 0/0";
        }

        // ─── Event Handlers ───────────────────────────────────────────────────
        private void HandleScoreChanged(int added, int total)
        {
            if (scoreText) scoreText.text = $"Skor: {total}";
        }

        private void HandleProgressUpdated()
        {
            var lm = Core.LevelManager.Instance;
            if (lm == null) return;

            if (cleanProgressSlider)
                cleanProgressSlider.value = lm.GetCleanProgress();

            if (evidenceCountText)
                evidenceCountText.text = $"Bukti: {lm.EvidenceCount}/{lm.TotalEvidence}";
        }
    }
}
