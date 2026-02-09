using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace DefenseGameCSharp
{
    public class InputManager
    {
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        public Vector2 MousePosition { get; private set; }
        public bool SpacePressed { get; private set; }

        public InputManager()
        {
            MousePosition = Vector2.Zero;
            SpacePressed = false;
        }

        public void Update()
        {
            _previousKeyboardState = _currentKeyboardState;
            _previousMouseState = _currentMouseState;

            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();

            MousePosition = new Vector2(_currentMouseState.X, _currentMouseState.Y);
            SpacePressed = _currentKeyboardState.IsKeyDown(Keys.Space);
        }

        public bool IsKeyPressed(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyJustPressed(Keys key)
        {
            return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
        }

        public bool IsKeyJustReleased(Keys key)
        {
            return _currentKeyboardState.IsKeyUp(key) && _previousKeyboardState.IsKeyDown(key);
        }
    }
}
