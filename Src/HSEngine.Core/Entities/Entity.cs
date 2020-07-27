using HSEngine.Core.Components;
using HSEngine.Core.Scenes;

namespace HSEngine.Core.Entities
{
    public abstract class Entity: IUpdatable, IInitializable
    {
        protected Entity()
        {
            this.Transform = new Transform();
        }

        public Transform Transform { get; set; }

        public virtual void Initialize() { }
        public virtual void Update(SceneContext sceneContext) { }
    }
}
