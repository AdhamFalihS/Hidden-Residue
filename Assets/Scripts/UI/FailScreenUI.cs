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


        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            Instance = this;
            if (failPanel) failPanel.SetActive(false);
        }

        // ─── Public API ───────────────────────────────────────────────────────

        /// <summary>Tampilkan layar gagal. Dipanggil oleh GameManager.TriggerFail().</summary>
        public void Show()
        {
            if (failPanel) failPanel.SetActive(true);
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
    }
}
