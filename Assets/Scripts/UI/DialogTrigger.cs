using UnityEngine;

namespace HiddenResidue.Dialog
{
    /// <summary>
    /// DialogTrigger — Letakkan di area level. Saat player masuk collider, dialog dimulai.
    ///
    /// Attach ke: GameObject kosong di scene (misal "DialogTrigger_Intro")
    /// Komponen yang dibutuhkan:
    ///   - BoxCollider2D (isTrigger: true)
    ///   - Script ini
    ///
    /// Cara pakai:
    ///   1. Buat ScriptableObject DialogData (Create → HiddenResidue → Dialog Data)
    ///   2. Drag DialogData SO ke field "Dialog Data" di Inspector
    ///   3. Sesuaikan ukuran BoxCollider2D dengan area yang ingin trigger dialog
    /// </summary>
    public class DialogTrigger : MonoBehaviour
    {
        // ─── Inspector ───────────────────────────────────────────────────────
        [Header("Dialog")]
        [SerializeField] private DialogData dialogData;

        [Header("Trigger Settings")]
        [Tooltip("Jika true: dialog hanya muncul sekali. Jika false: muncul setiap player masuk.")]
        [SerializeField] private bool triggerOnce = true;

        // ─── State ───────────────────────────────────────────────────────────
        private bool hasTriggered = false;

        // ─────────────────────────────────────────────────────────────────────
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (triggerOnce && hasTriggered) return;
            if (dialogData == null) return;
            if (DialogManager.Instance == null) return;

            hasTriggered = true;
            DialogManager.Instance.StartDialog(dialogData);
        }

        /// <summary>Reset trigger agar bisa dipakai lagi (opsional, panggil dari script lain).</summary>
        public void ResetTrigger() => hasTriggered = false;

        // ─── Gizmos — tampilkan area trigger di editor ───────────────────────
        private void OnDrawGizmos()
        {
            var col = GetComponent<BoxCollider2D>();
            if (col == null) return;

            Gizmos.color = new Color(0.3f, 0.6f, 1f, 0.25f);
            Gizmos.DrawCube(transform.position + (Vector3)col.offset, col.size);

            Gizmos.color = new Color(0.3f, 0.6f, 1f, 0.8f);
            Gizmos.DrawWireCube(transform.position + (Vector3)col.offset, col.size);
        }
    }
}
