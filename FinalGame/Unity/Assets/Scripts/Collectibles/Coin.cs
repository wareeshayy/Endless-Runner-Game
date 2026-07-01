using UnityEngine;
using ZillRunner.Core;

namespace ZillRunner.Collectibles
{
    public class Coin : MonoBehaviour
    {
        [SerializeField] private int value = GameConstants.CoinValue;
        [SerializeField] private float despawnZ = -10f;
        [SerializeField] private float rotateSpeed = 180f;

        private Transform _player;

        private void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) _player = player.transform;
        }

        private void Update()
        {
            if (GameManager.Instance == null) return;

            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

            if (GameManager.Instance.State == GameState.Playing)
            {
                transform.position += Vector3.back * GameManager.Instance.RunSpeed * Time.deltaTime;

                if (GameManager.Instance.HasMagnet && _player != null)
                {
                    float dist = Vector3.Distance(transform.position, _player.position);
                    if (dist < GameManager.Instance.MagnetRadius)
                    {
                        transform.position = Vector3.MoveTowards(
                            transform.position,
                            _player.position + Vector3.up,
                            15f * Time.deltaTime);
                    }
                }
            }

            if (transform.position.z < despawnZ)
                Destroy(gameObject);
        }

        public void Collect()
        {
            GameManager.Instance?.CollectCoin(value);
            Audio.AudioManager.Instance?.PlayCoin();
            Destroy(gameObject);
        }
    }
}
