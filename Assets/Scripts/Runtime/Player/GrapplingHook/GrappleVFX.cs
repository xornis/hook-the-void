using UnityEngine;

namespace Runtime.Player
{
    public class GrappleVFX : MonoBehaviour
    {
        [SerializeField] private Transform hook;

        [SerializeField] private Transform runtimeObjects;
        [SerializeField] private float hookSpeed = 100f;

        private TrailRenderer hookTrail;
        private SpriteRenderer hookSprite;
        private ParticleSystem hookParticles;
        
        private Vector2 target;

        private bool isFlying;
        private bool hideSpriteOnFinish;

        private void Awake()
        {
            hook = Instantiate(hook, runtimeObjects);

            hookSprite = hook.GetComponent<SpriteRenderer>();
            hookTrail = hook.GetComponent<TrailRenderer>();
            hookParticles = hook.GetComponent<ParticleSystem>();

            hookSprite.enabled = false;
            hookTrail.enabled = false;
        }

        private void Update()
        {
            MoveTrail();
        }

        private void MoveTrail()
        {
            if (!isFlying)
                return;

            if (Vector2.Distance(hook.position, target) < 0.01f)
            {
                hook.position = target;

                isFlying = false;

                hookTrail.Clear();
                hookTrail.enabled = false;

                if (hideSpriteOnFinish)
                {
                    hookSprite.enabled = false;
                    hookParticles.Stop();
                }

                return;
            }

            hook.position = Vector2.MoveTowards(hook.position, target, hookSpeed * Time.deltaTime);
        }

        public void PlayHookTravel(Vector2 start, Vector2 end, bool hideSpriteOnFinish)
        {
            this.hideSpriteOnFinish = hideSpriteOnFinish;
            hook.position = start;

            target = end;

            isFlying = true;

            hookTrail.Clear();
            hookTrail.enabled = true;
            hookSprite.enabled = true;
            hookParticles.Play();
        }
    }
}
