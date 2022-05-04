using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace GameProject4
{
    public class Booster
    {
        private Texture2D texture;
        public Vector2 position;
        private double animationTimer;
        private short animationRow = 3;
        private short animationColumn = 4;
        private float animationTime = 0.2f;
        int frames = 0;
        bool reversed;
        public bool released;

        Rectangle source;
        public BoundingRectangle bounds;

        //figure out animation and launch

        Rectangle boundsText;
        Texture2D bt;

        public Booster(Vector2 p)
        {
            position = p;
            bounds = new BoundingRectangle(position.X, position.Y + 35, 66, 40);
            boundsText = new Rectangle((int)bounds.X, (int)bounds.Y, 66, 40);
        }
        public void LoadContent(ContentManager content, GraphicsDeviceManager _graphics)
        {
            texture = content.Load<Texture2D>("WaterSplashSheet");
            bt = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            bt.SetData(new Color[] { Color.Red });
        }

        public void Update(GameTime gameTime)
        {
            
        }
        //50x64

        public void WarmupAnimation(GameTime gameTime)
        {
            if (animationColumn < 1)
            {
                animationColumn = 4;
            }
            if(animationRow == 3 && animationColumn == 1)
            {
                frames++;
                if(frames > 3)
                {
                    animationColumn = 4;
                    animationRow = 3;
                }
            }
            animationColumn--;
        }
        public void ReleaseAnimation(GameTime gameTime)
        {
            if(animationColumn < 1)
            {
                animationColumn = 4;
                animationRow--;
            }
            if(animationRow < 1)
            {
                animationRow = 3;
            }
            if(animationColumn == 1 && animationRow == 1)
            {
                reversed = true;
                released = false;
            }
            animationColumn--;
        }
        public void ReversedReleaseAnimation(GameTime gameTime)
        {
            animationColumn++;
            if (animationColumn > 4)
            {
                animationColumn = 0;
                animationRow++;
            }
            if (animationColumn == 4 && animationRow == 3)
            {
                frames = 0;
                reversed = false;
            }
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer > animationTime)
            {
                if (frames > 3 && !reversed)
                {
                    //Debug.WriteLine("release");
                    released = true;
                    ReleaseAnimation(gameTime);
                    animationTime = 0.03f;
                }
                else if(frames > 3 && reversed)
                {
                    //Debug.WriteLine("reversed");
                    ReversedReleaseAnimation(gameTime);
                    animationTime = 0.08f;
                }
                else
                {
                    //Debug.WriteLine("warmup");
                    WarmupAnimation(gameTime);
                    animationTime = 0.2f;
                }
                source = new Rectangle((animationColumn * 66), (animationRow * 77), 66, 75);
                animationTimer -= animationTime;
            }
            spriteBatch.Draw(texture, position, source, Color.LightGray * 0.85f, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            //spriteBatch.Draw(bt, boundsText, Color.Red * 0.5f);
        }
    }
}
