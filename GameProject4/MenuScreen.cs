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
using MessageBox = System.Windows.Forms.MessageBox;

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
            playBtn.LoadContent(Content);
            exitBtn.LoadContent(Content);
            spriteFont = Content.Load<SpriteFont>("arial");
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
            playBtn.Update(gameTime);
            exitBtn.Update(gameTime);
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
            _graphics.GraphicsDevice.Clear(Color.MediumPurple);

            transform = Matrix.CreateTranslation(xTranslate, 0, 0);
            _spriteBatch.Begin(transformMatrix: transform);
            _spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            _spriteBatch.Draw(title, new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2 - 170, 80), Color.LightGray);
            playBtn.Draw(gameTime, _spriteBatch);
            exitBtn.Draw(gameTime, _spriteBatch);
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
