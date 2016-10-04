using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPI311.GameEngine;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;

namespace Microcosm
{
    // SEE THE TENNIS BALL OBJECT FOR SIMILAR STRUCTURE
    public class ChocolateBar : GameObject
    {
        public bool isActive { get; set; }
        public bool slowedDown { get; set; }

        public double timeAlive { get; set; }

        public double timeSlow { get; set; }
        public Vector3 prevVelocity { get; set; }

        public bool firstPhase { get; set; }
        public bool secondPhase { get; set; }

        public ChocolateBar(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light)
            : base()
        {
            Random random = new Random();
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Vector3 direction = new Vector3(
              (float)random.NextDouble(), (float)random.NextDouble(),
              (float)random.NextDouble());
            direction.Normalize();
            rigidbody.Velocity =
                direction * 6;
            Add<Rigidbody>(rigidbody);
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Transform = Transform;
            sphereCollider.Radius = 4f;
            Add<Collider>(sphereCollider);
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(
                Content.Load<Model>("bar"), Transform, camera, Content, graphicsDevice, light, 1, null, 20f, texture);
            Add<Renderer>(renderer);
            isActive = true;
            slowedDown = false;
            timeAlive = Time.TotalGameTime.TotalSeconds;
            firstPhase = false;
            secondPhase = false;
        }

        public override void Update()
        {
            if (!isActive) return;

            if (slowedDown)
            {
                if (Time.TotalGameTime.TotalSeconds - timeSlow < 5)
                {
                    if (this.Get<Rigidbody>().Velocity.X > 0)
                        this.Get<Rigidbody>().Velocity /= 2;
                    else
                        this.Get<Rigidbody>().Velocity = Vector3.Zero;
                }

                if (Time.TotalGameTime.TotalSeconds - timeSlow >= 5)
                {
                    slowedDown = false;
                    this.Get<Rigidbody>().Velocity = prevVelocity;
                }
            }

            base.Update();
        }

        public override void Draw()
        {
            if (isActive) base.Draw();
        }

        public void slowDown()
        {
            timeSlow = Time.TotalGameTime.TotalSeconds;
            prevVelocity = this.Get<Rigidbody>().Velocity;
            slowedDown = true;
        }
    }
}
