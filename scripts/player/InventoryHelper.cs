using DarkRequiem.player;
using DarkRequiem.objects;

namespace DarkRequiem.manager
{
    public static class InventoryHelper
    {
        public static void AddPotion(ref Player player, int amount = 1)
        {
            player.Inventory.AddItem("potion", "consumable", amount);
            Console.WriteLine($"+{amount} potion(s) → Total : {player.Inventory.GetItemQuantity("potion")}");
        }

        public static void AddMoney(ref Player player, int amount)
        {
            player.Inventory.AddItem("gold", "currency", amount);
            Console.WriteLine($"+{amount} or → Total : {player.Inventory.GetItemQuantity("gold")}");
        }

        public static void AddQuestItem(ref Player player, QuestObject quest)
        {
            player.Inventory.AddItem(quest.Id, "quest", 1);
            Console.WriteLine($"Objet de quête '{quest.Name}' ajouté à l'inventaire.");
        }
    }
}