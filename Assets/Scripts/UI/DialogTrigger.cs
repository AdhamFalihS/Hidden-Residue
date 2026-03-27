using UnityEngine;
using HiddenResidue.Interaction;

namespace HiddenResidue.Dialog
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DialogTrigger : MonoBehaviour, IInteractable
    {
        [Header("Dialog")]
        [SerializeField] private DialogData dialogData;

        [Header("Trigger Settings")]
        [SerializeField] private bool triggerOnce = false;

        [Header("Visual Root (WAJIB untuk posisi atas)")]
        [Tooltip("Masukkan child sprite/visual di sini supaya indikator muncul di atas")]
        [SerializeField] private Transform visualRoot;

        [SerializeField] private float topOffset = 1.2f;

        private bool hasTriggered = false;
        private bool isPlayerInside = false;

        private void Awake()
        {
            var col = GetComponent<BoxCollider2D>();
            col.isTrigger = true;
        }

        // =========================
        // TRIGGER
        // =========================
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (dialogData == null) return;
            if (DialogManager.Instance == null) return;

            isPlayerInside = true;

            // Pertama kali auto dialog
            if (!hasTriggered)
            {
                hasTriggered = true;
                DialogManager.Instance.StartDialog(dialogData);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            isPlayerInside = false;
        }

        // =========================
        // INTERACT
        // =========================
        public void Interact()
        {
            if (!CanInteract) return;
            DialogManager.Instance.StartDialog(dialogData);
        }

        public bool CanInteract => hasTriggered && isPlayerInside && !triggerOnce;

        public string InteractPrompt => "Tekan E — Bicara";

        // =========================
        // 🔥 TRICK UTAMA: GESER POSISI
        // =========================
        private void LateUpdate()
        {
            // Kalau tidak ada visualRoot, skip
            if (visualRoot == null) return;

            // Kita geser posisi root mengikuti visual + offset
            Vector3 topPos = visualRoot.position + Vector3.up * topOffset;

            // Update posisi sementara supaya InteractionDetector baca ini
            transform.position = new Vector3(topPos.x, topPos.y - topOffset, transform.position.z);
        }

        // =========================
        // GIZMOS
        // =========================
        private void OnDrawGizmos()
        {
            if (visualRoot != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(visualRoot.position + Vector3.up * topOffset, 0.1f);
            }
        }

        public void ResetTrigger()
        {
            hasTriggered = false;
        }
    }
}