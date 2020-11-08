using HSEngine;
using HSEngine.Logging;
using System;

namespace Sandbox
{
    class SandboxApp : Application
    {
        static void Main(string[] args)
        {
            var app = new SandboxApp();
            app.Run();

            Log.ClientLogger.Trace("Exiting");
        }
    }
}
