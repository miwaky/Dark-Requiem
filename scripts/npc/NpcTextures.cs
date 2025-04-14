using Raylib_cs;
using static Raylib_cs.Raylib;

namespace DarkRequiem.npc
{
    public static class NpcTextures
    {
        // Dictionnaire centralisé contenant les textures des différentes catégories de NPCs
        public static Dictionary<string, Texture2D> CreatureTextures = new Dictionary<string, Texture2D>();

        // Chargement unique des textures au démarrage du jeu
        public static void LoadAll()
        {
            CreatureTextures["monster"] = LoadTexture("assets/images/npc/monster.png");
        }

        public static Texture2D GetTexture(string key)
        {
            return CreatureTextures[key];
        }

        // Nettoyage propre des textures à la fermeture du jeu
        public static void UnloadAll()
        {
            foreach (var texture in CreatureTextures.Values)
            {
                UnloadTexture(texture);
            }
        }
    }
}
