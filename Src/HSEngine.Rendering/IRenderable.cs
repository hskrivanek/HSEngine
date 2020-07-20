namespace HSEngine.Rendering
{
    public interface IRenderable
    {
        void InitializeMesh(Renderer renderer);
        void Draw(Renderer renderer, SceneRenderContext context);
    }
}
