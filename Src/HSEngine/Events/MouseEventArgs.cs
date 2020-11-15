namespace HSEngine.Events
{
    public class MouseMovedEventArgs : EngineEventArgs
    {
        public MouseMovedEventArgs(float x, float y)
        {
            this.X = x;
            this.Y = y;
            this.EventType = EventType.MouseMoved;
            this.EventCategory = EventCategory.Mouse | EventCategory.Input;
        }

        public float X { get; private set; }
        public float Y { get; private set; }

        public override string ToString()
        {
            return $"MouseMovedEvent: x({this.X}), y({this.Y})";
        }
    }

    public class MouseScrolledEventArgs : EngineEventArgs
    {
        public MouseScrolledEventArgs(float xOffset, float yOffset)
        {
            this.XOffset = xOffset;
            this.YOffset = yOffset;
            this.EventType = EventType.MouseScrolled;
            this.EventCategory = EventCategory.Mouse | EventCategory.Input;
        }

        public float XOffset { get; private set; }
        public float YOffset { get; private set; }

        public override string ToString()
        {
            return $"MouseScrolledEvent: x({this.XOffset}), y({this.YOffset})";
        }
    }

    public abstract class MouseButtonEventArgs : EngineEventArgs
    {
        protected MouseButtonEventArgs(MouseCode mouseCode)
        {
            MouseCode = mouseCode;
            this.EventCategory = EventCategory.Mouse | EventCategory.MouseButton | EventCategory.Input;
        }

        public MouseCode MouseCode { get; private set; }
    }

    public class MouseButtonDownEventArgs : MouseButtonEventArgs
    {
        public MouseButtonDownEventArgs(MouseCode mouseCode) : base(mouseCode)
        {
            this.EventType = EventType.MouseButtonDown;
        }

        public override string ToString()
        {
            return $"MouseButtonDownEvent: {this.MouseCode}";
        }
    }

    public class MouseButtonUpEventArgs : MouseButtonEventArgs
    {
        public MouseButtonUpEventArgs(MouseCode mouseCode) : base(mouseCode)
        {
            this.EventType = EventType.MouseButtonUp;
        }

        public override string ToString()
        {
            return $"MouseButtonUpEvent: {this.MouseCode}";
        }
    }
}
