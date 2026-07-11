using Runtime.Input;
using UnityEngine;

namespace Runtime.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement & Jump Settings")]
        [SerializeField] private float speed = 8f;
        [SerializeField] private float deceleration = 10f;
        [SerializeField] private float jumpForce = 12f;

        [Header("Ground Check Settings")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundRadius = 0.15f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Coyote Time Settings")]
        [SerializeField] private float coyoteTime = 0.1f;

        [Header("Jump Buffer Settings")]
        [SerializeField] private float jumpBuffer = 0.1f;

        [Header("Variable Jump Height Settings")]
        [SerializeField] private float jumpCutMultiplier = 0.5f;

        [Header("Gravity Settings")]
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2f;

        private Rigidbody2D rb;
        private PlayerInputReader input;

        private float coyoteTimer;
        private float jumpBufferTimer;
        private bool jumpCutApplied;

        private bool IsGrounded => Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            input = GetComponent<PlayerInputReader>();
        }

        private void FixedUpdate()
        {
            HandleJumpInput();
            UpdateTimers();
            Move();
            TryJump();
            VariableJump();
            BetterGravity();
        }

        private void Move()
        {
            Vector2 velocity = rb.linearVelocity;

            if (Mathf.Abs(input.Move.x) > 0.01f)
                velocity.x = Mathf.Lerp(velocity.x, input.Move.x * speed, Time.fixedDeltaTime * deceleration);
            else
                velocity.x = Mathf.Lerp(velocity.x, 0f, Time.fixedDeltaTime * deceleration);

            rb.linearVelocity = velocity;
        }

        private void Jump()
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.y = jumpForce;
            rb.linearVelocity = velocity;

            jumpCutApplied = false;
        }

        private void UpdateTimers()
        {
            if (IsGrounded)
                coyoteTimer = coyoteTime;
            else
                coyoteTimer = Mathf.Max(0, coyoteTimer - Time.fixedDeltaTime);

            jumpBufferTimer = Mathf.Max(0, jumpBufferTimer - Time.fixedDeltaTime);
        }

        private void TryJump()
        {
            if (jumpBufferTimer <= 0)
                return;

            if (coyoteTimer <= 0)
                return;

            Jump();

            jumpBufferTimer = 0;
            coyoteTimer = 0;
        }

        private void HandleJumpInput()
        {
            if (!input.JumpPressed)
                return;

            jumpBufferTimer = jumpBuffer;
            input.ConsumeJump();
        }

        private void VariableJump()
        {
            if (jumpCutApplied)
                return;

            if (input.JumpHeld)
                return;

            if (rb.linearVelocity.y <= 0)
                return;

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier);

            jumpCutApplied = true;
        }

        private void BetterGravity()
        {
            if (rb.linearVelocity.y < 0f)
            {
                rb.linearVelocity += Vector2.up *
                    Physics2D.gravity.y *
                    (fallMultiplier - 1f) *
                    Time.fixedDeltaTime;
            }
            else if (rb.linearVelocity.y > 0f && !input.JumpHeld)
            {
                rb.linearVelocity += Vector2.up *
                    Physics2D.gravity.y *
                    (lowJumpMultiplier - 1f) *
                    Time.fixedDeltaTime;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null)
                return;

            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundRadius);
        }
    }
}