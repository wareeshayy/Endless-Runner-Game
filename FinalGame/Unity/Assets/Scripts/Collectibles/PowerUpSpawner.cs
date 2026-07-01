using UnityEngine;
using ZillRunner.Collectibles;
using ZillRunner.Core;

namespace ZillRunner.Collectibles
{
    public class PowerUpSpawner : MonoBehaviour
    {
        [SerializeField] private float spawnInterval = 18f;
        [SerializeField] private GameObject powerUpPrefab;

        private float _timer;

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
                return;

            _timer -= Time.deltaTime;
            if (_timer > 0f) return;

            _timer = spawnInterval + Random.Range(-3f, 3f);
            SpawnPowerUp();
        }

        private void SpawnPowerUp()
        {
            if (powerUpPrefab == null)
                powerUpPrefab = CreateRuntimePrefab();

            int lane = Random.Range(0, GameConstants.Lanes);
            int center = GameConstants.Lanes / 2;
            float x = (lane - center) * GameConstants.LaneWidth;

            var go = Instantiate(powerUpPrefab, Vector3.zero, Quaternion.identity);
            var powerUp = go.GetComponent<PowerUp>();
            if (powerUp != null)
            {
                var type = (PowerUpType)Random.Range(0, 3);
                powerUp.Initialize(type, lane);
            }
        }

        private static GameObject CreateRuntimePrefab()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = "PowerUp_Runtime";
            go.transform.localScale = new Vector3(0.8f, 0.2f, 0.8f);
            go.GetComponent<Collider>().isTrigger = true;
            go.AddComponent<PowerUp>();
            var r = go.GetComponent<Renderer>();
            if (r != null) r.material.color = Color.magenta;
            return go;
        }
    }
}
