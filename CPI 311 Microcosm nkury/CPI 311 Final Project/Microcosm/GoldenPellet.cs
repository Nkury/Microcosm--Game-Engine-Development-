using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPI311.GameEngine;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
namespace Microcosm
{
    // The GoldenPellet class defines a custom game object
    // of golden nuggets scattered throughout the level. Getting
    // a certain amount gives the player a certain power-up. This class just
    // defines the game object's starting position, collider, and renderer with
    // an overall lifetime.
    public class GoldenPellet : GameObject
    {
        public bool isActive { get; set; }
        public double timeAlive { get; set; }
        public int interval = 0;
  
        // Golden nuggets are instantiated by spawning somewhere around the room, having a mass of 1, having a sphere collider,
        // and a "pellet" texture for the renderer.
          public GoldenPellet(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light)
            : base()
        {
            Random random = new Random();
            this.Transform.Position = new Vector3(random.Next(-15, 15), random.Next(-15, 15), random.Next(-15, 15)); //+ Vector3.Left * 5 + Vector3.Down * 3;
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Transform = Transform;
            sphereCollider.Radius = 2f;
            Add<Collider>(sphereCollider);
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(
                Content.Load<Model>("pellet"), Transform, camera, Content, graphicsDevice, light, 1, null, 20f, texture);
            Add<Renderer>(renderer);
            isActive = true;
            timeAlive = Time.TotalGameTime.TotalSeconds;
        }

        // golden nuggets only last for four seconds and we use an interval variable
        // to have it flicker when it gets to 2-4 seconds of life
          public override void Update()
          {
              interval++;
              if (!isActive && Time.TotalGameTime.TotalSeconds - timeAlive >= 4) return;
              this.Transform.Rotate(Vector3.Up, MathHelper.Pi / 2);
              // disappears after 4 seconds
              if (Time.TotalGameTime.TotalSeconds - timeAlive >= 2)
              {
                  if (interval % 15 == 0)
                  {
                      isActive = !isActive;
                  }
              }

              // when the time exceeds 4 seconds, the nugget disappears
              if (Time.TotalGameTime.TotalSeconds - timeAlive >= 4)
                  isActive = false;
              base.Update();
          }

          public override void Draw()
          {
              if (isActive) base.Draw();
          }

    }
}
