using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;
using DarkRequiem.manager;
using DarkRequiem.npc;
using DarkRequiem.map;

namespace DarkRequiem.map
{
    public class RenduMap
    {
        private Map _map;
        private Texture2D CreatureTexture;

        private Dictionary<int, List<Rectangle>> _rectanglesTilesets; // int=firstgid => Liste de rectangles
        private Dictionary<int, Texture2D> _texturesTilesets; // int=firstgid => La texture du tileset dont le firstgid est passé en clé
        private Dictionary<int, Tileset> _tuileTileset; // int=id de la tuile => Le tileset dont est extrait la tuile

        public RenduMap(Map map, string assetsPath)
        {
            _map = map;
            CreatureTexture = LoadTexture("assets/images/characters/Creatures.png");
            _rectanglesTilesets = new Dictionary<int, List<Rectangle>>();
            _texturesTilesets = new Dictionary<int, Texture2D>();
            _tuileTileset = new Dictionary<int, Tileset>();
            InitRectanglesTilesets();
            LoadTexturesTilesets(assetsPath);
            AssociateTuileToTilesets();

        }
        private void InitRectanglesTilesets()
        {
            foreach (Tileset tileset in _map.Tilesets)
            {
                List<Rectangle> rectangles = new List<Rectangle>();
                int tailleTuile = tileset.tilewidth;
                int colonnesTileset = tileset.columns;

                for (int i = 0; i < tileset.tilecount; i++)
                {
                    int colonne = i % colonnesTileset;
                    int ligne = i / colonnesTileset;
                    Rectangle rect = new Rectangle(colonne * tailleTuile, ligne * tailleTuile, tailleTuile, tailleTuile);
                    rectangles.Add(rect);
                }
                _rectanglesTilesets[tileset.firstgid] = rectangles;
            }
        }
        private void LoadTexturesTilesets(string assetsPath)
        {
            foreach (Tileset tileset in _map.Tilesets)
            {
                string path = System.IO.Path.Combine(assetsPath, tileset.image);
                Texture2D texture = Raylib.LoadTexture(path);
                _texturesTilesets[tileset.firstgid] = texture;
            }
        }
        private void AssociateTuileToTilesets()
        {
            foreach (Tileset tileset in _map.Tilesets)
            {
                for (int i = 0; i < tileset.tilecount; i++)
                {
                    int idTuile = tileset.firstgid + i;
                    _tuileTileset[idTuile] = tileset;
                }
            }
        }

        public void AfficherMap()
        {
            int tailleTuile = _map.TailleTuile;

            for (int y = 0; y < _map.Hauteur; y++)
            {
                for (int x = 0; x < _map.Largeur; x++)
                {
                    Raylib.DrawRectangle(x * tailleTuile, y * tailleTuile, tailleTuile, tailleTuile, Color.White);

                    foreach (Layer layer in _map.Calques)
                    {
                        if (layer.type != "tilelayer")
                        {
                            continue;
                        }
                        int index = y * _map.Largeur + x;
                        int idTuile = layer.data[index];
                        if (idTuile == 0)
                        {
                            continue;
                        }
                        Tileset tileset = _tuileTileset[idTuile];
                        int firstgid = tileset.firstgid;
                        Texture2D texture = _texturesTilesets[firstgid];
                        int idReel = idTuile - firstgid;
                        Rectangle rect = _rectanglesTilesets[firstgid][idReel];
                        Vector2 position = new Vector2(x * tailleTuile, y * tailleTuile);
                        Raylib.DrawTextureRec(texture, rect, position, Color.White);
                    }
                }
            }
        }

        public void Close()
        {
            foreach (Texture2D texture in _texturesTilesets.Values)
            {
                Raylib.UnloadTexture(texture);
            }
        }

        public void DrawMonsters()
        {
            foreach (Npc npc in GameManager.ActiveNpcs)
            {
                Rectangle sourceRect = npc.GetSpriteRectangle();
                // Console.WriteLine($" Dessin de {npc.Name} avec SpriteID {npc.SpriteID} -> Source Rect {sourceRect}");

                Vector2 position = new Vector2(npc.Colonne * _map.TailleTuile, npc.Ligne * _map.TailleTuile);
                DrawTextureRec(CreatureTexture, sourceRect, position, Color.White);
            }
        }
    }
}