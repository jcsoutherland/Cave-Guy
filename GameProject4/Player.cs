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
    public class Player
    {
        private Texture2D texture;
        private Texture2D climbTexture;
        private double animationTimer;
        public bool startAnimation = false;
        private short animationFrame = 0;
        private short animationRow = 0;
        public Vector2 defaultPos;
        public BoundingRectangle bounds;
        public bool running = false;
        public bool idle = true;
        public bool crouching = false;
        public bool attacking = false;
        public bool onBoss = false;
        KeyboardState prevState;
        private float animationTime = 0.3f;
        string direction = "r";
        public string attackState = "first";
        GamePlayScreen gps;
        private TimeSpan attackReset;
        private TimeSpan lastAttack;

        private SoundEffect punchHit;
        private SoundEffect punchMiss;
        public string ladderState = "none";
        public bool onPlatform;
        public Vector2 _position;
        public Vector2 _velocity;
        float slowVector = 1;
        bool hasJumped;
        public PlatformBox ladder;
        public bool boosted;
        float i = 1;

        public Life[] lives;
        public int numLives = 3;
        public bool inv;
        public TimeSpan invReset;
        public TimeSpan lastInv;
        public float alpha = 1f;
        bool alphaReversed;

        //To do:
            //add punch sound fx

        Rectangle boundsText;
        Texture2D bt;
        Rectangle source;
        Rectangle temp;
        GraphicsDeviceManager _graphics;

        public Player(GraphicsDeviceManager g, Vector2 p, GamePlayScreen gp)
        {
            bounds = new BoundingRectangle(_position.X, _position.Y, 20, 30);
            boundsText = new Rectangle((int)bounds.X, (int)bounds.Y, 20, 30);
            gps = gp;
            defaultPos = p;
            _graphics = g;
            _position = p;
            lives = new Life[]
                {
                new Life(0),
                new Life(1),
                new Life(2)
                };
            attackReset = TimeSpan.FromSeconds(1);
            invReset = TimeSpan.FromSeconds(2);
        }

        public void LoadContent(ContentManager content)
        {
            punchHit = content.Load<SoundEffect>("punchHit");
            punchMiss = content.Load<SoundEffect>("punchMiss");
            texture = content.Load<Texture2D>("adventurer-hand-combat-Sheet");
            climbTexture = content.Load<Texture2D>("ClimbingAnimation");
            bt = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            bt.SetData(new Color[] { Color.Red });
        }

        public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            float t = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (inv)
            {
                if(lastInv + invReset < gameTime.TotalGameTime)
                {
                    inv = false;
                    alpha = 1f;
                }
                else if(!alphaReversed && !gps.boss.bossAttack.nuke.inv && gps.boss.bossAttack.type != "None")
                {
                    alpha -= .05f;
                    if(alpha <= .2f)
                    {
                        alphaReversed = true;
                    }
                }
                else if(alphaReversed && !gps.boss.bossAttack.nuke.inv && gps.boss.bossAttack.type != "None")
                {
                    alpha += .05f;
                    if(alpha >= 1f)
                    {
                        alphaReversed = false;
                    }
                }
            }
            if (boosted)
            {
                onPlatform = false;
                hasJumped = true;
                ladderState = "none";
                if(_velocity.Y > 0)
                {
                    boosted = false;
                }
            }
            if (keyboardState.IsKeyDown(Keys.Space) && prevState.IsKeyUp(Keys.Space) && hasJumped == false && !keyboardState.IsKeyDown(Keys.Down) && !keyboardState.IsKeyDown(Keys.S))
            {
                _position.Y -= 8f;
                _velocity.Y = -5f;
                hasJumped = true;
                onPlatform = false;
                ladderState = "none";
            }
            if ((keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W)) && ladderState != "none")
            {
                Debug.WriteLine(ladderState);
                if (_position.Y + bounds.Height < ladder.bounds.Top)
                {
                    ladderState = "none";
                }
                else
                {
                    Debug.WriteLine("moving");
                    _position.X = ladder.bounds.Left;
                    ladderState = "on";
                    _velocity.Y = 0f;
                    hasJumped = false;
                    _position.Y -= 2f;
                }
            }
            if ((keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S)))
            {
                if(hasJumped == false && keyboardState.IsKeyDown(Keys.Space))
                {
                    _position.Y += 20f;
                    _velocity.Y = -2f;
                    onPlatform = false;
                    hasJumped = true;
                }
                else if(!hasJumped)
                {
                    crouching = true;
                }
                if(ladderState == "on" && _position.Y + bounds.Height < ladder.bounds.Bottom)
                {
                    Debug.WriteLine("moving");
                    _position.X = ladder.bounds.Left;
                    ladderState = "on";
                    _velocity.Y = 0f;
                    hasJumped = false;
                    _position.Y += 2f;
                }
                else
                {
                    ladderState = "none";
                }
            }
            else
            {
                crouching = false;
            }
            if ((keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A)) && ladderState != "on")
            {
                running = true;
                direction = "l";
                if (crouching || bounds.Y > _graphics.GraphicsDevice.Viewport.Height - 30)
                {
                    slowVector = 2;
                }
                else
                {
                    slowVector = 1;
                }
                _velocity.X = -((200 / slowVector) * t);
            }
            else if ((keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D)) && ladderState != "on")
            {
                running = true;
                direction = "r";
                if (crouching || bounds.Y > _graphics.GraphicsDevice.Viewport.Height - 30)
                {
                    slowVector = 2;
                }
                else
                {
                    slowVector = 1;
                }
                _velocity.X = ((200 / slowVector) * t);
            }
            else
            {
                _velocity.X = 0;
                running = false;
                idle = true;
            }
            if (!onPlatform && ladderState != "on")
            {
                if (bounds.Y > _graphics.GraphicsDevice.Viewport.Height - 30)
                {
                    _position.Y = _graphics.GraphicsDevice.Viewport.Height - 30;
                    slowVector = 1.5f;
                    hasJumped = false;
                }
                else
                {
                    if (hasJumped == true && (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S)))
                    {
                        i = 2;
                    }
                    else
                    {
                        i = 1;
                    }
                    _velocity.Y += 0.25f * i;
                    slowVector = 1f;
                }
            }
            if (onPlatform)
            {
                _velocity.Y = 0f;
                hasJumped = false;
                _position.Y = defaultPos.Y - bounds.Height;
            }
            if (((keyboardState.IsKeyDown(Keys.LeftControl) && prevState.IsKeyUp(Keys.LeftControl)) || (keyboardState.IsKeyDown(Keys.RightControl) && prevState.IsKeyUp(Keys.RightControl))) && !attacking && ladderState != "on")
            {
                if (lastAttack + attackReset < gameTime.TotalGameTime)
                {
                    attackState = "first";
                }
                if (bounds.CollidesWith(gps.boss.bounds) && (gps.boss.state == "Idle" || gps.boss.state == "Attacking") && gps.boss.Spawned)
                {
                    gps.boss.health--;
                    punchHit.Play();
                }
                else
                {
                    punchMiss.Play();
                }
                lastAttack = gameTime.TotalGameTime;
                attacking = true;
                startAnimation = true;
            }
            if (_position.X > _graphics.GraphicsDevice.Viewport.Width - 10)
            {
                _position.X = _graphics.GraphicsDevice.Viewport.Width - 10;
            }
            if (_position.X < 10)
            {
                _position.X = 10;
            }
            if(_position.Y < 0)
            {
                _position.Y = 0;
            }
            if(_position.Y > _graphics.GraphicsDevice.Viewport.Height - 30)
            {
                _position.Y = _graphics.GraphicsDevice.Viewport.Height - 30;
            }
            prevState = keyboardState;
            _position += _velocity;
            bounds.X = _position.X;
            bounds.Y = _position.Y;
            boundsText.X = (int)bounds.X;
            boundsText.Y = (int)bounds.Y;
        }

        public void RunAnimation()
        {
            if (crouching)
            {
                if (animationFrame > 6 || animationFrame < 3)
                {
                    animationFrame = 3;
                    animationRow++;
                }
                if(animationRow != 8 || animationRow != 7)
                {
                    animationRow = 7;
                }
                temp = new Rectangle((animationFrame * 50), animationRow * 37, 50, 37);
            }
            else
            {
                if (animationFrame > 6 || animationFrame < 2)
                {
                    animationFrame = 2;
                }
                temp = new Rectangle((animationFrame * 50), 8 * 37, 50, 37);
            }
            source = new Rectangle(temp.X + 11, temp.Y + 6, 32, 32);
        }

        public void AttackAnimation()
        {
            Debug.WriteLine(attackState);
            if(attackState == "first")
            {
                if (startAnimation == true)
                {
                    animationFrame = 0;
                    animationRow = 0;
                    startAnimation = false;
                }
                if (animationFrame > 4)
                {
                    attacking = false;
                    idle = true;
                    attackState = "second";
                }
            }
            else if(attackState == "second")
            {
                //animationTime = 0.5f;
                if (startAnimation == true)
                {
                    animationFrame = 4;
                    animationRow = 0;
                    startAnimation = false;
                }
                if (animationFrame > 6 && animationRow == 0)
                {
                    animationFrame = 0;
                    animationRow = 1;
                }
                if(animationFrame == 2 && animationRow == 1)
                {
                    attacking = false;
                    idle = true;
                    attackState = "third";
                }
            }
            else if(attackState == "third")
            {
                //animationTime = 0.5f;
                if (startAnimation == true)
                {
                    animationFrame = 1;
                    animationRow = 1;
                    startAnimation = false;
                }
                if (animationFrame > 6)
                {
                    attacking = false;
                    idle = true;
                    attackState = "first";
                }
            }
            temp = new Rectangle((animationFrame * 50), animationRow * 37, 50, 37);
            source = new Rectangle(temp.X + 11, temp.Y + 6, 32, 32);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer > animationTime)
            {
                animationFrame++;
                if (attacking)
                {
                    animationTime = 0.1f;
                    idle = false;
                    running = false;
                    AttackAnimation();
                }
                if (running)
                {
                    animationTime = 0.1f;
                    idle = false;
                    RunAnimation();
                }
                if (idle)
                {
                    running = false;
                    animationTime = 0.3f;
                    if (crouching)
                    {
                        temp = new Rectangle((5 * 50), 7 * 37, 50, 37);
                    }
                    else
                    {
                        if (animationFrame > 1)
                        {
                            animationFrame = 0;
                        }
                        temp = new Rectangle((animationFrame * 50), 0, 50, 37);
                    }
                    source = new Rectangle(temp.X + 11, temp.Y + 6, 32, 32);
                }
                animationTimer -= animationTime;
            }
            if (direction == "l")
            {
                if (ladderState == "on")
                {
                    spriteBatch.Draw(climbTexture, _position, null, Color.White * alpha, 0, new Vector2(10, 12), 1.25f, SpriteEffects.FlipHorizontally, 0);
                }
                else
                {
                    spriteBatch.Draw(texture, _position, source, Color.White * alpha, 0, new Vector2(8, 6), 1.25f, SpriteEffects.FlipHorizontally, 0);
                }
            }
            else if (direction == "r")
            {
                if (ladderState == "on")
                {
                    spriteBatch.Draw(climbTexture, _position, null, Color.White * alpha, 0, new Vector2(20, 12), 1.25f, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(texture, _position, source, Color.White * alpha, 0, new Vector2(8, 6), 1.25f, SpriteEffects.None, 0);
                }
            }
        }
    }
}
