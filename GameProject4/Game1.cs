using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject4
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private MenuScreen menu;
        private GamePlayScreen game;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            menu = new MenuScreen(_graphics);
            game = new GamePlayScreen(_graphics);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            menu.LoadContent(Content);
            game.LoadContent(Content);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (menu.play)
            {
                game.Update(gameTime);
            }
            else
            {
                game.exit = false;
                menu.play = false;
                menu.Update(gameTime);
            }
            if (menu.exit)
            {
                Exit();
            }
            if (game.exit)
            {
                //go back to menu and restart
                Initialize();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            if (menu.play)
            {
                game.Draw(gameTime);
            }
            else
            {
                game.exit = false;
                menu.play = false;
                menu.Draw(gameTime);
            }
        }
    }
}
