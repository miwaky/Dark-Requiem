using Raylib_cs;
using System.Collections.Generic;

namespace DarkRequiem.npc
{
    public class Npc : Character
    {
        public int Id { get; private set; }
        public string MapName { get; set; } = "undefined";
        public int Colonne { get; set; }
        public int Ligne { get; set; }
        public int SpriteID { get; private set; }
        public bool HasFought { get; set; } = false;
        public string TextureType { get; private set; }
        public int AggroRange { get; private set; }

        public Npc(int id, int spriteID, string name, string type, int maxHP, int hp, int attack, int defense, string textureType, int aggroRange)
                : base(name, type, maxHP, hp, attack, defense)
        {
            Id = id;
            SpriteID = spriteID;
            TextureType = textureType;
            AggroRange = aggroRange;
        }

    }
    public static class NpcData
    {
        // Dictionnaire des stats de mobs
        public static readonly Dictionary<string, NpcStats> StatsParType = new()
{
    { "slime", new NpcStats(1, 1, 1, 0, "monster", 3) },
    { "mushroom", new NpcStats(2, 2, 2, 1, "monster", 5) },

};
        //Dictionnaire des sprites (ID) des monstres : 
        //0 : Slime 
        //20 : Mushroom
        public static Dictionary<int, string> TypeParSpriteID = new()
{
    { 0, "slime" },
    { 20, "mushroom" },

};
    }

    public class NpcStats
    {
        public int MaxHp { get; }
        public int Hp { get; }
        public int Attack { get; }
        public int Defense { get; }
        public string TextureType { get; }
        public int AggroRange { get; }

        public NpcStats(int maxHp, int hp, int attack, int defense, string textureType, int aggroRange)
        {
            MaxHp = maxHp;
            Hp = hp;
            Attack = attack;
            Defense = defense;
            TextureType = textureType;
            AggroRange = aggroRange;

        }
    }
}
