namespace HSEngine.Events
{
    public abstract class KeyEventArgs : EngineEventArgs
    {
        protected KeyEventArgs()
        {
            
            this.EventCategory = EventCategory.Keyboard | EventCategory.Input;
        }
    }

    public class KeyPressedEventArgs : KeyEventArgs
    {
        public KeyPressedEventArgs(KeyCode keyCode, int repeatCount)
        {
            this.KeyCode = keyCode;
            this.RepeatCount = repeatCount;
            this.EventType = EventType.KeyPressed;
        }

        public KeyCode KeyCode { get; private set; }
        public int RepeatCount { get; private set; }

        public override string ToString()
        {
            return $"KeyPressedEvent: {this.KeyCode} ({this.RepeatCount} repeats)";
        }
    }

    public class KeyDownEventArgs : KeyEventArgs
    {
        public KeyDownEventArgs(KeyCode keyCode)
        {
            this.KeyCode = keyCode;
            this.EventType = EventType.KeyDown;
        }

        public KeyCode KeyCode { get; private set; }

        public override string ToString()
        {
            return $"KeyDownEvent: {this.KeyCode}";
        }
    }

    public class KeyUpEventArgs : KeyEventArgs
    {
        public KeyUpEventArgs(KeyCode keyCode)
        {
            this.KeyCode = keyCode;
            this.EventType = EventType.KeyUp;
        }

        public KeyCode KeyCode { get; private set; }

        public override string ToString()
        {
            return $"KeyUpEvent: {this.KeyCode}";
        }
    }

    public class KeyTypedEventArgs : KeyEventArgs
    {
        public KeyTypedEventArgs(char typedCharacter)
        {
            this.TypedCharacter = typedCharacter;
            this.EventType = EventType.KeyTyped;
        }

        public char TypedCharacter { get; private set; }

        public override string ToString()
        {
            return $"KeyTypedEvent: {TypedCharacter}";
        }
    }
}
