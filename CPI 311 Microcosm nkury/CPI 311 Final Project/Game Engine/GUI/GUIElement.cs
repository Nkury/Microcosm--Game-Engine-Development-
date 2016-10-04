using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace CPI311.GameEngine
{
    public class GUIElement
    {
        public delegate void EventHandler(GUIElement sender);
        public event EventHandler Action;

        protected void OnAction()
        {
            if (Action != null) Action(this); // Any method is not specified yet
        }
        public Rectangle Bounds { get; set; }
        public Texture2D Texture { get; set; }
        public String Text { get; set; }
        public bool Selected { get; set; }

        public virtual void Update() { }
        public virtual void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
          if (Texture != null)
           spriteBatch.Draw(Texture, Bounds, Selected ? Color.Yellow : Color.White);
        }
    }
}
