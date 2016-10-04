using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    // The InputManager is our game engine's own custom input controller, which borrows a lot of functions
    // defined in the XNA/MonoGame framework. We will use this in our Main method to take care of control
    // systems
    public static class InputManager
    {
        private static KeyboardState PreviousKeyboardState { get; set; }
        private static KeyboardState CurrentKeyboardState { get; set; }
        private static MouseState PreviousMouseState { get; set; }
        private static MouseState CurrentMouseState { get; set; }
        public static void Initialize()
        {
            PreviousKeyboardState = CurrentKeyboardState = Keyboard.GetState();
            PreviousMouseState = CurrentMouseState = Mouse.GetState();
        }

        // On every frame, update the current control states through checking keyboard and mouse states
        public static void Update()
        {
            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }

        // is key held down
        public static bool IsKeyDown(Keys key) { return CurrentKeyboardState.IsKeyDown(key); }

        // is key not held down
        public static bool IsKeyUp(Keys key) { return CurrentKeyboardState.IsKeyUp(key); }

        // is key pressed once
        public static bool IsKeyPressed(Keys key) { return CurrentKeyboardState.IsKeyDown(key) && PreviousKeyboardState.IsKeyUp(key); }

        // is key released
        public static bool IsKeyReleased(Keys key) { return CurrentKeyboardState.IsKeyUp(key) && PreviousKeyboardState.IsKeyDown(key); }

        // returns position of current mouse pointer on screen
        public static Vector2 GetMousePosition() { return new Vector2(CurrentMouseState.X, CurrentMouseState.Y); }

        // has mouse been pressed
        public static bool IsMousePressed(int mouseButton)
        {
            switch (mouseButton)
            {
                case 0: return PreviousMouseState.LeftButton == ButtonState.Released && CurrentMouseState.LeftButton == ButtonState.Pressed;
                case 1: return PreviousMouseState.RightButton == ButtonState.Released && CurrentMouseState.RightButton == ButtonState.Pressed;
                case 2: return PreviousMouseState.MiddleButton == ButtonState.Released && CurrentMouseState.MiddleButton == ButtonState.Pressed;
                default: return false;
            }
        }
        
        // has mouse been released
        public static bool IsMouseReleased(int mouseButton)
        {
            switch (mouseButton)
            {
                case 0: return PreviousMouseState.LeftButton == ButtonState.Pressed && CurrentMouseState.LeftButton == ButtonState.Released;
                case 1: return PreviousMouseState.RightButton == ButtonState.Pressed && CurrentMouseState.RightButton == ButtonState.Released;
                case 2: return PreviousMouseState.MiddleButton == ButtonState.Pressed && CurrentMouseState.MiddleButton == ButtonState.Released;
                default: return false;
            }
        }

        /* Old version
         * public static class InputManager
        {
            static KeyboardState PreviousKeyboardState { get; set; }
            static KeyboardState CurrentKeyboardState { get; set; }
            static MouseState PreviousMouseState { get; set; }
            static MouseState CurrentMouseState { get; set; }
            public static void Initialize()
            {
                PreviousKeyboardState = CurrentKeyboardState = Keyboard.GetState();
                PreviousMouseState = CurrentMouseState = Mouse.GetState();
            }
            public static void Update()
            {
                PreviousKeyboardState = CurrentKeyboardState;
                CurrentKeyboardState = Keyboard.GetState();
                PreviousMouseState = CurrentMouseState;
                CurrentMouseState = Mouse.GetState();
            }
            public static bool IsKeyDown(Keys key)
            {
                return CurrentKeyboardState.IsKeyDown(key);
            }
            public static bool IsKeyPressed(Keys key)
            {
                return CurrentKeyboardState.IsKeyDown(key) && PreviousKeyboardState.IsKeyUp(key);
            }
            public static bool IsKeyUp(Keys key)
            {
                return CurrentKeyboardState.IsKeyUp(key);
            }
            public static bool IsKeyReleased (Keys key){
                return PreviousKeyboardState.IsKeyDown(key) && CurrentKeyboardState.IsKeyUp(key);
            }
        }
         */
    }
}