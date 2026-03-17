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
            ChangeSprite,   // Dirty → Clean
            DestroyObject   // Object hilang
        }

        [Header("Mode")]
        [SerializeField] private CleanMode cleanMode = CleanMode.ChangeSprite;

        [Header("Sprites")]
        [SerializeField] private Sprite dirtySprite;
        [SerializeField] private Sprite cleanSprite;

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

        public bool CanInteract => !isCleaned && !isCleaning;
        public string InteractPrompt => "Tekan E — Bersihkan";

        private bool isCleaned = false;
        private bool isCleaning = false;
        private float progress = 0f;

        private SpriteRenderer sr;
        private Transform player;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();

            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;

            // Set awal sprite dirty
            if (cleanMode == CleanMode.ChangeSprite && dirtySprite != null)
                sr.sprite = dirtySprite;

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
                    yield break;

                float dist = Vector2.Distance(player.position, transform.position);

                if (dist > interactionRadius)
                {
                    CancelCleaning();
                    yield break;
                }

                if (holdToClean && !UnityEngine.InputSystem.Keyboard.current.eKey.isPressed)
                {
                    yield return null;
                    continue;
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

            if (progressImage)
                progressImage.fillAmount = 0f;

            if (labelText)
                labelText.gameObject.SetActive(false);

            if (progressImage)
                progressImage.gameObject.SetActive(false);
        }

        private void FinishCleaning()
        {
            isCleaned = true;
            isCleaning = false;

            // 🔥 MODE 1: Change Sprite
            if (cleanMode == CleanMode.ChangeSprite)
            {
                if (cleanSprite != null)
                    sr.sprite = cleanSprite;
            }
            // 🔥 MODE 2: Destroy Object
            else if (cleanMode == CleanMode.DestroyObject)
            {
                if (cleanEffect != null)
                    Instantiate(cleanEffect, transform.position, Quaternion.identity);

                Destroy(gameObject);
                return;
            }

            // Effect
            if (cleanEffect != null)
                Instantiate(cleanEffect, transform.position, Quaternion.identity);

            if (labelText)
                labelText.gameObject.SetActive(false);

            if (progressImage)
                progressImage.gameObject.SetActive(false);

            Core.ScoreManager.Instance?.AddCleanScore();
            Core.LevelManager.Instance?.RegisterCleaned();

            UI.ScorePopupUI.Show(transform.position, Core.ScoreManager.Instance?.GetCleanScore() ?? 10);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}