using Runtime.Input;
using UnityEngine;

namespace Runtime.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(GrapplingHook))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Header("Acceleration")]
        [SerializeField] private float groundAcceleration = 18f;
        [SerializeField] private float airAcceleration = 5f;
        [SerializeField] private float grappleAcceleration = 10f;

        [Header("Speed Limits")]
        [SerializeField] private float maxGroundSpeed = 10f;
        [SerializeField] private float maxAirSpeed = 50f;

        [Header("Jump Settings")]
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
        private GrapplingHook grapplingHook;

        private float coyoteTimer;
        private float jumpBufferTimer;
        private bool jumpCutApplied;

        private bool IsGrounded => Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        private Vector2 Velocity => rb.linearVelocity;
        private float HorizontalVelocity => Velocity.x;

        private float HorizontalInput => input.Move.x;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            input = GetComponent<PlayerInputReader>();
            grapplingHook = GetComponent<GrapplingHook>();
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
            if (grapplingHook.IsAttached)
                GrappleMove();
            else if (IsGrounded)
                GroundMove();
            else
                AirMove();
        }

        private void GrappleMove()
        {
            Vector2 grappleDirection = (grapplingHook.GrapplePoint - rb.position).normalized;
            Vector2 swingDirection = new Vector2(grappleDirection.y, -grappleDirection.x) * HorizontalInput;

            rb.AddForce(swingDirection * grappleAcceleration, ForceMode2D.Force);

            Debug.DrawRay(rb.position, grappleDirection, Color.red);
            Debug.DrawRay(rb.position, swingDirection, Color.blue);
        }

        private void GroundMove()
        {
            float horizontalSpeed = Mathf.Abs(HorizontalVelocity);

            bool movingSameDirection = HorizontalInput * HorizontalVelocity > 0;
            bool reachedMaxSpeed = horizontalSpeed >= maxGroundSpeed;

            if (movingSameDirection && reachedMaxSpeed)
                return;

            AddHorizontalForce(groundAcceleration);
        }

        private void AirMove()
        {
            float horizontalSpeed = Mathf.Abs(HorizontalVelocity);

            bool movingSameDirection = HorizontalInput * HorizontalVelocity > 0;
            bool reachedMaxAirSpeed = horizontalSpeed >= maxAirSpeed;

            if (reachedMaxAirSpeed && movingSameDirection)
                return;

            AddHorizontalForce(airAcceleration);
        }

        private void AddHorizontalForce(float acceleration) => rb.AddForce(HorizontalInput * acceleration * Vector2.right, ForceMode2D.Force);

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