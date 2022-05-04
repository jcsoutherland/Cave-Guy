using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace GameProject4
{
    /// <summary>
    /// A class representing the games menu screen
    /// </summary>
    public class MenuScreen
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Button exitBtn;
        private Button playBtn;
        private Button controlsBtn;
        private SpriteFont spriteFont;
        private Texture2D background;
        private Texture2D fadeToBlack;
        private Rectangle fadeRec;
        private float alpha = 0f;
        private Texture2D title;
        //private Texture2D player;
        Player player;

        private Song gameMusic;
        Matrix transform;
        float xTranslate;

        public bool animation;
        public bool exit;
        public bool play;
        public bool controlsOpen;

        /// <summary>
        /// Constructor for the menu screen
        /// </summary>
        /// <param name="g"></param>
        public MenuScreen(GraphicsDeviceManager g)
        {
            _graphics = g;
            xTranslate = 0;
            playBtn = new Button(_graphics.GraphicsDevice, 0, "PlayBtn");
            exitBtn = new Button(_graphics.GraphicsDevice, 1, "ExitButton");
            controlsBtn = new Button(_graphics.GraphicsDevice, 2, "None");
            player = new Player(_graphics, new Vector2(250, _graphics.GraphicsDevice.Viewport.Height - 144), null);
            fadeRec = new Rectangle(0,0, _graphics.GraphicsDevice.Viewport.Width, _graphics.GraphicsDevice.Viewport.Height);
        }

        /// <summary>
        /// Loads content from ContentManager
        /// </summary>
        /// <param name="Content"></param>
        public void LoadContent(ContentManager Content)
        {
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
            fadeToBlack = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            fadeToBlack.SetData(new Color[] { Color.Black });
            spriteFont = Content.Load<SpriteFont>("arial");
            playBtn.LoadContent(Content, spriteFont);
            exitBtn.LoadContent(Content, spriteFont);
            controlsBtn.LoadContent(Content, spriteFont);
            background = Content.Load<Texture2D>("GamePlayBg");
            //player = Content.Load<Texture2D>("adventurer");
            player.LoadContent(Content);
            title = Content.Load<Texture2D>("TitleArt");
            gameMusic = Content.Load<Song>("bg_music");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(gameMusic);
            MediaPlayer.Volume = .015f;
        }
        /// <summary>
        /// Game Loop updating screen every tick
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (!controlsOpen)
            {
                playBtn.Update(gameTime);
                exitBtn.Update(gameTime);
            }
            controlsBtn.Update(gameTime);
            if (exitBtn.Clicked)
            {
                exit = true;
            }
            if (playBtn.Clicked)
            {
                //Console.WriteLine("Hi");
                animation = true;
                playBtn.Clicked = false;
            }
            if (controlsBtn.Clicked)
            {
                if (!controlsOpen)
                {
                    controlsOpen = true;
                }
                else
                {
                    controlsOpen = false;
                }
                controlsBtn.Clicked = false;
            }
            if (animation)
            {
                player.running = true;
                xTranslate -= 5;
                if (xTranslate <= -800)
                {
                    animation = false;
                    player.running = false;
                    player.idle = true;
                    play = true;
                    //MediaPlayer.Stop();
                }
            }
        }
        /// <summary>
        /// Displays all assets of the screen
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.Black);

            if (controlsOpen)
            {
                _spriteBatch.Begin();
                controlsBtn.Draw(gameTime, _spriteBatch);
                _spriteBatch.DrawString(spriteFont, "Controls:\n - Movement: [Arrow keys] or [WASD]\n - Jump: [Space]\n - Attack: [Ctrl]\n - Interact: [E]\n - Ladder Movement: [Up/Down] or [W/S]\n - Drop Down: [Down + Space] or [S + Space]\n - Exit Game: [ESC]\n\n Note: you can't go down ladders from above", new Vector2(30, 30), Color.LightGray, 0, new Vector2(0, 0), .5f, SpriteEffects.None, 0);
                _spriteBatch.End();
            }
            else
            {
                transform = Matrix.CreateTranslation(xTranslate, 0, 0);
                _spriteBatch.Begin(transformMatrix: transform);
                _spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
                _spriteBatch.Draw(title, new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2 - 170, 80), Color.LightGray);
                playBtn.Draw(gameTime, _spriteBatch);
                exitBtn.Draw(gameTime, _spriteBatch);
                controlsBtn.Draw(gameTime, _spriteBatch);
                _spriteBatch.End();

                _spriteBatch.Begin();
                player.Draw(gameTime, _spriteBatch);
                if (animation && alpha < 1)
                {
                    alpha += .006f;
                    _spriteBatch.Draw(fadeToBlack, fadeRec, Color.Black * alpha);
                }
                _spriteBatch.End();
            }      
        }
    }
}
