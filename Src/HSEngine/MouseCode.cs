namespace HSEngine
{
#pragma warning disable CA1027 // Mark enums with FlagsAttribute
    public enum MouseCode
#pragma warning restore CA1027 // Mark enums with FlagsAttribute
    {
        Button0 = 0,
        Button1 = 1,
        Button2 = 2,
        Button3 = 3,
        Button4 = 4,

        // Equivalent to System.Windows.Input.MouseButton
        Left = Button0,
        Middle = Button1,
        Right = Button2
    }
}
