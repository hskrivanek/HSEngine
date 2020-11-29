using ImGuiNET;
using System;

namespace HSEngine.ImGuiUtils
{
    public interface IImGuiRenderer
    {
        void InitializeRenderer(ImGuiIOPtr io, IntPtr fontTexId);
        void RenderImDrawData(ImDrawDataPtr drawData);
    }
}
