using System;
using System.Windows.Input;

namespace HSEngine.Windows
{
    internal static class InputConverter
    {
        internal static KeyCode ConvertKeyToEngine(Key key)
        {
            // Directly convert as the engine uses key codes from Mono
            return (KeyCode)((int)key);
        }

        internal static MouseCode ConvertMouseButtonToEngine(MouseButton button)
        {
            return (MouseCode)((int)button);
        }
    }
}
