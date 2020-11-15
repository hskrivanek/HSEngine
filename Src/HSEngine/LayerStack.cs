using System;
using System.Collections.Generic;

namespace HSEngine
{
    public sealed class LayerStack : IDisposable
    {
        private int layerInsertIndex = 0;
        private readonly List<Layer> layers = new List<Layer>();

        public IReadOnlyList<Layer> Layers => layers;

        public void PushLayer(Layer layer)
        {
            layers.Insert(layerInsertIndex, layer);
            layerInsertIndex++;
        }

        public void PushOverlay(Layer layer)
        {
            layers.Add(layer);
        }

        public void RemoveLayer(Layer layer)
        {
            bool wasLayerRemoved = layers.Remove(layer);
            if (wasLayerRemoved)
            {
                layer.OnDetach();
                layerInsertIndex--;
            }
        }

        public void RemoveOverlay(Layer layer)
        {
            layer.OnDetach();
            layers.Remove(layer);
        }

        public void Dispose()
        {
            foreach (var layer in layers)
            {
                layer.OnDetach();
            }
        }
    }
}
