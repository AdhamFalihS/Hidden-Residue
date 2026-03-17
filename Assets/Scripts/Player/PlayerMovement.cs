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

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();

            rb.freezeRotation = true;
            rb.gravityScale = 0f;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
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
                return;
            }

            Vector2 targetVelocity = moveInput.normalized * moveSpeed;
            float rate = moveInput.sqrMagnitude > 0.01f ? acceleration : deceleration;

            currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, rate * Time.fixedDeltaTime);
            rb.linearVelocity = currentVelocity;

            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            // Arah (untuk Blend Tree 2D)
            animator.SetFloat(paramMoveX, moveInput.x);
            animator.SetFloat(paramMoveY, moveInput.y);

            // Speed untuk idle / jalan
            animator.SetFloat(paramSpeed, moveInput.sqrMagnitude);

            // Flip sprite (opsional, kalau pakai animasi kanan saja)
            if (moveInput.x > 0.01f)
                sr.flipX = false;
            else if (moveInput.x < -0.01f)
                sr.flipX = true;
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