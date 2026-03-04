using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixTransformations
{
    public class RenderObject
    {
        public IModel model;
        public TransformState transformState;

        public RenderObject(IModel model, TransformState transformState)
        {
            this.model = model;
            this.transformState = transformState;
        }
    }
}