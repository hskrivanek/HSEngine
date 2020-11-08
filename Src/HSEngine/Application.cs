using HSEngine.Logging;
using System;
using System.Reflection.Metadata.Ecma335;

namespace HSEngine
{
    public abstract class Application
    {
        protected Application()
        {
            Log.Init();
        }

        public virtual void Run()
        {
            Log.CoreLogger.Trace("HSEngine application startup");
        }
    }
}
