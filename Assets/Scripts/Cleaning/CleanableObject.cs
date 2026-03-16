using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace HiddenResidue.Interaction
{
    public class CleanableObject : MonoBehaviour, IInteractable
    {
        [Header("Sprites")]
        [SerializeField] private Sprite dirtySprite;
        [SerializeField] private Sprite cleanSprite;

        [Header("Cleaning")]
        [SerializeField] private float cleanDuration = 2.5f;
        [SerializeField] private bool holdToClean = false;

        [Header("UI")]
        [SerializeField] private GameObject progressBarRoot;
        [SerializeField] private Image progressImage; // ganti slider jadi image
        [SerializeField] private TextMeshProUGUI labelText;

        [Header("Effects")]
        [SerializeField] private GameObject cleanEffect;

        public bool CanInteract => !isCleaned && !isCleaning;
        public string InteractPrompt => "Tekan E — Bersihkan";

        private bool isCleaned = false;
        private bool isCleaning = false;
        private float progress = 0f;
        private SpriteRenderer sr;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            if (dirtySprite != null) sr.sprite = dirtySprite;

            progressBarRoot?.SetActive(false);

            if (progressImage != null)
                progressImage.fillAmount = 0f;
        }

        public void Interact()
        {
            if (isCleaned || isCleaning) return;
            StartCoroutine(DoClean());
        }

        private IEnumerator DoClean()
        {
            isCleaning = true;
            progress = 0f;

            progressBarRoot?.SetActive(true);
            if (labelText) labelText.text = "Membersihkan...";

            while (progress < 1f)
            {
                if (holdToClean && !UnityEngine.InputSystem.Keyboard.current.eKey.isPressed)
                {
                    yield return null;
                    continue;
                }

                progress += Time.deltaTime / cleanDuration;
                progress = Mathf.Clamp01(progress);

                if (progressImage != null)
                    progressImage.fillAmount = progress;

                yield return null;
            }

            FinishCleaning();
        }

        private void FinishCleaning()
        {
            isCleaned = true;
            isCleaning = false;

            if (cleanSprite != null)
                sr.sprite = cleanSprite;

            progressBarRoot?.SetActive(false);

            if (cleanEffect != null)
                Instantiate(cleanEffect, transform.position, Quaternion.identity);

            Core.ScoreManager.Instance?.AddCleanScore();
            Core.LevelManager.Instance?.RegisterCleaned();

            UI.ScorePopupUI.Show(transform.position, Core.ScoreManager.Instance?.GetCleanScore() ?? 10);

            Debug.Log($"[CleanableObject] {gameObject.name} selesai dibersihkan.");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }
}