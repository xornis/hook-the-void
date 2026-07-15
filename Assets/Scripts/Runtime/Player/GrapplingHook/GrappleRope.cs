using UnityEngine;

namespace Runtime.Player
{
    [RequireComponent(typeof(LineRenderer))]
    public class GrappleRope : MonoBehaviour
    {
        private LineRenderer line;

        private Transform grapplePoint;
        private Transform grappleOrigin;

        private void Awake()
        {
            line = GetComponent<LineRenderer>();
            line.enabled = false;
        }

        private void LateUpdate()
        {
            if (!line.enabled)
                return;

            line.SetPosition(0, grappleOrigin.position);
            line.SetPosition(1, grapplePoint.position);
        }

        public void Initialize(Transform grapplePoint, Transform grappleOrigin)
        {
            this.grapplePoint = grapplePoint;
            this.grappleOrigin = grappleOrigin;
        }

        public void Show()
        {
            line.enabled = true;
        }

        public void Hide()
        {
            line.enabled = false;
        }
    }
}