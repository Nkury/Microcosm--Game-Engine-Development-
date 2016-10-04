using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    // ** PARENT CLASS **
    // Describes a basic collider for both sphere and box collider child classes
    public class Collider : Component
    {
        // inherited by children; default is FALSE if not inherited by children
        public virtual bool Collides(Collider other, out Vector3 normal)
        {
            normal = Vector3.Zero;
            return false;
        }

        // checks if mouse intersects with 3D object collider
        public virtual float? Intersects(Ray ray) { return null; }
    }
}
