using UnityEngine;

namespace HiddenResidue.Quiz
{
    public class NPCQuizGiver : MonoBehaviour, Interaction.IInteractable
    {
        [Header("Quiz")]
        [SerializeField] private QuizData quizData;

        [Header("Reward Item")]
        [Tooltip("Prefab item/barang yang di-drop NPC setelah quiz selesai dengan benar")]
        [SerializeField] private GameObject rewardPrefab;

        [Tooltip("Offset spawn reward dari posisi NPC (misal: Vector2(0, -1) = di bawah NPC)")]
        [SerializeField] private Vector2 rewardSpawnOffset = new Vector2(0f, -1f);

        [Tooltip("Jika true, reward hanya bisa diberikan sekali")]
        [SerializeField] private bool oneTimeReward = true;

        [Header("NPC Prompt")]
        [SerializeField] private string interactPrompt = "Tekan E — Bicara dengan NPC";
        [SerializeField] private string alreadyGivenPrompt = "Sudah memberikan hadiah.";

        [Header("Dialog (Opsional)")]
        [Tooltip("Teks singkat yang muncul di NotificationUI sebelum quiz")]
        [SerializeField] private string preQuizMessage = "Jawab pertanyaanku untuk mendapatkan hadiah!";

        [Tooltip("Teks singkat yang muncul di NotificationUI setelah berhasil")]
        [SerializeField] private string postQuizMessage = "Bagus! Ini hadiahmu.";

        [Header("Audio (Opsional)")]
        [SerializeField] private AudioClip rewardSound;

        public bool CanInteract => !(oneTimeReward && rewardGiven);
        public string InteractPrompt => (oneTimeReward && rewardGiven) ? alreadyGivenPrompt : interactPrompt;

        private bool rewardGiven = false;
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Interact()
        {
            if (oneTimeReward && rewardGiven) return;

            if (quizData == null)
            {
                Debug.LogError("[NPCQuizGiver] QuizData belum di-assign!");
                return;
            }

            if (QuizManager.Instance == null)
            {
                Debug.LogError("[NPCQuizGiver] QuizManager tidak ada di scene!");
                return;
            }

            if (!string.IsNullOrEmpty(preQuizMessage))
                UI.NotificationUI.Show(preQuizMessage);

            QuizManager.Instance.StartQuiz(quizData, OnQuizResult);
        }

        private void OnQuizResult(bool passed)
        {
            if (!passed) return;

            SpawnReward();

            if (!string.IsNullOrEmpty(postQuizMessage))
                UI.NotificationUI.Show(postQuizMessage);

            if (oneTimeReward)
                rewardGiven = true;
        }

        private void SpawnReward()
        {
            if (rewardPrefab == null)
            {
                Debug.LogWarning("[NPCQuizGiver] rewardPrefab belum di-assign, tidak ada item yang di-drop.");
                return;
            }

            Vector3 spawnPos = transform.position + (Vector3)rewardSpawnOffset;
            GameObject spawned = Instantiate(rewardPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"[NPCQuizGiver] Reward di-spawn: {spawned.name} di {spawnPos}");

            if (rewardSound != null)
            {
                if (audioSource != null)
                    audioSource.PlayOneShot(rewardSound);
                else
                    AudioSource.PlayClipAtPoint(rewardSound, spawnPos);
            }

        }

        private void OnDrawGizmosSelected()

        {

            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(transform.position + (Vector3)rewardSpawnOffset, 0.2f);

            Gizmos.DrawLine(transform.position, transform.position + (Vector3)rewardSpawnOffset);

        }

    }

}