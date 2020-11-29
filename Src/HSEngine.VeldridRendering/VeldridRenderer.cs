using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;

namespace HSEngine.VeldridRendering
{
    public abstract class VeldridRenderer
    {
        protected GraphicsDevice gd;
        protected CommandList cl;

        protected VeldridRenderer(GraphicsDevice gd, CommandList cl)
        {
            this.gd = gd;
            this.cl = cl;
        }
    }
}
