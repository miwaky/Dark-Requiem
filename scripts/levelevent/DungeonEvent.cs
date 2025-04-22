using DarkRequiem.manager;
using DarkRequiem.map;
using DarkRequiem.npc;
using DarkRequiem.objects;
using DarkRequiem.player;
using System.Numerics;
namespace DarkRequiem.events
{
    public static class DungeonEvent
    {

        public static void InitEvents(Player player, MapInfo map)
        {
            InitChestSpawn(player, map);
            InitDoor1NeedKey(player, map);
            InitDoor2NeedKey(player, map);
            InitOpenChestActivateTrap(player, map);
            InitRedKey1(player, map);
            InitRedKey2(player, map);
            InitRedKey3(player, map);
            InitTeleportSwitches(player, map);
            InitSpherePlacementPuzzle(player, map);
            InitEndGame(player, map);
        }

        public static void InitEndGame(Player player, MapInfo map)
        {
            var winnerChest = new Chest(999, 23, 6, "dungeon", Money.GenerateMoney(23, 6))
            {
                OnOpenEvent = new SwitchToWinnerSceneCommand()
            };
            GameManager.ActiveChests.Add(winnerChest);
        }

        public static void InitChestSpawn(Player player, MapInfo map)
        {
            var chestCommand = new CreateInventoryChestCommand(121, 78, 64, "dungeon", QuestObject.Sphere());
            chestCommand.Execute();

        }
        public static void InitDoor1NeedKey(Player player, MapInfo map)
        {
            var DoorSwitch = new CompositeCommand(new List<IEventCommand>
{
    new RemoveTileCommand(map, 40, 57, "DoorClosed"),
   // new TeleportPlayerCommand(player, 40, 54)
});

            var requireDungeonKey1 = new RequireItemCommand(
                player,
                p => p.Inventory.HasItem("key"),
                DoorSwitch
            );

            // Activation sur la case (40,57)
            var DungeonDoorKey1 = new Switch(40, 58, "dungeon", requireDungeonKey1);
            GameManager.ActiveSwitches.Add(DungeonDoorKey1);

        }
        public static void InitDoor2NeedKey(Player player, MapInfo map)
        {
            var DoorSwitch = new CompositeCommand(new List<IEventCommand>
{
    new RemoveTileCommand(map, 40, 24, "DoorClosed"),

});

            var requireDungeonKey2 = new RequireItemCommand(
                player,
                p => p.Inventory.HasItem("Boss Key"),
                DoorSwitch
            );

            // Activation sur la case (40,57)
            var DungeonDoorKey2 = new Switch(40, 25, "dungeon", requireDungeonKey2);
            GameManager.ActiveSwitches.Add(DungeonDoorKey2);

        }

        public static void InitOpenChestActivateTrap(Player player, MapInfo map)
        {
            // 1. Tuile qui bloque
            var spawnTile = new SpawnTileCommand(map, 15, 61, 2700, "SwitchObstacle");

            // 2. Création des ennemis via la factory centralisée
            var slime = NpcData.CreateNpcFromType("slime", 200, 6, 59, "dungeon");
            var bat = NpcData.CreateNpcFromType("bat", 201, 11, 62, "dungeon");

            // 3. Spawn des ennemis
            var spawnSlime = new SpawnEnemyDirectCommand(slime);
            var spawnBat = new SpawnEnemyDirectCommand(bat);

            // 4. Supprimer le piège (sera exécuté plus tard)
            var removeTile = new RemoveTileCommand(map, 15, 61, "SwitchObstacle");

            // 5. Attente ennemis morts
            var waitKillCheck = new WaitUntilNpcKilledCommand(new List<int> { 200, 201 });

            // 6. Crée un wrapper pour ajouter la surveillance une fois les ennemis apparus
            var registerWaitCommand = new CustomCommand(() =>
            {
                GameManager.PendingEventCommands.Add((waitKillCheck, removeTile));
            });

            // 7. Création du coffre avec son événement
            var trapEvent = new CompositeCommand(new List<IEventCommand>
    {
        spawnTile,
        spawnSlime,
        spawnBat,
        registerWaitCommand
    });

            var trapChestCommand = new CreateInventoryChestCommand(
                id: 1,
                col: 7,
                row: 58,
                mapName: "dungeon",
                contenu: QuestObject.Key(),
                onOpenEvent: trapEvent
            );

            // 8. Exécute la commande de création
            trapChestCommand.Execute();
        }

        public static void InitRedKey1(Player player, MapInfo map)
        {
            Console.WriteLine("Test");
            // 1. Création des coffres secondaires à spawn plus tard
            var chest2 = new CreateInventoryChestCommand(111, 50, 40, "dungeon", Money.GenerateMoney(50, 40));
            var chest3 = new CreateInventoryChestCommand(112, 60, 40, "dungeon", Money.GenerateMoney(60, 40));

            // 2. Coffre final avec objet de quête
            var finalChest = new CreateInventoryChestCommand(113, 44, 36, "dungeon", QuestObject.Sphere());

            // 3. Condition : attendre que les coffres 11 et 12 soient ouverts
            var checkBothOpened = new CheckBothChestsOpenedCommand(new List<int> { 111, 112 });

            // 4. Action une fois les deux coffres ouverts
            var spawnFinal = new CustomCommand(() =>
            {
                finalChest.Execute(); // spawn coffre final
            });

            // 5. Enregistrement du check une fois les coffres secondaires spawnés
            var registerWait = new CustomCommand(() =>
            {
                GameManager.PendingEventCommands.Add((checkBothOpened, spawnFinal));
            });

            // 6. Création du coffre initial (déjà présent) qui lance tout
            var startChest = new CreateInventoryChestCommand(
                id: 110,
                col: 56,
                row: 36,
                mapName: "dungeon",
                contenu: Money.GenerateMoney(56, 36),
                onOpenEvent: new CompositeCommand(new List<IEventCommand>
                {
            chest2,
            chest3,
            registerWait
                })
            );

            // 7. On l’ajoute à la scène
            startChest.Execute();
        }

        public static void InitRedKey2(Player player, MapInfo map)
        {
            var tiles = new List<(int, int)> { (77, 63), (78, 63), (77, 64) };
            var switchPos = (77, 58);
            string layer = "SwitchObstacle";
            void RegisterNewChallenge()
            {
                var challenge = new TimedObstacleChallengeCommand(
                    map,
                    tiles,
                    tiles, // même pour restore ici
                    switchPos,
                    layer,
                    duration: 10f,
                    onFailure: new CustomCommand(() =>
                    {
                        // Recrée le switch après échec
                        var retryCommand = new CompositeCommand(new List<IEventCommand>
                        {
                    new RemoveTileCommand(map, switchPos.Item1, switchPos.Item2, "Switch"),
                    new CustomCommand(RegisterNewChallenge)
                        });

                        var newSwitch = new Switch(switchPos.Item1, switchPos.Item2, "dungeon", retryCommand);
                        GameManager.ActiveSwitches.Add(newSwitch);
                    })
                );

                // Ajouter le suivi du timer
                GameManager.PendingEventCommands.Add((challenge, new CustomCommand(() => { })));
                challenge.Execute(); // démarre le timer immédiatement
            }

            // Premier switch initial
            var initSwitch = new Switch(switchPos.Item1, switchPos.Item2, "dungeon", new CompositeCommand(new List<IEventCommand>
    {
        new RemoveTileCommand(map, switchPos.Item1, switchPos.Item2, "Switch"),
        new CustomCommand(RegisterNewChallenge)
    }));

            GameManager.ActiveSwitches.Add(initSwitch);
        }

        public static void InitRedKey3(Player player, MapInfo map)
        {
            var sequence = new List<(int, int)>
    {
        (20, 38),
        (20, 41),
        (26, 41)
    };

            var puzzleCommand = new SwitchSequencePuzzleCommand(
                expectedOrder: sequence,
                chestLocation: (24, 36),
                reward: QuestObject.Sphere(),
                mapName: "dungeon",
                map: map
            );

            foreach (var (col, row) in sequence)
            {
                var sw = new Switch(col, row, "dungeon", puzzleCommand);
                GameManager.ActiveSwitches.Add(sw);
            }
        }
        private static void AddTeleportSwitch(Player player, int fromX, int fromY, int toX, int toY)
        {
            var tp = new Switch(fromX, fromY, "dungeon", new TeleportPlayerCommand(player, toX, toY));
            GameManager.ActiveSwitches.Add(tp);
        }

        public static void InitTeleportSwitches(Player player, MapInfo map)
        {
            AddTeleportSwitch(player, 40, 57, 40, 53);
            AddTeleportSwitch(player, 40, 54, 40, 58);
            AddTeleportSwitch(player, 40, 24, 40, 20);
            AddTeleportSwitch(player, 40, 21, 40, 25);
        }
        public static void InitSpherePlacementPuzzle(Player player, MapInfo map)
        {
            var sphereSwitches = new List<(int, int)>
    {
        (3, 26), (3, 28), (3, 30)
    };

            var rewardPos = (2, 28);
            var reward = QuestObject.BossKey();

            var multiSwitchPuzzle = new MultiSwitchPuzzleCommand(
                switches: sphereSwitches,
                map: map,
                mapName: "dungeon",
                player: player,
                rewardPos: rewardPos,
                reward: reward
            );

            foreach (var (x, y) in sphereSwitches)
            {
                var sw = new Switch(x, y, "dungeon", multiSwitchPuzzle);
                GameManager.ActiveSwitches.Add(sw);
            }
        }

    }
}

