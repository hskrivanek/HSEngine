using HSEngine.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace HSEngine
{
    public abstract class Window
    {
        public event EventHandler<EngineEventArgs> EventEmitted;

        public abstract bool IsVSyncEnabled { get; }
        public abstract int Width { get; }
        public abstract int Height { get; }

        public abstract void OnUpdateStart();
        public abstract void OnUpdateEnd();
        public abstract void EnableVSync();
        public abstract void DisableVSync();

        protected void EmitEngineEvent(EngineEventArgs e)
        {
            EventEmitted?.Invoke(this, e);
        }
    }
}
