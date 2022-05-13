using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;

namespace GameProject4
{
    public class Boss
    {
        public bool Spawned;

        public string state = "None";
        public string attackState = "None";

        public int KC = 0;
        private Texture2D texture;
        private Texture2D attackTexture;
        public Vector2 position;
        private double animationTimer;
        private short animationFrame = 0;
        private int SpawnFrame = 26;
        private float animationTime = 0.2f;
        Color color;

        private Song gameMusic2;
        private Song gameMusic3;
        GraphicsDeviceManager _graphics;
        Rectangle source;
        public int health = 50;
        public BoundingRectangle bounds;
        Rectangle healthBar;
        Rectangle healthBarBg;
        SpriteFont spriteFont;

        public TimeSpan finishTime;
        public bool resetPhase3;
        public bool resetPhase4;
        public BossAttack bossAttack;
        Rectangle boundsText;
        Texture2D bt;
        int attackFrame = 0;
        //public Stopwatch timer;
        Player _player;

        public Boss(Vector2 p, Player player, GraphicsDeviceManager g)
        {
            position = p;
            _graphics = g;
            _player = player;
            bounds = new BoundingRectangle(new Vector2(position.X - 10, position.Y - 10), 135, 185);
            boundsText = new Rectangle((int)position.X, (int)position.Y, 135, 185);
            healthBar = new Rectangle(270, 120, 300, 10);
            healthBarBg = new Rectangle(267, 117, 300 + 6, 16);
            bossAttack = new BossAttack(player, _graphics, this);
        }
        public void LoadContent(ContentManager content, GraphicsDeviceManager g, SpriteFont sf)
        {
            spriteFont = sf;
            texture = content.Load<Texture2D>("BossSheet");
            attackTexture = content.Load<Texture2D>("BossAttackSheet");
            bt = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            bt.SetData(new Color[] { Color.White });
            gameMusic2 = content.Load<Song>("bossmusic");
            gameMusic3 = content.Load<Song>("bg_music");
            bossAttack.LoadContent(content, _graphics);
        }

        public void SpawnAnimation()
        {
            source = new Rectangle((SpawnFrame * 125), 0, 125, 175);
        }

        public void Update(GameTime gameTime)
        {           
            boundsText.Height = (int)bounds.Height;
            healthBar.Width = 6*health;
            bossAttack.Update(gameTime);
            if(health == 0 && state != "Killed" && state != "None")
            {
                _player.inv = true;
                state = "Killed";
                attackState = "None";
                bossAttack.type = attackState;
                //timer.Stop();
                finishTime = gameTime.TotalGameTime;
                MediaPlayer.Play(gameMusic3);
            }
            if(state == "Idle" || state == "Attacking")
            {
                if (bossAttack.fireState)//change for all attacks
                {
                    state = "Attacking";
                }
            }
            if(state == "Attacking")
            {
                if (health == 50)
                {                  
                    attackState = "FirePillar";
                    //attackState = "Nuke";
                }
                else if (health == 35)
                {
                    bossAttack.fireState = false;
                    attackState = "WindFloor";
                }
                else if (health == 20 && !resetPhase3)
                {
                    bossAttack.nextPhase = true;
                    bossAttack.windState = false;
                    resetPhase3 = true;
                    attackState = "Nuke";
                }
                else if(health == 10 && !resetPhase4)
                {
                    bossAttack.nextPhase = true;
                    resetPhase4 = true;
                    attackState = "FWN";
                }
                else if(health <= 0)
                {
                    attackState = "None";
                }
                bossAttack.type = attackState;
            }
        }
        //50x64
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(state != "None")
            {
                //Debug.WriteLine(state);
                animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (animationTimer > animationTime)
                {
                    if(state == "Down")
                    {
                        color = Color.LightGray;
                        source = new Rectangle((26 * 125), 0, 125, 175);
                    }
                    else if(state == "Idle")
                    {
                        color = Color.LightGray;
                        if (animationFrame == 4)
                        {
                            animationFrame = 0;
                        }
                        source = new Rectangle((animationFrame * 125), 0, 125, 175);
                        animationFrame++;
                    }
                    else if(state == "Spawned" && SpawnFrame >= 13)
                    {
                        color = Color.LightGray;
                        Debug.WriteLine("Spawned");
                        source = new Rectangle((SpawnFrame * 125), 0, 125, 175);
                        SpawnFrame--;
                    }
                    else if(state == "Killed" && SpawnFrame <= 26)
                    {
                        color = Color.LightGray;
                        source = new Rectangle((SpawnFrame * 125), 0, 125, 175);
                        SpawnFrame++;
                    }
                    else if (state == "Attacking")
                    {
                        //attacking animation
                        if(attackState == "FirePillar")
                        {
                            bossAttack.type = "FirePillar";
                            color = Color.LightGray;
                            if (attackFrame < 7 || attackFrame > 13)
                            {
                                attackFrame = 7;
                            }
                            source = new Rectangle((attackFrame * 125), 0, 125, 175);
                            Debug.WriteLine(attackFrame.ToString());
                            attackFrame++;
                        }
                        else if(attackState == "WindFloor")
                        {
                            bossAttack.type = "WindFloor";
                            color = Color.LightGray;
                            if (attackFrame < 14 || attackFrame > 20)
                            {
                                attackFrame = 14;
                            }
                            source = new Rectangle((attackFrame * 125), 0, 125, 175);
                            attackFrame++;
                        }
                        else if(attackState == "Nuke")
                        {
                            bossAttack.type = "Nuke";
                            color = Color.LightGray;
                            if (attackFrame < 0 || attackFrame > 6)
                            {
                                attackFrame = 0;
                            }
                            source = new Rectangle((attackFrame * 125), 0, 125, 175);
                            attackFrame++;
                        }
                        else if(attackState == "FWN")
                        {
                            color = Color.LightGray;
                            if (animationFrame == 4)
                            {
                                animationFrame = 0;
                            }
                            source = new Rectangle((animationFrame * 125), 0, 125, 175);
                            animationFrame++;
                        }
                    }
                    else
                    {
                        if(state == "Killed")
                        {
                            KC += 1;
                            state = "None";
                        }
                        else
                        {
                            state = "Idle";
                            MediaPlayer.Stop();
                            MediaPlayer.Play(gameMusic2);
                        }
                    }
                    animationTimer -= animationTime;
                }
                if (state == "Attacking" && attackState != "FWN")
                {

                    spriteBatch.Draw(attackTexture, position, source, Color.LightGray, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(texture, position, source, Color.LightGray, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                }
                if (state == "Idle" || state == "Attacking")
                {
                    spriteBatch.Draw(bt, healthBarBg, Color.Gray);
                    spriteBatch.Draw(bt, healthBar, Color.Red * 0.4f);
                    spriteBatch.DrawString(spriteFont, "Stone Steven", new Vector2(healthBarBg.X + 75, healthBarBg.Y - 30), Color.White, 0, new Vector2(0, 0), .4f, SpriteEffects.None, 0);
                }
                //spriteBatch.Draw(attackTexture, position, source, Color.LightGray, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                //spriteBatch.Draw(bt, boundsText, Color.Red * 0.5f);
            }
            else
            {
                source = new Rectangle((26 * 125), 0, 125, 175);
                color = Color.Gray;
                spriteBatch.Draw(texture, position, source, color, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            }
        }
    }
}
