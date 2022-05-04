using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace GameProject4
{
    public class Button
    {
        private Texture2D texture;
        private Vector2 position;
        private string filename;
        private Rectangle btnRec;
        private GraphicsDevice g;
        private SpriteFont font;

        private MouseState prev;
        private MouseState currentMouseState;

        /// <summary>
        /// sets button color
        /// </summary>
        public Color Color { get; set; } = Color.White;
        /// <summary>
        /// true if button clicked, false otherwise.
        /// </summary>
        public bool Clicked { get; set; } = false;

        /// <summary>
        /// Constructs a new exit button in the correct position
        /// </summary>
        /// <param name="graphics"></param>
        public Button(GraphicsDevice graphics, int count, string fn)
        {
            g = graphics;
            filename = fn;
            if (filename == "None")
            {
                position = new Vector2(graphics.Viewport.Width - 105, graphics.Viewport.Height - 45);
                btnRec = new Rectangle((int)position.X, (int)position.Y, 90, 30);
            }
            else
            {
                if (count > 0)
                {
                    position = new Vector2(graphics.Viewport.Width / 2 - 64, (graphics.Viewport.Height / 2 - 82) + (count * 64) + 6);
                }
                else
                {
                    position = new Vector2(graphics.Viewport.Width / 2 - 64, graphics.Viewport.Height / 2 - 82);
                }
            }
        }

        /// <summary>
        /// Loads sprite for exit button
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent(ContentManager content, SpriteFont f)
        {
            font = f;
            if(filename != "None")
            {
                texture = content.Load<Texture2D>(filename);
            }
            else
            {
                texture = new Texture2D(g, 1, 1);
                texture.SetData(new Color[] { Color.White });
            }
        }

        /// <summary>
        /// Checks if mouse is over the button and if mouse is clicked on the button
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            Color = Color.White;
            currentMouseState = Mouse.GetState();

            if(filename != "None"){
                if (currentMouseState.Position.X < (int)position.X + 128 && currentMouseState.Position.X > (int)position.X && currentMouseState.Position.Y > (int)position.Y && currentMouseState.Position.Y < (int)position.Y + 64)
                {
                    Color = Color.DarkGray;
                    if (currentMouseState.LeftButton == ButtonState.Pressed)
                    {
                        Clicked = true;
                    }
                }
            }
            else
            {
                if (currentMouseState.Position.X < (int)position.X + 90 && currentMouseState.Position.X > (int)position.X && currentMouseState.Position.Y > (int)position.Y && currentMouseState.Position.Y < (int)position.Y + 30)
                {
                    Color = Color.DarkGray;
                    if (currentMouseState.LeftButton == ButtonState.Pressed && prev.LeftButton != ButtonState.Pressed)
                    {
                        Clicked = true;
                    }
                }
            }
            prev = currentMouseState;
        }

        /// <summary>
        /// draws the button on the screen
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(filename != "None")
            {
                spriteBatch.Draw(texture, position, Color * 0.8f);
            }
            else
            {
                spriteBatch.Draw(texture, btnRec, Color * 0.45f);
                spriteBatch.DrawString(font, "CONTROLS", new Vector2(btnRec.X + 3, btnRec.Y + 7), Color.Black, 0, new Vector2(0,0), .23f, SpriteEffects.None, 0);
            }
        }
    }
}
