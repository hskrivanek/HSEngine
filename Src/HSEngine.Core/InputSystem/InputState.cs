using System.Linq;
using Veldrid;

namespace HSEngine.Core.InputSystem
{
    public class InputState
    {
        private InputSnapshot input;

        public InputState()
        {
        }

        public InputState(InputSnapshot input)
        {
            this.input = input;
        }
    }
}
