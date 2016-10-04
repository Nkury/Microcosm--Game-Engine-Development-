using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    // The GameObject class is just like game objects in the Unity3D game engine.
    // ------------------------------------------------------------------------------
    // Game objects have transforms-- positions, rotations, and scale
    // Game objects have camera viewing them
    // Game objects have rigidbodies that describe its interaction with the environment
    // Game objects have colliders which define collisions with other objects in the 
    //                   environment
    // Game objects have renderers which draw the shapes with predefined textures on
    //                   the screen
    // Game objects also have components, which can be child game objects and transforms
    // Game objects also have updateables, renderables, and drawables, which allow it
    //                        to consistently move around on screen and update its position
    //                        and velocity
    public class GameObject
    {
        public Transform Transform { get; protected set; }
        public Camera Camera { get { return Get<Camera>(); } }
        public Rigidbody Rigidbody { get { return Get<Rigidbody>(); } }
        public Collider Collider { get { return Get<Collider>(); } }
        public Renderer Renderer { get { return Get<Renderer>(); } }

        private Dictionary<Type, Component> Components { get; set; }
        private List<IUpdateable> Updateables { get; set; }
        private List<IRenderable> Renderables { get; set; }
        private List<IDrawable> Drawables { get; set; }

        // constructor for instantiating all the instance variables defined before
        public GameObject()
        {
            Transform = new Transform();
            Components = new Dictionary<Type, Component>();
            Updateables = new List<IUpdateable>();
            Renderables = new List<IRenderable>();
            Drawables = new List<IDrawable>();
        }

        // When we add a game object to a scene, we give it default values for the instance variables
        public T Add<T>() where T : Component, new()
        {
            Remove<T>();
            T component = new T();
            component.GameObject = this;
            component.Transform = Transform;
            Components.Add(typeof(T), component);
            if (component is IUpdateable) Updateables.Add(component as IUpdateable);
            if (component is IRenderable) Renderables.Add(component as IRenderable);
            if (component is IDrawable) Drawables.Add(component as IDrawable);
            return component;
        }

        // when we add a game object with specific values (in T component), we set those
        // components to their respective instance variables
        public void Add<T>(T component) where T : Component
        {
            Remove<T>();
            component.GameObject = this;
            component.Transform = this.Transform;
            Components.Add(typeof(T), component);
            if (component is IUpdateable) Updateables.Add(component as IUpdateable);
            if (component is IRenderable) Renderables.Add(component as IRenderable);
            if (component is IDrawable) Drawables.Add(component as IDrawable);
        }

        // retrieves gameobjects with a certain type
        public T Get<T>() where T : Component
        {
            if (Components.ContainsKey(typeof(T))) return Components[typeof(T)] as T;
            else return null;
        }

        // removes game object from a list of game objects
        public void Remove<T>() where T : Component
        {
            if (Components.ContainsKey(typeof(T)))
            {
                Component component = Components[typeof(T)];
                Components.Remove(typeof(T));
                if (component is IUpdateable)
                    Updateables.Remove(component as IUpdateable);
                if (component is IRenderable)
                    Renderables.Remove(component as IRenderable);
                if (component is IDrawable)
                    Drawables.Remove(component as IDrawable);
            }
        }

        public virtual void Update()
        {
            foreach (IUpdateable component in Updateables) component.Update();
        }

        public virtual void Draw()
        {
            foreach (IRenderable component in Renderables) component.Draw();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (IDrawable component in Drawables) component.Draw(spriteBatch);
        }


    }
}
