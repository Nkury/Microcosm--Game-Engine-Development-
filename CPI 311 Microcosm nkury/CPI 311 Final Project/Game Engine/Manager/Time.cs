using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace CPI311.GameEngine
{
    // The Time class defines time in the game world
    public static class Time
    {
        public static float ElapsedGameTime { get; private set; }
	    public static TimeSpan TotalGameTime { get; private set; }

        // when we start the game, we obviously begin with 0 seconds
	    public static void Initialize()
	    {
		    ElapsedGameTime = 0;
		    TotalGameTime = new TimeSpan(0);
	    }

        // on each frame, we use built-in XNA/MonoGame framework tools to
        // set the amount of game time that has passed with the overall amount of
        // game time. This will be used in the collision systems for physics kinematic
        // equations
	    public static void Update(GameTime gameTime)
	    {
		    ElapsedGameTime =
			    (float)gameTime.ElapsedGameTime.TotalSeconds;
		    TotalGameTime = gameTime.TotalGameTime;
	    }

    }
}
