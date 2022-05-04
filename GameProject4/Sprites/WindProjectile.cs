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
    public class WindProjectile
    {
        Player player;
        private Texture2D texture;
        Rectangle hitboxVisual;
        Texture2D hitboxVisualText;
        public Vector2 position;
        public bool hit = true;
        public bool flipped;
        Rectangle source;
        Rectangle sourceWind;
        

        public float alpha = 0f;
        BoundingRectangle hitbox;
        private short AnimationFrame = 0;
        //private short windAnimationFrame = 0;
        private short AnimationRow = 0;
        private float animationTime = 0.15f;
        private double animationTimer;
        BossAttack _ba;
        //public bool ready;

        public WindProjectile(Player p, Vector2 pos, bool f, BossAttack ba)
        {
            flipped = f;
            _ba = ba;
            player = p;
            position = pos;
            hitbox = new BoundingRectangle(position.X + 4, position.Y + 4, 24, 24);
            hitboxVisual = new Rectangle((int)hitbox.X, (int)hitbox.Y, 24, 24);
        }

        public void LoadContent(ContentManager content, GraphicsDeviceManager _graphics)
        {
            texture = content.Load<Texture2D>("Wind Projectile");
            hitboxVisualText = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            hitboxVisualText.SetData(new Color[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            if (!hit && player.numLives > 0)
            {
                if (hitbox.CollidesWith(player.bounds) && !player.inv && !_ba.nuke.safe && _ba.boss.state == "Attacking")
                {
                    hit = true;
                    player.numLives--;
                    _ba.loseLife.Play();
                    player.lives[player.numLives].dead = true;
                    player.inv = true;
                    player.lastInv = gameTime.TotalGameTime;
                }
                else if (position.X < -33 || position.X > 801 || hitbox.CollidesWith(_ba.nuke.hitbox))
                {
                    hit = true;
                }
                else
                {
                    if (flipped)
                    {
                        position.X -= 3;
                    }
                    else
                    {
                        position.X += 3;
                    }
                }
                hitbox.X = position.X + 4;
                hitbox.Y = position.Y + 4;
                //hitboxVisual.X = (int)hitbox.X;
                //hitboxVisual.Y = (int)hitbox.Y;
            }
        }

        public void WindAnimation()
        {
            animationTime = .15f;
            if (AnimationFrame > 2)
            {
                AnimationFrame = 0;
                if (AnimationRow == 0)
                {
                    AnimationRow = 1;
                }
                else
                {
                    AnimationRow = 0;
                }
            }
            source = new Rectangle((AnimationFrame * 32), (AnimationRow * 32), 32, 32);
            AnimationFrame++;
        }

        /*public void WindCurrent()
        {
            if (windAnimationFrame > 10)
            {
                windAnimationFrame = 0;
                ready = true;
            }
            else
            {
                ready = false;
            }
            sourceWind = new Rectangle((windAnimationFrame * 48), 0, 48, 32);
            windAnimationFrame++;
        }*/

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            /*if (!ready && hit)
            {
                WindCurrent();
                //spriteBatch.Draw(texture, );
            }*/
            if(!hit)
            {
                if(alpha < 1f)
                {
                    alpha += .01f;
                }
                animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (animationTimer > animationTime)
                {
                    WindAnimation();
                    animationTimer -= animationTime;
                }
                if (flipped)
                {
                    spriteBatch.Draw(texture, position, source, Color.White * alpha, 0f, new Vector2(0, 0), 1f, SpriteEffects.FlipHorizontally, 0);
                }
                else
                {
                    spriteBatch.Draw(texture, position, source, Color.White * alpha, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                }
                //spriteBatch.Draw(hitboxVisualText, hitboxVisual, Color.Red * 0.5f);
            }
        }
    }
}
