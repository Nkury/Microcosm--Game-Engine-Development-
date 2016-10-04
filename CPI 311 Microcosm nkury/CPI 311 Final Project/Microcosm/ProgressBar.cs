using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace CPI311.GameEngine
{
    public class ProgressBar : Sprite
    {
        public float Value { get; set; }
        public float Speed { get; set; }
        public float Max { get; set; }
        public float Height { get; set; }
        public bool Direction { get; set; }
        public ProgressBar(Texture2D texture, float maxValue = 1, float height = 1, bool dec = true) : 
            base(texture)
        {
            Speed = maxValue / 20;
            Max = maxValue;
            Height = height;
            if (dec)
                Value = 0;
            else
                Value = maxValue;
            Scale = new Vector2(maxValue, height);
            Direction = dec;
        }

        public override void Update()
        {
            if (Direction)
            {
                Value += Speed * Time.ElapsedGameTime;

                if (Value >= Max)
                    Value = Max;

                Scale = new Vector2(Max - Value, Height);
            }
            else
            {
                // for automatically increasing progress bars, uncomment the following section
               // Value -= Speed * Time.ElapsedGameTime;    
                
                // uncomment the following if you want a limit to how much the bar can increase
                //if (Value < 0)
                //    Value = 0; 

                Scale = new Vector2(Max - Value, Height);
            }
        }

        public void addTime()
        {
            if (Value - (Max / Speed / 10) < 0)
                Value = 0;
            else
                Value -= (Max / Speed / 10);
        }

        public void addDistance()
        {
            Value -= 1;//(1 / (Max / Speed * .1f));
        }
    
    }
}
