using HSEngine.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace HSEngine
{
    public abstract class Layer
    {
        protected Layer(string debugName)
        {
            this.DebugName = debugName;
        }

        public string DebugName { get; protected set; }

        public abstract void OnAttach();
        public abstract void OnDetach();
        public abstract void OnUpdate();
        public abstract void OnEvent(EngineEventArgs e);
    }
}
