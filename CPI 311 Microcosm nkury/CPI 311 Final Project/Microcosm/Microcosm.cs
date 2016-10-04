using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

using System;
using System.Collections.Generic;

using CPI311.GameEngine;

namespace Microcosm
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Microcosm : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera camera;
        Light light;
        SpriteFont font;
        Texture2D warningIcon;
        Sprite warn;
        Texture2D titleScreenTexture;
        Sprite titleScreen;
        Texture2D buttonTexture;

        SoundEffect music;
        SoundEffectInstance musicInstance;
        SoundEffect titleMusic;
        SoundEffectInstance titleMusicInstance;
        SoundEffect gameOverMusic;
        SoundEffectInstance gameOverInstance;
        SoundEffect freezeSound;
        SoundEffectInstance freezeInstance;
        SoundEffect powerSound;
        SoundEffectInstance powerInstance;
        SoundEffect warningSound;
        SoundEffectInstance warningInstance;
        SoundEffect invincSound;
        SoundEffectInstance invincInstance;

        List<TennisBall> tennisBalls;
        List<ChocolateBar> chocolateBars;
        List<GoldenPellet> goldenPellets;
        GameObject boxCollider;

        SphereCollider player;

        int numPellets = 0;
        int totalPellets = 0;
        int interval = 0; // for flickering the "Warning" text
        int timeAlive = 0;
        int lastTime = 0;

        Vector3 lastCameraPosition = new Vector3();
        Vector3 lastPlayerPosition = new Vector3();

        ProgressBar pelletsCollected;

        bool spawnPellet = true;
        bool spawnObj = false;
        bool warning = false;
        bool playOnce = false; // for playing the warning sound
        bool start = false;

        // booleans for power-ups
        bool slowDown = false;
        bool invinc = true;
        bool invincibility = false;
        double InvincTime;  // for tracking the five seconds of invincibility

        bool destroy = false;

        class Scene
        {
            public delegate void CallMethod();
            public CallMethod Update;
            public CallMethod Draw;
            public Scene(CallMethod update, CallMethod draw)
            {
                Update = update;
                Draw = draw;
            }
        }

        Dictionary<String, Scene> scenes;
        Scene currentScene;
        List<GUIElement> guiElements;
        List<GUIElement> titleElements;

        List<int> highScores = new List<int>();

        public Microcosm()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            guiElements = new List<GUIElement>();
            titleElements = new List<GUIElement>();
            scenes = new Dictionary<String, Scene>();
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(graphics);

            camera = new Camera();
            Transform cameraT = new Transform();
            cameraT.Position += Vector3.Backward * 10; // this is up to you where you want to go
            camera.Transform = cameraT;

            tennisBalls = new List<TennisBall>();
            chocolateBars = new List<ChocolateBar>();
            goldenPellets = new List<GoldenPellet>();

            player = new SphereCollider();

            highScores.Add(0);
            highScores.Add(0);
            highScores.Add(0);
      
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("font");
            buttonTexture = Content.Load<Texture2D>("buttonTexture");
            warningIcon = Content.Load<Texture2D>("warning-icon-hi");
            warn = new Sprite(warningIcon);
            warn.Position = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            warn.Scale = new Vector2(.2f, .2f);
            titleScreenTexture = Content.Load<Texture2D>("title screen");
            titleScreen = new Sprite(titleScreenTexture);
            titleScreen.Position = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            titleScreen.Scale = new Vector2(1, 1);


            freezeSound = Content.Load<SoundEffect>("FreezePower");
            music = Content.Load<SoundEffect>("level1 music");
            powerSound = Content.Load<SoundEffect>("Power Up");
            warningSound = Content.Load<SoundEffect>("nuke");
            invincSound = Content.Load<SoundEffect>("coin_rush");
            titleMusic = Content.Load<SoundEffect>("title screen 1");
            gameOverMusic = Content.Load<SoundEffect>("game over music");

            musicInstance = music.CreateInstance();
            musicInstance.IsLooped = true;

            gameOverInstance = gameOverMusic.CreateInstance();
            gameOverInstance.IsLooped = true;

            titleMusicInstance = titleMusic.CreateInstance();
            titleMusicInstance.IsLooped = true;
            titleMusicInstance.Play();           

            light = new Light();
            Transform lightTransform = new Transform();
            lightTransform.LocalPosition = Vector3.Backward * 2 + Vector3.Right * 5;
            light.Transform = lightTransform;

            boxCollider = new GameObject();
            BoxCollider collider = new BoxCollider();
            collider.Size = 17;
            pelletsCollected = new ProgressBar(Content.Load<Texture2D>("lazer_yellow"), 100,4, false);
            Texture2D texture = Content.Load<Texture2D>("cardboard-texture");
            Renderer renderer = new Renderer(
                Content.Load<Model>("arena"), boxCollider.Transform, camera, Content, GraphicsDevice, light, 1, null, 20f, texture);
            boxCollider.Add<Renderer>(renderer);
            boxCollider.Add<Collider>(collider);

            player.Radius = 1f;
            player.Transform = camera.Transform;

            tennisBalls.Add(new TennisBall(Content, camera, GraphicsDevice, light));

            GUIGroup group = new GUIGroup();
            GUIGroup titleGroup = new GUIGroup();

            Button exitButton = new Button();
            exitButton.Texture = buttonTexture;
            exitButton.Bounds = new Rectangle(GraphicsDevice.Viewport.Width/4, GraphicsDevice.Viewport.Height/2, 300, 50);
            exitButton.Action += RestartGame; 
            exitButton.Text = "\n                             Play Again";
            group.Children.Add(exitButton);

            guiElements.Add(group);

            Button playButton = new Button();
            playButton.Texture = buttonTexture;
            playButton.Bounds = new Rectangle(GraphicsDevice.Viewport.Width / 4 + 20, GraphicsDevice.Viewport.Height / 3, 300, 50);
            playButton.Action += RestartGame;
            playButton.Text = "\n                             Play Game";
            titleGroup.Children.Add(playButton);

            Button InstructionButton = new Button();
            InstructionButton.Texture = buttonTexture;
            InstructionButton.Bounds = new Rectangle(GraphicsDevice.Viewport.Width / 4 + 20, GraphicsDevice.Viewport.Height / 3 + 65, 300, 50);
            InstructionButton.Action += goToInstructions;
            InstructionButton.Text = "\n                             Instructions";
            titleGroup.Children.Add(InstructionButton);

            Button creditsButton = new Button();
            creditsButton.Texture = buttonTexture;
            creditsButton.Bounds = new Rectangle(GraphicsDevice.Viewport.Width / 4 + 20, GraphicsDevice.Viewport.Height / 3 + 130, 300, 50);
            creditsButton.Action += goToCredits;
            creditsButton.Text = "\n                             Credits";
            titleGroup.Children.Add(creditsButton);

            Button quitButton = new Button();
            quitButton.Texture = buttonTexture;
            quitButton.Bounds = new Rectangle(GraphicsDevice.Viewport.Width / 4 + 20, GraphicsDevice.Viewport.Height / 3 + 195, 300, 50);
            quitButton.Action += Quit;
            quitButton.Text = "\n                             Quit Game";
            titleGroup.Children.Add(quitButton);

            titleElements.Add(titleGroup);

            scenes.Add("Score Scene", new Scene(ScoreUpdate, ScoreDraw));
            scenes.Add("Play", new Scene(PlayUpdate, PlayDraw));
            scenes.Add("Title Screen", new Scene(TitleUpdate, TitleDraw));
            scenes.Add("Instructions", new Scene(InstructionUpdate, InstructionDraw));
            scenes.Add("Credits", new Scene(CreditsUpdate, CreditsDraw));
            currentScene = scenes["Title Screen"];
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Time.Update(gameTime);
            InputManager.Update();
            currentScene.Update();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            GraphicsDevice.BlendState = BlendState.Opaque; // to allow for 2D text to appear alongside 3D objects

            currentScene.Draw();
            base.Draw(gameTime);
        }


        // all the scene stuff

        // event handler for the restart game button
        void RestartGame(GUIElement element)
        {
            currentScene = scenes["Play"];
            tennisBalls.Clear();
            chocolateBars.Clear();
            tennisBalls.Add(new TennisBall(Content, camera, GraphicsDevice, light));
            numPellets = 0;
            totalPellets = 0;
            pelletsCollected.Value = 100; // reset progress bar
            slowDown = false;
            invinc = false;
            invincibility = false;
            destroy = false;
            Transform cameraT = new Transform();
            cameraT.Position += Vector3.Backward * 10; // this is up to you where you want to go
            camera.Transform = cameraT;
            player.Transform = camera.Transform;
            start = true;
            musicInstance.Play();
            titleMusicInstance.Stop();
            gameOverInstance.Stop();
        }

        void goToInstructions(GUIElement element)
        {
            currentScene = scenes["Instructions"];
        }

        void goToCredits(GUIElement element)
        {
            currentScene = scenes["Credits"];
        }

        void Quit(GUIElement element) {
            Exit();
        }

        void ScoreUpdate()
        {
            foreach (GUIElement element in guiElements)
                element.Update();
        }

        void ScoreDraw()
        {
            int score = 100 * (timeAlive - lastTime) + 30 * totalPellets;
            spriteBatch.Begin();
            titleScreen.Draw(spriteBatch);
            foreach (GUIElement element in guiElements)
                element.Draw(spriteBatch, font);
            spriteBatch.DrawString(font, "SCORE = 100 * " + (timeAlive - lastTime) + "s + 30 * " + totalPellets + " Nuggets = " + score,
                new Vector2(GraphicsDevice.Viewport.Width/4, GraphicsDevice.Viewport.Height/3), Color.Black, 0, Vector2.Zero, 1.3f, SpriteEffects.None,
                0);
            spriteBatch.DrawString(font, "\n\nHIGH SCORES:", new Vector2(GraphicsDevice.Viewport.Width / 12, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n\n1. " + highScores[0], new Vector2(GraphicsDevice.Viewport.Width / 12, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n\n\n2. " + highScores[1], new Vector2(GraphicsDevice.Viewport.Width / 12, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n\n\n\n3. " + highScores[2], new Vector2(GraphicsDevice.Viewport.Width / 12, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.End();

        }

        void TitleUpdate()
        {
            foreach (GUIElement element in titleElements)
                element.Update();
        }

        void TitleDraw()
        {
            spriteBatch.Begin();
            titleScreen.Draw(spriteBatch);
            foreach (GUIElement element in titleElements)
                element.Draw(spriteBatch, font);
            spriteBatch.End();
        }

        void PlayUpdate(){    
            
            pelletsCollected.Update();        
            timeAlive = (int)Time.TotalGameTime.TotalSeconds; // for the score scene
            if (start)
            {
                lastTime = timeAlive;
                start = false;
            }
            int seconds = timeAlive - lastTime;
            interval++; // for flickering the "Warning" text

            // responsible for spawning golden pellets every 2 seconds
            if ((int)Time.TotalGameTime.TotalSeconds % 2 == 0)
            {
                if(spawnPellet){
                    goldenPellets.Add(new GoldenPellet(Content, camera, GraphicsDevice, light));
                    spawnPellet = false;
                }
            }
            else
                spawnPellet = true;

            // every other minute, a tennis ball spawns. Every two minutes, a chocolate bar spawns.
            if (seconds % 60 >= 57)
            {
                if (playOnce)
                {
                    warningInstance = warningSound.CreateInstance();
                    warningInstance.IsLooped = false;
                    warningInstance.Play();
                    playOnce = false;
                }
                if (interval % 16 < 8)
                    warning = true;
                else
                    warning = false;
            }
            else
            {
                warning = false;
                playOnce = true;
            }

            if (seconds % 60 == 0 && seconds != 0)
            {     
                if (spawnObj)
                 {
                    if (seconds % 120 == 0)
                        chocolateBars.Add(new ChocolateBar(Content, camera, GraphicsDevice, light));
                    else
                        tennisBalls.Add(new TennisBall(Content, camera, GraphicsDevice, light));
                    spawnObj = false;
                 }
            }
            else
            {
                spawnObj = true;
            }
            if (invincibility && Time.TotalGameTime.TotalSeconds - InvincTime > 15)
            {
                invincibility = false;
                musicInstance.Resume();
            }
            // update all the game objects
            for (int i = 0; i < goldenPellets.Count; i++)
                goldenPellets[i].Update();
            for (int i = 0; i < tennisBalls.Count; i++)
                tennisBalls[i].Update();
            for (int i = 0; i < chocolateBars.Count; i++)
                chocolateBars[i].Update();

            // responsible for moderating the power-ups
            if (numPellets >= 25)
            {
                destroy = true;
                slowDown = false;
                invinc = false;
            }
            else if (numPellets >= 15) {
                destroy = false;
                slowDown = false;
                invinc = true;
            }
            else if (numPellets >= 10)
            {
                destroy = false;
                slowDown = true;
                invinc = false;
            }

            if (InputManager.IsKeyPressed(Keys.E))
            {
                if (slowDown)
                {
                    freezeInstance = freezeSound.CreateInstance();
                    freezeInstance.IsLooped = false;
                    freezeInstance.Play();
                    slowDown = false;
                    for (int i = 0; i < tennisBalls.Count; i++)
                        tennisBalls[i].slowDown();
                    for (int i = 0; i < chocolateBars.Count; i++)
                        chocolateBars[i].slowDown();
                    numPellets -= 10;
                    pelletsCollected.Value += 10;                    
                }
                else if (invinc)
                {
                    freezeInstance = freezeSound.CreateInstance();
                    freezeInstance.IsLooped = false;
                    freezeInstance.Play();
                    invincInstance = invincSound.CreateInstance();
                    invincInstance.IsLooped = false;
                    invincInstance.Play();
                    invinc = false;
                    invincibility = true;
                    InvincTime = Time.TotalGameTime.TotalSeconds;
                    musicInstance.Pause();
                    numPellets -= 15;
                    pelletsCollected.Value += 15;  
                }
                else if (destroy)
                {
                    freezeInstance = freezeSound.CreateInstance();
                    freezeInstance.IsLooped = false;
                    freezeInstance.Play();
                    destroy = false;
                    if (chocolateBars.Count > 0)
                    {
                        chocolateBars.RemoveAt(0);
                    }
                    else
                    {
                        if (tennisBalls.Count > 0)
                            tennisBalls.RemoveAt(0);
                    }
                    numPellets -= 25;
                    pelletsCollected.Value += 25;
                }
            }

            // movement controls that check whether or not you are still within the box
            if (InputManager.IsKeyDown(Keys.W) || (Mouse.GetState().LeftButton == ButtonState.Pressed) ||
                InputManager.IsKeyDown(Keys.S) || InputManager.IsKeyDown(Keys.A) || InputManager.IsKeyDown(Keys.D) ||
                InputManager.IsKeyDown(Keys.Space) || InputManager.IsKeyDown(Keys.LeftShift))
            {
                lastCameraPosition = camera.Transform.Position;
                lastPlayerPosition = player.Transform.Position;
                if (InputManager.IsKeyDown(Keys.W) || (Mouse.GetState().LeftButton == ButtonState.Pressed))
                {
                    camera.Transform.Position += camera.Transform.Forward * Time.ElapsedGameTime * 5;
                    player.Transform.Position += camera.Transform.Forward * Time.ElapsedGameTime * 5;
                }
                if (InputManager.IsKeyDown(Keys.S))
                {
                    camera.Transform.Position += camera.Transform.Backward * Time.ElapsedGameTime * 5;
                    player.Transform.Position += camera.Transform.Backward * Time.ElapsedGameTime * 5;
                }
                if (InputManager.IsKeyDown(Keys.D))
                {
                    camera.Transform.Position += camera.Transform.Right * Time.ElapsedGameTime * 5;
                    player.Transform.Position += camera.Transform.Right * Time.ElapsedGameTime * 5;
                }
                if (InputManager.IsKeyDown(Keys.A))
                {
                    camera.Transform.Position += camera.Transform.Left * Time.ElapsedGameTime * 5;
                    player.Transform.Position += camera.Transform.Left * Time.ElapsedGameTime * 5;
                }

                if (InputManager.IsKeyDown(Keys.LeftShift))
                {
                    camera.Transform.Position += camera.Transform.Down * Time.ElapsedGameTime * 5;
                    player.Transform.Position += camera.Transform.Down * Time.ElapsedGameTime * 5;
                }
                if (InputManager.IsKeyDown(Keys.Space))
                {
                    camera.Transform.Position += camera.Transform.Up * Time.ElapsedGameTime * 5;
                    player.Transform.Position += camera.Transform.Up * Time.ElapsedGameTime * 5;
                }
            }

            // MOUSE CONTROLS
            if (InputManager.IsKeyDown(Keys.Left) || Mouse.GetState().X < GraphicsDevice.Viewport.Width / 4)
                camera.Transform.Rotate(Vector3.Up, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Right) || Mouse.GetState().X > GraphicsDevice.Viewport.Width - GraphicsDevice.Viewport.Width / 4)
                camera.Transform.Rotate(Vector3.Up, -Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Up) || Mouse.GetState().Y < GraphicsDevice.Viewport.Height / 4)
                camera.Transform.Rotate(Vector3.Right, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Down) || Mouse.GetState().Y > GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Height / 4)
                camera.Transform.Rotate(Vector3.Right, -Time.ElapsedGameTime); 

            Vector3 normal; // it is updated if a collision happens
            
            // collisions for tennis balls
            for (int i = 0; i < tennisBalls.Count; i++)
            {
                Rigidbody rigidbody = tennisBalls[i].Get<Rigidbody>();
                Collider collider = tennisBalls[i].Get<Collider>();

                if (boxCollider.Get<Collider>().Collides(collider, out normal))
                {
                    if (Vector3.Dot(normal, rigidbody.Velocity) < 0)
                        rigidbody.Impulse +=
                           Vector3.Dot(normal, rigidbody.Velocity) * -2 * normal;
                }

                if (player.Collides(collider, out normal) && !invincibility)
                {
                    musicInstance.Stop();
                    gameOverInstance.Play();
                    int score = 100 * seconds + 30 * totalPellets;
                    if (highScores.Count > 2)
                    {
                        if (highScores[0] < score)
                        {
                            highScores.Insert(0, score);
                        }
                        else if (highScores[1] < score)
                        {
                            highScores.Insert(1, score);
                        }
                        else if (highScores[2] < score)
                        {
                            highScores.Insert(2, score);
                        }
                    }
                    else if (highScores.Count > 1)
                    {
                        if (highScores[0] < score)
                        {
                            highScores.Insert(0, score);
                        }
                        else if (highScores[1] < score)
                        {
                            highScores.Insert(1, score);
                        }
                        else
                        {
                            highScores.Add(score);
                        }
                    }
                    else if (highScores.Count > 0)
                    {
                        if (highScores[0] < score)
                        {
                            highScores.Insert(0, score);
                        }
                        else
                        {
                            highScores.Add(score);
                        }
                    }
                    else
                    {
                        highScores.Add(score);
                    }
                    currentScene = scenes["Score Scene"];
                }
              
            }

            // collisions for chocolate bars
            for (int i = 0; i < chocolateBars.Count; i++)
            {
                Rigidbody rigidbody = chocolateBars[i].Get<Rigidbody>();
                Collider collider = chocolateBars[i].Get<Collider>();

                if (boxCollider.Get<Collider>().Collides(collider, out normal))
                {
                    if (Vector3.Dot(normal, rigidbody.Velocity) < 0)
                        rigidbody.Impulse +=
                           Vector3.Dot(normal, rigidbody.Velocity) * -2 * normal;
                }

                if (player.Collides(collider, out normal) && !invincibility)
                {
                    musicInstance.Stop();
                    gameOverInstance.Play();
                    int score = 100 * seconds + 30 * totalPellets;
                    if (highScores.Count > 2)
                    {
                        if (highScores[0] < score)
                        {
                            highScores.Insert(0, score);
                        }
                        else if (highScores[1] < score)
                        {
                            highScores.Insert(1, score);
                        }
                        else if (highScores[2] < score)
                        {
                            highScores.Insert(2, score);
                        }
                    }
                    else if (highScores.Count > 1)
                    {
                        if (highScores[0] < score)
                        {
                            highScores.Insert(0, score);
                        }
                        else if (highScores[1] < score)
                        {
                            highScores.Insert(1, score);
                        }
                        else
                        {
                            highScores.Add(score);
                        }
                    }
                    else if (highScores.Count > 0)
                    {
                        if (highScores[0] < score)
                        {
                            highScores.Insert(0, score);
                        }
                        else
                        {
                            highScores.Add(score);
                        }
                    }
                    else
                    {
                        highScores.Add(score);
                    }
                    
                    
                    currentScene = scenes["Score Scene"];
                }               
            }

            // collisions for golden pellets
            for (int i = 0; i < goldenPellets.Count; i++)
            {
                if (goldenPellets[i].isActive)
                {
                    Rigidbody rigidbody = goldenPellets[i].Get<Rigidbody>();
                    Collider collider = goldenPellets[i].Get<Collider>();

                    if (player.Collides(collider, out normal))
                    {
                        powerInstance = powerSound.CreateInstance();
                        powerInstance.IsLooped = false;
                        powerInstance.Play();
                        goldenPellets.RemoveAt(i);
                        numPellets++;
                        totalPellets++;
                        pelletsCollected.addDistance();
                    }
                }
            }

            // if player collides with box collider
            if(player.Collides(boxCollider.Get<Collider>(), out normal)){
                camera.Transform.Position = lastCameraPosition;
                player.Transform.Position = lastPlayerPosition;
            }
        }

        void PlayDraw(){

            // draw the golden pellets
            for (int i = 0; i < goldenPellets.Count; i++)
                goldenPellets[i].Draw();

            // draw the tennis balls
            for (int i = 0; i < tennisBalls.Count; i++)
                tennisBalls[i].Draw();

            // draw the chocolate bars
            for (int i = 0; i < chocolateBars.Count; i++)
                chocolateBars[i].Draw();

            boxCollider.Draw();

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Current Nuggets: " + numPellets, new Vector2(5, 13), Color.Gold);
            spriteBatch.DrawString(font, "" + (timeAlive - lastTime) + "s", new Vector2(GraphicsDevice.Viewport.Width / 1.3f,
                GraphicsDevice.Viewport.Height / 1.2f), Color.White, 0, Vector2.Zero, 1.15f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "\nTotal Nuggets: " + totalPellets, new Vector2(GraphicsDevice.Viewport.Width / 1.3f,
                GraphicsDevice.Viewport.Height / 1.2f), Color.Gold, 0, Vector2.Zero, 1.15f, SpriteEffects.None, 0);
            if (warning)
            {
                 warn.Draw(spriteBatch);
            }
            if (slowDown)
            {
                spriteBatch.DrawString(font, "Press \"E\" to slow down time!",
                  new Vector2(10, GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Height / 12), Color.White);
            }
            else if (invinc)
            {
                spriteBatch.DrawString(font, "Press \"E\" to be invincible!",
                  new Vector2(10, GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Height / 12), Color.White);
            }
            else if (destroy)
            {
                spriteBatch.DrawString(font, "Press \"E\" to destroy a random object!",
                  new Vector2(10, GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Height / 12), Color.White);
            }
            else
            {
                spriteBatch.DrawString(font, "Collect golden nuggets to gain a power-up!",
                new Vector2(10, GraphicsDevice.Viewport.Height - GraphicsDevice.Viewport.Height / 12), Color.White);
            }
            pelletsCollected.Draw(spriteBatch);
            spriteBatch.End();
        }

        void InstructionUpdate()
        {
            if (InputManager.IsKeyPressed(Keys.Space))
            {
                currentScene = scenes["Title Screen"];
            }
        }

        void InstructionDraw()
        {
            spriteBatch.Begin();
            titleScreen.Draw(spriteBatch);
            spriteBatch.DrawString(font, "CONTROLS", new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n    -WASD to move up, left, down, right, respectively ", new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n    -E to use power-up", new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n\n    -Shift/SPACE to descend and ascend, respectively", new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n\n\n    -Mouse to move camera", new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n\n\n\n\nHOW TO PLAY", new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n\n\n\n\n\n    -Survive as long as possible while avoiding the flying objects"
                , new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n\n\n\n\n\n\n\n\nPress SPACE to go back to the title screen!"
               , new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.End();
        }

        void CreditsUpdate()
        {
            if (InputManager.IsKeyPressed(Keys.Space))
            {
                currentScene = scenes["Title Screen"];
            }
        }

        void CreditsDraw()
        {
            spriteBatch.Begin();
            titleScreen.Draw(spriteBatch);
            spriteBatch.DrawString(font, "DEVELOPER and MODELER", new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n    Nizar Kury ", new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n\nMUSIC", new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n\n\n    Created by Nizar Kury using buttonbass.com", new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n\n\n\n\nRESOURCES", new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n\n\n\n\n\n    Power-up sound effects by FX Sounds"
                , new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n\n\n\n\n\n\n    Cardboard Texture by Public Domain Photos"
                , new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n\n\n\n\n\n\n\n    Chocolate Bar Texture by Wallpapers Craft"
               , new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n\n\n\n\n\n\n\n\n    Tennis Ball Texture by RobinWood.com"
           , new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n\n\n\n\n\n\n\n\n\n    Golden Pellet Texture by Deviant Art"
       , new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.DrawString(font, "\n\n\n\n\n\n\n\n\n\n\n\n\nPress SPACE to go back to the title screen!"
               , new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 3), Color.Black);
            spriteBatch.End();
        }
        
    }
}
