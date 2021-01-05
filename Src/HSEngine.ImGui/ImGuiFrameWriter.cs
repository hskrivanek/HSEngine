using ImGuiNET;
using System;
using System.Collections.Generic;

namespace HSEngine.ImGuiUtils
{
    public static class ImGuiFrameWriter
    {
        private readonly static Queue<Action> actions = new Queue<Action>();

        public static void DrawQueueToFrame()
        {
            while (actions.Count > 0)
            {
                var action = actions.Dequeue();
                action();
            }
        }

        public static void EnqueueDemoWindowDrawing()
        { 
            static void action()
            { 
                var show = true;
                ImGui.ShowDemoWindow(ref show);
            }

            actions.Enqueue(action);
        }

        public static void EnqueueCustomDrawingAction(Action action)
        {
            actions.Enqueue(action);
        }
    }
}
