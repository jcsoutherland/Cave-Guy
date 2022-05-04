using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System.Text;

namespace GameProject4
{
    public class PlatformBox
    {
        public BoundingRectangle bounds;
        Rectangle rec;
        private Texture2D texture;
        public bool playerColliding = false;
        public string _type;


        public PlatformBox(BoundingRectangle b, string type)
        {
            bounds = b;
            //posx,posy,width,height
            rec.X = (int)b.X;
            rec.Y = (int)b.Y;
            rec.Width = (int)b.Width;
            rec.Height = (int)b.Height;
            _type = type;
        }

        public void LoadContent(ContentManager content, GraphicsDeviceManager _graphics)
        {
            texture = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.White });
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(_type == "l")
            {
                //spriteBatch.Draw(texture, rec, Color.Yellow * 0.5f);
            }
            else
            {
                //spriteBatch.Draw(texture, rec, Color.White * 0.5f);
            }
        }
    }
}
