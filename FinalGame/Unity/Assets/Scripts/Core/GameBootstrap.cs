using UnityEngine;
using ZillRunner.CameraSystem;
using ZillRunner.Collectibles;
using ZillRunner.Environment;
using ZillRunner.Input;
using ZillRunner.Obstacles;
using ZillRunner.Player;
using ZillRunner.UI;

namespace ZillRunner.Core
{
    /// <summary>
    /// Runtime bootstrap — builds a playable scene when no editor setup was run.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private bool autoBuildOnStart = true;

        private void Start()
        {
            if (autoBuildOnStart && FindObjectOfType<PlayerController>() == null)
                BuildScene();
        }

        [ContextMenu("Build Scene")]
        public void BuildScene()
        {
            BuildGround();
            var player = BuildPlayer();
            BuildManagers();
            BuildCamera(player.transform);
            BuildLighting();
        }

        private static void BuildGround()
        {
            if (GameObject.Find("Track") != null) return;

            var track = new GameObject("Track");
            for (int i = 0; i < GameConstants.Lanes; i++)
            {
                int center = GameConstants.Lanes / 2;
                float x = (i - center) * GameConstants.LaneWidth;
                var lane = GameObject.CreatePrimitive(PrimitiveType.Cube);
                lane.name = $"Lane_{i}";
                lane.transform.SetParent(track.transform);
                lane.transform.position = new Vector3(x, -0.5f, 40f);
                lane.transform.localScale = new Vector3(2.5f, 1f, 120f);
                var r = lane.GetComponent<Renderer>();
                if (r != null) r.material.color = new Color(0.15f, 0.15f, 0.18f);
            }

            var borderL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            borderL.name = "BorderLeft";
            borderL.transform.SetParent(track.transform);
            borderL.transform.position = new Vector3(-6f, 0.5f, 40f);
            borderL.transform.localScale = new Vector3(0.3f, 1f, 120f);

            var borderR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            borderR.name = "BorderRight";
            borderR.transform.SetParent(track.transform);
            borderR.transform.position = new Vector3(6f, 0.5f, 40f);
            borderR.transform.localScale = new Vector3(0.3f, 1f, 120f);
        }

        private static GameObject BuildPlayer()
        {
            if (GameObject.FindGameObjectWithTag("Player") != null)
                return GameObject.FindGameObjectWithTag("Player");

            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = new Vector3(0f, 1f, 0f);

            var col = player.GetComponent<Collider>();
            col.isTrigger = true;

            var cc = player.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.center = new Vector3(0f, 1f, 0f);
            cc.radius = 0.4f;

            player.AddComponent<PlayerController>();
            player.AddComponent<PlayerCollision>();

            var body = player.GetComponent<Renderer>();
            if (body != null) body.material.color = Color.cyan;

            return player;
        }

        private static void BuildManagers()
        {
            if (FindObjectOfType<GameManager>() != null) return;

            var root = new GameObject("Managers");
            root.AddComponent<GameManager>();
            root.AddComponent<InputManager>();
            root.AddComponent<ObstaclePool>();
            root.AddComponent<ObstacleSpawner>();
            root.AddComponent<PowerUpSpawner>();
            root.AddComponent<TrackManager>();
            root.AddComponent<AudioManager>();

            var canvas = new GameObject("Canvas");
            var canvasComp = canvas.AddComponent<Canvas>();
            canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            canvas.AddComponent<UIManager>();
        }

        private static void BuildCamera(Transform target)
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                cam = camGo.AddComponent<Camera>();
                camGo.tag = "MainCamera";
                camGo.AddComponent<AudioListener>();
            }

            var follow = cam.GetComponent<CameraFollow>();
            if (follow == null) follow = cam.gameObject.AddComponent<CameraFollow>();
            follow.SetTarget(target);
            cam.transform.position = target.position + new Vector3(0f, 6f, -10f);
            cam.backgroundColor = new Color(0.05f, 0.08f, 0.12f);
        }

        private static void BuildLighting()
        {
            if (FindObjectOfType<Light>() != null) return;

            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }
    }
}
