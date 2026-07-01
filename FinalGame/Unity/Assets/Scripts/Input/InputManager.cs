using System;
using UnityEngine;

namespace ZillRunner.Input
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        [Header("Touch swipe")]
        [SerializeField] private float swipeThreshold = 50f;

        public event Action<PlayerInputAction> OnInput;

        private Vector2 _touchStart;
        private bool _touchActive;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            HandleKeyboard();
            HandleTouch();
        }

        private void HandleKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                Fire(PlayerInputAction.MoveLeft);
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                Fire(PlayerInputAction.MoveRight);
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
                Fire(PlayerInputAction.Jump);
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                Fire(PlayerInputAction.Slide);
            if (Input.GetKeyDown(KeyCode.Escape))
                Fire(PlayerInputAction.Pause);
        }

        private void HandleTouch()
        {
            if (Input.touchCount == 0)
            {
                _touchActive = false;
                return;
            }

            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _touchStart = touch.position;
                    _touchActive = true;
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (!_touchActive) break;
                    Vector2 delta = touch.position - _touchStart;
                    if (delta.magnitude < swipeThreshold) break;

                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                        Fire(delta.x > 0 ? PlayerInputAction.MoveRight : PlayerInputAction.MoveLeft);
                    else
                        Fire(delta.y > 0 ? PlayerInputAction.Jump : PlayerInputAction.Slide);

                    _touchActive = false;
                    break;
            }
        }

        public void Fire(PlayerInputAction action) => OnInput?.Invoke(action);
    }
}
