using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DefenseGameCSharp
{
    public class UIManager
    {
        private GameState _gameState;
        private NotificationManager _notificationManager;
        private int _notificationTimer;

        public UIManager()
        {
            _notificationManager = new NotificationManager();
            _notificationTimer = 0;
        }

        public void Initialize(GameState gameState)
        {
            _gameState = gameState;
            ShowNotification("DEFENSE SYSTEMS ONLINE - PRESS C FOR CYBER ATTACK", "cyber");
            ShowNotification("PRESS C FOR CYBER ATTACK | SPACE TO FIRE | ESC TO PAUSE", "cyber");
        }

        public void Update(GameState gameState)
        {
            _notificationManager.Update();
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            if (_gameState == null)
                return;

            // Health display
            string healthText = $"HEALTH: {Math.Floor((double)_gameState.Defense.Health)}";
            spriteBatch.DrawString(font, healthText, new Vector2(10, 10), Color.White);

            // Heat display
            float heatPercent = (_gameState.Defense.Heat / _gameState.Defense.MaxHeat) * 100;
            string heatText = $"HEAT: {Math.Floor(heatPercent)}%";
            Color heatColor = _gameState.Defense.Heat > _gameState.Defense.OverheatThreshold
                ? Color.Red
                : Color.LightCoral;
            spriteBatch.DrawString(font, heatText, new Vector2(10, 40), heatColor);

            // Score display
            string scoreText = $"SCORE: {_gameState.Score}";
            spriteBatch.DrawString(font, scoreText, new Vector2(10, 70), Color.LimeGreen);

            // Planes destroyed display
            string planesDestroyedText = $"DESTROYED: {_gameState.PlanesDestroyed}";
            spriteBatch.DrawString(font, planesDestroyedText, new Vector2(10, 100), Color.LimeGreen);

            // Enemy count display
            float airSuperiority = Math.Max(0, 100 - (_gameState.Planes.Count / (float)_gameState.EnemyLimit * 100));
            string enemyCountText = $"ENEMIES: {_gameState.Planes.Count} | AIR SUPERIORITY: {Math.Floor(airSuperiority)}%";
            spriteBatch.DrawString(font, enemyCountText, new Vector2(10, 130), Color.Cyan);

            // Wave display
            string waveText = $"WAVE: {_gameState.Wave}";
            spriteBatch.DrawString(font, waveText, new Vector2(10, 160), Color.Yellow);

            // Spawn timer display
            string spawnTimerText;
            Color spawnTimerColor;
            if (_gameState.WaveActive)
            {
                int enemiesLeft = Math.Max(0, _gameState.TotalEnemiesThisWave - _gameState.EnemiesSpawnedThisWave);
                spawnTimerText = $"REMAINING: {enemiesLeft}";
                spawnTimerColor = Color.Red;
            }
            else
            {
                int seconds = (int)Math.Ceiling(_gameState.SpawnTimer / 60f);
                spawnTimerText = $"NEXT WAVE IN: {seconds}s";
                if (seconds <= 10) spawnTimerColor = Color.Red;
                else if (seconds <= 20) spawnTimerColor = Color.Yellow;
                else spawnTimerColor = Color.Cyan;
            }
            spriteBatch.DrawString(font, spawnTimerText, new Vector2(10, 190), spawnTimerColor);

            // Cyber attack status display
            string cyberStatusText;
            Color cyberStatusColor;
            if (_gameState.CyberAttackCooldown > 0)
            {
                int cooldownSecs = (int)Math.Ceiling(_gameState.CyberAttackCooldown / 60f);
                cyberStatusText = $"CYBER: {cooldownSecs}s";
                cyberStatusColor = Color.LightCoral;
            }
            else
            {
                cyberStatusText = "CYBER: READY";
                cyberStatusColor = Color.Cyan;
            }
            spriteBatch.DrawString(font, cyberStatusText, new Vector2(10, 220), cyberStatusColor);

            // Draw notifications
            _notificationManager.Draw(spriteBatch, font);
        }

        public void ShowNotification(string text, string type = "")
        {
            _notificationManager.AddNotification(new Notification { Text = text, Type = type });
        }
    }

    public class NotificationManager
    {
        private Queue<Notification> _notifications;
        private int _displayTime = 120; // 2 seconds at 60 FPS

        public NotificationManager()
        {
            _notifications = new Queue<Notification>();
        }

        public void AddNotification(Notification notification)
        {
            notification.TimeRemaining = _displayTime;
            _notifications.Enqueue(notification);
        }

        public void Update()
        {
            foreach (var notification in _notifications)
            {
                notification.TimeRemaining--;
            }

            if (_notifications.Count > 0 && _notifications.Peek().TimeRemaining <= 0)
            {
                _notifications.Dequeue();
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            if (_notifications.Count > 0)
            {
                var notification = _notifications.Peek();
                Color color = notification.Type == "cyber" ? Color.Cyan : Color.Yellow;
                Vector2 position = new Vector2(GameState.CanvasWidth / 2 - 200, GameState.CanvasHeight / 2);
                spriteBatch.DrawString(font, notification.Text, position, color);
            }
        }
    }

    public class Notification
    {
        public string Text { get; set; }
        public string Type { get; set; }
        public int TimeRemaining { get; set; }
    }
}
