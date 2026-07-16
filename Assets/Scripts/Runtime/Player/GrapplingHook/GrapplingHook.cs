using UnityEngine;
using Runtime.Input;

namespace Runtime.Player
{
    [RequireComponent(typeof(DistanceJoint2D))]
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(GrappleRope))]
    public class GrapplingHook : MonoBehaviour
    {
        [SerializeField] private Transform grappleOrigin;
        [SerializeField] private Transform grapplePointPrefab;
        [SerializeField] private Transform runtimeObjects;
        [SerializeField] private LayerMask grappleLayer;
        [SerializeField] private float maxDistance = 6f;

        [SerializeField] private bool showDebug;

        private Transform grapplePoint;

        private DistanceJoint2D joint;
        private GrappleRope rope;

        private PlayerInputReader input;

        public bool IsAttached => joint.enabled;

        public Vector2 GrapplePoint => grapplePoint.position;

        private void Awake()
        {
            grapplePoint = Instantiate(grapplePointPrefab, runtimeObjects);

            joint = GetComponent<DistanceJoint2D>();
            rope = GetComponent<GrappleRope>();
            input = GetComponent<PlayerInputReader>();
         
            rope.Initialize(grapplePoint, grappleOrigin);

            Initialize();
        }

        private void FixedUpdate()
        {
            HandleGrappleInput();
        }

        private void Initialize()
        {
            grapplePoint.gameObject.SetActive(false);
            joint.enabled = false;
        }

        private void Release()
        {
            grapplePoint.gameObject.SetActive(false);
            joint.enabled = false;

            rope.Hide();
        }

        private void Attach(RaycastHit2D hit)
        {
            grapplePoint.gameObject.SetActive(true);
            joint.enabled = true;

            joint.distance = Vector2.Distance(grappleOrigin.position, hit.point);

            grapplePoint.position = hit.point;
            joint.connectedAnchor = hit.point;

            rope.Show();
        }

        private void HandleGrappleInput()
        {
            if (input.GrapplePressed)
            {
                TryAttach();
                input.ConsumeGrapple();
            }
            if (!input.GrappleHeld && IsAttached)
            {
                Release();
            }
        }

        private void TryAttach()
        {
            Vector2 direction = GetGrappleDirection();
            RaycastHit2D hit = Physics2D.Raycast(grappleOrigin.position, direction, maxDistance, grappleLayer);

            if (hit.collider != null)
            {
                Attach(hit);

                if (showDebug)
                {
                    Debug.DrawLine(grappleOrigin.position, hit.point, Color.green, 0.1f);
                    Debug.Log($"Hit: {hit.collider.name}, {hit.point}");
                }
            }
            else
                if (showDebug)
                    Debug.DrawRay(grappleOrigin.position, direction * maxDistance, Color.red, 0.1f);
        }

        private Vector2 GetPointerWorldPosition() => Camera.main.ScreenToWorldPoint(input.PointerScreenPosition); 
    
        private Vector2 GetGrappleDirection() => (GetPointerWorldPosition() - (Vector2)grappleOrigin.position).normalized;
    }
}
