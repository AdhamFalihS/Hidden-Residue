using UnityEngine;
using UnityEngine.InputSystem;

namespace HiddenResidue.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float acceleration = 15f;
        [SerializeField] private float deceleration = 20f;

        [Header("Depth Scale Settings")]
        [SerializeField] private float scaleMultiplierMin = 0.8f;

        [SerializeField] private float scaleMultiplierMax = 1.2f;

        [SerializeField] private float minY = -5f;

        [SerializeField] private float maxY = 5f;

        [Header("Ignore Scale Objects")]

        [SerializeField] private Transform[] ignoreScaleObjects;

        private Vector3[] ignoreBaseScales;

        [Header("Animator Parameters")]

        [SerializeField] private string paramMoveX = "MoveX";

        [SerializeField] private string paramMoveY = "MoveY";

        [SerializeField] private string paramSpeed = "Speed";

        private Rigidbody2D rb;

        private Animator animator;

        private SpriteRenderer sr;

        private Vector2 moveInput;

        private Vector2 currentVelocity;

        private bool canMove = true;

        private Vector3 baseScale;

        private void Awake()

        {

            rb = GetComponent<Rigidbody2D>();

            animator = GetComponent<Animator>();

            sr = GetComponent<SpriteRenderer>();

            rb.freezeRotation = true;

            rb.gravityScale = 0f;

            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            baseScale = transform.localScale;

            if (ignoreScaleObjects != null && ignoreScaleObjects.Length > 0)

            {

                ignoreBaseScales = new Vector3[ignoreScaleObjects.Length];

                for (int i = 0; i < ignoreScaleObjects.Length; i++)

                {

                    if (ignoreScaleObjects[i] != null)

                        ignoreBaseScales[i] = ignoreScaleObjects[i].localScale;

                }

            }

        }

        private void OnEnable()  => Core.GameManager.OnGameStateChanged += HandleGameStateChanged;

        private void OnDisable() => Core.GameManager.OnGameStateChanged -= HandleGameStateChanged;

        private void OnMove(InputValue value)

        {

            moveInput = value.Get<Vector2>();

        }

        private void FixedUpdate()

        {

            if (!canMove)

            {

                currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);

                rb.linearVelocity = currentVelocity;

                UpdateAnimator();

                UpdateScaleByY();

                return;

            }

            Vector2 targetVelocity = moveInput.normalized * moveSpeed;

            float rate = moveInput.sqrMagnitude > 0.01f ? acceleration : deceleration;

            currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, rate * Time.fixedDeltaTime);

            rb.linearVelocity = currentVelocity;

            UpdateAnimator();

            UpdateScaleByY();

        }

        private void UpdateAnimator()

        {

            animator.SetFloat(paramMoveX, moveInput.x);

            animator.SetFloat(paramMoveY, moveInput.y);

            animator.SetFloat(paramSpeed, moveInput.sqrMagnitude);

            if (moveInput.x > 0.01f)

                sr.flipX = false;

            else if (moveInput.x < -0.01f)

                sr.flipX = true;

        }

        private void UpdateScaleByY()

        {

            float t = Mathf.InverseLerp(maxY, minY, transform.position.y);

            float multiplier = Mathf.Lerp(scaleMultiplierMin, scaleMultiplierMax, t);

            transform.localScale = baseScale * multiplier;

            if (ignoreScaleObjects != null && ignoreBaseScales != null)

            {

                for (int i = 0; i < ignoreScaleObjects.Length; i++)

                {

                    if (ignoreScaleObjects[i] == null) continue;

                    ignoreScaleObjects[i].localScale =

                        ignoreBaseScales[i] / multiplier;

                }

            }

        }

        private void HandleGameStateChanged(Core.GameManager.GameState state)

        {

            canMove = state == Core.GameManager.GameState.Playing;

            if (!canMove)

            {

                moveInput = Vector2.zero;

                currentVelocity = Vector2.zero;

                rb.linearVelocity = Vector2.zero;

                animator.SetFloat(paramMoveX, 0f);

                animator.SetFloat(paramMoveY, 0f);

                animator.SetFloat(paramSpeed, 0f);

            }

        }

        public void SetCanMove(bool value) => canMove = value;

        public bool IsMoving => moveInput.sqrMagnitude > 0.01f;

        public Vector2 MoveInput => moveInput;

    }

}