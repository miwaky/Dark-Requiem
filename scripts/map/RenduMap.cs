using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;
using DarkRequiem.npc;
using DarkRequiem.objects;
using DarkRequiem.manager;

namespace DarkRequiem.map
{
    public class RenduMap
    {
        public MapInfo _map;

        private Dictionary<int, List<Rectangle>> _rectanglesTilesets; // int=firstgid => Liste de rectangles
        private Dictionary<int, Texture2D> _texturesTilesets; // int=firstgid => La texture du tileset dont le firstgid est passé en clé
        private Dictionary<int, Tileset> _tuileTileset; // int=id de la tuile => Le tileset dont est extrait la tuile

        public RenduMap(MapInfo map, string assetsPath)
        {
            _map = map;
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
                        if (_tuileTileset.ContainsKey(idTuile))
                        {
                            Tileset tileset = _tuileTileset[idTuile];
                            int firstgid = tileset.firstgid;
                            Texture2D texture = _texturesTilesets[firstgid];
                            int idReel = idTuile - firstgid;

                            if (idReel >= 0 && idReel < _rectanglesTilesets[firstgid].Count)
                            {
                                Rectangle rect = _rectanglesTilesets[firstgid][idReel];
                                Vector2 position = new Vector2(x * tailleTuile, y * tailleTuile);
                                Raylib.DrawTextureRec(texture, rect, position, Color.White);
                            }
                            else
                            {
                                //Console.WriteLine($"[AVERTISSEMENT] ID Reel {idReel} hors limite pour tileset {tileset.image}");
                            }
                        }
                        else
                        {
                            //Console.WriteLine($"[AVERTISSEMENT] Tuile inconnue : ID {idTuile}, position ({x},{y}) dans layer {layer.name}");
                        }
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
        public void DrawNpcs(List<Npc> npcs)
        {
            int spriteSize = 48;
            int columns = 4;
            int scaleSize = 24;

            foreach (var npc in npcs)
            {
                Texture2D texture = NpcTextures.GetTexture(npc.TextureType);

                int frameX = npc.SpriteID % columns;
                int frameY = npc.SpriteID / columns;

                Rectangle sourceRect = new Rectangle(
                    frameX * spriteSize,
                    frameY * spriteSize,
                    spriteSize,
                    spriteSize
                );

                // Centrage : décalage pour garder le monstre bien placé dans la tuile
                Vector2 destPosition = new Vector2(
                    npc.Colonne * 16 + 8 - scaleSize / 2,
                    npc.Ligne * 16 + 8 - scaleSize / 2 - 4 // decalage des pixel pour centrage du monstre.
                );

                Rectangle destRect = new Rectangle(
                    destPosition.X,
                    destPosition.Y,
                    scaleSize,
                    scaleSize
                );

                DrawTexturePro(
                    texture,
                    sourceRect,
                    destRect,
                    Vector2.Zero,
                    0f,
                    Color.White
                );

                // affiche l'ID pour debug
                // DrawText(npc.SpriteID.ToString(), (int)destRect.X, (int)destRect.Y, 10, Color.White);
            }
        }

        public void DrawObjects(List<Objects> objets)
        {
            int spriteSize = 16; // adapte selon ton atlas
            int columns = 5;     // 5 sprites dans ton image objects.png
            int tileSize = _map.TailleTuile;

            foreach (var obj in objets)
            {
                Texture2D texture = obj.SpriteObjects;

                int frameX = obj.spriteId % columns;
                int frameY = obj.spriteId / columns;

                Rectangle sourceRect = new Rectangle(frameX * spriteSize, frameY * spriteSize, spriteSize, spriteSize);
                Rectangle destRect = new Rectangle(obj.colonne * tileSize, obj.ligne * tileSize, tileSize, tileSize);

                DrawTexturePro(texture, sourceRect, destRect, Vector2.Zero, 0f, Color.White);
            }
        }
        public void DrawChests(List<Chest> coffres)
        {
            string currentMap = JsonManager.CurrentMap.NomCarte.ToLower();

            foreach (var chest in coffres)
            {
                if (chest.MapName.ToLower() != currentMap) continue;

                Texture2D texture = chest.IsOpened ? Chest.TextureOpen : Chest.TextureClosed;

                DrawTexture(
                    texture,
                    chest.Colonne * 16,
                    chest.Ligne * 16,
                    Color.White
                );
            }
        }



    }
}