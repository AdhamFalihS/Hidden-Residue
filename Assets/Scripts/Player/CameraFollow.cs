using UnityEngine;

namespace HiddenResidue.Player
{
    /// <summary>
    /// CameraFollow — Kamera mengikuti player dengan smooth damp.
    /// Attach ke Main Camera.
    ///
    /// Opsional: set batas kamera agar tidak keluar dari level (boundary).
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        // ─── Inspector ───────────────────────────────────────────────────────
        [Header("Target")]
        [SerializeField] private Transform target;   // Drag Jojo ke sini

        [Header("Follow Settings")]
        [SerializeField] private float smoothTime   = 0.15f;
        [SerializeField] private Vector3 offset     = new Vector3(0f, 0f, -10f);

        [Header("Boundary (opsional, isi jika ingin batasi kamera)")]
        [SerializeField] private bool  useBoundary = false;
        [SerializeField] private float minX = -10f, maxX = 10f;
        [SerializeField] private float minY = -10f, maxY = 10f;

        // ─── State ───────────────────────────────────────────────────────────
        private Vector3 velocity = Vector3.zero;

        // ─────────────────────────────────────────────────────────────────────
        private void Start()
        {
            // Auto-find player jika belum di-assign
            if (target == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) target = player.transform;
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desired = target.position + offset;

            // Clamp ke boundary jika aktif
            if (useBoundary)
            {
                desired.x = Mathf.Clamp(desired.x, minX, maxX);
                desired.y = Mathf.Clamp(desired.y, minY, maxY);
            }

            transform.position = Vector3.SmoothDamp(
                transform.position, desired, ref velocity, smoothTime);
        }

        // ─── Public API ──────────────────────────────────────────────────────
        public void SetTarget(Transform newTarget) => target = newTarget;

        /// <summary>Langsung snap kamera ke posisi target (tanpa smooth).</summary>
        public void SnapToTarget()
        {
            if (target == null) return;
            transform.position = target.position + offset;
        }
    }
}
