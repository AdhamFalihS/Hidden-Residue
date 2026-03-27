using UnityEngine;
using System.Collections;

namespace HiddenResidue.Quiz
{
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

        [Header("Dialog (Opsional)")]
        [Tooltip("Teks yang muncul sebelum quiz dimulai")]
        [SerializeField] private string preQuizMessage = "Jawab kuis untuk membuka pintu!";

        [Header("Audio (Opsional)")]
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip lockedSound;

        public bool CanInteract => !isUnlocked;

        public string InteractPrompt => lockedPrompt;

        private bool isUnlocked = false;
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (lockedVisual != null) lockedVisual.SetActive(true);
            if (openVisual != null) openVisual.SetActive(false);
        }

        public void Interact()
        {
            if (isUnlocked || quizData == null) return;

            StartCoroutine(StartQuizFlow());
        }

        private IEnumerator StartQuizFlow()

        {

            if (!string.IsNullOrEmpty(preQuizMessage))

            {

                UI.NotificationUI.Show(preQuizMessage);

                yield return new WaitForSeconds(1.5f);

            }

            if (lockedSound && audioSource)

                audioSource.PlayOneShot(lockedSound);

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

            if (lockedVisual != null) lockedVisual.SetActive(false);

            if (openVisual != null)   openVisual.SetActive(true);

            if (openSound && audioSource)

                audioSource.PlayOneShot(openSound);

            Debug.Log($"[LockedDoor] {gameObject.name} berhasil dibuka!");

        }

        private void OnDrawGizmosSelected()

        {

            Gizmos.color = Color.cyan;

            Gizmos.DrawWireCube(transform.position, new Vector3(1f, 2f, 0f));

        }

    }

}