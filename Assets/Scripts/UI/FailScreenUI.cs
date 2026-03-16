using UnityEngine;
using TMPro;
using System.Collections;

namespace HiddenResidue.UI
{
    /// <summary>
    /// FailScreenUI — Layar "GAGAL" yang muncul saat kuis salah atau waktu habis.
    ///
    /// Attach ke: GameObject "FailScreen" di Canvas.
    ///
    /// Struktur UI:
    ///   FailScreen
    ///   ├── BackgroundOverlay  (Image gelap semi-transparan, CanvasGroup)
    ///   ├── TitleText          (TextMeshProUGUI — "GAGAL")
    ///   ├── FailReasonText     (TextMeshProUGUI — alasan gagal)
    ///   ├── RetryButton        (Button → OnClick: FailScreenUI.OnRetryClicked)
    ///   └── MainMenuButton     (Button → OnClick: FailScreenUI.OnMainMenuClicked)
    /// </summary>
    public class FailScreenUI : MonoBehaviour
    {
        public static FailScreenUI Instance { get; private set; }

        [Header("Panel")]
        [SerializeField] private GameObject  failPanel;
        [SerializeField] private CanvasGroup canvasGroup;   // Untuk animasi fade-in

        [Header("Teks")]
        [SerializeField] private TextMeshProUGUI failReasonText;

        [Header("Animasi")]
        [SerializeField] private float fadeInDuration = 0.6f;

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            Instance = this;
            if (failPanel) failPanel.SetActive(false);
        }

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>Tampilkan layar gagal. Dipanggil oleh GameManager.TriggerFail().</summary>
        public void Show(string reason = "Jawaban salah atau waktu habis!")
        {
            if (failPanel) failPanel.SetActive(true);
            if (failReasonText) failReasonText.text = reason;

            if (canvasGroup != null)
                StartCoroutine(FadeIn());
        }

        /// <summary>Dipanggil oleh tombol "Coba Lagi".</summary>
        public void OnRetryClicked()
        {
            Core.GameManager.Instance?.RetryLevel();
        }

        /// <summary>Dipanggil oleh tombol "Menu Utama".</summary>
        public void OnMainMenuClicked()
        {
            Core.GameManager.Instance?.GoToMainMenu();
        }

        // ─── Private ──────────────────────────────────────────────────────────
        private IEnumerator FadeIn()
        {
            canvasGroup.alpha = 0f;
            float elapsed = 0f;

            while (elapsed < fadeInDuration)
            {
                elapsed           += Time.unscaledDeltaTime;
                canvasGroup.alpha  = Mathf.Clamp01(elapsed / fadeInDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }
    }
}
