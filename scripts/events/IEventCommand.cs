using DarkRequiem.map;
using DarkRequiem.manager;
using DarkRequiem.npc;
using DarkRequiem.interact;
using DarkRequiem.player;
public interface IEventCommand
{
    void Execute();
}


namespace DarkRequiem.events
{

    public static class EventConditions
    {
        public static bool IsSwitchActivated(int col, int row, string map)
        {
            return GameManager.ActiveSwitches.Any(sw =>
                sw.Colonne == col && sw.Ligne == row &&
                sw.MapName.ToLower() == map.ToLower() &&
                sw.IsActivated);
        }

        public static bool IsChestOpened(int chestId)
        {
            return GameManager.ActiveChests.Any(chest =>
                chest.Id == chestId && chest.IsOpened);
        }

        public static bool HasPlayerPotion(Player player, int minAmount = 1)
        {
            return player.Inventory.Potions >= minAmount;
        }

        public static bool IsObjectAt(int col, int row, string map)
        {
            return GameManager.ActiveObjects.Any(obj =>
                obj.colonne == col && obj.ligne == row &&
                map.ToLower() == JsonManager.CurrentMap.NomCarte.ToLower());
        }
    }
    //Jouer un son : 
    //var cmd = new PlaySoundCommand("open");
    //cmd.Execute();
    public class PlaySoundCommand : IEventCommand
    {
        private readonly string soundKey;

        public PlaySoundCommand(string soundKey)
        {
            this.soundKey = soundKey;
        }

        public void Execute()
        {
            AudioManager.Play(soundKey);
            Console.WriteLine($"[Command] Son joué : {soundKey}");
        }
    }

    //A besoin de l'item pour fonctionner : 
    //     var condition = new RequireItemCommand(
    //     heros, // ou _player
    //     (p) => p.PotionInventory >= 1, // condition
    //     new RemoveTileCommand(map, 5, 5, "Breakable") // action à exécuter si vrai
    // );

    // condition.Execute(); // Appelle la commande seulement si la condition est remplie
    public class RequireItemCommand : IEventCommand
    {
        private readonly Player player;
        private readonly Func<Player, bool> condition;
        private readonly IEventCommand command;

        public RequireItemCommand(Player player, Func<Player, bool> condition, IEventCommand command)
        {
            this.player = player;
            this.condition = condition;
            this.command = command;
        }

        public void Execute()
        {
            if (!condition(player))
            {
                Console.WriteLine("[Command] Condition non remplie, rien ne se passe.");
                AudioManager.Play("denied");

                // Ne marque pas le switch comme activé si la condition échoue
                return;
            }

            command.Execute();
            AudioManager.Play("switch");
        }
    }

    //Supprimer une seule tuile : 
    //exemple : 
    //var cmd = new RemoveZoneCommand(map, 27, 24, 31, 25, "Breakable");
    //cmd.Execute();

    public class RemoveZoneCommand : IEventCommand
    {
        private readonly MapInfo map;
        private readonly int startX, startY, endX, endY;
        private readonly string layer;

        public RemoveZoneCommand(MapInfo map, int startX, int startY, int endX, int endY, string layer)
        {
            this.map = map;
            this.startX = startX;
            this.startY = startY;
            this.endX = endX;
            this.endY = endY;
            this.layer = layer;
        }

        public void Execute()
        {
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    map.SetTuileToZero(x, y, layer);
                    Console.WriteLine($"[Command] Tuile supprimée ({x},{y}) sur '{layer}'");
                }
            }
            AudioManager.Play("open");
        }
    }

    //Supprimer une seule tuile : 
    //exemple : 
    //var cmd = new RemoveTileCommand(map, 5, 5, "Breakable");
    //cmd.Execute();

    public class RemoveTileCommand : IEventCommand
    {
        private readonly MapInfo map;
        private readonly int col;
        private readonly int row;
        private readonly string layer;

        public RemoveTileCommand(MapInfo map, int col, int row, string layer)
        {
            this.map = map;
            this.col = col;
            this.row = row;
            this.layer = layer;
        }

        public void Execute()
        {
            map.SetTuileToZero(col, row, layer);
            Console.WriteLine($"[Command] Tuile supprimée à ({col},{row}) sur le calque '{layer}'");
        }
    }

    //Ouvrir la porte : 
    //var cmd = new OpenDoorCommand(1); // ID de la porte
    //cmd.Execute();
    public class OpenDoorCommand : IEventCommand
    {
        private readonly int doorId;

        public OpenDoorCommand(int doorId)
        {
            this.doorId = doorId;
        }

        public void Execute()
        {
            var door = InteractDoor.GetDoorDictionary.GetValueOrDefault(doorId);
            if (door != null)
            {
                GameManager.CreateNewDoor(
                    door.IdDoor,
                    door.OriginLigne,
                    door.OriginColonne,
                    door.OriginMap,
                    door.TargetColonne,
                    door.TargetLigne,
                    door.TargetMap
                );
                Console.WriteLine($"[Command] Porte ID {doorId} ouverte via script");
            }
        }
    }

    //Faire spawn un ennemi 
    // var enemy = new Npc(4, 0, "slime", "ennemy", 10, 10, 2, 1, "monster", 3)
    // {
    //     Colonne = 12,
    //     Ligne = 8,
    //     MapName = "forest"
    // };

    // var cmd = new SpawnEnemyCommand(enemy);
    // cmd.Execute();
    public class SpawnEnemyCommand : IEventCommand
    {
        private readonly Npc enemy;

        public SpawnEnemyCommand(Npc enemy)
        {
            this.enemy = enemy;
        }

        public void Execute()
        {
            GameManager.ActiveNpcs.Add(enemy);
            Console.WriteLine($"[Command] Ennemi spawné : {enemy.Name} en ({enemy.Colonne},{enemy.Ligne})");
        }
    }

    //Une fois ça fait on utilise switch : 
    //exemple : 
    // var hasItem = new RequireItemCommand(
    //     heros,
    //     (p) => p.PotionInventory > 0,
    //     new RemoveZoneCommand(map, 27, 24, 31, 25, "Breakable")
    // );

    // var sw = new Switch(19, 24, "forest", hasItem);
    // GameManager.ActiveSwitches.Add(sw);
    public class Switch
    {
        public int Colonne { get; private set; }
        public int Ligne { get; private set; }
        public string MapName { get; private set; }
        public bool IsActivated { get; private set; } = false;

        private IEventCommand command;

        public Switch(int col, int lig, string mapName, IEventCommand cmd)
        {
            Colonne = col;
            Ligne = lig;
            MapName = mapName;
            command = cmd;
        }

        public void Activate()
        {
            if (IsActivated) return;

            var previousState = IsActivated;

            command.Execute();

            // Ne passe à true que si la commande a effectivement supprimé des choses
            if (!previousState && !IsActivated)
            {
                return;
            }

            IsActivated = true;
        }
    }

    // //Pour en enchainer plusieurs : 
    //     // -------------------- Les actions à exécuter si le joueur a la clé------------------
    // var switchActions = new CompositeCommand(new List<IEventCommand>
    // {
    //     new RemoveZoneCommand(map, 35, 5, 43, 10, "SwitchObstacle"),
    //     new RemoveTileCommand(map, 46, 1, "Switch")
    // });

    // //  --------------------La commande conditionnelle--------------------------
    // var requireKeyTuto = new RequireItemCommand(
    //     player,
    //     p => p.Inventory.HasQuestItem("tuto_key"),
    //     switchActions
    // );

    // //  ------------------------Le switch à (41,6)-----------------------------------
    // var smartSwitch = new Switch(41, 6, "forest", requireKeyTuto);
    // GameManager.ActiveSwitches.Add(smartSwitch);

    public class CompositeCommand : IEventCommand
    {
        private readonly List<IEventCommand> commands;

        public CompositeCommand(List<IEventCommand> commands)
        {
            this.commands = commands;
        }

        public void Execute()
        {
            foreach (var cmd in commands)
            {
                cmd.Execute();
            }
        }
    }

}