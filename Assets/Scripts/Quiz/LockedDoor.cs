using UnityEngine;

namespace HiddenResidue.Quiz
{
    /// <summary>
    /// LockedDoor — Pintu yang memiliki dua state visual (Locked & Open).
    /// Attach script ini ke Parent Object (misal: QuizDoor).
    /// </summary>
    public class LockedDoor : MonoBehaviour, Interaction.IInteractable
    {
        [Header("Quiz Data")]
        [SerializeField] private QuizData quizData;

        [Header("Door Visual States")]
        [Tooltip("GameObject untuk visual pintu tertutup (biasanya punya Collider2D)")]
        [SerializeField] private GameObject lockedVisual; 
        
        [Tooltip("GameObject untuk visual pintu terbuka (biasanya tanpa Collider2D)")]
        [SerializeField] private GameObject openVisual;

        [Header("Settings")]
        [SerializeField] private string lockedPrompt = "Tekan E — Jawab Kuis untuk Membuka";

        [Header("Audio (Opsional)")]
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip lockedSound;

        // ─── IInteractable Implementation ──────────────────────────────────────
        public bool CanInteract => !isUnlocked;
        public string InteractPrompt => lockedPrompt;

        // ─── Private State ──────────────────────────────────────────────────────
        private bool isUnlocked = false;
        private AudioSource audioSource;

        private void Awake()
        {
            // Mengambil AudioSource dari object ini (jika ada)
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            // Setup kondisi awal pintu saat game dimulai
            if (lockedVisual != null) lockedVisual.SetActive(true);
            if (openVisual != null)   openVisual.SetActive(false);
        }

        public void Interact()
        {
            if (isUnlocked || quizData == null) return;

            // Suara saat mencoba buka pintu yang masih terkunci
            if (lockedSound && audioSource) audioSource.PlayOneShot(lockedSound);

            // Munculkan UI Kuis
            QuizManager.Instance?.StartQuiz(quizData, OnQuizResult);
        }

        private void OnQuizResult(bool passed)
        {
            if (passed)
            {
                OpenDoor();
            }
        }

        private void OpenDoor()
        {
            isUnlocked = true;

            // Tukar visual: Matikan yang terkunci, nyalakan yang terbuka
            if (lockedVisual != null) lockedVisual.SetActive(false);
            if (openVisual != null)   openVisual.SetActive(true);

            // Play SFX Terbuka
            if (openSound && audioSource) audioSource.PlayOneShot(openSound);

            Debug.Log($"[LockedDoor] {gameObject.name} berhasil dibuka!");
        }
    }
}