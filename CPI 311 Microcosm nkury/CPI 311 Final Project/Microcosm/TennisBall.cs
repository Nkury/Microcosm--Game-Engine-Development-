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
    // a TennisBall is a custom game object for Microcosm. It
    // is a sphere collider with times and velocities associated with it.
    // It moves around on the screen and harms the player when it collides
    // with the camera (in this case, a box collider).
    public class TennisBall : GameObject
    {
        public bool isActive { get; set; }
        public bool slowedDown { get; set; }

        public double timeAlive { get; set; }
        
        public double timeSlow { get; set; }
        public Vector3 prevVelocity { get; set; }

        public bool firstPhase { get; set; }
        public bool secondPhase { get; set; }

        // constructor for tennis ball. The mass is set to 1, the direction is set to a random 3D vector and then normalized,
        // the velocity starts at 3, it is given a default sphere collider, a texture called "Square", and a 3D rendered
        // texture called "ball". It is set to active (meaning it can move), and it has two phases-- normal speed and fast speed.
        // Time alive will also be defined to keep track of what phase the tennis ball will be in.
          public TennisBall(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light)
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
                direction * 3;
            Add<Rigidbody>(rigidbody);
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Transform = Transform;
            sphereCollider.Radius = 3f;
            Add<Collider>(sphereCollider);
            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(
                Content.Load<Model>("ball"), Transform, camera, Content, graphicsDevice, light, 1, null, 20f, texture);
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
              
              // slowly increases the tennis balls speed over time by specifying velocities at certain time intervals
              // 15 - 30 seconds 
              if (Time.TotalGameTime.TotalSeconds - timeAlive >= 15 && Time.TotalGameTime.TotalSeconds - timeAlive < 30 && !firstPhase)
              {
                  // get whatever direction it was going before
                  this.Get<Rigidbody>().Velocity *= 1.5f;
                  firstPhase = true;
              }
                  // 30+ seconds 
              else if (Time.TotalGameTime.TotalSeconds - timeAlive >= 30 && !secondPhase)
              {
                  this.Get<Rigidbody>().Velocity *= 2;
                  secondPhase = true;
              }

              // if the tennis ball is slowed down, then the velocity is halved
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

        // draws the tennis ball on screen
          public override void Draw()
          {
              if (isActive) base.Draw();
          }


          // when the player has the slow down power-up, it causes
         // all the tennisballs to slow their velocities for five seconds
          public void slowDown(){
              timeSlow = Time.TotalGameTime.TotalSeconds;
              prevVelocity = this.Get<Rigidbody>().Velocity;
              slowedDown = true;
            
              // CODE THAT DOESN'T WORK, BUT GOOD TO LEARN FROM MISTAKES
              //Vector3 prevVelocity = this.Get<Rigidbody>().Velocity;
              //this.Get<Rigidbody>().Velocity = Vector3.Zero;
              //double time = Time.TotalGameTime.TotalSeconds;
              //while (Time.TotalGameTime.TotalSeconds - time < 5)
              //{
              //}
              //this.Get<Rigidbody>().Velocity = prevVelocity;
              //slowedDown = true;
              //while(Time.TotalGameTime.TotalSeconds - time < 5){
              //    if (this.Get<Rigidbody>().Velocity.X > 0)
              //        this.Get<Rigidbody>().Velocity /= 2;
              //    else
              //        this.Get<Rigidbody>().Velocity = Vector3.Zero;
              //}
              //slowedDown = false;
          }
    }
}
