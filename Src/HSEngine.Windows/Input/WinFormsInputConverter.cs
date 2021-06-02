using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

namespace HSEngine.Windows.Input
{
    internal static class WinFormsInputConverter
    {
        internal static KeyCode ConvertKeyToEngine(Keys key)
        {
            var wpfKey = KeyInterop.KeyFromVirtualKey((int)key);
            // Directly convert as the engine uses key codes from Mono
            return (KeyCode)((int)wpfKey);
        }

        internal static MouseCode ConvertMouseButtonToEngine(MouseButtons button)
        {
            return button switch
            {
                MouseButtons.Left => MouseCode.Left,
                MouseButtons.None => MouseCode.None,
                MouseButtons.Right => MouseCode.Right,
                MouseButtons.Middle => MouseCode.Middle,
                MouseButtons.XButton1 => MouseCode.Button3,
                MouseButtons.XButton2 => MouseCode.Button4,
                _ => MouseCode.None,
            };
        }
    }
}
