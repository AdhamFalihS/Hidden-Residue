using UnityEngine;
using UnityEngine.InputSystem;

namespace HiddenResidue.Interaction
{
    /// <summary>
    /// InteractionDetector — Mendeteksi objek IInteractable di sekitar player
    /// dan menampilkan indikator "Press E". Saat E ditekan → panggil Interact().
    ///
    /// Attach ke GameObject Player (Jojo).
    /// Cara kerja: OverlapCircle setiap frame untuk cari objek terdekat.
    /// </summary>
    public class InteractionDetector : MonoBehaviour
    {
        // ─── Inspector ───────────────────────────────────────────────────────
        [Header("Detection")]
        [SerializeField] private float   detectRadius = 1.5f;
        [SerializeField] private LayerMask interactLayer;     // Set layer "Interactable"

        [Header("Indicator")]
        [SerializeField] private GameObject pressEIndicator;  // Drag UI "Press E" Canvas

        // ─── State ───────────────────────────────────────────────────────────
        private IInteractable currentTarget;
        private GameObject    currentTargetGO;

        // ─────────────────────────────────────────────────────────────────────
        private void Update()
        {
            // Jangan proses interaksi saat dialog/quiz/fail
            if (Core.GameManager.Instance != null && !Core.GameManager.Instance.IsPlaying)
            {
                HideIndicator();
                return;
            }

            FindNearestInteractable();
            HandleIndicator();
            HandleInput();
        }

        // ─── Private ─────────────────────────────────────────────────────────

        private void FindNearestInteractable()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius, interactLayer);

            IInteractable closest   = null;
            GameObject    closestGO = null;
            float minDist           = float.MaxValue;

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IInteractable>(out var interactable) && interactable.CanInteract)
                {
                    float dist = Vector2.Distance(transform.position, hit.transform.position);
                    if (dist < minDist)
                    {
                        minDist   = dist;
                        closest   = interactable;
                        closestGO = hit.gameObject;
                    }
                }
            }

            currentTarget   = closest;
            currentTargetGO = closestGO;
        }

        private void HandleIndicator()
        {
            if (pressEIndicator == null) return;

            if (currentTarget != null)
            {
                pressEIndicator.SetActive(true);

                // Posisikan indikator di atas objek target
                if (currentTargetGO != null)
                {
                    Vector3 pos = currentTargetGO.transform.position + Vector3.up * 1.2f;
                    pressEIndicator.transform.position = pos;
                }

                // Update teks jika ada komponen Text
                var txt = pressEIndicator.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (txt != null)
                    txt.text = currentTarget.InteractPrompt;
            }
            else
            {
                HideIndicator();
            }
        }

        private void HandleInput()
        {
            // Gunakan new Input System: Keyboard.current
            if (currentTarget == null) return;

            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                currentTarget.Interact();
            }
        }

        private void HideIndicator()
        {
            pressEIndicator?.SetActive(false);
        }

        // ─── Gizmos (debug di editor) ─────────────────────────────────────────
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectRadius);
        }
    }
}
