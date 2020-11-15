namespace HSEngine.Events
{
    public class WindowResizeEventArgs : EngineEventArgs
    {
        public WindowResizeEventArgs(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.EventType = EventType.WindowResize;
            this.EventCategory = EventCategory.Application;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public override string ToString()
        {
            return $"WindowResizeEvent: w({this.Width}), h({this.Height})"; 
        }
    }

    public class WindowCloseEventArgs : EngineEventArgs
    {
        public WindowCloseEventArgs()
        {
            this.EventType = EventType.WindowClose;
            this.EventCategory = EventCategory.Application;
        }
    }

    public class AppTickEventArgs : EngineEventArgs
    {
        public AppTickEventArgs()
        {
            this.EventType = EventType.AppTick;
            this.EventCategory = EventCategory.Application;
        }
    }

    public class AppUpdateEventArgs : EngineEventArgs
    {
        public AppUpdateEventArgs()
        {
            this.EventType = EventType.AppUpdate;
            this.EventCategory = EventCategory.Application;
        }
    }

    public class AppRenderEventArgs : EngineEventArgs
    {
        public AppRenderEventArgs()
        {
            this.EventType = EventType.AppRender;
            this.EventCategory = EventCategory.Application;
        }
    }
}
