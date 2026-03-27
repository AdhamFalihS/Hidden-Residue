using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace HiddenResidue.UI
{
    public class LevelCompleteUI : MonoBehaviour
    {
        public static LevelCompleteUI Instance { get; private set; }

        [Header("Panel")]
        [SerializeField] private GameObject completePanel;

        [Header("Teks")]
        [SerializeField] private TextMeshProUGUI totalScoreText;
        [SerializeField] private TextMeshProUGUI levelNameText;

        private void Awake()

        {

            if (Instance != null && Instance != this)

            {

                Destroy(gameObject);

                return;

            }

            Instance = this;

            if (completePanel == null)

                Debug.LogError("[LevelCompleteUI] completePanel belum di-assign di Inspector!");

            if (completePanel) completePanel.SetActive(false);

        }

        private void OnDestroy()

        {

            if (Instance == this) Instance = null;

        }

        public void Show()

        {

            if (completePanel == null)

            {

                Debug.LogError("[LevelCompleteUI] completePanel NULL! Assign di Inspector.");

                return;

            }

            completePanel.SetActive(true);

            int score = Core.ScoreManager.Instance?.CurrentScore ?? 0;

            if (totalScoreText) totalScoreText.text = $"Total Skor: {score}";

            if (levelNameText) levelNameText.text = SceneManager.GetActiveScene().name;

            Debug.Log($"[LevelCompleteUI] Panel ditampilkan. Skor: {score}");

        }

        public void OnNextLevelClicked()

        {

            Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.ButtonClick);
            Core.GameManager.Instance?.LoadNextLevel();

        }

        public void OnMainMenuClicked()

        {

            Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.ButtonClick);
            Core.GameManager.Instance?.GoToMainMenu();

        }

    }

}