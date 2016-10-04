using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    // the Camera class acts like the Camera object in Unity3D game engine.
    // The camera has an aspect ratio, field of view, a near plane and a far plane,
    // along with its transform position, rotation, and scale along with viewports, which
    // come with different views like orthographic and projection views
    public class Camera : Component
    {
	    public float FieldOfView { get; set; }
	    public float AspectRatio { get; set; }
	    public float NearPlane { get; set; }
	    public float FarPlane { get; set; }

	    public Transform Transform { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }

        // set the viewport of the camera the same as what is defined
        // in the Screen Manager
        public Viewport Viewport
        {
            get
            {
                return new Viewport((int)(ScreenManager.Width * Position.X),
                            (int)(ScreenManager.Height * Position.Y),
                            (int)(ScreenManager.Width * Size.X),
                            (int)(ScreenManager.Height * Size.Y));
            }
        }

        // default camera values
        public Camera()
        {
            FieldOfView = MathHelper.PiOver2;
            AspectRatio = 1.0f;
            NearPlane = 0.1f;
            FarPlane = 1000f;
            Transform = null;
            Position = Vector2.Zero;
            Size = Vector2.One;
        }

        // projection view of the camera
	    public Matrix Projection
	    {
		    get { return Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane); }
	    }

	    public Matrix View
	    {
		    get
		    {
			    return Matrix.CreateLookAt(Transform.Position,
				    Transform.Position + Transform.Forward,
				    Transform.Up);
		    }
	    }

        // Handles raycasting to 3D objects on screen (helps with the Intersect functions defined in
        // box and sphere colliders). Takes the viewport into consideration when it unprojects the
        // screen value with the world values.
        public Ray ScreenPointToWorldRay(Vector2 position)
        {
            Vector3 start = Viewport.Unproject(new Vector3(position, 0),
                Projection, View, Matrix.Identity);
            Vector3 end = Viewport.Unproject(new Vector3(position, 1),
                Projection, View, Matrix.Identity);
            return new Ray(start, end - start);
        }

    }
}
