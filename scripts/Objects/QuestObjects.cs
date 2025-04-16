namespace DarkRequiem.objects
{
    public class QuestObject : Objects
    {
        public string Id { get; private set; }
        public string Name { get; private set; }


        public QuestObject(string id, string name)
            : base(0, name, "quest", 0) // ID = 0, type = quest, spriteId = 0 (à adapter si besoin)
        {

            Id = id;
            Name = name;
        }



        public static QuestObject TutoKey()
        {
            return new QuestObject("tuto_key", "Sphère Rouge");
        }

        public static QuestObject DungeonKey()
        {
            return new QuestObject("dungeon_key", "Clé du Donjon");
        }
    }
}