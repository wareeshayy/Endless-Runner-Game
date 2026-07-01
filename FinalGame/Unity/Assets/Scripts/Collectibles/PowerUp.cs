using UnityEngine;
using ZillRunner.Core;

namespace ZillRunner.Collectibles
{
    public enum PowerUpType
    {
        Shield,
        Magnet,
        ScoreBoost
    }

    public class PowerUp : MonoBehaviour
    {
        [SerializeField] private PowerUpType type = PowerUpType.Shield;
        [SerializeField] private float duration = 6f;
        [SerializeField] private float despawnZ = -10f;

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
                return;

            transform.Rotate(Vector3.up, 120f * Time.deltaTime);
            transform.position += Vector3.back * GameManager.Instance.RunSpeed * Time.deltaTime;

            if (transform.position.z < despawnZ)
                Destroy(gameObject);
        }

        public void Initialize(PowerUpType powerUpType, int lane)
        {
            type = powerUpType;
            int center = GameConstants.Lanes / 2;
            float x = (lane - center) * GameConstants.LaneWidth;
            transform.position = new Vector3(x, 1.2f, 65f);
            ApplyColor();
        }

        private void ApplyColor()
        {
            var renderer = GetComponent<Renderer>();
            if (renderer == null) return;
            Color color = type switch
            {
                PowerUpType.Shield => new Color(0.2f, 0.9f, 1f),
                PowerUpType.Magnet => new Color(1f, 0.4f, 0.9f),
                PowerUpType.ScoreBoost => new Color(1f, 0.85f, 0.2f),
                _ => Color.white
            };
            if (renderer.material.HasProperty("_Color"))
                renderer.material.color = color;
        }

        public void Collect()
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            switch (type)
            {
                case PowerUpType.Shield:
                    gm.ActivateShield(duration);
                    break;
                case PowerUpType.Magnet:
                    gm.ActivateMagnet(duration);
                    break;
                case PowerUpType.ScoreBoost:
                    gm.AddScore(500);
                    break;
            }

            Audio.AudioManager.Instance?.PlayPowerUp();
            Destroy(gameObject);
        }
    }
}
