using UnityEngine;
using ZillRunner.Collectibles;
using ZillRunner.Core;
using ZillRunner.Obstacles;

namespace ZillRunner.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerCollision : MonoBehaviour
    {
        private PlayerController _player;

        private void Awake() => _player = GetComponent<PlayerController>();

        private void OnTriggerEnter(Collider other)
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
                return;

            if (other.TryGetComponent<Obstacle>(out var obstacle))
            {
                HandleObstacle(obstacle);
                return;
            }

            if (other.TryGetComponent<Coin>(out var coin))
            {
                coin.Collect();
                return;
            }

            if (other.TryGetComponent<PowerUp>(out var powerUp))
            {
                powerUp.Collect();
            }
        }

        private void HandleObstacle(Obstacle obstacle)
        {
            if (_player.IsInvincible) return;

            bool avoided = obstacle.CanAvoid(_player.IsJumping, _player.IsSliding);
            if (avoided)
            {
                obstacle.MarkPassed();
                GameManager.Instance.RegisterObstaclePassed();
            }
            else
            {
                obstacle.OnHit();
                GameManager.Instance.TakeDamage();
            }
        }
    }
}
