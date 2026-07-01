using System.Collections.Generic;
using UnityEngine;

namespace ZillRunner.Obstacles
{
    public class ObstaclePool : MonoBehaviour
    {
        public static ObstaclePool Instance { get; private set; }

        [SerializeField] private Obstacle fullBarrierPrefab;
        [SerializeField] private Obstacle jumpBarrierPrefab;
        [SerializeField] private int initialPoolSize = 20;

        private readonly Queue<Obstacle> _fullPool = new();
        private readonly Queue<Obstacle> _jumpPool = new();
        private Transform _poolRoot;

        private void Awake()
        {
            Instance = this;
            _poolRoot = new GameObject("ObstaclePool").transform;
            _poolRoot.SetParent(transform);

            if (fullBarrierPrefab == null)
                fullBarrierPrefab = CreateDefaultPrefab(ObstacleType.FullBarrier);
            if (jumpBarrierPrefab == null)
                jumpBarrierPrefab = CreateDefaultPrefab(ObstacleType.JumpBarrier);

            for (int i = 0; i < initialPoolSize / 2; i++)
            {
                _fullPool.Enqueue(CreateInstance(fullBarrierPrefab));
                _jumpPool.Enqueue(CreateInstance(jumpBarrierPrefab));
            }
        }

        public Obstacle Get(ObstacleType type)
        {
            var queue = type == ObstacleType.FullBarrier ? _fullPool : _jumpPool;
            var prefab = type == ObstacleType.FullBarrier ? fullBarrierPrefab : jumpBarrierPrefab;

            Obstacle obstacle = queue.Count > 0 ? queue.Dequeue() : CreateInstance(prefab);
            obstacle.gameObject.SetActive(true);
            return obstacle;
        }

        public void Release(Obstacle obstacle)
        {
            obstacle.ResetState();
            obstacle.gameObject.SetActive(false);
            obstacle.transform.SetParent(_poolRoot);

            if (obstacle.Type == ObstacleType.FullBarrier)
                _fullPool.Enqueue(obstacle);
            else
                _jumpPool.Enqueue(obstacle);
        }

        private Obstacle CreateInstance(Obstacle prefab)
        {
            var instance = Instantiate(prefab, _poolRoot);
            instance.gameObject.SetActive(false);
            return instance;
        }

        private static Obstacle CreateDefaultPrefab(ObstacleType type)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = type == ObstacleType.FullBarrier ? "FullBarrier_Prefab" : "JumpBarrier_Prefab";
            go.transform.localScale = new Vector3(2.2f, 2.5f, 1.5f);

            var col = go.GetComponent<Collider>();
            col.isTrigger = true;

            var obstacle = go.AddComponent<Obstacle>();
            obstacle.Initialize(type, 1);
            go.SetActive(false);
            return obstacle;
        }
    }
}
