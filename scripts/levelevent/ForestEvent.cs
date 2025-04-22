using DarkRequiem.manager;
using DarkRequiem.map;
using DarkRequiem.objects;
using DarkRequiem.player;
using System.Collections.Generic;
namespace DarkRequiem.events
{
    public static class ForestEvent
    {
        public static void InitEvents(Player player, MapInfo map)
        {
            InitChestEvents(player, map);
            InitSwitchNeedKey(player, map);
        }


        public static void InitChestEvents(Player player, MapInfo map)
        {
            var chests = new List<IEventCommand>
    {
        //  Coffre avec de l'argent
        new CreateInventoryChestCommand(101, 21, 0, "forest", Money.GenerateMoney(21, 0)),

        //  Coffre avec une potion
        new CreateInventoryChestCommand(102, 52, 4, "forest", Potion.GenerateHeal(52, 4)),

        //  Coffre avec une clé pour ouvrir un passage
        new CreateInventoryChestCommand(103, 41, 3, "forest", QuestObject.TutoKey()),

        // Coffre avec clé pour ouvrir le donjon
        new CreateInventoryChestCommand(104, 19, 27, "forest", QuestObject.DungeonKey())
    };

            foreach (var cmd in chests)
            {
                cmd.Execute(); //  Exécution directe des commandes de création
            }
        }
        public static void InitSwitchNeedKey(Player player, MapInfo map)
        {

            //Switch : 

            // Switch de départ(zone 4, 0)
            //   Condition: le joueur doit avoir keyTuto
            //   Les actions à exécuter si le joueur a la clé
            var SwitchTuto = new CompositeCommand(new List<IEventCommand>
            {
         new RemoveZoneCommand(map, 35, 5, 43, 10, "SwitchObstacle"),
          new RemoveTileCommand(map, 41, 6, "Switch")
            });

            // La commande conditionnelle
            var requireKeyTuto = new RequireItemCommand(
                player,
                p => p.Inventory.HasItem("Little Sphere"),
                SwitchTuto
            );

            //Le switch à(41, 6)
            var TutoSwitchCondition = new Switch(41, 6, "forest", requireKeyTuto);
            GameManager.ActiveSwitches.Add(TutoSwitchCondition);


            // Switch deblocage donjon(zone 2, 2)
            // Condition: le joueur doit avoir dungeon_key
            // Les actions à exécuter si le joueur a la clé
            var SwitchDungeon = new CompositeCommand(new List<IEventCommand>
            {
            //Switch pour ouvrir le donjon
            new RemoveZoneCommand(map, 27, 24, 31, 25, "SwitchObstacle"),
            new RemoveTileCommand(map, 19, 24, "Switch")
            });

            // La commande conditionnelle
            var requireDungeonKey = new RequireItemCommand(
                player,
                p => p.Inventory.HasItem("strange sphere"),
                SwitchDungeon
            );

            // Le switch à(19, 24)
            var DungeonEnterSwitchCondition = new Switch(19, 24, "forest", requireDungeonKey);
            GameManager.ActiveSwitches.Add(DungeonEnterSwitchCondition);
        }


    }
}



