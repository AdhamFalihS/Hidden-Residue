using UnityEngine;
using System.Collections;

namespace HiddenResidue.Dialog
{
    public class DialogManager : MonoBehaviour
    {
        // ─── Singleton ───────────────────────────────────────────────────────
        public static DialogManager Instance { get; private set; }

        // ─── State ───────────────────────────────────────────────────────────
        private DialogData currentDialog;
        private int lineIndex;
        private bool isRunning = false;
        private bool isTyping = false;
        private Coroutine typeRoutine;

        // ─── Events ──────────────────────────────────────────────────────────
        public static event System.Action<DialogLine> OnLineStarted;
        public static event System.Action<string> OnCharTyped;
        public static event System.Action OnLineComplete;
        public static event System.Action OnDialogEnd;

        private System.Action onComplete;

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        // ─── Public API ──────────────────────────────────────────────────────

        public void StartDialog(DialogData data, System.Action callback = null)
        {
            if (isRunning || data == null || data.lines.Length == 0) return;

            currentDialog = data;
            lineIndex = 0;
            onComplete = callback;
            isRunning = true;

            Core.GameManager.Instance?.SetState(Core.GameManager.GameState.Dialog);

            ShowLine();
        }

        /// <summary>
        /// Dipanggil saat player tekan tombol untuk lanjut dialog
        /// </summary>
        public void Advance()
        {
            if (!isRunning) return;

            // ❌ TIDAK BOLEH SKIP SAAT MASIH MENGETIK
            if (isTyping)
            {
                return;
            }

            // ✅ Hanya lanjut jika sudah selesai ketik
            NextLine();
        }

        // ─── Private ─────────────────────────────────────────────────────────

        private void ShowLine()
        {
            var line = currentDialog.lines[lineIndex];

            OnLineStarted?.Invoke(line);

            if (typeRoutine != null)
                StopCoroutine(typeRoutine);

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

                yield return new WaitForSecondsRealtime(0.035f);
            }

            isTyping = false;

            // ✅ Baru boleh lanjut setelah ini
            OnLineComplete?.Invoke();
        }

        private void NextLine()
        {
            lineIndex++;

            if (lineIndex < currentDialog.lines.Length)
            {
                ShowLine();
            }
            else
            {
                EndDialog();
            }
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
}