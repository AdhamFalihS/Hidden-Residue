using UnityEngine;

namespace HiddenResidue.Evidence
{
    /// <summary>
    /// EvidenceObject — Barang bukti tersembunyi di level.
    /// Pemain menekan E → barang masuk inventory, objek hilang dari scene.
    ///
    /// Attach ke setiap prefab barang bukti di scene.
    /// </summary>
    public class EvidenceObject : MonoBehaviour, Interaction.IInteractable
    {
        // ─── Inspector ───────────────────────────────────────────────────────
        [Header("Evidence Data")]
        [SerializeField] private EvidenceData data;

        [Header("Visual Hint")]
        [Tooltip("Aktifkan agar objek punya efek glow/shimmer saat player dekat")]
        [SerializeField] private bool  enableGlow = true;
        [SerializeField] private float glowRadius = 2f;   // Radius untuk mulai glow
        [SerializeField] private float glowSpeed  = 2f;
        [SerializeField] private Color glowColor  = new Color(1f, 0.9f, 0.3f, 1f);  // Kuning

        [Header("Pickup Effect")]
        [SerializeField] private GameObject pickupEffect;  // Particle effect saat diambil

        // ─── IInteractable ────────────────────────────────────────────────────
        public bool   CanInteract    => !isPickedUp;
        public string InteractPrompt => $"Tekan E — Ambil {(data != null ? data.evidenceName : "Bukti")}";

        // ─── State ───────────────────────────────────────────────────────────
        private bool           isPickedUp = false;
        private SpriteRenderer sr;
        private Transform      playerTransform;
        private Color          originalColor;

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            sr            = GetComponent<SpriteRenderer>();
            originalColor = sr != null ? sr.color : Color.white;
        }

        private void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player) playerTransform = player.transform;
        }

        private void Update()
        {
            if (isPickedUp || !enableGlow || sr == null || playerTransform == null) return;

            float dist = Vector2.Distance(transform.position, playerTransform.position);
            if (dist <= glowRadius)
            {
                // Pulse glow: interpolasi antara warna asli dan glowColor
                float t  = (Mathf.Sin(Time.time * glowSpeed) + 1f) / 2f;
                sr.color = Color.Lerp(originalColor, glowColor, t * 0.6f);
            }
            else
            {
                sr.color = originalColor;
            }
        }

        // ─── IInteractable.Interact() ─────────────────────────────────────────
        public void Interact()
        {
            if (isPickedUp || data == null) return;

            isPickedUp = true;

            // Tambahkan ke EvidenceManager
            EvidenceManager.Instance?.AddEvidence(data);

            // Efek pickup
            if (pickupEffect)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            // Tambah skor
            Core.ScoreManager.Instance?.AddEvidenceScore();

            // Beritahu level manager
            Core.LevelManager.Instance?.RegisterEvidenceFound();

            // Tampilkan notifikasi
            UI.NotificationUI.Show($"Barang Bukti Ditemukan: {data.evidenceName}");

            // Tampilkan score popup
            UI.ScorePopupUI.Show(transform.position, Core.ScoreManager.Instance?.GetEvidenceScore() ?? 25);

            // Sembunyikan objek
            gameObject.SetActive(false);

            Debug.Log($"[EvidenceObject] Ditemukan: {data.evidenceName}");
        }
    }
}
