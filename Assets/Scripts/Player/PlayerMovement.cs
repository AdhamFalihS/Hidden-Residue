using UnityEngine;
using UnityEngine.InputSystem;

namespace HiddenResidue.Player
{
    /// <summary>
    /// PlayerMovement — Mengontrol pergerakan karakter Jojo menggunakan
    /// Unity Input System (Player Input component + Send Messages).
    ///
    /// Setup:
    ///   1. Add komponen "Player Input" ke GameObject Jojo
    ///   2. Buat Input Action Asset → Action Map: "Player" → Action: "Move" (Value, Vector2)
    ///   3. Bind WASD / Arrow Keys ke 2D Composite
    ///   4. Set Behavior: "Send Messages"
    ///   5. Attach script ini ke Jojo
    ///
    /// Komponen yang dibutuhkan di GameObject:
    ///   - Rigidbody2D      (Gravity Scale: 0, Freeze Rotation Z: true)
    ///   - Animator         (Parameter: SpeedX float, SpeedY float)
    ///   - SpriteRenderer   (untuk flipX)
    ///   - CapsuleCollider2D
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerMovement : MonoBehaviour
    {
        // ─── Inspector ───────────────────────────────────────────────────────
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 4f;

        [Header("Animator Parameters")]
        [SerializeField] private string paramSpeedX = "SpeedX";
        [SerializeField] private string paramSpeedY = "SpeedY";

        // ─── Components ──────────────────────────────────────────────────────
        private Rigidbody2D   rb;
        private Animator      animator;
        private SpriteRenderer spriteRenderer;

        // ─── State ───────────────────────────────────────────────────────────
        private Vector2 moveInput;
        private bool    canMove = true;

        // ─────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            rb             = GetComponent<Rigidbody2D>();
            animator       = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Pastikan Rigidbody2D tidak berputar
            rb.freezeRotation = true;
            rb.gravityScale   = 0f;
        }

        private void OnEnable()
        {
            // Dengarkan perubahan state game agar player berhenti saat dialog/quiz
            Core.GameManager.OnGameStateChanged += HandleGameStateChanged;
        }

        private void OnDisable()
        {
            Core.GameManager.OnGameStateChanged -= HandleGameStateChanged;
        }

        // ─── Unity Input System Callback ─────────────────────────────────────
        // Dipanggil otomatis oleh "Player Input" component (behavior: Send Messages)

        private void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        // ─── Physics Update ──────────────────────────────────────────────────
        private void FixedUpdate()
        {
            if (!canMove)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            rb.linearVelocity = moveInput.normalized * moveSpeed;
            UpdateAnimatorAndSprite();
        }

        // ─── Private ─────────────────────────────────────────────────────────

        private void UpdateAnimatorAndSprite()
        {
            // Update Animator
            animator.SetFloat(paramSpeedX, Mathf.Abs(moveInput.x));
            animator.SetFloat(paramSpeedY, moveInput.y);

            // Flip sprite: hanya berdasarkan arah horizontal
            // Jika bergerak ke kiri → flipX = true
            // Jika bergerak ke kanan → flipX = false
            if (moveInput.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (moveInput.x < -0.01f)
                spriteRenderer.flipX = true;
            // Arah atas/bawah: tidak flip, gunakan animasi yang sama
        }

        private void HandleGameStateChanged(Core.GameManager.GameState state)
        {
            // Player tidak bisa bergerak saat dialog, quiz, fail, atau complete
            canMove = state == Core.GameManager.GameState.Playing;
            if (!canMove)
            {
                moveInput         = Vector2.zero;
                rb.linearVelocity = Vector2.zero;
                animator.SetFloat(paramSpeedX, 0f);
                animator.SetFloat(paramSpeedY, 0f);
            }
        }

        // ─── Public API ──────────────────────────────────────────────────────
        public void SetCanMove(bool value) => canMove = value;
        public bool IsMoving => moveInput.sqrMagnitude > 0.01f;
        public Vector2 MoveInput => moveInput;
    }
}
