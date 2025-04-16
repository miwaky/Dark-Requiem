using DarkRequiem.manager;
using DarkRequiem.objects;
using DarkRequiem.player;

namespace DarkRequiem.events
{
    public static class ForestEvent
    {
        public static void InitChestEvents()
        {
            Money money = Money.GenerateMoney(10, 5);
            Chest coffre = new Chest(1, 10, 5, "forest", money);
            GameManager.ActiveChests.Add(coffre);
        }
    }
}
