using UnityEngine;

namespace HiddenResidue.Evidence

{

    public class EvidenceObject : MonoBehaviour, Interaction.IInteractable

    {

        [Header("Evidence Data")]

        [SerializeField] private EvidenceData data;

        [Header("Visual Hint")]

        [Tooltip("Aktifkan agar objek punya efek glow/shimmer saat player dekat")]

        [SerializeField] private bool  enableGlow = true;

        [SerializeField] private float glowRadius = 2f;

        [SerializeField] private float glowSpeed  = 2f;

        [SerializeField] private Color glowColor  = new Color(1f, 0.9f, 0.3f, 1f);

        [Header("Pickup Effect")]

        [SerializeField] private GameObject pickupEffect;

        public bool   CanInteract    => !isPickedUp;

        public string InteractPrompt => $"Tekan E — Ambil {(data != null ? data.evidenceName : "Bukti")}";

        private bool           isPickedUp = false;

        private SpriteRenderer sr;

        private Transform      playerTransform;

        private Color          originalColor;

        private void Awake()

        {

            sr            = GetComponent<SpriteRenderer>();

            originalColor = sr != null ? sr.color : Color.white;

            Debug.Log($"[EvidenceObject] Awake: {gameObject.name}");

        }

        private void Start()

        {

            var player = GameObject.FindGameObjectWithTag("Player");

            if (player)

            {

                playerTransform = player.transform;

                Debug.Log($"[EvidenceObject] Player found: {player.name}");

            }

            else

            {

                Debug.LogWarning("[EvidenceObject] Player not found with tag 'Player'");

            }

        }

        private void Update()

        {

            if (isPickedUp || !enableGlow || sr == null || playerTransform == null) return;

            float dist = Vector2.Distance(transform.position, playerTransform.position);

            if (dist <= glowRadius)

            {

                float t  = (Mathf.Sin(Time.time * glowSpeed) + 1f) / 2f;

                sr.color = Color.Lerp(originalColor, glowColor, t * 0.6f);

            }

            else

            {

                sr.color = originalColor;

            }

        }

        public void Interact()

        {

            if (isPickedUp)

            {

                Debug.Log("[EvidenceObject] Already picked up");

                return;

            }

            if (data == null)

            {

                Debug.LogError("[EvidenceObject] EvidenceData is null!");

                return;

            }

            Debug.Log($"[EvidenceObject] Interact called for: {data.evidenceName}");

            isPickedUp = true;

            if (EvidenceManager.Instance == null)

            {

                Debug.LogError("[EvidenceObject] EvidenceManager.Instance is NULL!");

                return;

            }

            EvidenceManager.Instance.AddEvidence(data);

            if (pickupEffect != null)

            {

                Instantiate(pickupEffect, transform.position, Quaternion.identity);

                Debug.Log("[EvidenceObject] Pickup effect spawned");

            }

            if (Core.ScoreManager.Instance != null)

                Core.ScoreManager.Instance.AddEvidenceScore();

            if (Core.LevelManager.Instance != null)

                Core.LevelManager.Instance.RegisterEvidenceFound();

            UI.NotificationUI.Show($"Barang Bukti Ditemukan: {data.evidenceName}");

            UI.ScorePopupUI.Show(transform.position, Core.ScoreManager.Instance?.GetEvidenceScore() ?? 25);

            gameObject.SetActive(false);

            Debug.Log($"[EvidenceObject] SUCCESS: {data.evidenceName} added to inventory");

        }

    }

}