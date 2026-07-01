using System.Collections;
using UnityEngine;
using ZillRunner.Core;
using ZillRunner.Input;

namespace ZillRunner.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Lane")]
        [SerializeField] private int startLane = 1;

        [Header("Collider")]
        [SerializeField] private float standHeight = 2f;
        [SerializeField] private float slideHeight = 1f;
        [SerializeField] private float standCenterY = 1f;
        [SerializeField] private float slideCenterY = 0.5f;

        [Header("Visual")]
        [SerializeField] private Transform visualRoot;
        [SerializeField] private Renderer bodyRenderer;

        private CharacterController _controller;
        private int _currentLane;
        private float _laneX;
        private float _verticalVelocity;
        private bool _isJumping;
        private bool _isSliding;
        private bool _isInvincible;
        private Coroutine _laneCoroutine;
        private Coroutine _slideCoroutine;
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        public bool IsJumping => _isJumping;
        public bool IsSliding => _isSliding;
        public bool IsInvincible => _isInvincible;
        public int CurrentLane => _currentLane;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (visualRoot == null) visualRoot = transform;
            if (bodyRenderer == null) bodyRenderer = GetComponentInChildren<Renderer>();
        }

        private void OnEnable()
        {
            if (InputManager.Instance != null)
                InputManager.Instance.OnInput += HandleInput;
        }

        private void OnDisable()
        {
            if (InputManager.Instance != null)
                InputManager.Instance.OnInput -= HandleInput;
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
                return;

            ApplyForwardMotion();
            ApplyGravity();
            BlinkWhenInvincible();
        }

        public void ResetPlayer()
        {
            StopAllCoroutines();
            _isJumping = false;
            _isSliding = false;
            _isInvincible = false;
            _verticalVelocity = 0f;
            _currentLane = Mathf.Clamp(startLane, 0, GameConstants.Lanes - 1);
            _laneX = LaneToX(_currentLane);
            transform.position = new Vector3(_laneX, transform.position.y, 0f);
            SetColliderStand();
            ResetVisualRotation();
            SetBodyColor(Color.cyan);
        }

        private void HandleInput(PlayerInputAction action)
        {
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
                return;

            switch (action)
            {
                case PlayerInputAction.MoveLeft:
                    TryChangeLane(-1);
                    break;
                case PlayerInputAction.MoveRight:
                    TryChangeLane(1);
                    break;
                case PlayerInputAction.Jump:
                    TryJump();
                    break;
                case PlayerInputAction.Slide:
                    TrySlide();
                    break;
            }
        }

        private void TryChangeLane(int direction)
        {
            if (_isSliding) return;
            int target = _currentLane + direction;
            if (target < 0 || target >= GameConstants.Lanes) return;

            _currentLane = target;
            if (_laneCoroutine != null) StopCoroutine(_laneCoroutine);
            _laneCoroutine = StartCoroutine(SmoothLaneChange(LaneToX(_currentLane)));
        }

        private IEnumerator SmoothLaneChange(float targetX)
        {
            float startX = transform.position.x;
            float elapsed = 0f;
            while (elapsed < GameConstants.LaneSwitchDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / GameConstants.LaneSwitchDuration);
                float x = Mathf.Lerp(startX, targetX, t);
                transform.position = new Vector3(x, transform.position.y, transform.position.z);
                yield return null;
            }
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
            _laneX = targetX;
        }

        private void TryJump()
        {
            if (_isJumping || _isSliding) return;
            if (!_controller.isGrounded && transform.position.y > 0.05f) return;

            _isJumping = true;
            _verticalVelocity = Mathf.Sqrt(2f * GameConstants.JumpHeight * 20f);
            StartCoroutine(JumpRoutine());
        }

        private IEnumerator JumpRoutine()
        {
            float peakTime = 0.4f;
            float elapsed = 0f;
            Vector3 baseScale = visualRoot.localScale;

            while (elapsed < peakTime && _isJumping)
            {
                elapsed += Time.deltaTime;
                float squash = 1f + Mathf.Sin(elapsed / peakTime * Mathf.PI) * 0.15f;
                visualRoot.localScale = new Vector3(baseScale.x, baseScale.y * squash, baseScale.z);
                yield return null;
            }

            while (transform.position.y > 0.01f || _verticalVelocity > 0f)
                yield return null;

            _isJumping = false;
            visualRoot.localScale = baseScale;
        }

        private void TrySlide()
        {
            if (_isSliding || _isJumping) return;
            if (_slideCoroutine != null) StopCoroutine(_slideCoroutine);
            _slideCoroutine = StartCoroutine(SlideRoutine());
        }

        private IEnumerator SlideRoutine()
        {
            _isSliding = true;
            SetColliderSlide();
            RotateVisualForSlide();

            float elapsed = 0f;
            while (elapsed < GameConstants.SlideDuration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            SetColliderStand();
            ResetVisualRotation();
            _isSliding = false;
        }

        private void ApplyForwardMotion()
        {
            float speed = GameManager.Instance.RunSpeed;
            Vector3 motion = Vector3.forward * speed * Time.deltaTime;
            motion.y = _verticalVelocity * Time.deltaTime;
            _controller.Move(motion);

            if (_controller.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;
        }

        private void ApplyGravity()
        {
            if (!_controller.isGrounded || _verticalVelocity > 0f)
                _verticalVelocity -= 20f * Time.deltaTime;
        }

        public void TriggerInvincibility(float duration)
        {
            StopCoroutine(nameof(InvincibilityRoutine));
            StartCoroutine(InvincibilityRoutine(duration));
        }

        private IEnumerator InvincibilityRoutine(float duration)
        {
            _isInvincible = true;
            yield return new WaitForSeconds(duration);
            _isInvincible = false;
            SetBodyColor(Color.cyan);
        }

        private void BlinkWhenInvincible()
        {
            if (!_isInvincible || bodyRenderer == null) return;
            float blink = Mathf.PingPong(Time.time * 10f, 1f);
            SetBodyColor(Color.Lerp(Color.cyan, Color.white, blink));
        }

        private void SetColliderSlide()
        {
            _controller.height = slideHeight;
            _controller.center = new Vector3(0f, slideCenterY, 0f);
        }

        private void SetColliderStand()
        {
            _controller.height = standHeight;
            _controller.center = new Vector3(0f, standCenterY, 0f);
        }

        private void RotateVisualForSlide()
        {
            visualRoot.localRotation = Quaternion.Euler(90f, 0f, 0f);
        }

        private void ResetVisualRotation()
        {
            visualRoot.localRotation = Quaternion.identity;
        }

        private void SetBodyColor(Color color)
        {
            if (bodyRenderer != null && bodyRenderer.material.HasProperty(ColorId))
                bodyRenderer.material.SetColor(ColorId, color);
        }

        private static float LaneToX(int lane)
        {
            int center = GameConstants.Lanes / 2;
            return (lane - center) * GameConstants.LaneWidth;
        }
    }
}
