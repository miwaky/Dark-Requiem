using Raylib_cs;

namespace DarkRequiem.npc
{
    public class Npc : CharacterManager
    {
        public int Id { get; private set; }

        // Ces données seront définies lors de la création d'un monstre dans GameManager
        public string MapName { get; set; } = "undefined";
        public int Colonne { get; set; }
        public int Ligne { get; set; }
        public int SpriteID { get; private set; }

        public Npc(int id, int spriteID, string name, string type, int hp, int attack, int defense)
       : base(name, type, hp, attack, defense)
        {
            Id = id;
            SpriteID = spriteID;  //  Assigne le SpriteID correctement
        }

        // Dictionnaire des types de NPCs
        public static Dictionary<int, Npc> GetNpcDictionary()
        {
            return new Dictionary<int, Npc>
    {   //Id du dico (id du npc, id du sprite, nom du perso, type, vie, attaque, defense, nom de la map où il apparait, colonne, ligne)
        { 1, new Npc(1, 37, "King", "allie", 200, 25, 10) { MapName = "map_village", Colonne = 5, Ligne = 5 } },
        { 2, new Npc(2, 0, "Knight", "ennemy", 50, 5, 8) { MapName = "map_village", Colonne = 10, Ligne = 5 } },
        { 3, new Npc(3, 6, "Goblin", "ennemy", 90, 15, 5) { MapName = "map_village_basement", Colonne = 5, Ligne = 5 } }
    };
        }

        public Rectangle GetSpriteRectangle()
        {
            int spriteWidth = 16;  // Largeur d'un sprite
            int spriteHeight = 16; // Hauteur d'un sprite
            int spritesPerRow = 9; // Nombre de colonnes dans Creatures.png

            int colonne = SpriteID % spritesPerRow;  //  Trouve la colonne correcte
            int ligne = SpriteID / spritesPerRow;    //  Trouve la ligne correcte

            //Console.WriteLine($" Sprite pour {Name} (SpriteID {SpriteID}) : Ligne {ligne}, Colonne {colonne}");

            return new Rectangle(colonne * spriteWidth, ligne * spriteHeight, spriteWidth, spriteHeight);
        }


    }
}