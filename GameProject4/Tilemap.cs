﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Windows.Forms;

namespace GameProject4
{
    public class Tilemap
    {
        int _tileWidth, _tileHeight, _mapWidth, _mapHeight;
        Texture2D _tilesetTexture;
        Rectangle[] _tiles;
        int[] _map;
        string _filename;

        public Tilemap(string filename)
        {
            _filename = filename;
        }

        public void LoadContent(ContentManager content)
        {
            string data = File.ReadAllText(Path.Join(content.RootDirectory, "map.txt"));
            var lines = data.Split('\n');

            var tilesetFilename = lines[0].Trim();
            _tilesetTexture = content.Load<Texture2D>(tilesetFilename);

            var secondLine = lines[1].Split(',');
            _tileWidth = int.Parse(secondLine[0]);
            _tileHeight = int.Parse(secondLine[1]);

            int tilesetColumns = _tilesetTexture.Width / _tileWidth;
            int tilesetRows = _tilesetTexture.Height / _tileHeight;

            _tiles = new Rectangle[tilesetColumns * tilesetRows];


            for (int y = 0; y < tilesetColumns; y++)
            {
                for (int x = 0; x < tilesetRows; x++)
                {
                    int index = y * tilesetColumns + x;
                    _tiles[index] = new Rectangle(x * _tileWidth, y * _tileHeight, _tileWidth, _tileHeight);
                }
            }

            var thirdLine = lines[2].Split(',');
            _mapWidth = int.Parse(thirdLine[0]);
            _mapHeight = int.Parse(thirdLine[1]);

            var fourthLine = lines[3].Split(',');
            _map = new int[_mapWidth * _mapHeight];
            //MessageBox.Show(_map.Length.ToString());
            for (int i = 0; i < _mapWidth * _mapHeight; i++)
            {
                /*if(i > 188)
                {
                    MessageBox.Show(i.ToString());
                }*/
                _map[i] = int.Parse(fourthLine[i]);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    int index = _map[y * _mapWidth + x] - 1;
                    if (index == -1) continue;
                    spriteBatch.Draw(_tilesetTexture, new Vector2(x * _tileWidth, y * _tileHeight), _tiles[index],  Color.White);
                }
            }
        }
    }
}
