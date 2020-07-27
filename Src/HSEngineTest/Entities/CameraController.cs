using HSEngine.Core.Entities;
using HSEngine.Core.InputSystem;
using HSEngine.Core.Scenes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Veldrid;

namespace HSEngineTest.Entities
{
    public class CameraController: Entity
    {
        public override void Update(SceneContext sceneContext)
        {
            var camera = sceneContext.MainCamera;

            if (Input.GetKey(Key.A))
            {
                camera.Transform.Translate(new Vector3(-0.05f, 0, 0));
            }
            else if (Input.GetKey(Key.D))
            {
                camera.Transform.Translate(new Vector3(0.05f, 0, 0));
            }

            if (Input.GetKey(Key.W))
            {
                camera.Transform.Translate(new Vector3(0, 0, -0.05f));
            }
            else if (Input.GetKey(Key.S))
            {
                camera.Transform.Translate(new Vector3(0, 0, 0.05f));
            }
        }
    }
}
