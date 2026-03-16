using UnityEngine;

namespace HiddenResidue.Quiz
{
    /// <summary>
    /// LockedDoor — Pintu atau area terkunci yang membutuhkan kuis untuk dibuka.
    /// Attach ke objek pintu di scene.
    /// Saat E ditekan → QuizManager.StartQuiz() → jika benar → pintu terbuka.
    /// </summary>
    public class LockedDoor : MonoBehaviour, Interaction.IInteractable
    {
        // ─── Inspector ───────────────────────────────────────────────────────
        [Header("Quiz")]
        [SerializeField] private QuizData quizData;   // Drag QuizData SO ke sini

        [Header("Door Behavior")]
        [SerializeField] private GameObject doorObject;          // Objek visual pintu
        [SerializeField] private Collider2D doorBlocker;         // Collider penghalang
        [Tooltip("Kalimat hint sebelum buka kuis")]
        [SerializeField] private string lockedPrompt = "Tekan E — Jawab Kuis untuk Membuka";

        [Header("Animation (opsional)")]
        [SerializeField] private Animator doorAnimator;
        [SerializeField] private string openTrigger = "Open";

        [Header("Audio (opsional)")]
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip lockedSound;

        // ─── IInteractable ────────────────────────────────────────────────────
        public bool   CanInteract    => !isUnlocked;
        public string InteractPrompt => lockedPrompt;

        // ─── State ───────────────────────────────────────────────────────────
        private bool         isUnlocked = false;
        private AudioSource  audioSource;

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        // ─── IInteractable.Interact() ─────────────────────────────────────────
        public void Interact()
        {
            if (isUnlocked || quizData == null) return;

            // Mainkan suara terkunci
            if (lockedSound && audioSource) audioSource.PlayOneShot(lockedSound);

            // Tampilkan kuis
            QuizManager.Instance?.StartQuiz(quizData, OnQuizResult);
        }

        // ─── Callback dari QuizManager ────────────────────────────────────────
        private void OnQuizResult(bool passed)
        {
            if (passed)
                OpenDoor();
            // Jika gagal, GameManager sudah handle TriggerFail()
        }

        private void OpenDoor()
        {
            isUnlocked = true;

            // Animasi pintu terbuka
            if (doorAnimator != null)
                doorAnimator.SetTrigger(openTrigger);

            // Nonaktifkan collider penghalang
            if (doorBlocker != null)
                doorBlocker.enabled = false;

            // Sembunyikan visual pintu (jika tidak ada animasi)
            if (doorAnimator == null && doorObject != null)
                doorObject.SetActive(false);

            // Suara pintu terbuka
            if (openSound && audioSource) audioSource.PlayOneShot(openSound);

            Debug.Log($"[LockedDoor] {gameObject.name} terbuka!");
        }
    }
}
