using UnityEngine;

namespace Runtime.Player
{
    public class GrappleVFX : MonoBehaviour
    {
        [SerializeField] private TrailRenderer hookTrail;

        [SerializeField] private Transform runtimeObjects;
        [SerializeField] private float hookSpeed = 100f;

        private SpriteRenderer hookSprite;
        
        private Vector2 target;

        private bool isFlying;
        private bool hideSpriteOnFinish;

        private void Awake()
        {
            hookTrail = Instantiate(hookTrail, runtimeObjects);
            hookSprite = hookTrail.GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            MoveTrail();
        }

        private void MoveTrail()
        {
            if (!isFlying)
                return;

            if (Vector2.Distance(hookTrail.transform.position, target) < 0.01f)
            {
                hookTrail.transform.position = target;

                isFlying = false;

                hookTrail.Clear();
                hookTrail.enabled = false;

                if (hideSpriteOnFinish)
                    hookSprite.enabled = false;

                return;
            }

            hookTrail.transform.position = Vector2.MoveTowards(hookTrail.transform.position, target, hookSpeed * Time.deltaTime);
        }

        public void PlayHookTravel(Vector2 start, Vector2 end, bool hideSpriteOnFinish)
        {
            this.hideSpriteOnFinish = hideSpriteOnFinish;
            hookTrail.transform.position = start;

            target = end;

            isFlying = true;

            hookTrail.Clear();
            hookTrail.enabled = true;
            hookSprite.enabled = true;
        }
    }
}
