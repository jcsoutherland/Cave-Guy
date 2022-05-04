using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace GameProject4
{
    public class Key
    {
        private Texture2D texture;
        public BoundingRectangle bounds;
        public Vector2 position;
        private double animationTimer;
        private float animationTime = 0.1f;
        public bool collected;

        int frameVector = 1;
        int frame = 0;
        Rectangle boundsText;
        Texture2D bt;
        Texture2D shadow;
        Rectangle shadowText;
        public Key(Vector2 p)
        {
            position = p;
            bounds = new BoundingRectangle(position, 16, 20);
            boundsText = new Rectangle((int)position.X, (int)position.Y, 16, 20);
            shadowText = new Rectangle((int)position.X + 2, (int)position.Y + 20, 12, 4);
        }

        public void LoadContent(ContentManager content, GraphicsDeviceManager _graphics)
        {
            texture = content.Load<Texture2D>("key");
            bt = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            bt.SetData(new Color[] { Color.Red });
            shadow = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            shadow.SetData(new Color[] { Color.Black });
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!collected)
            {
                animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (animationTimer > animationTime)
                {
                    if (frame > 5 || frame < 0)
                    {
                        frameVector *= -1;
                    }
                    frame += frameVector;
                    animationTimer -= animationTime;
                }
                spriteBatch.Draw(texture, new Vector2(position.X, position.Y - frame), null, Color.Gold * 0.75f, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(shadow, shadowText, Color.Black * 0.4f);
            }
            else
            {
                spriteBatch.Draw(texture, position, null, Color.Gold * 0.75f, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            }
            //spriteBatch.Draw(bt, boundsText, Color.Red * 0.5f);
        }
    }
}
