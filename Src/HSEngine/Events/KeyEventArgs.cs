namespace HSEngine.Events
{
    public abstract class KeyEventArgs : EngineEventArgs
    {
        protected KeyEventArgs(KeyCode keyCode)
        {
            this.KeyCode = keyCode;
            this.EventCategory = EventCategory.Keyboard | EventCategory.Input;
        }

        public KeyCode KeyCode { get; private set; }
    }

    public class KeyPressedEventArgs : KeyEventArgs
    {
        public KeyPressedEventArgs(KeyCode keyCode, int repeatCount) : base(keyCode)
        {
            this.RepeatCount = repeatCount;
            this.EventType = EventType.KeyPressed;
        }

        public int RepeatCount { get; private set; }

        public override string ToString()
        {
            return $"KeyPressedEvent: {this.KeyCode} ({this.RepeatCount} repeats)";
        }
    }

    public class KeyDownEventArgs : KeyEventArgs
    {
        public KeyDownEventArgs(KeyCode keyCode) : base(keyCode)
        {
            this.EventType = EventType.KeyDown;
        }

        public override string ToString()
        {
            return $"KeyDownEvent: {this.KeyCode}";
        }
    }

    public class KeyUpEventArgs : KeyEventArgs
    {
        public KeyUpEventArgs(KeyCode keyCode) : base(keyCode)
        {
            this.EventType = EventType.KeyUp;
        }

        public override string ToString()
        {
            return $"KeyUpEvent: {this.KeyCode}";
        }
    }
}
