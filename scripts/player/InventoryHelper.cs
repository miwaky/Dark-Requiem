using DarkRequiem.player;
using DarkRequiem.objects;

namespace DarkRequiem.manager
{
    public static class InventoryHelper
    {
        public static void AddPotion(ref Player player, int amount = 1)
        {
            player.Inventory.AddItem("potion", "consumable", amount);
            NotificationManager.Add($"+{amount} Potion");
        }

        public static void AddMoney(ref Player player, int amount)
        {
            player.Inventory.AddItem("gold", "currency", amount);
            int total = player.Inventory.GetItemQuantity("gold");
            NotificationManager.Add($"+{amount} or (Total: {total})");
        }

        public static void AddQuestItem(ref Player player, QuestObject quest)
        {
            player.Inventory.AddItem(quest.Id, "quest", 1);
            NotificationManager.Add($"Objet de quête obtenu : {quest.Name}");
        }
    }
}