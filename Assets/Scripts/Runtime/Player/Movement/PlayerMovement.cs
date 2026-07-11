using UnityEngine;
using Runtime.Input;

namespace Runtime.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float speed = 8f;
        [SerializeField] private float jumpForce = 12f;

        [Header("Jump & Grounding Settings")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundRadius = 0.15f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float coyoteTime = 0.1f;
        [SerializeField] private float jumpBuffer = 0.1f;

        private Rigidbody2D rb;
        private PlayerInputReader input;

        private float coyoteTimer;
        private float jumpBufferTimer;

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
        }

        private void Move()
        {
            Vector2 velocity = rb.linearVelocity;

            velocity.x = input.Move.x * speed;

            rb.linearVelocity = velocity;
        }

        private void Jump()
        {
            Vector2 velocity = rb.linearVelocity;

            velocity.y = jumpForce;

            rb.linearVelocity = velocity;
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