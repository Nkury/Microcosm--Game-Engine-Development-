using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
namespace CPI311.GameEngine
{
    // The SphereCollider class is responsible for drawing a collision box around a spherical object and checking if other objects
    // collide with said object. Upon collision, basic kinematic pyhysics equations will be used to calculate the force 
    //repelled from either 3D object.
    public class SphereCollider : Collider
    {
        public float Radius { get; set; }

        // returns TRUE if sphere collides with either another sphere or box by checking if 
        // the edges of the 3D objects touch.
        public override bool Collides(Collider other, out Vector3 normal)
        {
            if (other is SphereCollider)
            {
                SphereCollider collider = other as SphereCollider;

                // if the distance between the two spheres squared is less than the radiuses of both spheres squared, then we 
                // know the two spheres have collided with each other right at the edges and have not overlapped with eachother
                if ((Transform.Position - collider.Transform.Position).LengthSquared() < System.Math.Pow(Radius + collider.Radius, 2))
                {
                    // return then normal vector of the intersection point of the two spheres in order to 
                    // repel spheres in a perpendicular like fashion
                    normal = Vector3.Normalize
                        (Transform.Position - collider.Transform.Position);
                    return true;
                }
            }
            
            else if (other is BoxCollider) return other.Collides(this, out normal);

            return base.Collides(other, out normal);

        }

        // handles collisions with either a sphere collider or a box collider.
        public  bool SweptCollides(Collider other, Vector3 otherLastPosition, 
                                   Vector3 lastPosition, out Vector3 normal)
        {
            if (other is SphereCollider)
            {
                SphereCollider collider = other as SphereCollider;

                // calculate the vectors for two spheres (vector for sphere P [vp] and vector for sphere Q [vq]
                Vector3 vp = Transform.Position - lastPosition;
                Vector3 vq = collider.Transform.Position - otherLastPosition;
	
		        // calculate the A and B 
                Vector3 A = lastPosition - otherLastPosition;
                Vector3 B = vp - vq;

		        // calculate the a, b, and c
                float a = Vector3.Dot(B, B);
                float b = 2 * Vector3.Dot(A, B);
                float c = - ((Vector3.Dot(A, B) * Vector3.Dot(A, B)) / Vector3.Dot(B, B));
                float disc = b*b - 4*a*c; // discriminant (b^2 – 4ac)
                
                if (disc >=0 )
                {
                    float t = (-Vector3.Dot(A, B) - (float)Math.Sqrt(disc)) / Vector3.Dot(B, B);
                    Vector3 p = lastPosition + t * vp;
                    Vector3 q = otherLastPosition + t * vq;
                    Vector3 intersect = Vector3.Lerp(
                       p, q, this.Radius / (this.Radius + collider.Radius));
                    normal = Vector3.Normalize(p - q);
                    return true;
                }
            }
            else if (other is BoxCollider)
                return other.Collides(this, out normal);
            return base.Collides(other, out normal);
        }

        // Checks to see if mouse position touches the bounds of a sphere
        // through a raycasting system and returns the distance from 
        // mouse pointer to selected object
        public override float? Intersects (Ray ray)
        {
	        Matrix worldInv = Matrix.Invert(Transform.World);

            // calculate position of mouse pointer and direction it is pointing by
            // transforming it around the inverse of the world values.
	        ray.Position = Vector3.Transform(ray.Position, worldInv);
	        ray.Direction = Vector3.TransformNormal (ray.Direction,
								        worldInv);
	        float length = ray.Direction.Length();
	        ray.Direction /= length; // same as normalization
	        float? p = new BoundingSphere(Vector3.Zero, Radius).
			        Intersects(ray);
	        if(p != null)
		        return (float)p * length;
	        return null;
        }
    }
}
