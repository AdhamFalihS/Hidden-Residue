using UnityEngine;

using UnityEngine.InputSystem;

namespace HiddenResidue.Interaction

{

    public class InteractionDetector : MonoBehaviour

    {

        [Header("Detection")]

        [SerializeField] private float   detectRadius = 1.5f;

        [SerializeField] private LayerMask interactLayer;

        [Header("Indicator")]

        [SerializeField] private GameObject pressEIndicator;

        private IInteractable currentTarget;

        private GameObject    currentTargetGO;

        private void Update()

        {

            if (Core.GameManager.Instance != null && !Core.GameManager.Instance.IsPlaying)

            {

                HideIndicator();

                return;

            }

            FindNearestInteractable();

            HandleIndicator();

            HandleInput();

        }

        private void FindNearestInteractable()

        {

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius, interactLayer);

            IInteractable closest   = null;

            GameObject    closestGO = null;

            float minDist           = float.MaxValue;

            foreach (var hit in hits)

            {

                if (hit.TryGetComponent<IInteractable>(out var interactable) && interactable.CanInteract)

                {

                    float dist = Vector2.Distance(transform.position, hit.transform.position);

                    if (dist < minDist)

                    {

                        minDist   = dist;

                        closest   = interactable;

                        closestGO = hit.gameObject;

                    }

                }

            }

            currentTarget   = closest;

            currentTargetGO = closestGO;

        }

        private void HandleIndicator()

        {

            if (pressEIndicator == null) return;

            if (currentTarget != null)

            {

                pressEIndicator.SetActive(true);

                if (currentTargetGO != null)

                {

                    Vector3 pos = currentTargetGO.transform.position + Vector3.up * 1.2f;

                    pressEIndicator.transform.position = pos;

                }

                var txt = pressEIndicator.GetComponentInChildren<TMPro.TextMeshProUGUI>();

                if (txt != null)

                    txt.text = currentTarget.InteractPrompt;

            }

            else

            {

                HideIndicator();

            }

        }

        private void HandleInput()

        {

            if (currentTarget == null) return;

            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)

            {

                currentTarget.Interact();

            }

        }

        private void HideIndicator()

        {

            pressEIndicator?.SetActive(false);

        }

        private void OnDrawGizmosSelected()

        {

            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(transform.position, detectRadius);

        }

    }

}