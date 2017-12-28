using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using OpenTK;

namespace opengl.Engine
{
    public static class Input
    {
        private static List<Key> _KeysDown;
        private static List<Key> _KeysDownLast;

        private static List<MouseButton> _ButtonsDown;
        private static List<MouseButton> _ButtonsDownLast;

        public static void Initialize(GameWindow game)
        {
            _KeysDown = new List<Key>();
            _KeysDownLast = new List<Key>();
            _ButtonsDown = new List<MouseButton>();
            _ButtonsDownLast = new List<MouseButton>();

            game.KeyDown += Game_KeyDown;
            game.KeyUp += Game_KeyUp;

            game.MouseDown += Game_MouseDown;
            game.MouseUp += Game_MouseUp;
        }

        #region Public Methods

        public static void Update()
        {
            _KeysDownLast = new List<Key>(_KeysDown);
            _ButtonsDownLast = new List<MouseButton>(_ButtonsDown);
        }

        public static bool KeyPress(Key key)
        {
            return _KeysDown.Contains(key) && !_KeysDownLast.Contains(key);
        }

        public static bool KeyRelease(Key key)
        {
            return !_KeysDown.Contains(key) && _KeysDownLast.Contains(key);
        }

        public static bool KeyDown(Key key)
        {
            return _KeysDown.Contains(key);
        }

        public static bool MousePress(MouseButton button)
        {
            return _ButtonsDown.Contains(button) && !_ButtonsDownLast.Contains(button);
        }

        public static bool MouseRelease(MouseButton button)
        {
            return !_ButtonsDown.Contains(button) && _ButtonsDownLast.Contains(button);
        }

        public static bool MouseDown(MouseButton button)
        {
            return _ButtonsDown.Contains(button);
        }

        #endregion

        #region Event Handlers

        private static void Game_MouseUp(object sender, MouseButtonEventArgs e)
        {
            while(_ButtonsDown.Contains(e.Button))
                _ButtonsDown.Remove(e.Button);
        }

        private static void Game_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _ButtonsDown.Add(e.Button);
        }

        private static void Game_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            while(_KeysDown.Contains(e.Key))
                _KeysDown.Remove(e.Key);
        }

        private static void Game_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            _KeysDown.Add(e.Key);
        }

        #endregion
    }
}
