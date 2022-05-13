using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
//using System.Windows.Forms;

namespace GameProject4
{
    /// <summary>
    /// A class representing the games gameplay screen
    /// </summary>
    public class GamePlayScreen
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        SpriteFont spriteFont;

        public Boss boss;
        bool bossMessage;
        Key[] keys;
        Player player;
        private Texture2D background;
        int keysCollected = 0;
        Texture2D recText;
        Rectangle rec;
        Door door;
        Texture2D fireColumn;

        KeyboardState prev;
        PlatformBox[] platforms;
        PlatformBox[] ladders;
        int count = 0;
        //private Tilemap _tileMap;
        private SoundEffect keyPickup;
        //private Song gameMusic;
        public bool exit;
        Booster booster;

        private Texture2D fadeToBlack;
        private Rectangle fadeRec;
        private float alpha = 1.3f;
        bool nextLevel = false;

        int hours = 0;
        int seconds = 0;
        int minutes = 0;
        //double timerDelay = 1;
        double timeCheck = 0;
        bool timerStarted;
        Stopwatch timer;
        //to do:
        //add more sound fx


        /// <summary>
        /// Constructor for the gameplay screen
        /// </summary>
        /// <param name="g"></param>
        public GamePlayScreen(GraphicsDeviceManager g)
        {
            _graphics = g;
            System.Random rand = new System.Random();
            //_tileMap = new Tilemap("map.txt");
            keys = new Key[]
                {
                new Key(new Vector2(145, 175)),
                new Key(new Vector2(663, 255)),
                new Key(new Vector2(320, _graphics.GraphicsDevice.Viewport.Height - 134)),
                new Key(new Vector2(80, _graphics.GraphicsDevice.Viewport.Height - 20))
                };
            rec = new Rectangle(15, 20, 120, 32);
            platforms = new PlatformBox[]
            {
                new PlatformBox(new BoundingRectangle(-10, 367, 820, 15), "p"),
                new PlatformBox(new BoundingRectangle(-10, 194, 213, 7), "p"),
                new PlatformBox(new BoundingRectangle(650, 275, 160, 15), "p"),
                new PlatformBox(new BoundingRectangle(735, 182, 100, 7), "p")
            };
            ladders = new PlatformBox[] {new PlatformBox(new BoundingRectangle(682, 280, 20, 80), "l"), new PlatformBox(new BoundingRectangle(331, 372, 20, 103), "l")};
            booster = new Booster(new Vector2(30, 295));
            door = new Door(new Vector2(750, 319));
            fadeRec = new Rectangle(0, 0, _graphics.GraphicsDevice.Viewport.Width, _graphics.GraphicsDevice.Viewport.Height);
            player = new Player(_graphics, new Vector2(250, _graphics.GraphicsDevice.Viewport.Height - 144), this);
            boss = new Boss(new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2 - 44, _graphics.GraphicsDevice.Viewport.Height / 2 - 48), player, g);
            timer = new Stopwatch();
        }

        /// <summary>
        /// Loads content from ContentManager
        /// </summary>
        /// <param name="Content"></param>
        public void LoadContent(ContentManager Content)
        {
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
            player.LoadContent(Content);
            keyPickup = Content.Load<SoundEffect>("Pickup_Coin15");
            fireColumn = Content.Load<Texture2D>("FireColumn");
            fadeToBlack = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            fadeToBlack.SetData(new Color[] { Color.Black });
            foreach (Key k in keys) k.LoadContent(Content, _graphics);
            SoundEffect.MasterVolume = .03f;
            spriteFont = Content.Load<SpriteFont>("arial");
            background = Content.Load<Texture2D>("LevelBg");
            foreach (Life l in player.lives) l.LoadContent(Content);
            foreach (PlatformBox p in platforms) p.LoadContent(Content, _graphics);
            foreach (PlatformBox l in ladders) l.LoadContent(Content, _graphics);
            recText = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            recText.SetData(new Color[] { Color.White });
            boss.LoadContent(Content, _graphics, spriteFont);
            booster.LoadContent(Content, _graphics);
            door.LoadContent(Content, _graphics);
            //_tileMap.LoadContent(Content);
        }
        /// <summary>
        /// Game Loop updating screen every tick
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                exit = true;

            if(exit == false && player.numLives > 0)
            {
                boss.Update(gameTime);
                player.Update(gameTime);
                if(player.bounds.CollidesWith(booster.bounds) && booster.released)
                {
                    player.boosted = true;
                    player._velocity.Y = -10f;
                    player._position.Y -= 42f;
                }
                if (player.bounds.CollidesWith(door.bounds))
                {
                    if (keyboardState.IsKeyDown(Keys.E) && door.opened)
                    {
                        Debug.WriteLine("Next Level");
                        nextLevel = true;
                    }
                }
                foreach (PlatformBox p in platforms)
                {
                    if (player.bounds.CollidesWith(p.bounds, "p") && player.ladderState != "on")
                    {
                        player.defaultPos.Y = p.bounds.Top;
                        count++;
                    }
                }
                foreach(PlatformBox l in ladders)
                {
                    if(player.ladderState != "on")
                    {
                        if (player.bounds.CollidesWith(l.bounds, "l") && player.ladderState == "none" && !player.hasJumped)
                        {
                            player.ladder = l;
                            player.ladderState = "near";
                        }
                        else if(player.ladderState == "none")
                        {
                            player.ladder = null;
                        }
                    }

                }
                if(count > 0)
                {
                    player.onPlatform = true;
                }
                else
                {
                    player.onPlatform = false;
                }
                count = 0;
                foreach (Key k in keys)
                {
                    if (player.bounds.CollidesWith(k.bounds))
                    {
                        if (keyboardState.IsKeyDown(Keys.E))
                        {
                            keysCollected++;
                            keyPickup.Play(.2f, 0 , 0);
                            k.position = new Vector2(keysCollected * 27, 28);
                            k.collected = true;
                            k.bounds = new BoundingRectangle(0, 0, 0, 0);
                        }
                    }
                }
                if (player.bounds.CollidesWith(boss.bounds))
                {
                    //take damage
                    player.onBoss = true;
                    if(boss.state == "Down")
                    {
                        if (keyboardState.IsKeyDown(Keys.E))
                        {
                            boss.bounds.Height = 125;
                            boss.state = "Spawned";
                            bossMessage = false;
                        }
                        else
                        {
                            bossMessage = true;
                        }
                    }
                }
                else
                {
                    player.onBoss = false;
                }
                prev = keyboardState;
            }
        }
        /// <summary>
        /// Displays all assets of the screen
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.MediumPurple);
            if(1 + timeCheck < timer.Elapsed.TotalSeconds)
            {
                timeCheck = timer.Elapsed.TotalSeconds;
                seconds++;
            }
            //Debug.WriteLine(((int)timer.Elapsed.Seconds).ToString("00"));
            if (seconds > 59) { seconds = 0; minutes++; }
            if (minutes > 59) { minutes = 0; hours++; }
            string time = $"{hours.ToString("00")}:{minutes.ToString("00")}:{seconds.ToString("00")}";
            _spriteBatch.Begin();
            //_tileMap.Draw(gameTime, _spriteBatch);
            _spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            door.Draw(gameTime, _spriteBatch);
            foreach (PlatformBox p in platforms) p.Draw(gameTime, _spriteBatch);
            foreach (PlatformBox l in ladders) l.Draw(gameTime, _spriteBatch);
            boss.Draw(gameTime, _spriteBatch);
            booster.Draw(gameTime, _spriteBatch);
            boss.bossAttack.Draw(gameTime, _spriteBatch);
            /*if (!boss.bossAttack.nuke.active && boss.attackState != "Nuke")
            {
                booster.Draw(gameTime, _spriteBatch);
            }*/
            player.Draw(gameTime, _spriteBatch);
            foreach (Key k in keys)
            {
                k.Draw(gameTime, _spriteBatch);
            }
            //_spriteBatch.Draw(fireColumn, new Vector2(20, 0), Color.White * 0.6f);
            _spriteBatch.Draw(recText, rec, Color.White * 0.5f);
            _spriteBatch.DrawString(spriteFont, "Keys: ", new Vector2(15, 2), Color.White, 0, new Vector2(0, 0), .25f, SpriteEffects.None, 0);
            foreach (Life l in player.lives) l.Draw(gameTime, _spriteBatch);

            if (keysCollected == 0)
            {
                _spriteBatch.DrawString(spriteFont, "Press E on the keys to pick them up\nCollect all four to wake up Steven", new Vector2((_graphics.GraphicsDevice.Viewport.Width / 2) - 135, (_graphics.GraphicsDevice.Viewport.Height / 2) - 20), Color.White, 0, new Vector2(0, 0), .333f, SpriteEffects.None, 0);
            }
            if (bossMessage == true)
            {
                _spriteBatch.DrawString(spriteFont, "Press E on the boss to wake him up", new Vector2((_graphics.GraphicsDevice.Viewport.Width / 2) - 135, (_graphics.GraphicsDevice.Viewport.Height / 2) - 20), Color.White, 0, new Vector2(0, 0), .333f, SpriteEffects.None, 0);
            }
            //erase later
            //keysCollected = 4;
            if (keysCollected == 4 && !boss.Spawned)
            {
                boss.state = "Down";
                boss.Spawned = true;
            }
            if (boss.KC > 0 && !nextLevel)
            {
                timer.Stop();
                door.opened = true;
                _spriteBatch.DrawString(spriteFont, "Door Unlocked!", new Vector2((_graphics.GraphicsDevice.Viewport.Width / 2) - 170, (_graphics.GraphicsDevice.Viewport.Height / 2) - 96), Color.White);
                _spriteBatch.DrawString(spriteFont, "Press E on the door to leave", new Vector2((_graphics.GraphicsDevice.Viewport.Width / 2) - 98, (_graphics.GraphicsDevice.Viewport.Height / 2) - 32), Color.White, 0, new Vector2(0, 0), .333f, SpriteEffects.None, 0);
                //_spriteBatch.DrawString(spriteFont, boss.timer.ToString(), new Vector2((_graphics.GraphicsDevice.Viewport.Width / 2) - 55, 20), Color.Yellow, 0, new Vector2(0, 0), .5f, SpriteEffects.None, 0);
            }
            if (alpha > 0)
            {
                alpha -= .01f;
                _spriteBatch.Draw(fadeToBlack, fadeRec, Color.Black * alpha);
            }
            else if(alpha <= .2f && !timerStarted)
            {
                timerStarted = true;
                timer.Start();
            }
            if (player.numLives == 0 || nextLevel)
            {
                if (alpha < 1f)
                {
                    alpha += .02f;
                }
                _spriteBatch.Draw(fadeToBlack, fadeRec, Color.Black * alpha);
                if (nextLevel)
                {
                    _spriteBatch.DrawString(spriteFont, "You Win!", new Vector2((_graphics.GraphicsDevice.Viewport.Width / 2) - 155, (_graphics.GraphicsDevice.Viewport.Height / 2) - 96), Color.Gold);
                    _spriteBatch.DrawString(spriteFont, time, new Vector2((_graphics.GraphicsDevice.Viewport.Width / 2) - 75, (_graphics.GraphicsDevice.Viewport.Height / 2) - 32), Color.White, 0, new Vector2(0, 0), .5f, SpriteEffects.None, 0);
                    _spriteBatch.DrawString(spriteFont, "Press Esc to exit", new Vector2((_graphics.GraphicsDevice.Viewport.Width / 2) - 93, (_graphics.GraphicsDevice.Viewport.Height / 2) + 5), Color.Gold, 0, new Vector2(0, 0), .333f, SpriteEffects.None, 0);
                    player.inv = false;
                }
                else
                {
                    _spriteBatch.DrawString(spriteFont, "You Lose!", new Vector2((_graphics.GraphicsDevice.Viewport.Width / 2) - 155, (_graphics.GraphicsDevice.Viewport.Height / 2) - 96), Color.IndianRed);
                    _spriteBatch.DrawString(spriteFont, "Press Esc to exit and retry", new Vector2((_graphics.GraphicsDevice.Viewport.Width / 2) - 124, (_graphics.GraphicsDevice.Viewport.Height / 2) - 32), Color.IndianRed, 0, new Vector2(0, 0), .333f, SpriteEffects.None, 0);
                }
            }
            else
            {
                _spriteBatch.DrawString(spriteFont, time, new Vector2((_graphics.GraphicsDevice.Viewport.Width / 2) - 55, 20), Color.Yellow, 0, new Vector2(0, 0), .5f, SpriteEffects.None, 0);
            }
            _spriteBatch.End();
        }
    }
}
