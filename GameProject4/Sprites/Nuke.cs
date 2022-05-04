using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace GameProject4
{
    public class Nuke
    {
        public Vector2 position;
        Player player;
        Texture2D nukeTexture;
        Rectangle nuke;
        Texture2D safeTexture;
        Texture2D outlineSafe;
        BoundingCircle hitbox;
        float radius = 90f;
        GraphicsDeviceManager _graphics;
        public float alpha = -1f;
        public bool active;
        public bool safe;
        BossAttack _ba;
        public bool inv;
        public bool first;

        TimeSpan invReset = TimeSpan.FromSeconds(1);
        TimeSpan lastInv;

        public Nuke(Vector2 pos, Player p, GraphicsDeviceManager g, BossAttack ba)
        {
            _graphics = g;
            position = pos;
            _ba = ba;
            player = p;
            hitbox = new BoundingCircle(new Vector2(position.X + radius + 3, position.Y + radius + 3), radius + 3);
            nuke = new Rectangle(0, 0, 800, 480);
        }

        //Generate a circle texture
        //Credits: https://stackoverflow.com/questions/34832450/2d-xna-draw-a-circle
        public static Texture2D GenerateCircleTexture(GraphicsDevice graphicsDevice, int radius, Color color, float sharpness)
        {
            int diameter = radius * 2;
            Texture2D circleTexture = new Texture2D(graphicsDevice, diameter, diameter, false, SurfaceFormat.Color);
            Color[] colorData = new Color[circleTexture.Width * circleTexture.Height];
            Vector2 center = new Vector2(radius);
            for (int colIndex = 0; colIndex < circleTexture.Width; colIndex++)
            {
                for (int rowIndex = 0; rowIndex < circleTexture.Height; rowIndex++)
                {
                    Vector2 position = new Vector2(colIndex, rowIndex);
                    float distance = Vector2.Distance(center, position);

                    // hermite iterpolation
                    float x = distance / diameter;
                    float edge0 = (radius * sharpness) / (float)diameter;
                    float edge1 = radius / (float)diameter;
                    float temp = MathHelper.Clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
                    float result = temp * temp * (3.0f - 2.0f * temp);

                    colorData[rowIndex * circleTexture.Width + colIndex] = color * (1f - result);
                }
            }
            circleTexture.SetData<Color>(colorData);
            return circleTexture;
        }

        public void LoadContent(ContentManager content, GraphicsDeviceManager _graphics)
        {
            nukeTexture = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            nukeTexture.SetData(new Color[] { Color.White });
            safeTexture = GenerateCircleTexture(_graphics.GraphicsDevice, (int)radius, Color.White, .85f);
            outlineSafe = GenerateCircleTexture(_graphics.GraphicsDevice, (int)radius + 3, Color.White, 1f);
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (active)
            {
                hitbox.Center = new Vector2(position.X + radius + 3, position.Y + radius + 3);
                if (player.bounds.CollidesWith(hitbox))
                {
                    //Debug.WriteLine("Safe");
                    player.alpha = 0.5f;
                    safe = true;
                }
                else if(!player.inv)
                {
                    //Debug.WriteLine("Dead");
                    player.alpha = 1f;
                }
                else
                {
                    safe = false;
                }
                if (alpha < .85f)
                {
                    alpha += .01f;
                }
                spriteBatch.Draw(nukeTexture, nuke, Color.DarkCyan * alpha);
                spriteBatch.Draw(outlineSafe, new Vector2(position.X - 3, position.Y - 3), Color.White * 0.7f);
                spriteBatch.Draw(safeTexture, position, Color.DarkBlue * 0.3f);
                if (alpha >= .85f)
                {
                    //delete player.inv condition later
                    if (!safe && player.numLives > 0)
                    {
                        player.numLives--;
                        _ba.loseLife.Play();
                        player.lives[player.numLives].dead = true;
                        player.invReset = TimeSpan.FromSeconds(2);
                        player.inv = true;
                        player.lastInv = gameTime.TotalGameTime;
                    }
                    else if (!player.inv)
                    {
                        player.alpha = 1f;
                        player.invReset = TimeSpan.FromSeconds(1);
                        inv = true;
                        lastInv = gameTime.TotalGameTime;
                    }
                    _ba.lastNukeAttack = gameTime.TotalGameTime;
                    active = false;
                    alpha = -1f;
                    if (first)
                    {
                        first = false;
                    }
                }

            }
            else
            {
                if (inv)
                {
                    if(lastInv + invReset < gameTime.TotalGameTime)
                    {
                        inv = false;
                    }
                }
                else
                {
                    player.invReset = TimeSpan.FromSeconds(2);
                }
                safe = false;
            }

            //spriteBatch.Draw(texture, position, source, Color.White * alpha, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            //spriteBatch.Draw(outlineSafe, new Vector2(hitbox.Center.X - radius - 6, hitbox.Center.Y - radius - 6), Color.Red * 0.5f);
        }
    }
}
