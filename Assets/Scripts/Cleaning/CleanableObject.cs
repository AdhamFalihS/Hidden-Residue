using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace HiddenResidue.Interaction
{
    public class CleanableObject : MonoBehaviour, IInteractable
    {
        public enum CleanMode
        {
            ChangeObject,
            DestroyObject
        }

        [Header("Mode")]
        [SerializeField] private CleanMode cleanMode = CleanMode.ChangeObject;

        [Header("Objects")]
        [SerializeField] private GameObject dirtyObject;
        [SerializeField] private GameObject cleanObject;

        [Header("Cleaning")]

        [SerializeField] private float cleanDuration = 2.5f;

        [SerializeField] private bool holdToClean = false;

        [Header("Interaction")]

        [SerializeField] private float interactionRadius = 1.5f;

        [Header("UI")]

        [SerializeField] private Image progressImage;

        [SerializeField] private TextMeshProUGUI labelText;

        [Header("Effects")]
        [SerializeField] private GameObject cleanEffect;

        [Header("Reward (Opsional)")]
        [Tooltip("Prefab item/barang yang di-spawn setelah bersih")]
        [SerializeField] private GameObject rewardPrefab;

        [Tooltip("Offset spawn reward dari posisi objek (misal: Vector2(0, 1) = di atas objek)")]
        [SerializeField] private Vector2 rewardSpawnOffset = new Vector2(0f, 1f);

        [Tooltip("Jika true, reward hanya bisa di-spawn sekali")]
        [SerializeField] private bool oneTimeReward = true;

        [Tooltip("Teks notifikasi setelah bersih")]
        [SerializeField] private string rewardMessage = "Objek berhasil dibersihkan!";

        [Header("Audio Reward (Opsional)")]
        [SerializeField] private AudioClip rewardSound;

        public bool CanInteract => !isCleaned && !isCleaning;

        public string InteractPrompt => "Tekan E — Bersihkan";

        private bool isCleaned = false;

        private bool isCleaning = false;

        private float progress = 0f;

        private Transform player;

        private bool rewardGiven = false;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;

            if (cleanMode == CleanMode.ChangeObject)
            {
                if (dirtyObject != null) dirtyObject.SetActive(true);
                if (cleanObject != null) cleanObject.SetActive(false);
            }

            if (labelText != null)
                labelText.gameObject.SetActive(false);

            if (progressImage != null)
            {
                progressImage.fillAmount = 0f;
                progressImage.gameObject.SetActive(false);
            }
        }

        public void Interact()

        {

            if (isCleaned || isCleaning) return;

            Core.AudioManager.Instance?.PlaySFXLoop(Core.AudioManager.SFX.Cleaning);

            if (cleanEffect != null) cleanEffect.SetActive(true);

            if (labelText)

            {

                labelText.gameObject.SetActive(true);

                labelText.text = "Cleaning...";

            }

            if (progressImage)

            {

                progressImage.gameObject.SetActive(true);

                progressImage.fillAmount = 0f;

            }

            StartCoroutine(DoClean());

        }

        private IEnumerator DoClean()

        {

            isCleaning = true;

            progress = 0f;

            while (progress < 1f)

            {

                if (player == null)

                {

                    CancelCleaning();

                    yield break;

                }

                Collider2D col = GetComponent<Collider2D>();

                Vector2 closest = col.ClosestPoint(player.position);

                float dist = Vector2.Distance(player.position, closest);

                if (dist > interactionRadius)

                {

                    CancelCleaning();

                    yield break;

                }

                if (holdToClean)
                {
                    if (!UnityEngine.InputSystem.Keyboard.current.eKey.isPressed)
                    {
                        if (cleanEffect != null) cleanEffect.SetActive(false);
                        Core.AudioManager.Instance?.StopSFXLoop();
                        yield return null;
                        continue;
                    }

                    if (cleanEffect != null) cleanEffect.SetActive(true);
                    Core.AudioManager.Instance?.PlaySFXLoop(Core.AudioManager.SFX.Cleaning);
                }

                progress += Time.deltaTime / cleanDuration;

                progress = Mathf.Clamp01(progress);

                if (progressImage)

                    progressImage.fillAmount = progress;

                yield return null;

            }

            FinishCleaning();

        }

        private void CancelCleaning()

        {

            isCleaning = false;

            progress = 0f;

            if (cleanEffect != null) cleanEffect.SetActive(false);

            Core.AudioManager.Instance?.StopSFXLoop();

            if (progressImage)

            {

                progressImage.fillAmount = 0f;

                progressImage.gameObject.SetActive(false);

            }

            if (labelText)

                labelText.gameObject.SetActive(false);

        }

        private void FinishCleaning()

{

    isCleaned = true;

    isCleaning = false;

    if (cleanEffect != null) cleanEffect.SetActive(false);

    if (progressImage) progressImage.gameObject.SetActive(false);

    if (labelText) labelText.gameObject.SetActive(false);

    Core.AudioManager.Instance?.StopSFXLoop();
    Core.AudioManager.Instance?.PlaySFX(Core.AudioManager.SFX.CleaningDone);

    Core.ScoreManager.Instance?.AddCleanScore();

    Core.LevelManager.Instance?.RegisterCleaned();

    UI.ScorePopupUI.Show(

        transform.position,

        Core.ScoreManager.Instance?.GetCleanScore() ?? 10

    );

    if (cleanEffect != null)

        Instantiate(cleanEffect, transform.position, Quaternion.identity);

    if (cleanMode == CleanMode.ChangeObject)
    {
        if (dirtyObject != null) dirtyObject.SetActive(false);
        if (cleanObject != null) cleanObject.SetActive(true);
    }
    else if (cleanMode == CleanMode.DestroyObject)
    {
        Destroy(gameObject);
    }

    SpawnReward();
}

        private void SpawnReward()
        {
            if (oneTimeReward && rewardGiven) return;

            if (rewardPrefab == null)
            {
                Debug.LogWarning("[CleanableObject] rewardPrefab belum di-assign, tidak ada item yang di-spawn.");
                return;
            }

            Vector3 spawnPos = transform.position + (Vector3)rewardSpawnOffset;
            GameObject spawned = Instantiate(rewardPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"[CleanableObject] Reward di-spawn: {spawned.name} di {spawnPos}");

            if (!string.IsNullOrEmpty(rewardMessage))
                UI.NotificationUI.Show(rewardMessage);

            if (rewardSound != null)
            {
                if (audioSource != null)
                    audioSource.PlayOneShot(rewardSound);
                else
                    AudioSource.PlayClipAtPoint(rewardSound, spawnPos);
            }

            if (oneTimeReward)
                rewardGiven = true;
        }

        private void OnDrawGizmosSelected()
        {
            // Hitung posisi spawn final
            Vector3 spawnPos = transform.position + (Vector3)rewardSpawnOffset;

            // 1. Gambar garis putus-putus atau biasa dari NPC ke titik spawn
            Gizmos.color = Color.cyan; // Warna cyan agar beda dengan gizmos interaksi
            Gizmos.DrawLine(transform.position, spawnPos);

            // 2. Gambar bola solid kecil di titik spawn
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(spawnPos, 0.15f);

            // 3. Gambar lingkaran kawat untuk menunjukkan area drop
            Gizmos.DrawWireSphere(spawnPos, 0.2f);

            // 4. Menampilkan label teks di Scene View (Hanya muncul di Editor)
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(spawnPos + new Vector3(0.1f, 0.1f, 0), "Reward Drop Point");
            #endif
        }

    }

}

