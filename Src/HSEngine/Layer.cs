using HSEngine.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace HSEngine
{
    public abstract class Layer
    {
        protected Layer(string debugName = "Layer")
        {
            this.DebugName = debugName;
        }

        public string DebugName { get; protected set; }

        public virtual void OnAttach() { }
        public virtual void OnDetach() { }
        public virtual void OnUpdate() { }
        public virtual void OnEvent(EngineEventArgs e) { }
    }
}
