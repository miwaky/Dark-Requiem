using DarkRequiem.map;
using DarkRequiem.manager;
using DarkRequiem.npc;
using DarkRequiem.interact;
using DarkRequiem.player;
using DarkRequiem.objects;
using System.Numerics;
using DarkRequiem.scenes;
public interface IEventCommand
{
    void Execute();
}


namespace DarkRequiem.events
{
    public class CustomCommand : IEventCommand
    {
        private readonly Action action;

        public CustomCommand(Action action)
        {
            this.action = action;
        }

        public void Execute() => action();
    }

    public static class InventoryConditions
    {
        public static bool HasPotion(Player player, int amount = 1)
        {
            return player.Inventory.HasItem("potion", amount);
        }

        public static bool HasGold(Player player, int amount = 1)
        {
            return player.Inventory.HasItem("gold", amount);
        }

        public static bool HasQuestItem(Player player, string questId)
        {
            return player.Inventory.HasItem(questId);
        }

        public static int GetItemQuantity(Player player, string itemId)
        {
            return player.Inventory.GetItemQuantity(itemId);
        }
    }
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

        public static bool IsObjectAt(int col, int row, string map)
        {
            return GameManager.ActiveObjects.Any(obj =>
                obj.colonne == col && obj.ligne == row &&
                map.ToLower() == JsonManager.CurrentMap.NomCarte.ToLower());
        }
    }

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
            //Console.WriteLine($"[Command] Son joué : {soundKey}");
        }
    }

    public class SpawnObjectCommand : IEventCommand
    {
        private readonly Objects obj;

        public SpawnObjectCommand(Objects obj)
        {
            this.obj = obj;
        }

        public void Execute()
        {
            GameManager.ActiveObjects.Add(obj);
            //Console.WriteLine($"[Command] Objet ajouté : {obj.name} à ({obj.colonne},{obj.ligne})");
        }
    }

    public class SpawnChestCommand : IEventCommand
    {
        private readonly Chest chest;

        public SpawnChestCommand(Chest chest)
        {
            this.chest = chest;
        }

        public void Execute()
        {
            GameManager.ActiveChests.Add(chest);
            //Console.WriteLine($"[Command] Coffre ID {chest.Id} spawné en ({chest.Colonne},{chest.Ligne}) sur {chest.MapName}");
        }
    }

    public class SpawnTileCommand : IEventCommand
    {
        private readonly MapInfo map;
        private readonly int col;
        private readonly int row;
        private readonly int tileId;
        private readonly string layer;

        public SpawnTileCommand(MapInfo map, int col, int row, int tileId, string layer)
        {
            this.map = map;
            this.col = col;
            this.row = row;
            this.tileId = tileId;
            this.layer = layer;
        }

        public void Execute()
        {
            var targetLayer = map.Calques.FirstOrDefault(l => l.name == layer);
            if (targetLayer == null)
            {
                //Console.WriteLine($"[ERROR] Layer {layer} introuvable !");
                return;
            }

            int index = row * map.Largeur + col;
            if (index >= 0 && index < targetLayer.data.Count)
            {
                targetLayer.data[index] = tileId;
                //Console.WriteLine($"[Command] Tuile placée : ID {tileId} à ({col},{row}) sur le calque {layer}");
            }
        }
    }
    public class CreateInventoryChestCommand : IEventCommand
    {
        private int id, col, row;
        private string mapName;
        private Objects contenu;
        private IEventCommand? onOpenEvent;

        public CreateInventoryChestCommand(int id, int col, int row, string mapName, Objects contenu, IEventCommand? onOpenEvent = null)
        {
            this.id = id;
            this.col = col;
            this.row = row;
            this.mapName = mapName;
            this.contenu = contenu;
            this.onOpenEvent = onOpenEvent;
        }

        public void Execute()
        {
            var chest = new Chest(id, col, row, mapName, contenu)
            {
                OnOpenEvent = onOpenEvent
            };
            GameManager.ActiveChests.Add(chest);
        }
    }
    public class RequireItemCommand : IEventCommand
    {
        private readonly Player player;
        private readonly Func<Player, bool> condition;
        private readonly IEventCommand command;
        private readonly string? itemToRemove;
        private readonly int amountToRemove;

        public bool WasExecuted { get; private set; } = false;

        public RequireItemCommand(
            Player player,
            Func<Player, bool> condition,
            IEventCommand command,
            string? itemToRemove = null,
            int amountToRemove = 1
        )
        {
            this.player = player;
            this.condition = condition;
            this.command = command;
            this.itemToRemove = itemToRemove;
            this.amountToRemove = amountToRemove;
        }

        public void Execute()
        {
            if (!condition(player))
            {
                WasExecuted = false;
                return;
            }

            // Supprime l’objet si spécifié
            if (itemToRemove != null)
            {
                player.Inventory.RemoveItem(itemToRemove, amountToRemove);
                NotificationManager.Add($"- {amountToRemove} {itemToRemove}");
            }

            command.Execute();
            AudioManager.Play("switch");
            WasExecuted = true;
        }
    }

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
                    //Console.WriteLine($"[Command] Tuile supprimée ({x},{y}) sur '{layer}'");
                }
            }
            AudioManager.Play("open");
        }
    }

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
            //Console.WriteLine($"[Command] Tuile supprimée à ({col},{row}) sur le calque '{layer}'");
        }
    }

    public class SwitchSequencePuzzleCommand : IEventCommand
    {
        private HashSet<(int col, int row)> usedSwitches = new();
        private readonly List<(int col, int row)> expectedOrder;
        private readonly (int col, int row) chestLocation;
        private readonly Objects reward;
        private readonly string mapName;
        private readonly MapInfo map;

        private int currentIndex = 0;
        private bool[] activated;

        public SwitchSequencePuzzleCommand(
            List<(int, int)> expectedOrder,
            (int, int) chestLocation,
            Objects reward,
            string mapName,
            MapInfo map
        )
        {
            this.expectedOrder = expectedOrder;
            this.chestLocation = chestLocation;
            this.reward = reward;
            this.mapName = mapName;
            this.map = map;
            this.activated = new bool[expectedOrder.Count];
        }

        public void Execute()
        {
            var player = GameManager.GetPlayer();
            var playerPos = (player.colonne, player.ligne);

            // Vérifie si on est adjacent à un switch
            (int col, int row) targetSwitch = (-1, -1);
            foreach (var sw in expectedOrder)
            {
                bool isAdjacent =
                    (Math.Abs(playerPos.colonne - sw.col) == 1 && playerPos.ligne == sw.row) ||
                    (Math.Abs(playerPos.ligne - sw.row) == 1 && playerPos.colonne == sw.col);

                if (isAdjacent)
                {
                    targetSwitch = sw;
                    break;
                }
            }

            // Aucun switch à proximité
            if (targetSwitch == (-1, -1)) return;

            // Si déjà utilisé, on ignore (pas d’erreur)
            if (usedSwitches.Contains(targetSwitch))
            {
                Console.WriteLine("[Puzzle] Ce switch a déjà été utilisé pour cette tentative.");
                return;
            }

            // Si ce n’est pas le bon dans la séquence
            if (targetSwitch != expectedOrder[currentIndex])
            {
                Console.WriteLine("[Puzzle] Mauvais switch. Réinitialisation du puzzle.");
                RestoreSwitchTiles();
                ResetPuzzle();
                return;
            }

            // Bonne étape
            usedSwitches.Add(targetSwitch);
            new RemoveTileCommand(map, targetSwitch.col, targetSwitch.row, "Switch").Execute();
            activated[currentIndex] = true;
            currentIndex++;

            if (currentIndex == expectedOrder.Count)
            {
                Console.WriteLine("[Puzzle] Succès du puzzle. Apparition du coffre !");
                new CreateInventoryChestCommand(131, chestLocation.col, chestLocation.row, mapName, reward).Execute();
            }
            else
            {
                Console.WriteLine($"[Puzzle] Étape {currentIndex} sur {expectedOrder.Count} validée.");
            }
        }

        private void RestoreSwitchTiles()
        {
            for (int i = 0; i < expectedOrder.Count; i++)
            {
                if (activated[i])
                {
                    var (col, row) = expectedOrder[i];
                    new SpawnTileCommand(map, col, row, 2599, "Switch").Execute();
                }
            }
        }

        private void ResetPuzzle()
        {
            currentIndex = 0;
            usedSwitches.Clear();
            for (int i = 0; i < activated.Length; i++)
                activated[i] = false;
        }
    }

    public class MultiSwitchPuzzleCommand : IEventCommand
    {
        private readonly List<(int x, int y)> requiredSwitches;
        private readonly MapInfo map;
        private readonly string mapName;
        private readonly Player player;
        private readonly (int x, int y) rewardChestPos;
        private readonly Objects reward;

        private HashSet<(int x, int y)> activatedSwitches = new();

        public MultiSwitchPuzzleCommand(List<(int x, int y)> switches, MapInfo map, string mapName, Player player, (int x, int y) rewardPos, Objects reward)
        {
            requiredSwitches = switches;
            this.map = map;
            this.mapName = mapName;
            this.player = player;
            rewardChestPos = rewardPos;
            this.reward = reward;
        }

        public void Execute()
        {
            var pos = (player.colonne, player.ligne);

            // Le joueur doit être adjacent à un switch non encore activé
            var matched = requiredSwitches.FirstOrDefault(sw =>
                !activatedSwitches.Contains(sw) &&
                (
                    (Math.Abs(sw.x - pos.Item1) == 1 && sw.y == pos.Item2) ||
                    (Math.Abs(sw.y - pos.Item2) == 1 && sw.x == pos.Item1)
                )
            );

            if (matched == default)
            {
                Console.WriteLine("[MultiSwitch] Aucun switch valide à proximité ou déjà activé.");
                return;
            }

            if (!player.Inventory.HasItem("Sphere", 1))
            {
                Console.WriteLine("[MultiSwitch] Il faut une sphère pour activer ce switch.");
                AudioManager.Play("denied");
                return;
            }

            // On retire la sphère de l'inventaire
            player.Inventory.RemoveItem("Sphere", 1);
            activatedSwitches.Add(matched);

            new RemoveTileCommand(map, matched.x, matched.y, "Switch").Execute();
            AudioManager.Play("switch");
            Console.WriteLine($"[MultiSwitch] Switch activé en {matched}. {activatedSwitches.Count}/{requiredSwitches.Count}");

            if (activatedSwitches.Count == requiredSwitches.Count)
            {
                Console.WriteLine("[MultiSwitch] Tous les switches sont activés, apparition du coffre !");
                new CreateInventoryChestCommand(
                    id: 200,
                    col: rewardChestPos.x,
                    row: rewardChestPos.y,
                    mapName: mapName,
                    contenu: reward
                ).Execute();
            }
        }
    }

    public class SwitchToWinnerSceneCommand : IEventCommand
    {
        public void Execute()
        {
            SceneManager.SetScene(new WinnerScene());
        }
    }

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
                //Console.WriteLine($"[Command] Porte ID {doorId} ouverte via script");
            }
        }
    }

    public class TimedObstacleChallengeCommand : IEventCommand
    {
        private readonly MapInfo map;
        private readonly List<(int x, int y)> tilesToClear;
        private readonly List<(int x, int y)> tilesToRestore;
        private readonly (int x, int y) switchTile;
        private readonly string layer;
        private readonly float totalDuration;
        private readonly IEventCommand? onFailure;

        private float timer;
        private bool isRunning = false;
        private bool hasPlayerSucceeded = false;
        public bool IsCompleted { get; private set; }

        public TimedObstacleChallengeCommand(
            MapInfo map,
            List<(int x, int y)> tilesToClear,
            List<(int x, int y)> tilesToRestore,
            (int x, int y) switchTile,
            string layer,
            float duration = 10f,
            IEventCommand? onFailure = null
        )
        {
            this.map = map;
            this.tilesToClear = tilesToClear;
            this.tilesToRestore = tilesToRestore;
            this.switchTile = switchTile;
            this.layer = layer;
            this.totalDuration = duration;
            this.timer = duration;
            this.onFailure = onFailure;
        }

        public void Execute()
        {
            if (IsCompleted) return;

            if (!isRunning)
            {
                foreach (var (x, y) in tilesToClear)
                    map.SetTuileToZero(x, y, layer);

                isRunning = true;
                Console.WriteLine("[TimerEvent] Début du compte à rebours !");
            }

            UpdateCountdown();
        }

        private void UpdateCountdown()
        {
            float deltaTime = Raylib_cs.Raylib.GetFrameTime();
            timer -= deltaTime;

            var player = GameManager.GetPlayer();
            bool inside = tilesToClear.Any(pos => pos.x == player.colonne && pos.y == player.ligne);

            if (inside)
            {
                Console.WriteLine("[TimerEvent] Objectif atteint à temps !");
                hasPlayerSucceeded = true;
                IsCompleted = true;
                return;
            }

            if (timer <= 0 && !hasPlayerSucceeded)
            {
                Console.WriteLine("[TimerEvent] Échec. Obstacles restaurés et switch réactivé.");

                //Pique
                foreach (var (x, y) in tilesToRestore)
                    new SpawnTileCommand(map, x, y, 2700, layer).Execute();

                // Réactive le switch en remettant une tuile visible
                const int switchTileId = 2599; // ID du sprite de switch réactivable
                new SpawnTileCommand(map, switchTile.x, switchTile.y, switchTileId, "Switch").Execute();

                // Supprime les anciens switches pour éviter doublons
                GameManager.ActiveSwitches.RemoveAll(sw =>
                    sw.Colonne == switchTile.x && sw.Ligne == switchTile.y && sw.MapName == map.NomCarte);

                // Exécute la commande à lancer après l’échec (comme recréer un Switch)
                onFailure?.Execute();

                IsCompleted = true;
            }
        }
    }

    public class CheckBothChestsOpenedCommand : IEventCommand
    {
        private readonly List<int> chestIdsToCheck;
        public bool IsCompleted { get; private set; } = false;

        public CheckBothChestsOpenedCommand(List<int> chestIds)
        {
            chestIdsToCheck = chestIds;
        }

        public void Execute()
        {
            if (IsCompleted) return;

            bool allOpened = chestIdsToCheck.All(id => EventConditions.IsChestOpened(id));
            if (allOpened)
            {
                IsCompleted = true;
            }
        }
    }
    public class SpawnEnemyDirectCommand : IEventCommand
    {
        private readonly Npc enemy;

        public SpawnEnemyDirectCommand(Npc enemy)
        {
            this.enemy = enemy;
        }

        public void Execute()
        {
            // Évite le double ajout
            if (GameManager.ActiveNpcs.Exists(n => n.Id == enemy.Id))
            {
                //Console.WriteLine($"[SpawnEnemy] NPC ID {enemy.Id} est déjà présent sur la map !");
                return;
            }

            GameManager.ActiveNpcs.Add(enemy);
            //Console.WriteLine($"[SpawnEnemy] {enemy.Name} spawn à ({enemy.Colonne},{enemy.Ligne}) sur {enemy.MapName}");
        }
    }

    public class WaitUntilNpcKilledCommand : IEventCommand
    {
        private readonly List<int> npcIdsToWaitFor;
        public bool IsCompleted { get; private set; } = false;

        public WaitUntilNpcKilledCommand(List<int> npcIds)
        {
            npcIdsToWaitFor = npcIds;
        }

        public void Execute()
        {
            if (IsCompleted) return;

            bool allDead = npcIdsToWaitFor.All(id =>
                GameManager.ActiveNpcs.All(npc => npc.Id != id)
            );

            if (allDead)
            {
                //Console.WriteLine("[Command] Tous les NPC ciblés sont morts. Condition validée.");
                IsCompleted = true;
            }
        }
    }


    public class TeleportPlayerCommand : IEventCommand
    {
        private readonly Player player;
        private readonly int targetCol;
        private readonly int targetRow;

        public TeleportPlayerCommand(Player player, int col, int row)
        {
            this.player = player;
            targetCol = col;
            targetRow = row;
        }

        public void Execute()
        {
            player.colonne = targetCol;
            player.ligne = targetRow;
            player.TargetPositionPixel = new Vector2(targetCol * 16, targetRow * 16);
            //Console.WriteLine($"[Teleport] Le joueur a été téléporté en ({targetCol}, {targetRow})");
        }
    }

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

            command.Execute();

            if (command is RequireItemCommand req && req.WasExecuted)
                IsActivated = true;
            else if (command is CompositeCommand)
                IsActivated = true;
        }
    }
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