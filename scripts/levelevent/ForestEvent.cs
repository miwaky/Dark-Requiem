using DarkRequiem.manager;
using DarkRequiem.map;
using DarkRequiem.objects;
using DarkRequiem.player;

namespace DarkRequiem.events
{
    public static class ForestEvent
    {
        public static void InitChestEvents(Player player, MapInfo map)
        {
            // Coffre avec de l'argent
            Money money = Money.GenerateMoney(21, 0);
            Chest coffre1 = new Chest(1, 21, 0, "forest", money);
            GameManager.ActiveChests.Add(coffre1);

            //  Coffre avec une potion
            Potion potion = Potion.GenerateHeal(52, 4);
            Chest coffre2 = new Chest(2, 52, 4, "forest", potion);
            GameManager.ActiveChests.Add(coffre2);

            //  pour ouvrir le passage
            var keyTuto = QuestObject.TutoKey();
            var coffre3 = new Chest(3, 41, 3, "forest", keyTuto);
            GameManager.ActiveChests.Add(coffre3);

            //  pour ouvrir le donjon
            var keyDonjon = QuestObject.DungeonKey();
            var coffre4 = new Chest(4, 19, 27, "forest", keyDonjon);
            GameManager.ActiveChests.Add(coffre4);

            //  Switch : 

            //Switch de départ (zone 4,0)
            //  Condition : le joueur doit avoir keyTuto
            //  Les actions à exécuter si le joueur a la clé
            var SwitchTuto = new CompositeCommand(new List<IEventCommand>
{
    new RemoveZoneCommand(map, 35, 5, 43, 10, "SwitchObstacle"),
    new RemoveTileCommand(map, 41, 6, "Switch")
});

            // La commande conditionnelle
            var requireKeyTuto = new RequireItemCommand(
                player,
                p => p.Inventory.HasQuestItem("tuto_key"),
                SwitchTuto
            );

            //  Le switch à (41,6)
            var TutoSwitchCondition = new Switch(41, 6, "forest", requireKeyTuto);
            GameManager.ActiveSwitches.Add(TutoSwitchCondition);


            //Switch deblocage donjon (zone 2,2)
            //  Condition : le joueur doit avoir dungeon_key
            //  Les actions à exécuter si le joueur a la clé
            var SwitchDungeon = new CompositeCommand(new List<IEventCommand>
{
    new RemoveZoneCommand(map, 27, 24, 31, 25, "SwitchObstacle"),
    new RemoveTileCommand(map, 41, 6, "Switch")
});

            // La commande conditionnelle
            var requireDungeonKey = new RequireItemCommand(
                player,
                p => p.Inventory.HasQuestItem("dungeon_key"),
                SwitchDungeon
            );

            //  Le switch à (41,6)
            var DungeonEnterSwitchCondition = new Switch(41, 6, "forest", requireDungeonKey);
            GameManager.ActiveSwitches.Add(DungeonEnterSwitchCondition);
        }

    }

}

