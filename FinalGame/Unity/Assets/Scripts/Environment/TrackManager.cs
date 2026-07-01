using UnityEngine;
using ZillRunner.Core;

namespace ZillRunner.Environment
{
    /// <summary>
    /// Recycles ground segments for an endless track feel.
    /// </summary>
    public class TrackManager : MonoBehaviour
    {
        [SerializeField] private int segmentCount = 6;
        [SerializeField] private float segmentLength = 30f;
        [SerializeField] private float laneWidth = GameConstants.LaneWidth;

        private Transform[] _segments;
        private float _speed;

        private void Start()
        {
            BuildSegments();
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
                return;

            _speed = GameManager.Instance.RunSpeed;
            float move = _speed * Time.deltaTime;

            for (int i = 0; i < _segments.Length; i++)
            {
                var seg = _segments[i];
                seg.position += Vector3.back * move;
                if (seg.position.z < -segmentLength)
                {
                    float maxZ = GetMaxSegmentZ();
                    seg.position = new Vector3(0f, seg.position.y, maxZ + segmentLength);
                }
            }
        }

        private float GetMaxSegmentZ()
        {
            float max = float.MinValue;
            foreach (var seg in _segments)
                if (seg.position.z > max) max = seg.position.z;
            return max;
        }

        private void BuildSegments()
        {
            _segments = new Transform[segmentCount];
            var root = new GameObject("ScrollingTrack").transform;
            root.SetParent(transform);

            for (int s = 0; s < segmentCount; s++)
            {
                var segment = new GameObject($"Segment_{s}");
                segment.transform.SetParent(root);
                segment.transform.position = new Vector3(0f, 0f, s * segmentLength);

                for (int lane = 0; lane < GameConstants.Lanes; lane++)
                {
                    int center = GameConstants.Lanes / 2;
                    float x = (lane - center) * laneWidth;
                    var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    tile.transform.SetParent(segment.transform);
                    tile.transform.localPosition = new Vector3(x, -0.5f, segmentLength * 0.5f);
                    tile.transform.localScale = new Vector3(2.5f, 1f, segmentLength);
                    var r = tile.GetComponent<Renderer>();
                    if (r != null)
                    {
                        float shade = 0.12f + (lane * 0.02f) + ((s % 2) * 0.03f);
                        r.material.color = new Color(shade, shade, shade + 0.03f);
                    }
                }

                _segments[s] = segment.transform;
            }
        }
    }
}
