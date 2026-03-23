using UnityEngine;
using UnityEngine.SceneManagement;

namespace HiddenResidue.Core
{
    /// <summary>
    /// ScoreManager — Singleton DontDestroyOnLoad.
    /// FIX: ResetScore() otomatis dipanggil saat scene di-reload (Retry).
    /// Score hanya carry-over ke scene BARU (LoadNextLevel), bukan saat Retry.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [Header("Score Values")]
        [SerializeField] private int cleanScore    = 10;
        [SerializeField] private int evidenceScore = 25;
        [SerializeField] private int quizScore     = 50;
        [SerializeField] private int levelBonus    = 100;

        public int CurrentScore { get; private set; }

        // Flag: apakah ini load level baru (bukan retry)?
        private bool _isLoadingNextLevel = false;

        public static event System.Action<int, int> OnScoreChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!_isLoadingNextLevel)
            {
                // Retry atau load scene apapun selain "next level" → reset score
                ResetScore();
                Debug.Log("[ScoreManager] Score di-reset karena scene di-load ulang (Retry).");
            }
            // Reset flag setelah dipakai
            _isLoadingNextLevel = false;
        }

        /// <summary>
        /// Panggil ini SEBELUM LoadNextLevel() agar score tidak di-reset.
        /// Dipanggil oleh GameManager.LoadNextLevel().
        /// </summary>
        public void SetLoadingNextLevel()
        {
            _isLoadingNextLevel = true;
        }

        // ─── Public API ──────────────────────────────────────────────────────

        public void AddCleanScore()    => AddScore(cleanScore);
        public void AddEvidenceScore() => AddScore(evidenceScore);
        public void AddQuizScore()     => AddScore(quizScore);
        public void AddLevelBonus()    => AddScore(levelBonus);

        public void AddScore(int amount)
        {
            if (amount <= 0) return;
            CurrentScore += amount;
            OnScoreChanged?.Invoke(amount, CurrentScore);
            Debug.Log($"[ScoreManager] +{amount} → Total: {CurrentScore}");
        }

        public void ResetScore()
        {
            CurrentScore = 0;
            OnScoreChanged?.Invoke(0, 0);
        }

        public int GetCleanScore()    => cleanScore;
        public int GetEvidenceScore() => evidenceScore;
        public int GetQuizScore()     => quizScore;
    }
}