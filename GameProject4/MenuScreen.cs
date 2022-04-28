using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

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
        public bool exit;
        public bool play;

        /// <summary>
        /// Constructor for the menu screen
        /// </summary>
        /// <param name="g"></param>
        public MenuScreen(GraphicsDeviceManager g)
        {
            _graphics = g;
            playBtn = new Button(_graphics.GraphicsDevice, 0, "PlayBtn");
            exitBtn = new Button(_graphics.GraphicsDevice, 1, "ExitButton");
        }

        /// <summary>
        /// Loads content from ContentManager
        /// </summary>
        /// <param name="Content"></param>
        public void LoadContent(ContentManager Content)
        {
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
            playBtn.LoadContent(Content);
            exitBtn.LoadContent(Content);
            spriteFont = Content.Load<SpriteFont>("arial");
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
                play = true;
            }
        }
        /// <summary>
        /// Displays all assets of the screen
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.MediumPurple);
            _spriteBatch.Begin();
            playBtn.Draw(gameTime, _spriteBatch);
            exitBtn.Draw(gameTime, _spriteBatch);
            _spriteBatch.DrawString(spriteFont, "Game Project 1", new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2 - 184, 80), Color.White);
            _spriteBatch.End();
        }
    }
}
