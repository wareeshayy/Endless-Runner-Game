namespace ZillRunner.Core
{
    public static class GameConstants
    {
        public const int MaxLives = 3;
        public const int Lanes = 3;
        public const float LaneWidth = 3f;
        public const float BaseRunSpeed = 12f;
        public const float MaxRunSpeed = 28f;
        public const float SpeedIncreasePerMinute = 2f;
        public const float JumpHeight = 2.2f;
        public const float SlideDuration = 0.85f;
        public const float LaneSwitchDuration = 0.18f;
        public const float InvincibilityDuration = 1.5f;
        public const int ScorePerSecond = 10;
        public const int ScorePerObstaclePassed = 50;
        public const int CoinValue = 25;
        public const string HighScoreKey = "ZillRunner_HighScore";
        public const string PlayerNameKey = "ZillRunner_PlayerName";
    }
}
