using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
namespace CPI311.GameEngine
{
    // Just like how a Rigidbody acts in Unity, a Rigidbody
    // in this game engine has a velocity, a mass, an acceleration,
    // and an impulse (or force upon the object) used for different
    // physical interactions with the environment
    public class Rigidbody : Component, IUpdateable
    {
        public Vector3 Velocity { get; set; }
        public float Mass { get; set; }
        public Vector3 Acceleration { get; set; }
        public Vector3 Impulse { get; set; }

        // On every frame, employ the basic kinematic properties on the game object
        // velocity = velocity + acceleration * time + impulse / mass
        // position = velocity * time
        // impulse = 0
        public void Update()
        {
            Velocity += Acceleration * Time.ElapsedGameTime + Impulse / Mass;
            Transform.LocalPosition += Velocity * Time.ElapsedGameTime;
            Impulse = Vector3.Zero;
        }

    }
}
