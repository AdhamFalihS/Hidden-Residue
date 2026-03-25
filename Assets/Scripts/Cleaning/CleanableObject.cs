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

            ChangeSprite,

            DestroyObject

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

            if (p != null)

                player = p.transform;

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

    if (progressImage) progressImage.gameObject.SetActive(false);

    if (labelText) labelText.gameObject.SetActive(false);

    Core.ScoreManager.Instance?.AddCleanScore();

    Core.LevelManager.Instance?.RegisterCleaned();

    UI.ScorePopupUI.Show(

        transform.position,

        Core.ScoreManager.Instance?.GetCleanScore() ?? 10

    );

    if (cleanEffect != null)

        Instantiate(cleanEffect, transform.position, Quaternion.identity);

    if (cleanMode == CleanMode.ChangeSprite)

    {

        if (cleanSprite != null)

            sr.sprite = cleanSprite;

    }

    else if (cleanMode == CleanMode.DestroyObject)

    {

        Destroy(gameObject);

    }

}

        private void OnDrawGizmosSelected()

        {

            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(transform.position, interactionRadius);

        }

    }

}