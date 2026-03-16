using UnityEngine;
using UnityEngine.SceneManagement;

namespace HiddenResidue.Core
{
    /// <summary>
    /// GameManager — Singleton utama yang mengontrol state game secara global.
    /// Attach ke GameObject "GameManager" yang ada di setiap scene.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // ─── Singleton ───────────────────────────────────────────────────────
        public static GameManager Instance { get; private set; }

        // ─── Game State ──────────────────────────────────────────────────────
        public enum GameState { Playing, Paused, Dialog, Quiz, Failed, LevelComplete }
        public GameState CurrentState { get; private set; } = GameState.Playing;

        // ─── Events ──────────────────────────────────────────────────────────
        public static event System.Action<GameState> OnGameStateChanged;

        // ─── Inspector ───────────────────────────────────────────────────────
        [Header("Scene Names")]
        [SerializeField] private string mainMenuScene = "MainMenu";

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance   = this;
            DontDestroyOnLoad(gameObject);
        }

        // ─── Public API ──────────────────────────────────────────────────────

        /// <summary>Ubah state game dan broadcast event.</summary>
        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;

            // Pause / unpause Time
            Time.timeScale = (newState == GameState.Failed ||
                              newState == GameState.Paused) ? 0f : 1f;

            OnGameStateChanged?.Invoke(newState);
            Debug.Log($"[GameManager] State → {newState}");
        }

        public bool IsPlaying   => CurrentState == GameState.Playing;
        public bool IsInDialog  => CurrentState == GameState.Dialog;
        public bool IsInQuiz    => CurrentState == GameState.Quiz;

        public void PauseGame()  => SetState(GameState.Paused);
        public void ResumeGame() => SetState(GameState.Playing);

        public void TriggerFail()
        {
            SetState(GameState.Failed);
            UI.FailScreenUI.Instance?.Show();
        }

        public void TriggerLevelComplete()
        {
            SetState(GameState.LevelComplete);
            UI.LevelCompleteUI.Instance?.Show();
        }

        public void RetryLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void LoadNextLevel()
        {
            Time.timeScale = 1f;
            int next = SceneManager.GetActiveScene().buildIndex + 1;
            if (next < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(next);
            else
                SceneManager.LoadScene(mainMenuScene);
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuScene);
        }
    }
}
