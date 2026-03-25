using UnityEngine;

using UnityEngine.UI;

using TMPro;

namespace HiddenResidue.UI

{

    public class HUD : MonoBehaviour

    {

        public static HUD Instance { get; private set; }

        [Header("Score")]

        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("Cleaning Progress")]

        [SerializeField] private Slider cleanProgressSlider;

        [SerializeField] private TextMeshProUGUI cleanCountText;

        [Header("Evidence Progress")]

        [SerializeField] private TextMeshProUGUI evidenceCountText;

        private void Awake()

        {

            if (Instance != null && Instance != this)

            {

                Destroy(gameObject);

                return;

            }

            Instance = this;

        }

        private void OnEnable()

        {

            Core.ScoreManager.OnScoreChanged += HandleScoreChanged;

            Core.LevelManager.OnLevelProgressUpdated += HandleProgressUpdated;

        }

        private void OnDisable()

        {

            Core.ScoreManager.OnScoreChanged -= HandleScoreChanged;

            Core.LevelManager.OnLevelProgressUpdated -= HandleProgressUpdated;

        }

        private void Start()

        {

            if (scoreText) scoreText.text = "Skor: 0";

            if (cleanProgressSlider) cleanProgressSlider.value = 0f;

            if (cleanCountText) cleanCountText.text = "Cleaning: 0/0";

            if (evidenceCountText) evidenceCountText.text = "Bukti: 0/0";

            HandleProgressUpdated();

        }

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

            if (cleanCountText)

            {

                cleanCountText.text = $"Cleaning: {lm.CleanedCount}/{lm.TotalCleanable}";

            }

            if (evidenceCountText)

            {

                evidenceCountText.text = $"Bukti: {lm.EvidenceCount}/{lm.TotalEvidence}";

            }

        }

    }

}