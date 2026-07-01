using UnityEngine;
using ZillRunner.Core;

namespace ZillRunner.Obstacles
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [Header("Spawn timing (frames in C++ were ~30)")]
        [SerializeField] private float fullBarrierInterval = 2.5f;
        [SerializeField] private float jumpBarrierInterval = 4f;
        [SerializeField] private float minGapBetweenSpawns = 0.8f;

        [Header("Advanced")]
        [SerializeField] private bool spawnCoins = true;
        [SerializeField] private float coinSpawnChance = 0.45f;
        [SerializeField] private GameObject coinPrefab;

        private float _fullTimer;
        private float _jumpTimer;
        private float _lastSpawnTime;

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
                return;

            _fullTimer -= Time.deltaTime;
            _jumpTimer -= Time.deltaTime;

            if (_fullTimer <= 0f && CanSpawn())
            {
                SpawnObstacle(ObstacleType.FullBarrier);
                _fullTimer = fullBarrierInterval + Random.Range(-0.5f, 0.5f);
            }

            if (_jumpTimer <= 0f && CanSpawn())
            {
                SpawnObstacle(ObstacleType.JumpBarrier);
                _jumpTimer = jumpBarrierInterval + Random.Range(-0.5f, 0.5f);
            }
        }

        private bool CanSpawn()
        {
            return Time.time - _lastSpawnTime >= minGapBetweenSpawns;
        }

        private void SpawnObstacle(ObstacleType type)
        {
            if (ObstaclePool.Instance == null) return;

            int lane = Random.Range(0, GameConstants.Lanes);
            var obstacle = ObstaclePool.Instance.Get(type);
            obstacle.Initialize(type, lane);
            GameManager.Instance?.AddScore(GameConstants.ScorePerObstaclePassed);
            _lastSpawnTime = Time.time;

            if (spawnCoins && Random.value < coinSpawnChance)
                TrySpawnCoin(lane);
        }

        private void TrySpawnCoin(int obstacleLane)
        {
            if (coinPrefab == null)
            {
                coinPrefab = CreateRuntimeCoinPrefab();
            }

            int lane = obstacleLane;
            while (lane == obstacleLane)
                lane = Random.Range(0, GameConstants.Lanes);

            int center = GameConstants.Lanes / 2;
            float x = (lane - center) * GameConstants.LaneWidth;
            Instantiate(coinPrefab, new Vector3(x, 1f, 55f), Quaternion.identity);
        }

        private static GameObject CreateRuntimeCoinPrefab()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Coin_Runtime";
            go.transform.localScale = Vector3.one * 0.6f;
            go.GetComponent<Collider>().isTrigger = true;
            go.AddComponent<Collectibles.Coin>();
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = Color.yellow;
            return go;
        }
    }
}
