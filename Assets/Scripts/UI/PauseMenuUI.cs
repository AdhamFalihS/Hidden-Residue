using UnityEngine;
using UnityEngine.UI;

namespace HiddenResidue.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        public static PauseMenuUI Instance { get; private set; }

        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);

                return;
            }

            Instance = this;
        }

        private void Start()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeClicked);
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        public void OnResumeClicked()
        {
            Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.ButtonClick);
            Core.GameManager.Instance?.ResumeGame();
        }

        public void OnRestartClicked()
        {
            Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.ButtonClick);
            Core.GameManager.Instance?.RetryLevel();
        }

        public void OnMainMenuClicked()
        {
            Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.ButtonClick);
            Core.GameManager.Instance?.GoToMainMenu();
        }

        // Call this from pause button (e.g., in HUD or scene button)
        public static void OnPauseClicked()
        {
            Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.ButtonClick);

            Core.GameManager.Instance?.PauseGame();
        }
    }
}
