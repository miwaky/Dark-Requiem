using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace DarkRequiem.objects
{
    public class Objects
    {
        public int id { get; private set; }
        public string name { get; private set; }
        public string type { get; private set; }
        public int spriteId { get; private set; }
        public int colonne { get; set; }
        public int ligne { get; set; }
        public Texture2D SpriteObjects;

        public Objects(int pId, string pName, string pType, int pSpriteId)
        {
            id = pId;
            name = pName;
            type = pType;
            spriteId = pSpriteId;
            SpriteObjects = LoadTexture("assets/images/objects/objects.png");
        }
        public void Draw(int tileSize, int colonne, int ligne)
        {
            int spriteSize = 16;
            int columns = SpriteObjects.Width / spriteSize;

            int frameX = spriteId % columns;
            int frameY = spriteId / columns;

            Rectangle source = new Rectangle(frameX * spriteSize, frameY * spriteSize, spriteSize, spriteSize);
            Rectangle destination = new Rectangle(colonne * tileSize, ligne * tileSize, spriteSize, spriteSize);

            Raylib.DrawTexturePro(SpriteObjects, source, destination, Vector2.Zero, 0f, Color.White);
        }

    }
}