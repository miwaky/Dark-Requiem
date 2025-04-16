using DarkRequiem.objects;

public class Inventory
{
    public int Gold { get; set; } = 0;
    public int Potions { get; set; } = 0;
    public int KeyDungeon { get; set; } = 0;
    public List<QuestObject> QuestItems { get; set; } = new();

    public Inventory(int gold = 0, int potions = 0, int keyDungeon = 0)
    {
        Gold = gold;
        Potions = potions;
        KeyDungeon = keyDungeon;
    }

    public void AddGold(int amount)
    {
        Gold += amount;
    }

    public void AddPotion(int amount)
    {
        Potions += amount;
    }

    public void AddQuestItem(QuestObject item)
    {
        QuestItems.Add(item);
    }

    public bool HasQuestItem(string itemId)
    {
        return QuestItems.Any(q => q.Id == itemId);
    }
}