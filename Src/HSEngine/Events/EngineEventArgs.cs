using System;

namespace HSEngine.Events
{
    public abstract class EngineEventArgs : EventArgs
    {
        public bool Handled { get; set; }
        public EventType EventType { get; protected set; }
        public EventCategory EventCategory { get; protected set; }

        public override string ToString() => $"{this.EventType}Event";
    }
}
