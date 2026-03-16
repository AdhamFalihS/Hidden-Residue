using UnityEngine;

namespace HiddenResidue.Core
{
    /// <summary>
    /// ScoreManager — Singleton yang menyimpan dan mengelola skor pemain.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        // ─── Singleton ───────────────────────────────────────────────────────
        public static ScoreManager Instance { get; private set; }

        // ─── Score Values (bisa diubah di Inspector) ─────────────────────────
        [Header("Score Values")]
        [SerializeField] private int cleanScore    = 10;
        [SerializeField] private int evidenceScore = 25;
        [SerializeField] private int quizScore     = 50;
        [SerializeField] private int levelBonus    = 100;

        // ─── State ───────────────────────────────────────────────────────────
        public int CurrentScore { get; private set; }

        // ─── Events ──────────────────────────────────────────────────────────
        public static event System.Action<int, int> OnScoreChanged; // (added, total)

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
