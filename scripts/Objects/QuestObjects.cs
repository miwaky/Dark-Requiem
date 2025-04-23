namespace DarkRequiem.objects
{
    public class QuestObject : Objects
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public QuestObject(string id, string name)
            : base(0, name, "quest", 0) // 0 = default id, "quest" = type, spriteId = 0
        {
            Id = id;
            Name = name;
        }

        public static QuestObject TutoKey()
        {
            return new QuestObject("Little Sphere", "Sphere");
        }
        public static QuestObject BossKey()
        {
            return new QuestObject("Boss Key", "Big Key");
        }
        public static QuestObject DungeonKey()
        {
            return new QuestObject("strange sphere", "Strange Sphere");
        }

        public static QuestObject Sphere()
        {
            return new QuestObject("Sphere", "Green Sphere");
        }

        public static QuestObject Key()
        {
            return new QuestObject("key", "Key");
        }
    }
}
