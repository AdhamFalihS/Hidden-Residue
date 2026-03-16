using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace HiddenResidue.Interaction
{
    /// <summary>
    /// CleanableObject — Objek noda/sampah yang bisa dibersihkan pemain.
    /// Saat E ditekan, progress bar muncul dan mengisi selama durasi tertentu.
    /// Setelah selesai → sprite berubah bersih, skor ditambah.
    ///
    /// Attach ke objek sampah/noda di scene.
    /// Komponen yang perlu ada di objek ini (atau child-nya):
    ///   - SpriteRenderer (untuk dirty/clean sprite)
    ///   - Canvas World Space → Slider (progress bar) — taruh sebagai child
    /// </summary>
    public class CleanableObject : MonoBehaviour, IInteractable
    {
        // ─── Inspector ───────────────────────────────────────────────────────
        [Header("Sprites")]
        [SerializeField] private Sprite dirtySprite;    // Sprite kotor (awal)
        [SerializeField] private Sprite cleanSprite;    // Sprite bersih (setelah clean)

        [Header("Cleaning")]
        [SerializeField] private float cleanDuration = 2.5f;  // Durasi bersihkan (detik)
        [Tooltip("Jika true, pemain harus tahan E. Jika false, sekali tekan E langsung mulai.")]
        [SerializeField] private bool holdToClean = false;

        [Header("UI")]
        [SerializeField] private GameObject progressBarRoot;   // Root canvas progress bar
        [SerializeField] private Slider     progressSlider;    // Slider UI
        [SerializeField] private TextMeshProUGUI labelText;     // Opsional: teks "Membersihkan..."

        [Header("Effects")]
        [SerializeField] private GameObject cleanEffect;       // Particle/animasi setelah bersih

        // ─── IInteractable ────────────────────────────────────────────────────
        public bool   CanInteract   => !isCleaned && !isCleaning;
        public string InteractPrompt => "Tekan E — Bersihkan";

        // ─── State ───────────────────────────────────────────────────────────
        private bool    isCleaned  = false;
        private bool    isCleaning = false;
        private float   progress   = 0f;
        private SpriteRenderer sr;

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            if (dirtySprite != null) sr.sprite = dirtySprite;

            // Sembunyikan progress bar di awal
            progressBarRoot?.SetActive(false);
            if (progressSlider) progressSlider.value = 0f;
        }

        // ─── IInteractable.Interact() ─────────────────────────────────────────
        public void Interact()
        {
            if (isCleaned || isCleaning) return;
            StartCoroutine(DoClean());
        }

        // ─── Coroutine Cleaning ───────────────────────────────────────────────
        private IEnumerator DoClean()
        {
            isCleaning = true;
            progress   = 0f;

            progressBarRoot?.SetActive(true);
            if (labelText) labelText.text = "Membersihkan...";

            while (progress < 1f)
            {
                // Jika holdToClean: cek apakah E masih ditekan
                if (holdToClean && !UnityEngine.InputSystem.Keyboard.current.eKey.isPressed)
                {
                    // Pause progress saat E dilepas
                    yield return null;
                    continue;
                }

                progress += Time.deltaTime / cleanDuration;
                progress  = Mathf.Clamp01(progress);

                if (progressSlider) progressSlider.value = progress;

                yield return null;
            }

            FinishCleaning();
        }

        private void FinishCleaning()
        {
            isCleaned  = true;
            isCleaning = false;

            // Ganti sprite
            if (cleanSprite != null) sr.sprite = cleanSprite;

            // Sembunyikan progress bar
            progressBarRoot?.SetActive(false);

            // Efek visual
            if (cleanEffect != null)
                Instantiate(cleanEffect, transform.position, Quaternion.identity);

            // Tambah skor
            Core.ScoreManager.Instance?.AddCleanScore();

            // Beritahu level manager
            Core.LevelManager.Instance?.RegisterCleaned();

            // Tampilkan score popup
            UI.ScorePopupUI.Show(transform.position, Core.ScoreManager.Instance?.GetCleanScore() ?? 10);

            Debug.Log($"[CleanableObject] {gameObject.name} selesai dibersihkan.");
        }

        // ─── Debug ────────────────────────────────────────────────────────────
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }
}
