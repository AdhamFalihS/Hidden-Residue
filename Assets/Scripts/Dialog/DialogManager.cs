using UnityEngine;
using System.Collections;

namespace HiddenResidue.Dialog
{
    /// <summary>
    /// DialogManager — Singleton yang mengelola tampilan dialog Visual Novel.
    /// Dipanggil oleh DialogTrigger saat player masuk collider.
    /// </summary>
    public class DialogManager : MonoBehaviour
    {
        // ─── Singleton ───────────────────────────────────────────────────────
        public static DialogManager Instance { get; private set; }

        // ─── State ───────────────────────────────────────────────────────────
        private DialogData currentDialog;
        private int        lineIndex;
        private bool       isRunning    = false;
        private bool       isTyping     = false;
        private Coroutine  typeRoutine;

        // ─── Events (untuk DialogBoxUI) ───────────────────────────────────────
        public static event System.Action<DialogLine>  OnLineStarted;  // Mulai baris baru
        public static event System.Action<string>      OnCharTyped;    // Per karakter typewriter
        public static event System.Action              OnLineComplete; // Baris selesai diketik
        public static event System.Action              OnDialogEnd;    // Semua dialog selesai

        // Callback opsional setelah dialog selesai
        private System.Action onComplete;

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        // ─── Public API ──────────────────────────────────────────────────────

        public void StartDialog(DialogData data, System.Action callback = null)
        {
            if (isRunning || data == null || data.lines.Length == 0) return;

            currentDialog = data;
            lineIndex     = 0;
            onComplete    = callback;
            isRunning     = true;

            Core.GameManager.Instance?.SetState(Core.GameManager.GameState.Dialog);
            ShowLine();
        }

        /// <summary>Dipanggil saat pemain klik/tekan Space/Enter untuk lanjut.</summary>
        public void Advance()
        {
            if (!isRunning) return;

            if (isTyping)
            {
                // Skip typewriter: tampilkan langsung teks lengkap
                if (typeRoutine != null) StopCoroutine(typeRoutine);
                isTyping = false;
                OnLineComplete?.Invoke();
            }
            else
            {
                NextLine();
            }
        }

        // ─── Private ─────────────────────────────────────────────────────────

        private void ShowLine()
        {
            var line = currentDialog.lines[lineIndex];
            OnLineStarted?.Invoke(line);
            if (typeRoutine != null) StopCoroutine(typeRoutine);
            typeRoutine = StartCoroutine(TypeLine(line.text));
        }

        private IEnumerator TypeLine(string fullText)
        {
            isTyping = true;
            string displayed = "";
            foreach (char c in fullText)
            {
                displayed += c;
                OnCharTyped?.Invoke(displayed);
                yield return new WaitForSecondsRealtime(0.035f);  // Kecepatan ketik
            }
            isTyping = false;
            OnLineComplete?.Invoke();
        }

        private void NextLine()
        {
            lineIndex++;
            if (lineIndex < currentDialog.lines.Length)
                ShowLine();
            else
                EndDialog();
        }

        private void EndDialog()
        {
            isRunning = false;
            OnDialogEnd?.Invoke();
            Core.GameManager.Instance?.SetState(Core.GameManager.GameState.Playing);
            onComplete?.Invoke();
            Debug.Log("[DialogManager] Dialog selesai.");
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// DialogTrigger — Letakkan di area level. Saat player masuk, dialog dimulai.
    /// Attach ke GameObject kosong dengan BoxCollider2D (isTrigger = true).
    /// </summary>
    
}
