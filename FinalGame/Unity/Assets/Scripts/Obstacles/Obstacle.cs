using UnityEngine;
using ZillRunner.Core;

namespace ZillRunner.Obstacles
{
    public class Obstacle : MonoBehaviour
    {
        [SerializeField] private ObstacleType type = ObstacleType.FullBarrier;
        [SerializeField] private float despawnZ = -15f;
        [SerializeField] private ParticleSystem hitParticles;

        public ObstacleType Type => type;

        public bool CanAvoid(bool isJumping, bool isSliding)
        {
            return type switch
            {
                ObstacleType.FullBarrier => isJumping || isSliding,
                ObstacleType.JumpBarrier => isJumping,
                _ => false
            };
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
                return;

            transform.position += Vector3.back * GameManager.Instance.RunSpeed * Time.deltaTime;

            if (transform.position.z < despawnZ)
                ReturnToPool();
        }

        public void MarkPassed() { }

        public void OnHit()
        {
            if (hitParticles != null)
                Instantiate(hitParticles, transform.position, Quaternion.identity);
        }

        public void Initialize(ObstacleType obstacleType, int lane)
        {
            type = obstacleType;

            int center = GameConstants.Lanes / 2;
            float x = (lane - center) * GameConstants.LaneWidth;
            transform.position = new Vector3(x, 0f, 60f);
            ApplyVisualStyle();
        }

        private void ApplyVisualStyle()
        {
            var renderer = GetComponentInChildren<Renderer>();
            if (renderer == null) return;

            Color color = type == ObstacleType.FullBarrier
                ? new Color(0.2f, 0.6f, 1f)
                : new Color(1f, 0.35f, 0.2f);
            if (renderer.material.HasProperty("_Color"))
                renderer.material.color = color;
        }

        private void ReturnToPool()
        {
            if (ObstaclePool.Instance != null)
                ObstaclePool.Instance.Release(this);
            else
                Destroy(gameObject);
        }

        public void ResetState() { }
    }
}
