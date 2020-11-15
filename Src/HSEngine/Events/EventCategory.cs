using System;

namespace HSEngine.Events
{
    [Flags]
    public enum EventCategory
    {
		None = 0,
		Application = 1 << 0,
		Input = 1 << 2,
		Keyboard = 1 << 3,
		Mouse = 1 << 4,
		MouseButton = 1 << 5
	}
}
