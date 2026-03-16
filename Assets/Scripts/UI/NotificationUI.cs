using UnityEngine;
using TMPro;
using System.Collections;

namespace HiddenResidue.UI
{
    /// <summary>
    /// NotificationUI — Banner notifikasi singkat yang muncul di layar.
    /// Contoh: "Barang Bukti Ditemukan: Kartu Identitas"
    ///
    /// Cara pakai (dari script lain):
    ///   NotificationUI.Show("Barang Bukti Ditemukan: Kartu Identitas");
    ///
    /// Attach ke: GameObject "NotificationPanel" di Canvas.
    ///
    /// Struktur UI:
    ///   NotificationPanel
    ///   └── NotifText   (TextMeshProUGUI)
    /// </summary>
    public class NotificationUI : MonoBehaviour
    {
        public static NotificationUI Instance { get; private set; }

        [Header("Panel")]
        [SerializeField] private GameObject      notifPanel;
        [SerializeField] private TextMeshProUGUI notifText;

        [Header("Durasi")]
        [SerializeField] private float displayDuration = 3f;

        private Coroutine _hideRoutine;

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            Instance = this;
            if (notifPanel) notifPanel.SetActive(false);
        }

        // ─── Static Helper ────────────────────────────────────────────────────

        public static void Show(string message)
        {
            if (Instance == null)
            {
                Debug.LogWarning("[NotificationUI] Instance belum ada di scene!");
                return;
            }
            Instance.ShowMessage(message);
        }

        // ─── Instance ─────────────────────────────────────────────────────────

        public void ShowMessage(string message)
        {
            if (notifPanel) notifPanel.SetActive(true);
            if (notifText)  notifText.text = message;

            // Jika sudah ada coroutine berjalan, stop dulu
            if (_hideRoutine != null) StopCoroutine(_hideRoutine);
            _hideRoutine = StartCoroutine(HideAfterDelay());
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(displayDuration);
            if (notifPanel) notifPanel.SetActive(false);
        }
    }
}
