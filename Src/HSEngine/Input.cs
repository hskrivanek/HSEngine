using HSEngine.Events;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace HSEngine
{
    public static class Input
    {
        private readonly static HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>();
        private static HashSet<KeyCode> previouslyPressedKeys = new HashSet<KeyCode>();

        private readonly static HashSet<MouseCode> pressedMouseButtons = new HashSet<MouseCode>();
        private static HashSet<MouseCode> previouslyPressedMouseButtons = new HashSet<MouseCode>();

        private static Vector2 mousePosition = new Vector2();

        public static bool Key(KeyCode key) => pressedKeys.Contains(key);
        public static bool KeyDown(KeyCode key) => pressedKeys.Contains(key) && !previouslyPressedKeys.Contains(key);
        public static bool KeyUp(KeyCode key) => !pressedKeys.Contains(key) && previouslyPressedKeys.Contains(key);

        public static bool MouseButton(MouseCode button) => pressedMouseButtons.Contains(button);
        public static bool MouseButtonDown(MouseCode button) => pressedMouseButtons.Contains(button) 
                                                             && !previouslyPressedMouseButtons.Contains(button);
        public static bool MouseButtonUp(MouseCode button) => !pressedMouseButtons.Contains(button) 
                                                           && previouslyPressedMouseButtons.Contains(button);

        public static Vector2 MousePosition => mousePosition;

        internal static void UpdateFrame()
        {
            previouslyPressedKeys = new HashSet<KeyCode>(pressedKeys);
            previouslyPressedMouseButtons = new HashSet<MouseCode>(pressedMouseButtons);
        }

        internal static void UpdateState(EngineEventArgs engineEvent)
        {
            switch (engineEvent)
            {
                case KeyDownEventArgs e:
                    pressedKeys.Add(e.KeyCode);
                    break;
                case KeyUpEventArgs e:
                    pressedKeys.Remove(e.KeyCode);
                    break;
                case MouseButtonDownEventArgs e:
                    pressedMouseButtons.Add(e.MouseCode);
                    break;
                case MouseButtonUpEventArgs e:
                    pressedMouseButtons.Remove(e.MouseCode);
                    break;
                case MouseMovedEventArgs e:
                    mousePosition = new Vector2(e.X, e.Y);
                    break;
                default:
                    break;
            }
        }
    }
}
