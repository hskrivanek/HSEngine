using HSEngine.Core.Scenes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HSEngine.Core.Entities
{
    interface IUpdatable
    {
        void Update(SceneContext sceneContext);
    }
}
