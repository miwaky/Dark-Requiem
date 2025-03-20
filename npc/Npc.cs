using Raylib_cs;
using System.Collections.Generic;

namespace DarkRequiem.npc
{
    public class Npc : CharacterManager
    {
        public int Id { get; private set; }

        // Ces données seront définies lors de la création d'un NPC dans GameManager
        public string MapName { get; set; } = "undefined";
        public int Colonne { get; set; }
        public int Ligne { get; set; }
        public int SpriteID { get; private set; }
        public bool HasFought { get; set; } = false;
        public string TextureType { get; private set; }

        public Npc(int id, int spriteID, string name, string type, int maxHP, int hp, int attack, int defense, string textureType)
            : base(name, type, maxHP, hp, attack, defense)
        {
            Id = id;
            SpriteID = spriteID;
            TextureType = textureType;
        }

        // Définition claire du dictionnaire NPC avec SpriteID correct
        public static Dictionary<int, Npc> GetNpcDictionary()
        {
            return new Dictionary<int, Npc>
            {
                // Exemple : si le sprite du slime est situé en ligne 4, colonne 2 dans ta grille
                // SpriteID = (ligne * nombre_colonnes) + colonne => (4 * 6) + 2 = 26
                //G2 
                {1,new Npc(1, 7, "Slime", "ennemy", 2000, 2000, 1, 1, "slime"){MapName = "forest",Colonne = 25,Ligne = 7}},
                {2,new Npc(1, 7, "Slime", "ennemy", 2000, 2000, 1, 1, "slime"){MapName = "forest",Colonne = 22,Ligne = 7}}
            };
        }

        // Retourne le rectangle correct du sprite dans une grille de 6 colonnes
        public Rectangle GetSpriteRectangle()
        {

            int spriteWidth = 32;  // Largeur du sprite
            int spriteHeight = 32; // Hauteur d'un sprite
            int spritesPerRow = 6;  // Nombre de colonnes dans la spritesheet

            int colonne = SpriteID % spritesPerRow;  // colonne correcte
            int ligne = SpriteID / spritesPerRow;    // ligne correcte

            return new Rectangle(colonne * spriteWidth, ligne * spriteHeight, spriteWidth, spriteHeight);
        }
    }
}
