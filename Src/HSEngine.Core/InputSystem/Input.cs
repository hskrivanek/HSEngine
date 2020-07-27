using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;

namespace HSEngine.Core.InputSystem
{
    public static class Input
    {
        private readonly static HashSet<Key> currentlyPressedKeys = new HashSet<Key>();
        private readonly static HashSet<Key> newlyPressedKeys = new HashSet<Key>();

        public static bool GetKey(Key key) => currentlyPressedKeys.Contains(key);
        public static bool GetKeyDown(Key key) => newlyPressedKeys.Contains(key);

        public static void UpdateFrameInput(InputSnapshot snapshot)
        {
            newlyPressedKeys.Clear();
            foreach (KeyEvent keyEvent in snapshot.KeyEvents)
            {
                if (keyEvent.Down)
                {
                    KeyDown(keyEvent.Key);
                }
                else
                {
                    KeyUp(keyEvent.Key);
                }
            }
        }

        private static void KeyDown(Key key)
        {
            if (currentlyPressedKeys.Add(key))
            {
                newlyPressedKeys.Add(key);
            }
        }

        private static void KeyUp(Key key)
        {
            currentlyPressedKeys.Remove(key);
            newlyPressedKeys.Remove(key);
        }
    }
}
