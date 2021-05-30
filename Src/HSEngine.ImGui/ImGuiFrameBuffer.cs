using ImGuiNET;
using System;
using System.Collections.Generic;

namespace HSEngine.ImGuiUtils
{
    public class ImGuiFrameBuffer
    {
        private readonly Queue<Action> actions = new Queue<Action>();

        public void DrawFromQueueToFrame()
        {
            while (actions.Count > 0)
            {
                var action = actions.Dequeue();
                action();
            }
        }

        public void EnqueueDemoWindowDrawing()
        { 
            static void action()
            { 
                var show = true;
                ImGui.ShowDemoWindow(ref show);
            }

            actions.Enqueue(action);
        }

        public void EnqueueCustomDrawingAction(Action action)
        {
            actions.Enqueue(action);
        }
    }
}
