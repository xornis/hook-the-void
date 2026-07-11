using UnityEngine;

namespace Runtime.Input
{
    public class PlayerInputReader : MonoBehaviour
    {
        private PlayerInputActions input;

        public Vector2 Move { get; private set; }
        public bool JumpPressed { get; private set; }

        private void Awake()
        {
            input = new PlayerInputActions();

            input.Player.Move.performed += ctx => Move = ctx.ReadValue<Vector2>();

            input.Player.Move.canceled += ctx => Move = Vector2.zero;

            input.Player.Jump.performed += ctx => JumpPressed = true;
        }

        private void OnEnable()
        {
            input.Enable();
        }

        private void OnDisable()
        {
            input.Disable();
        }

        public void ConsumeJump() => JumpPressed = false;
    }
}
