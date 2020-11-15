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

        MouseButtonDown,
        MouseButtonUp,
        MouseMoved,
        MouseScrolled
    }
}
