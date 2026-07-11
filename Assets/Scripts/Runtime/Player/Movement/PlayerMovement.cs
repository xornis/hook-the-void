using UnityEngine;
using Runtime.Input;

namespace Runtime.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private float speed = 8f;

        [SerializeField]
        private float jumpForce = 12f;

        private Rigidbody2D rb;
        private PlayerInputReader input;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            input = GetComponent<PlayerInputReader>();
        }

        private void FixedUpdate()
        {
            Move();

            if (input.JumpPressed)
            {
                Jump();
                input.ConsumeJump();
            }
        }

        private void Move()
        {
            Vector2 velocity = rb.linearVelocity;

            velocity.x = input.Move.x * speed;

            rb.linearVelocity = velocity;
        }

        private void Jump()
        {
            rb.AddForce(
                Vector2.up * jumpForce,
                ForceMode2D.Impulse
            );
        }
    }
}