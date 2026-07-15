using UnityEngine;

namespace Runtime.Input
{
    public class PlayerInputReader : MonoBehaviour
    {
        private PlayerInputActions input;

        public Vector2 Move { get; private set; }

        public Vector2 PointerScreenPosition { get; private set; }

        public bool JumpPressed { get; private set; }
        public bool JumpHeld { get; private set; }

        public bool GrapplePressed { get; private set; }
        public bool GrappleHeld { get; private set; }

        private void Awake()
        {
            input = new PlayerInputActions();

            input.Player.Move.performed += ctx => Move = ctx.ReadValue<Vector2>();
            input.Player.Move.canceled += ctx => Move = Vector2.zero;

            input.Player.Jump.started += _ =>
            {
                JumpPressed = true;
                JumpHeld = true;
            };
            input.Player.Jump.canceled += _ => JumpHeld = false;

            input.Player.Grapple.started += _ =>
            {
                GrapplePressed = true;
                GrappleHeld = true;
            };
            input.Player.Grapple.canceled += _ => GrappleHeld = false;

            input.Player.Pointer.performed += ctx => PointerScreenPosition = ctx.ReadValue<Vector2>(); 
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
        public void ConsumeGrapple() => GrapplePressed = false;
    }
}
