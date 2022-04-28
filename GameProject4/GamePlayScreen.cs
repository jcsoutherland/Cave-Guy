using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace GameProject4
{
    /// <summary>
    /// A class representing the games gameplay screen
    /// </summary>
    public class GamePlayScreen
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private PumpkinSprite pumpkin;
        SpriteFont spriteFont;
        private int coinsLeft;
        private int score = 0;

        int currCoin = 0;

        private Tilemap _tileMap;
        private SoundEffect coinPickup;
        private Song gameMusic;
        public bool exit;

        List<CoinSprite> coins = new List<CoinSprite>();

        /// <summary>
        /// Constructor for the gameplay screen
        /// </summary>
        /// <param name="g"></param>
        public GamePlayScreen(GraphicsDeviceManager g)
        {
            _graphics = g;
            System.Random rand = new System.Random();
            pumpkin = new PumpkinSprite(new Vector2(50, _graphics.GraphicsDevice.Viewport.Height - 188));
            _tileMap = new Tilemap("map.txt");
            SpawnCoins();
        }

        public void SpawnCoins()
        {
            coins.Clear();
            System.Random rand = new System.Random();
            for (int i = 0; i < 10; i++)
            {
                int varNum = 50;
                float number = (float)rand.NextDouble() * varNum;
                float rHeight = (float)Math.Round(rand.NextDouble());
                //int number = rand.Next(1, 2);
                if (number > 50 - number)
                {
                    coins.Add(new CoinSprite(new Vector2(_graphics.GraphicsDevice.Viewport.Width, _graphics.GraphicsDevice.Viewport.Height - 165 - (rHeight*100)), "good"));
                    varNum -= 10;
                }
                else
                {
                    //bad coin
                    coins.Add(new CoinSprite(new Vector2(_graphics.GraphicsDevice.Viewport.Width, _graphics.GraphicsDevice.Viewport.Height - 165 - (rHeight * 100)), "bad"));
                    varNum += 10;
                }
            }
            coinsLeft = coins.Count;
        }

        /// <summary>
        /// Loads content from ContentManager
        /// </summary>
        /// <param name="Content"></param>
        public void LoadContent(ContentManager Content)
        {
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
            foreach (var coin in coins) coin.LoadContent(Content);
            pumpkin.LoadContent(Content);
            gameMusic = Content.Load<Song>("DeeYan-Key-TheGame");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(gameMusic);
            MediaPlayer.Volume = .02f;
            coinPickup = Content.Load<SoundEffect>("Pickup_Coin15");
            SoundEffect.MasterVolume = .02f;
            spriteFont = Content.Load<SpriteFont>("arial");
            _tileMap.LoadContent(Content);
        }
        /// <summary>
        /// Game Loop updating screen every tick
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                exit = true;

            pumpkin.Update(gameTime);
            foreach (var coin in coins)
            {
                coin.Update(gameTime, _graphics);
                if (coin == coins[currCoin] && !coin.Collected)
                {
                    coin.MovementVector = new Vector2(2, 0);
                }
                if (!coin.Collected && coin.position.X <= 0)
                {
                    coin.Collected = true;
                    coinsLeft--;
                    if (coinsLeft != 0)
                    {
                        currCoin++;
                    }
                }
                if (!coin.Collected && coin.Bounds.CollidesWith(pumpkin.Bounds))
                {
                    if(coin.type == "good")
                    {
                        coinPickup.Play();
                        score++;
                    }
                    else
                    {
                        score--;
                    }
                    coin.Collected = true;
                    coinsLeft--;
                    if (coinsLeft != 0)
                    {
                        currCoin++;
                    }
                }
            }
        }
        /// <summary>
        /// Displays all assets of the screen
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.MediumPurple);

            _spriteBatch.Begin();
            _tileMap.Draw(gameTime, _spriteBatch);
            if (coinsLeft == 0)
            {
                _spriteBatch.DrawString(spriteFont, "You Win!", new Vector2((_graphics.GraphicsDevice.Viewport.Width / 2) - 110, (_graphics.GraphicsDevice.Viewport.Height / 2) - 96), Color.White);
                _spriteBatch.DrawString(spriteFont, "Score: " + score.ToString(), new Vector2((_graphics.GraphicsDevice.Viewport.Width / 2) - 100, (_graphics.GraphicsDevice.Viewport.Height / 2) - 32), Color.White);
                _spriteBatch.DrawString(spriteFont, "Press Esc or Back to Exit", new Vector2((_graphics.GraphicsDevice.Viewport.Width / 2) - 98, (_graphics.GraphicsDevice.Viewport.Height / 2) + 64), Color.White, 0, new Vector2(0, 0), .333f, SpriteEffects.None, 0);
            }
            else
            {
                foreach (var coin in coins) coin.Draw(gameTime, _spriteBatch);
                pumpkin.Draw(gameTime, _spriteBatch);
            }
            _spriteBatch.End();
        }
    }
}
