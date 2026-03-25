using UnityEngine;

using UnityEngine.SceneManagement;

namespace HiddenResidue.Core

{

    public class GameManager : MonoBehaviour

    {

        public static GameManager Instance { get; private set; }

        public enum GameState { Playing, Paused, Dialog, Quiz, Failed, LevelComplete }

        public GameState CurrentState { get; private set; } = GameState.Playing;

        public static event System.Action<GameState> OnGameStateChanged;

        [Header("Scene Names")]

        [SerializeField] private string mainMenuScene = "MainMenu";

        [Header("Pause System")]

        [SerializeField] private GameObject[] gameplayObjects;

        [SerializeField] private GameObject   pausePanel;

        private void Awake()

        {

            Instance = this;

        }

        private void Start()

        {

            if (pausePanel != null) pausePanel.SetActive(false);

        }

        public void SetState(GameState newState)

        {

            if (CurrentState == newState) return;

            CurrentState = newState;

            OnGameStateChanged?.Invoke(newState);

            Debug.Log("[GameManager] State → " + newState);

        }

        public bool IsPlaying  => CurrentState == GameState.Playing;

        public bool IsInDialog => CurrentState == GameState.Dialog;

        public bool IsInQuiz   => CurrentState == GameState.Quiz;

        public void PauseGame()

        {

            SetState(GameState.Paused);

            SetGameplayActive(false);

            if (pausePanel != null) pausePanel.SetActive(true);

        }

        public void ResumeGame()

        {

            SetState(GameState.Playing);

            SetGameplayActive(true);

            if (pausePanel != null) pausePanel.SetActive(false);

        }

        private void SetGameplayActive(bool state)

        {

            if (gameplayObjects == null) return;

            foreach (var obj in gameplayObjects)

                if (obj != null) obj.SetActive(state);

        }

        public void TriggerFail()

        {

            SetState(GameState.Failed);

            if (UI.FailScreenUI.Instance == null)

            {

                Debug.LogError("[GameManager] FailScreenUI NULL! Pastikan ada di scene dan field Fail Panel sudah di-assign.");

                return;

            }

            UI.FailScreenUI.Instance.Show();

        }

        public void TriggerLevelComplete()

        {

            SetState(GameState.LevelComplete);

            if (UI.LevelCompleteUI.Instance == null)

            {

                Debug.LogError("[GameManager] LevelCompleteUI NULL!");

                return;

            }

            UI.LevelCompleteUI.Instance.Show();

        }

        public void RetryLevel()

        {

            SceneLoader.Instance?.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }

        public void LoadNextLevel()

        {

            int next = SceneManager.GetActiveScene().buildIndex + 1;

            ScoreManager.Instance?.SetLoadingNextLevel();

            LevelProgressManager.Instance?.UnlockLevel(next);

            if (next < SceneManager.sceneCountInBuildSettings)

                SceneLoader.Instance?.LoadScene(next);

            else

                SceneLoader.Instance?.LoadSceneByName(mainMenuScene);

        }

        public void GoToMainMenu()

        {

            ScoreManager.Instance?.ResetScore();

            SceneLoader.Instance?.LoadSceneByName(mainMenuScene);

        }

    }

}