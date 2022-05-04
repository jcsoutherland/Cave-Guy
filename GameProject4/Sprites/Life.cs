using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameProject4
{
    public class Life
    {
        //Vector2 position;
        Texture2D texture;
        int _id;
        public bool dead;
        public Life(int id)
        {
            //position = p;
            _id = id;
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("LifeImg");
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (dead)
            {
                return;
            }
            spriteBatch.Draw(texture, new Vector2(15 + (35*_id), 60), Color.White);
        }
    }
}
