namespace HSEngine.Events
{
    public enum EventType
    {
        None,

        WindowClose,
        WindowResize,
        WindowFocus,
        WindowLostFocus,
        WindowMoved,

        AppTick,
        AppUpdate,
        AppRender,

        KeyPressed,
        KeyUp,
        KeyDown,
        KeyTyped,

        MouseButtonDown,
        MouseButtonUp,
        MouseMoved,
        MouseScrolled
    }
}
