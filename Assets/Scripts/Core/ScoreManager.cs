using UnityEngine;
using UnityEngine.SceneManagement;

namespace HiddenResidue.Core
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [Header("Score Values")]
        [SerializeField] private int cleanScore = 10;
        [SerializeField] private int evidenceScore = 25;
        [SerializeField] private int quizScore = 50;
        [SerializeField] private int levelBonus = 100;

        public int CurrentScore { get; private set; }

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

                ResetScore();

                Debug.Log("[ScoreManager] Score di-reset karena scene di-load ulang (Retry).");

            }

            _isLoadingNextLevel = false;

        }

        public void SetLoadingNextLevel()

        {

            _isLoadingNextLevel = true;

        }

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