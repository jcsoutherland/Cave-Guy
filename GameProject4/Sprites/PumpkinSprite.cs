using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameProject4
{
    public class PumpkinSprite
    {
        private float scale;

        private KeyboardState keyboardState;

        private KeyboardState oldState;

        private float defaultY;

        private float defaultX;

        bool Jumped = false;

        private Vector2 MovementVector;

        private Texture2D texture;

        private Vector2 position;

        private BoundingRectangle bounds;

        public BoundingRectangle Bounds => bounds;

        /// <summary>
        /// constructor for pumpkin
        /// </summary>
        /// <param name="position">position where sprite is drawn in game</param>
        public PumpkinSprite(Vector2 p)
        {
            //367x410
            MovementVector = new Vector2(0, 0);
            position = p;
            defaultY = p.Y;
            defaultX = p.X;
            scale = .15f;
            this.bounds = new BoundingRectangle(position, (367 * scale), (410 * scale));
        }

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Jack-O-LanternHead");
        }

        /// <summary>
        /// Updates the sprite's position based on user input
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        public void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
           if(position.Y <= 185)
            {
                MovementVector = new Vector2(0, 5);
            }
           if(position.Y >= defaultY && Jumped == true)
            {
                MovementVector = new Vector2(0, 0);
                position.Y = defaultY;
            }
            // Apply keyboard movement
            if (keyboardState.IsKeyDown(Keys.Up) && oldState.IsKeyUp(Keys.Up) && position.Y == defaultY)
            {
                MovementVector = new Vector2(0, -5);
                Jumped = true;
            }
            if (keyboardState.IsKeyDown(Keys.Right) && position.X < 500)
            {
                position.X += 3;
            }
            if (keyboardState.IsKeyDown(Keys.Left) && position.X > defaultX)
            {
                position.X -= 3;
            }
            position += MovementVector;
            oldState = keyboardState;
            bounds.X = position.X;
            bounds.Y = position.Y;
        }

        /// <summary>
        /// Draws the sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
        }
    }
}
