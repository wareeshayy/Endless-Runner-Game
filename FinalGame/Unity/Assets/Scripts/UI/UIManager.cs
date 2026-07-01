using UnityEngine;
using ZillRunner.Core;
using ZillRunner.Input;

namespace ZillRunner.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject instructionsPanel;
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private GameObject loadingPanel;

        [Header("HUD")]
        [SerializeField] private TMPro.TMP_Text playerNameText;
        [SerializeField] private TMPro.TMP_Text livesText;
        [SerializeField] private TMPro.TMP_Text scoreText;
        [SerializeField] private TMPro.TMP_Text comboText;
        [SerializeField] private TMPro.TMP_Text speedText;
        [SerializeField] private TMPro.TMP_Text coinsText;
        [SerializeField] private TMPro.TMP_Text shieldIndicator;
        [SerializeField] private TMPro.TMP_Text magnetIndicator;

        [Header("Menus")]
        [SerializeField] private TMPro.TMP_InputField nameInput;
        [SerializeField] private TMPro.TMP_Text highScoreMenuText;
        [SerializeField] private TMPro.TMP_Text finalScoreText;
        [SerializeField] private TMPro.TMP_Text finalHighScoreText;

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
                GameManager.Instance.OnLivesChanged += UpdateLives;
                GameManager.Instance.OnScoreChanged += UpdateScore;
                GameManager.Instance.OnCoinsChanged += UpdateCoins;
                GameManager.Instance.OnSpeedChanged += UpdateSpeed;
                GameManager.Instance.OnObstaclePassed += UpdateCombo;
            }

            if (InputManager.Instance != null)
                InputManager.Instance.OnInput += HandlePauseInput;
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
                GameManager.Instance.OnLivesChanged -= UpdateLives;
                GameManager.Instance.OnScoreChanged -= UpdateScore;
                GameManager.Instance.OnCoinsChanged -= UpdateCoins;
                GameManager.Instance.OnSpeedChanged -= UpdateSpeed;
                GameManager.Instance.OnObstaclePassed -= UpdateCombo;
            }

            if (InputManager.Instance != null)
                InputManager.Instance.OnInput -= HandlePauseInput;
        }

        private void Start()
        {
            if (nameInput != null && GameManager.Instance != null)
                nameInput.text = GameManager.Instance.PlayerName;
            RefreshHighScoreDisplay();
            ShowMainMenu();
        }

        private void HandleStateChanged(GameState state)
        {
            SetPanelActive(mainMenuPanel, state == GameState.MainMenu);
            SetPanelActive(hudPanel, state == GameState.Playing || state == GameState.Paused);
            SetPanelActive(pausePanel, state == GameState.Paused);
            SetPanelActive(gameOverPanel, state == GameState.GameOver);
            SetPanelActive(loadingPanel, state == GameState.Loading);

            if (state == GameState.Playing)
                RefreshHud();
            if (state == GameState.GameOver)
                ShowGameOver();
        }

        private void HandlePauseInput(PlayerInputAction action)
        {
            if (action != PlayerInputAction.Pause) return;
            if (GameManager.Instance == null) return;

            if (GameManager.Instance.State == GameState.Playing)
                GameManager.Instance.PauseGame();
            else if (GameManager.Instance.State == GameState.Paused)
                GameManager.Instance.ResumeGame();
        }

        public void OnStartClicked()
        {
            string name = nameInput != null ? nameInput.text : "Player";
            GameManager.Instance?.SetPlayerName(name);
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(loadingPanel, true);
            Invoke(nameof(BeginPlay), 1.2f);
        }

        private void BeginPlay()
        {
            SetPanelActive(loadingPanel, false);
            GameManager.Instance?.StartGame();
        }

        public void OnInstructionsClicked()
        {
            SetPanelActive(instructionsPanel, true);
        }

        public void OnCreditsClicked()
        {
            SetPanelActive(creditsPanel, true);
        }

        public void OnCloseOverlayClicked()
        {
            SetPanelActive(instructionsPanel, false);
            SetPanelActive(creditsPanel, false);
        }

        public void OnResumeClicked() => GameManager.Instance?.ResumeGame();
        public void OnQuitToMenuClicked() => GameManager.Instance?.ReturnToMenu();
        public void OnRestartClicked() => GameManager.Instance?.StartGame();
        public void OnExitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void ShowMainMenu()
        {
            RefreshHighScoreDisplay();
            HandleStateChanged(GameState.MainMenu);
        }

        private void RefreshHud()
        {
            if (GameManager.Instance == null) return;
            if (playerNameText != null)
                playerNameText.text = $"PLAYER: {GameManager.Instance.PlayerName}";
            UpdateLives(GameManager.Instance.Lives);
            UpdateScore(GameManager.Instance.Score);
            UpdateCoins(GameManager.Instance.CoinsCollected);
            UpdateSpeed(GameManager.Instance.RunSpeed);
            UpdateCombo();
        }

        private void UpdateLives(int lives)
        {
            if (livesText == null) return;
            livesText.text = "LIVES: " + new string('♥', lives);
        }

        private void UpdateScore(int score)
        {
            if (scoreText != null) scoreText.text = $"SCORE: {score:N0}";
        }

        private void UpdateCoins(int coins)
        {
            if (coinsText != null) coinsText.text = $"COINS: {coins}";
        }

        private void UpdateSpeed(float speed)
        {
            if (speedText != null) speedText.text = $"SPEED: {speed:F0}";
        }

        private void UpdateCombo()
        {
            if (comboText == null || GameManager.Instance == null) return;
            int combo = GameManager.Instance.ComboMultiplier;
            comboText.text = combo > 1 ? $"COMBO x{combo}" : "";
            if (shieldIndicator != null)
                shieldIndicator.text = GameManager.Instance.HasShield ? "SHIELD" : "";
            if (magnetIndicator != null)
                magnetIndicator.text = GameManager.Instance.HasMagnet ? "MAGNET" : "";
        }

        private void RefreshHighScoreDisplay()
        {
            int hs = GameManager.Instance != null ? GameManager.Instance.HighScore : 0;
            if (highScoreMenuText != null)
                highScoreMenuText.text = $"HIGH SCORE: {hs:N0}";
        }

        private void ShowGameOver()
        {
            if (GameManager.Instance == null) return;
            if (finalScoreText != null)
                finalScoreText.text = $"{GameManager.Instance.PlayerName}'s SCORE: {GameManager.Instance.Score:N0}";
            if (finalHighScoreText != null)
                finalHighScoreText.text = $"HIGH SCORE: {GameManager.Instance.HighScore:N0}";
        }

        private static void SetPanelActive(GameObject panel, bool active)
        {
            if (panel != null) panel.SetActive(active);
        }
    }
}
