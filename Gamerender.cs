using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DefenseGameCSharp
{
    public class GameRenderer
    {
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private Texture2D _whitePixel;
        private SpriteFont _font;

        // Radar
        private RenderTarget2D _radarTarget;
        private const int RadarWidth = 130;
        private const int RadarHeight = 130;

        public void Initialize(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Texture2D whitePixel, SpriteFont font)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            _whitePixel = whitePixel;
            _font = font;

            _radarTarget = new RenderTarget2D(graphicsDevice, RadarWidth, RadarHeight);
        }

        public void DrawGame(GameState gameState)
        {
            DrawSky(gameState);
            DrawBuildings(gameState);
            DrawGround(gameState);

            // Draw aircraft
            foreach (var aircraft in gameState.Planes)
            {
                DrawAircraft(aircraft);
            }

            // Draw missiles
            foreach (var missile in gameState.Missiles)
            {
                DrawMissile(missile);
            }

            // Draw enemy missiles
            foreach (var missile in gameState.EnemyMissiles)
            {
                DrawEnemyMissile(missile);
            }

            // Draw explosions
            foreach (var explosion in gameState.Explosions)
            {
                DrawExplosion(explosion);
            }

            foreach (var explosion in gameState.CyberExplosions)
            {
                DrawCyberExplosion(explosion);
            }

            DrawDefense(gameState);
            DrawRadar(gameState);
        }

        private void DrawSky(GameState gameState)
        {
            gameState.DayNightCycle = (gameState.DayNightCycle + 0.00003f) % 1f;
            float time = gameState.DayNightCycle;

            Color skyColor1, skyColor2;

            if (time < 0.25f)
            {
                float dawnFactor = time * 4;
                skyColor1 = new Color(
                    (int)(20 + 100 * dawnFactor),
                    (int)(25 + 50 * dawnFactor),
                    (int)(40 + 150 * dawnFactor)
                );
                skyColor2 = new Color(
                    (int)(40 + 140 * dawnFactor),
                    (int)(45 + 90 * dawnFactor),
                    (int)(60 + 100 * dawnFactor)
                );
            }
            else if (time < 0.5f)
            {
                float dayFactor = (time - 0.25f) * 4;
                skyColor1 = new Color(
                    (int)(120 + 50 * dayFactor),
                    (int)(140 + 50 * dayFactor),
                    (int)(190 + 30 * dayFactor)
                );
                skyColor2 = new Color(
                    (int)(180 + 30 * dayFactor),
                    (int)(200 + 20 * dayFactor),
                    220
                );
            }
            else if (time < 0.75f)
            {
                float duskFactor = (time - 0.5f) * 4;
                skyColor1 = new Color(
                    (int)(170 - 100 * duskFactor),
                    (int)(150 - 70 * duskFactor),
                    (int)(220 - 120 * duskFactor)
                );
                skyColor2 = new Color(
                    (int)(210 - 130 * duskFactor),
                    (int)(180 - 100 * duskFactor),
                    (int)(220 - 100 * duskFactor)
                );
            }
            else
            {
                float nightFactor = (time - 0.75f) * 4;
                skyColor1 = new Color(
                    (int)(70 - 50 * nightFactor),
                    (int)(50 - 25 * nightFactor),
                    (int)(100 - 60 * nightFactor)
                );
                skyColor2 = new Color(
                    (int)(80 - 40 * nightFactor),
                    (int)(80 - 60 * nightFactor),
                    (int)(120 - 60 * nightFactor)
                );
            }

            // Draw gradient sky
            DrawGradientRectangle(0, 0, GameState.CanvasWidth, GameState.CanvasHeight, skyColor1, skyColor2);
        }

        private void DrawBuildings(GameState gameState)
        {
            int baseY = GameState.CanvasHeight - 100;
            float time = gameState.DayNightCycle;

            float brightness = 0.25f;
            if (time < 0.25f || time > 0.75f) brightness = 0.15f;
            else if (time < 0.5f) brightness = 0.35f;
            else brightness = 0.2f;

            foreach (var building in gameState.Buildings)
            {
                int buildingY = baseY - (int)building.Height;
                float parallaxX = building.X * building.Parallax;

                DrawFilledRectangle(
                    (int)parallaxX,
                    buildingY,
                    (int)(building.Width * building.Parallax),
                    (int)building.Height,
                    ApplyBrightness(building.Color, brightness)
                );
            }
        }

        private void DrawGround(GameState gameState)
        {
            int groundHeight = 100;

            // Ground base
            DrawFilledRectangle(0, GameState.CanvasHeight - groundHeight, GameState.CanvasWidth, groundHeight, new Color(17, 17, 17));

            // Ground texture
            for (int i = 0; i < GameState.CanvasWidth; i += 40)
            {
                DrawFilledRectangle(i, GameState.CanvasHeight - groundHeight, 1, 10, new Color(26, 26, 26));
            }

            // Platform
            DrawFilledRectangle(100, GameState.CanvasHeight - groundHeight + 20, 100, 15, new Color(34, 34, 34));
        }

        private void DrawDefense(GameState gameState)
        {
            // Base
            DrawFilledRectangle(
                (int)gameState.Defense.Position.X - 25,
                (int)gameState.Defense.Position.Y,
                50, 20,
                new Color(51, 51, 51)
            );

            // Turret body
            DrawFilledCircle(
                (int)gameState.Defense.Position.X,
                (int)gameState.Defense.Position.Y - 10,
                20,
                new Color(138, 138, 255)
            );

            // Turret barrel
            Vector2 barrelEnd = gameState.Defense.Position + new Vector2(50 * (float)Math.Cos(gameState.Defense.Angle), 50 * (float)Math.Sin(gameState.Defense.Angle));
            DrawLine(gameState.Defense.Position, barrelEnd, 8, new Color(85, 85, 85));

            // Muzzle flash (would need to check fire timer)
        }

        private void DrawAircraft(Aircraft aircraft)
        {
            Vector2 pos = aircraft.Position;

            // Health bar
            float healthPercent = (float)aircraft.Health / (aircraft.Type == "BOMBER" ? 500 : 350);
            int barWidth = (int)(aircraft.Size * 2);

            DrawFilledRectangle((int)pos.X - barWidth / 2, (int)pos.Y - (int)aircraft.Size - 15, barWidth, 4, new Color(34, 34, 34));

            Color healthColor = healthPercent > 0.5f ? Color.LimeGreen : healthPercent > 0.25f ? Color.Yellow : Color.Red;
            DrawFilledRectangle((int)pos.X - barWidth / 2, (int)pos.Y - (int)aircraft.Size - 15, (int)(healthPercent * barWidth), 4, healthColor);

            // Aircraft body
            if (aircraft.Type == "BOMBER")
            {
                DrawFilledEllipse((int)pos.X, (int)pos.Y, (int)(aircraft.Size * 1.2), (int)(aircraft.Size * 0.6), aircraft.Color);
            }
            else
            {
                DrawFilledTriangle(
                    (int)(pos.X + aircraft.Size),
                    (int)pos.Y,
                    (int)(pos.X - aircraft.Size * 0.8f),
                    (int)(pos.Y + aircraft.Size * 0.4f),
                    (int)(pos.X - aircraft.Size),
                    (int)pos.Y,
                    aircraft.Color
                );
            }
        }

        private void DrawMissile(Missile missile)
        {
            DrawFilledCircle((int)missile.Position.X, (int)missile.Position.Y, (int)missile.Size, new Color(138, 138, 255));
        }

        private void DrawEnemyMissile(EnemyMissile missile)
        {
            DrawFilledCircle((int)missile.Position.X, (int)missile.Position.Y, (int)missile.Size, new Color(255, 85, 85));
        }

        private void DrawExplosion(Explosion explosion)
        {
            float alpha = explosion.Life / (float)explosion.MaxLife;
            float currentSize = explosion.Size * (1 - alpha);

            Color color1 = new Color(255, 150, 50, (int)(alpha * 0.6f * 255));
            DrawFilledCircle((int)explosion.Position.X, (int)explosion.Position.Y, (int)currentSize, color1);

            Color color2 = new Color(255, 100, 0, (int)(alpha * 0.4f * 255));
            DrawFilledCircle((int)explosion.Position.X, (int)explosion.Position.Y, (int)(currentSize * 0.7f), color2);
        }

        private void DrawCyberExplosion(CyberExplosion explosion)
        {
            float alpha = explosion.Life / (float)explosion.MaxLife;
            float currentSize = explosion.Size * (1 - alpha * 0.5f);

            Color outerColor = new Color(100, 200, 255, (int)(alpha * 0.3f * 255));
            DrawFilledCircle((int)explosion.Position.X, (int)explosion.Position.Y, (int)(currentSize * 1.5f), outerColor);

            Color mainColor = new Color(138, 138, 255, (int)(alpha * 0.7f * 255));
            DrawFilledCircle((int)explosion.Position.X, (int)explosion.Position.Y, (int)currentSize, mainColor);

            Color coreColor = new Color(255, 255, 255, (int)(alpha * 0.9f * 255));
            DrawFilledCircle((int)explosion.Position.X, (int)explosion.Position.Y, (int)(currentSize * 0.3f), coreColor);
        }

        private void DrawRadar(GameState gameState)
        {
            // Draw radar background
            int radarX = GameState.CanvasWidth - RadarWidth - 10;
            int radarY = 10;

            DrawFilledRectangle(radarX, radarY, RadarWidth, RadarHeight, new Color(10, 10, 15, 200));

            // Draw radar grid
            int centerX = radarX + RadarWidth / 2;
            int centerY = radarY + RadarHeight / 2;

            for (int i = 1; i <= 3; i++)
            {
                DrawCircle(centerX, centerY, i * 20, new Color(138, 138, 255, 75));
            }

            // Draw center point
            DrawFilledCircle(centerX, centerY, 2, new Color(138, 138, 255));

            // Draw aircraft on radar
            foreach (var aircraft in gameState.Planes)
            {
                Vector2 delta = aircraft.Position - gameState.Defense.Position;
                float dist = delta.Length();
                float radarDist = Math.Min(dist / 15, 55);
                float angle = (float)Math.Atan2(delta.Y, delta.X);

                int radarAircraftX = (int)(centerX + radarDist * Math.Cos(angle));
                int radarAircraftY = (int)(centerY + radarDist * Math.Sin(angle));

                Color radarColor = aircraft.Type == "BOMBER" ? new Color(255, 136, 68) : new Color(255, 68, 68);
                DrawFilledCircle(radarAircraftX, radarAircraftY, 4, radarColor);
            }
        }

        #region Drawing Helper Methods

        private void DrawFilledRectangle(int x, int y, int width, int height, Color color)
        {
            _spriteBatch.Draw(_whitePixel, new Rectangle(x, y, width, height), color);
        }

        private void DrawFilledCircle(int x, int y, int radius, Color color)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    if (dx * dx + dy * dy <= radius * radius)
                    {
                        _spriteBatch.Draw(_whitePixel, new Rectangle(x + dx, y + dy, 1, 1), color);
                    }
                }
            }
        }

        private void DrawFilledEllipse(int x, int y, int radiusX, int radiusY, Color color)
        {
            for (int dy = -radiusY; dy <= radiusY; dy++)
            {
                for (int dx = -radiusX; dx <= radiusX; dx++)
                {
                    float normalized = (dx * dx) / (float)(radiusX * radiusX) + (dy * dy) / (float)(radiusY * radiusY);
                    if (normalized <= 1)
                    {
                        _spriteBatch.Draw(_whitePixel, new Rectangle(x + dx, y + dy, 1, 1), color);
                    }
                }
            }
        }

        private void DrawFilledTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {
            // Simplified triangle drawing
            int minX = Math.Min(x1, Math.Min(x2, x3));
            int maxX = Math.Max(x1, Math.Max(x2, x3));
            int minY = Math.Min(y1, Math.Min(y2, y3));
            int maxY = Math.Max(y1, Math.Max(y2, y3));

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (IsPointInTriangle(x, y, x1, y1, x2, y2, x3, y3))
                    {
                        _spriteBatch.Draw(_whitePixel, new Rectangle(x, y, 1, 1), color);
                    }
                }
            }
        }

        private void DrawCircle(int x, int y, int radius, Color color)
        {
            for (int angle = 0; angle < 360; angle += 5)
            {
                float rad = MathHelper.ToRadians(angle);
                int px = x + (int)(radius * Math.Cos(rad));
                int py = y + (int)(radius * Math.Sin(rad));
                _spriteBatch.Draw(_whitePixel, new Rectangle(px, py, 1, 1), color);
            }
        }

        private void DrawLine(Vector2 from, Vector2 to, int thickness, Color color)
        {
            Vector2 direction = to - from;
            float angle = (float)Math.Atan2(direction.Y, direction.X);
            float length = direction.Length();

            _spriteBatch.Draw(_whitePixel,
                new Rectangle((int)from.X, (int)from.Y, (int)length, thickness),
                null,
                color,
                angle,
                Vector2.Zero,
                SpriteEffects.None,
                0);
        }

        private void DrawGradientRectangle(int x, int y, int width, int height, Color color1, Color color2)
        {
            for (int i = 0; i < height; i++)
            {
                float lerp = i / (float)height;
                Color lerpedColor = Color.Lerp(color1, color2, lerp);
                DrawFilledRectangle(x, y + i, width, 1, lerpedColor);
            }
        }

        private Color ApplyBrightness(Color color, float brightness)
        {
            return new Color(
                (int)(color.R * brightness),
                (int)(color.G * brightness),
                (int)(color.B * brightness),
                color.A
            );
        }

        private bool IsPointInTriangle(int px, int py, int x1, int y1, int x2, int y2, int x3, int y3)
        {
            int area = Math.Abs((x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1));
            int area1 = Math.Abs((px - x2) * (y3 - y2) - (x3 - x2) * (py - y2));
            int area2 = Math.Abs((x1 - px) * (y3 - py) - (x3 - px) * (y1 - py));
            int area3 = Math.Abs((x1 - x2) * (py - y2) - (px - x2) * (y1 - y2));

            return area == area1 + area2 + area3;
        }

        #endregion
    }
}
