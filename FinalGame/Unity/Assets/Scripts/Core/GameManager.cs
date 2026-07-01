using System;
using UnityEngine;
using ZillRunner.Player;

namespace ZillRunner.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private PlayerController player;

        public GameState State { get; private set; } = GameState.MainMenu;
        public int Lives { get; private set; } = GameConstants.MaxLives;
        public int Score { get; private set; }
        public int HighScore { get; private set; }
        public int CoinsCollected { get; private set; }
        public float RunSpeed { get; private set; } = GameConstants.BaseRunSpeed;
        public float DistanceTraveled { get; private set; }
        public int ComboMultiplier { get; private set; } = 1;
        public string PlayerName { get; private set; } = "Player";
        public bool HasShield { get; private set; }
        public bool HasMagnet { get; private set; }
        public float MagnetRadius { get; private set; } = 5f;

        public event Action<GameState> OnStateChanged;
        public event Action<int> OnLivesChanged;
        public event Action<int> OnScoreChanged;
        public event Action<int> OnCoinsChanged;
        public event Action<float> OnSpeedChanged;
        public event Action OnPlayerHit;
        public event Action OnObstaclePassed;

        private float _playTime;
        private float _comboTimer;
        private const float ComboDecayTime = 3f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            HighScore = PlayerPrefs.GetInt(GameConstants.HighScoreKey, 0);
            PlayerName = PlayerPrefs.GetString(GameConstants.PlayerNameKey, "Player");
            if (player == null)
                player = FindObjectOfType<PlayerController>();
        }

        private void Update()
        {
            if (State != GameState.Playing) return;

            float dt = Time.deltaTime;
            _playTime += dt;
            DistanceTraveled += RunSpeed * dt;

            RunSpeed = Mathf.Min(
                GameConstants.BaseRunSpeed + (_playTime / 60f) * GameConstants.SpeedIncreasePerMinute,
                GameConstants.MaxRunSpeed);

            AddScore(Mathf.RoundToInt(GameConstants.ScorePerSecond * dt * ComboMultiplier));

            if (_comboTimer > 0f)
            {
                _comboTimer -= dt;
                if (_comboTimer <= 0f)
                    ComboMultiplier = 1;
            }
        }

        public void SetPlayerName(string name)
        {
            PlayerName = string.IsNullOrWhiteSpace(name) ? "Player" : name.Trim();
            PlayerPrefs.SetString(GameConstants.PlayerNameKey, PlayerName);
        }

        public void StartGame()
        {
            Lives = GameConstants.MaxLives;
            Score = 0;
            CoinsCollected = 0;
            DistanceTraveled = 0f;
            RunSpeed = GameConstants.BaseRunSpeed;
            ComboMultiplier = 1;
            HasShield = false;
            HasMagnet = false;
            _playTime = 0f;
            _comboTimer = 0f;

            OnLivesChanged?.Invoke(Lives);
            OnScoreChanged?.Invoke(Score);
            OnCoinsChanged?.Invoke(CoinsCollected);
            OnSpeedChanged?.Invoke(RunSpeed);

            SetState(GameState.Playing);
            player?.ResetPlayer();
        }

        public void PauseGame()
        {
            if (State == GameState.Playing)
            {
                Time.timeScale = 0f;
                SetState(GameState.Paused);
            }
        }

        public void ResumeGame()
        {
            if (State == GameState.Paused)
            {
                Time.timeScale = 1f;
                SetState(GameState.Playing);
            }
        }

        public void EndGame()
        {
            Time.timeScale = 1f;
            if (Score > HighScore)
            {
                HighScore = Score;
                PlayerPrefs.SetInt(GameConstants.HighScoreKey, HighScore);
            }
            SetState(GameState.GameOver);
        }

        public void ReturnToMenu()
        {
            Time.timeScale = 1f;
            SetState(GameState.MainMenu);
        }

        private void SetState(GameState newState)
        {
            State = newState;
            OnStateChanged?.Invoke(State);
        }

        public void AddScore(int amount)
        {
            if (amount <= 0) return;
            Score += amount;
            OnScoreChanged?.Invoke(Score);
        }

        public void RegisterObstaclePassed()
        {
            AddScore(GameConstants.ScorePerObstaclePassed * ComboMultiplier);
            ComboMultiplier = Mathf.Min(ComboMultiplier + 1, 5);
            _comboTimer = ComboDecayTime;
            OnObstaclePassed?.Invoke();
        }

        public void CollectCoin(int value = GameConstants.CoinValue)
        {
            CoinsCollected++;
            AddScore(value * ComboMultiplier);
            OnCoinsChanged?.Invoke(CoinsCollected);
        }

        public void ActivateShield(float duration = 8f)
        {
            HasShield = true;
            CancelInvoke(nameof(DeactivateShield));
            Invoke(nameof(DeactivateShield), duration);
        }

        private void DeactivateShield() => HasShield = false;

        public void ActivateMagnet(float duration = 6f)
        {
            HasMagnet = true;
            CancelInvoke(nameof(DeactivateMagnet));
            Invoke(nameof(DeactivateMagnet), duration);
        }

        private void DeactivateMagnet() => HasMagnet = false;

        public void TakeDamage()
        {
            if (State != GameState.Playing) return;

            if (HasShield)
            {
                HasShield = false;
                CancelInvoke(nameof(DeactivateShield));
                OnPlayerHit?.Invoke();
                return;
            }

            Lives--;
            OnLivesChanged?.Invoke(Lives);
            OnPlayerHit?.Invoke();
            player?.TriggerInvincibility(GameConstants.InvincibilityDuration);

            if (Lives <= 0)
                EndGame();
        }
    }
}
