using HSEngine.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace HSEngine
{
    public abstract class Window
    {
        public event EventHandler<EngineEventArgs> EventEmitted;

        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public bool IsVSyncEnabled { get; protected set; }

        public abstract void OnUpdate();
        public abstract void EnableVSync();
        public abstract void DisableVSync();

        protected void EmitEngineEvent(EngineEventArgs e)
        {
            EventEmitted?.Invoke(this, e);
        }
    }
}
