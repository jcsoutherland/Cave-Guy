using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;

namespace GameProject4
{
    public class BossAttack
    {
        public string type = "None";//"None";
        //private Vector2 position;
        private Texture2D fireTexture;
        private Texture2D windBreathTexture;
        private Texture2D nukeTexture;
        private double animationTimer;
        private double fireAnimationTimer;
        private short fireAnimationFrame = 0;
        private short windAnimationFrame = 0;
        private float animationTime = 0.15f;
        private Rectangle sourceFire;
        private Rectangle sourceWind;
        public bool fireState = true;
        public bool windState = false;
        public bool windBreath = false;
        private TimeSpan attackWindReset = TimeSpan.FromSeconds(12);
        private TimeSpan lastWindAttack;
        private TimeSpan windTimer = TimeSpan.FromSeconds(3);
        private TimeSpan attackFireReset = TimeSpan.FromSeconds(6);
        private TimeSpan lastFireAttack;
        private GameTime g;
        private float alpha = 1f;
        private int count = 0;
        public List<BoundingRectangle> hitboxes;
        private int offset = 0;
        private int windCount = 0;
        public Nuke nuke;
        public bool nukeState;
        private TimeSpan attackNukeReset = TimeSpan.FromSeconds(12);
        public TimeSpan lastNukeAttack;
        public SoundEffect loseLife;

        public bool nextPhase;
        GraphicsDeviceManager graphics;
        private List<Rectangle> drawHitBoxes;
        Texture2D bt;
        public Player player;
        WindProjectile[] windProjectiles;
        Random rand;

        public BossAttack(Player p, GraphicsDeviceManager _graphics)
        {
            graphics = _graphics;
            player = p;
            hitboxes = new List<BoundingRectangle>();
            drawHitBoxes = new List<Rectangle>();
            windProjectiles = new WindProjectile[] { 
                new WindProjectile(player, new Vector2(-32, 330), false, this),
                new WindProjectile(player, new Vector2(800, 330), true, this),
                new WindProjectile(player, new Vector2(-32, 330), false, this),
                new WindProjectile(player, new Vector2(800, 330), true, this)
            };
            rand = new Random();
            float randX = (float)rand.Next(120, 687);
            nuke = new Nuke(new Vector2(randX, 240), player, graphics, this);
            //hitboxes
            for (int j = 0; j <= 1; j++)
            {
                for (int i = 0; i <= 5; i++)
                {
                    //first 6 hitboxes = fire attack
                    hitboxes.Add(new BoundingRectangle(new Vector2(-80 + (160 * i) + (80 * j) + 2, 0), 47, 480));
                    drawHitBoxes.Add(new Rectangle(-80 + (160 * i) + (80*j) + 2, 0, 47, 480));
                }
            }
        }

        public void LoadContent(ContentManager content, GraphicsDeviceManager _graphics)
        {
            nuke.LoadContent(content, _graphics);
            fireTexture = content.Load<Texture2D>("FireColumn");
            windBreathTexture = content.Load<Texture2D>("Wind Breath");
            loseLife = content.Load<SoundEffect>("loseLife");
            nukeTexture = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            nukeTexture.SetData(new Color[] { Color.LightBlue });
            foreach (WindProjectile w in windProjectiles) w.LoadContent(content, _graphics);
            bt = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            bt.SetData(new Color[] { Color.White });
        }
        
        public void FireColumn()
        {
            if (fireAnimationFrame > 4)
            {
                animationTime = 0.1f;
            }
            else if (fireAnimationFrame > 0)
            {
                alpha = 0.8f;
                animationTime = 0.15f;
            }
            else
            {
                animationTime = 0.5f;
                alpha = 0.5f;
            }
            if (!nextPhase)
            {
                sourceFire = new Rectangle((fireAnimationFrame * 51), 0, 51, 480);
                fireAnimationFrame++;
                if (fireAnimationFrame > 8)
                {
                    fireAnimationFrame = 0;
                    if (type != "FWN" && type != "Nuke")
                    {
                        type = "None";
                    }
                    lastFireAttack = g.TotalGameTime;
                    count++;
                    offset = (count % 2);
                }
            }
        }

        public void WindFloor()
        {
            lastWindAttack = g.TotalGameTime;
            int sub = 0;
            if (windCount == 0)
            {
                sub = 2;
            }
            for (int i = (windCount * 2); i < windProjectiles.Length - sub; i++)
            {
                windProjectiles[i].hit = false;
                windProjectiles[i].alpha = 0f;
                if (!windProjectiles[i].flipped)
                {
                    windProjectiles[i].position = new Vector2(-32, 330);
                }
                else
                {
                    windProjectiles[i].position = new Vector2(800, 330);
                }
            }
            windCount++;
            if (windCount >= 2)
            {
                windCount = 0;
                windState = false;
                if (type != "FWN")
                {
                    type = "None";
                }
            }
        }

        public void Nuke()
        {
            nuke.active = true;
        }

        public void Update(GameTime gameTime)
        {
            foreach (WindProjectile w in windProjectiles) w.Update(gameTime);
            if (!fireState)
            {
                if (lastFireAttack + attackFireReset < gameTime.TotalGameTime)
                {
                    fireState = true;
                }
            }
            if (!windState)
            {
                if(lastWindAttack + attackWindReset < gameTime.TotalGameTime)
                {
                    windState = true;
                }
            }
            if (!nuke.active)
            {
                if(lastNukeAttack + attackNukeReset < gameTime.TotalGameTime)
                {
                    nuke.active = true;
                    float randX = (float)rand.Next(120, 687);
                    nuke.position = new Vector2(randX, 240);
                    
                }
            }
            if((type == "FirePillar" || type == "FWN" || type == "Nuke") && fireState && (fireAnimationFrame == 4 || fireAnimationFrame == 5) && !player.inv)
            {
                if(offset == 0)
                {
                    for(int i = 0; i < 6; i++)
                    {
                        if (player.bounds.CollidesWith(hitboxes[i]) && !nuke.safe && player.numLives > 0)
                        {
                            player.numLives--;
                            loseLife.Play();
                            player.lives[player.numLives].dead = true;
                            player.inv = true;
                            player.lastInv = gameTime.TotalGameTime;
                        }
                    }
                }
                else
                {
                    for (int i = 6; i < 12; i++)
                    {
                        if (player.bounds.CollidesWith(hitboxes[i]) && !nuke.safe && player.numLives > 0)
                        {
                            player.numLives--;
                            loseLife.Play();
                            player.lives[player.numLives].dead = true;
                            player.inv = true;
                            player.lastInv = gameTime.TotalGameTime;
                        }
                    }
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(type != "None")
            {
                animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                fireAnimationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                g = gameTime;
                if (animationTimer > animationTime)
                {
                    switch (type)
                    {
                        case "FirePillar":
                            if (fireState)
                            {
                                FireColumn();
                            }
                            break;
                        case "WindFloor":
                            attackWindReset = TimeSpan.FromSeconds(4);
                            if (windState)
                            {
                                if (lastWindAttack + windTimer < gameTime.TotalGameTime)
                                {
                                    WindFloor();
                                }
                            }
                            break;
                        case "Nuke":
                            if (nextPhase)
                            {
                                fireAnimationFrame = 0;
                                count = 0;
                                animationTime = 0.5f;
                                alpha = 0.5f;
                                nextPhase = false;
                                nuke.first = true;
                            }
                            if (nuke.active)
                            {
                                Nuke();
                            }
                            if (fireState && !nuke.first)
                            {
                                attackFireReset = TimeSpan.FromSeconds(1);
                                if (lastFireAttack + attackFireReset < gameTime.TotalGameTime)
                                {
                                    FireColumn();
                                }
                            }
                            break;
                        case "FWN":
                            if (nextPhase)
                            {
                                fireAnimationFrame = 0;
                                count = 0;
                                animationTime = 0.5f;
                                alpha = 0.5f;
                                nextPhase = false;
                                nuke.first = true;
                            }
                            if (nuke.active)
                            {
                                Nuke();
                            }
                            if (fireState && !nuke.first)
                            {
                                attackFireReset = TimeSpan.FromSeconds(1);
                                if (lastFireAttack + attackFireReset < gameTime.TotalGameTime)
                                {
                                    FireColumn();
                                }
                            }
                            attackWindReset = TimeSpan.FromSeconds(12);
                            if (windState)
                            {
                                if (lastWindAttack + windTimer < gameTime.TotalGameTime)
                                {
                                    WindFloor();
                                }
                            }
                            break;
                        case "None":
                            player.inv = true;
                            fireAnimationFrame = 0;
                            count = 0;
                            animationTime = 0.5f;
                            alpha = 0.5f;
                            nextPhase = false;
                            nuke.first = true;
                            break;
                    }
                    animationTimer -= animationTime;
                }
                if ((type == "FirePillar" || type == "Nuke" || type == "FWN") && !nuke.first)
                {
                    spriteBatch.Draw(fireTexture, new Vector2(-80 + (80 * offset), 0), sourceFire, Color.White * alpha);
                    spriteBatch.Draw(fireTexture, new Vector2(80 + (80 * offset), 0), sourceFire, Color.White * alpha);
                    spriteBatch.Draw(fireTexture, new Vector2(240 + (80 * offset), 0), sourceFire, Color.White * alpha);
                    spriteBatch.Draw(fireTexture, new Vector2(400 + (80 * offset), 0), sourceFire, Color.White * alpha);
                    spriteBatch.Draw(fireTexture, new Vector2(560 + (80 * offset), 0), sourceFire, Color.White * alpha);
                    spriteBatch.Draw(fireTexture, new Vector2(720 + (80 * offset), 0), sourceFire, Color.White * alpha);
                }
                if (type == "WindFloor" || type == "FWN")
                {
                    foreach (WindProjectile w in windProjectiles) w.Draw(gameTime, spriteBatch);
                    spriteBatch.Draw(windBreathTexture, new Vector2(0, 330), sourceWind, Color.White * 0.8f);
                    spriteBatch.Draw(windBreathTexture, new Vector2(800 - 48, 330), sourceWind, Color.White * 0.8f, 0f, new Vector2(0, 0), 1f, SpriteEffects.FlipHorizontally, 0);
                }
                if (type == "Nuke" || type == "FWN")
                {
                    nuke.Draw(gameTime, spriteBatch);
                }
                /*foreach (Rectangle r in drawHitBoxes)
                {
                    spriteBatch.Draw(bt, r, Color.Green * 0.6f);
                }*/
            }

        }
    }
}
