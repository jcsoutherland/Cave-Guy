using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Text;

namespace GameProject4
{
    public class Door
    {
        private Texture2D texture;
        public BoundingRectangle bounds;
        public Vector2 position;
       //private double animationTimer;
        //private float animationTime = 0.1f;
        public bool opened;

        //int frameVector = 1;
        //int frame = 0;
        Rectangle boundsText;
        Texture2D bt;
        //Texture2D shadow;
        //Rectangle shadowText;
        public Door(Vector2 p)
        {
            position = p;
            bounds = new BoundingRectangle(position, 82*.4f, 120*.4f);
            boundsText = new Rectangle((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height);
            //shadowText = new Rectangle((int)position.X + 2, (int)position.Y + 20, 12, 4);
        }

        public void LoadContent(ContentManager content, GraphicsDeviceManager _graphics)
        {
            texture = content.Load<Texture2D>("Door");
            bt = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            bt.SetData(new Color[] { Color.Red });
            //shadow = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            //shadow.SetData(new Color[] { Color.Black });
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (opened)
            {
                spriteBatch.Draw(texture, position, null, Color.LightGray, 0, new Vector2(9, 0), .4f, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(texture, position, null, Color.Gray, 0, new Vector2(9, 0), .4f, SpriteEffects.None, 0);
            }
            //spriteBatch.Draw(bt, new Vector2(boundsText.X, boundsText.Y), boundsText, Color.Red * 0.5f, 0, new Vector2(0,0), 1f, SpriteEffects.None, 0);
        }
    }
}
