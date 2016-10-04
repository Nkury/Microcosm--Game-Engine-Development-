using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CPI311.GameEngine
{
    // the Screen Manager class deals with the multiple levels in a game
    public static class ScreenManager
    {
	    private static GraphicsDeviceManager graphics;

	    public static GraphicsDevice GraphicsDevice
	    {
		    get { return graphics.GraphicsDevice; }
	    }

	    public static void Initialize(GraphicsDeviceManager g)
	    {
		    ScreenManager.graphics = g;
	    }

        public static int Width
        {
            get { return GraphicsDevice.PresentationParameters.BackBufferWidth; }
            set
            {
                graphics.PreferredBackBufferWidth = value;
                graphics.ApplyChanges();
            }
        }

        public static int Height
        {
            get { return GraphicsDevice.PresentationParameters.BackBufferHeight; }
            set
            {
                graphics.PreferredBackBufferHeight = value;
                graphics.ApplyChanges();
            }
        }

        // can set the game to full screen using built-in MonoGame/XNA features
        public static bool IsFullScreen{
            get{return graphics.IsFullScreen;}
            set
            {
                graphics.IsFullScreen = value;
                graphics.ApplyChanges();
            }
        }

        //  has a default viewport variable that returns a viewport
        // with the current width and height of the screen
	    public static Viewport DefaultViewport
	    {
		    get { return new Viewport(0, 0, Width, Height); }
	    }

	    public static void Setup(int width = 0, int height = 0)
	    {
		    Setup(IsFullScreen, width, height);
	    }

	    public static void Setup(bool fullScreen,
			    int width = 0, int height = 0)
	    {
		    if(width > 0)
			    graphics.PreferredBackBufferWidth = width;
		    if(height > 0)
			    graphics.PreferredBackBufferHeight= height;
		    graphics.IsFullScreen = fullScreen;
		    graphics.ApplyChanges();
	    }
}

}
