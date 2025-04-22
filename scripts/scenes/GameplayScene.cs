using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using DarkRequiem.controller;
using DarkRequiem.manager;
using DarkRequiem.map;
using DarkRequiem.interact;
using DarkRequiem.player;
using DarkRequiem.npc;
using DarkRequiem.scenes;
using DarkRequiem.objects;
using DarkRequiem.events;


namespace DarkRequiem.scene
{
    public class GameplayScene : IScene
    {
        public string Name => "Gameplay";
        private MapInfo map;
        private RenduMap renduMap;
        private Player heros;
        private Input playerInput;
        private CameraManager cameraManager;
        public CameraManager CameraManager => cameraManager;
        private bool isInventoryOpen = false;

        private int screenWidth = 768;
        private int screenHeight = 528;

        public GameplayScene()
        {
            JsonManager.LoadMaps();
            map = new MapInfo("assets/maps/forest.json");
            renduMap = new RenduMap(map, "assets");
            JsonManager.InitGame(ref renduMap, ref map, "forest");
            AudioManager.PlayMusic("forest");

            cameraManager = new CameraManager(screenWidth, screenHeight);

            heros = Player.GeneratePlayer(37, 3);
            GameManager.CurrentPlayer = heros;

            playerInput = new Input(heros, map, renduMap);

            cameraManager.InitCameraPosition(heros.colonne, heros.ligne);
            Chest.Load();
            NpcTextures.LoadAll();
            Ui.TextureLoadUi();

        }
        private string? currentEventMap = null;

        public void Update()
        {

            if (IsKeyPressed(KeyboardKey.Tab))
            {
                isInventoryOpen = !isInventoryOpen;
            }

            // Bloque les inputs normaux si inventaire ouvert
            if (isInventoryOpen)
                return;
            AudioManager.Update();

            if (heros.Hp <= 0)
            {
                SceneManager.SetScene(new GameOverScene());
                return;
            }

            if (IsKeyPressed(KeyboardKey.E))
            {
                SceneManager.SetScene(new PauseScene(this));
                return;
            }

            float deltaTime = GetFrameTime();

            map = JsonManager.CurrentMap!;
            renduMap = JsonManager.CurrentRenduMap!;

            // Vérifie si on est sur une nouvelle carte pour lancer les events associés
            if (map.NomCarte.ToLower() != currentEventMap)
            {
                currentEventMap = map.NomCarte.ToLower();

                if (currentEventMap == "forest")
                    ForestEvent.InitEvents(heros, map);
                else if (currentEventMap == "dungeon")
                    DungeonEvent.InitEvents(heros, map);
            }

            playerInput.Movement();

            bool isMoving = Vector2.Distance(heros.PositionPixel, heros.TargetPositionPixel) > 1f;
            heros.UpdateAnimation(deltaTime, isMoving, playerInput.CurrentDirection);

            playerInput.Action();
            cameraManager.UpdateZoneCamera(heros.colonne, heros.ligne, deltaTime);
            heros.UpdatePositionSmooth(deltaTime);

            foreach (var (waitCmd, onComplete) in GameManager.PendingEventCommands.ToList())
            {
                waitCmd.Execute();

                bool completed = waitCmd switch
                {
                    WaitUntilNpcKilledCommand npcKilled => npcKilled.IsCompleted,
                    CheckBothChestsOpenedCommand chestsOpened => chestsOpened.IsCompleted,
                    _ => false // autres commandes éventuelles
                };

                if (completed)
                {
                    onComplete.Execute();
                    GameManager.PendingEventCommands.Remove((waitCmd, onComplete));
                }
            }
            float deltatime = GetFrameTime();
            foreach (var tick in GameManager.PendingTickActions.ToList())
            {
                tick(deltaTime);
            }

        }

        public void Draw()
        {
            BeginDrawing();
            ClearBackground(new Color(32, 33, 55, 255));

            BeginMode2D(cameraManager.GetCamera());

            renduMap.AfficherMap();
            renduMap.DrawObjects(GameManager.ActiveObjects);
            renduMap.DrawChests(GameManager.ActiveChests);
            renduMap.DrawNpcs(GameManager.ActiveNpcs);
            heros.Draw();
            InteractNpc.ShowTalk();

            EndMode2D();
            Ui.UiPlayer(heros);
            if (isInventoryOpen)
            {
                DrawRectangle(50, 50, 300, 400, new Color(0, 0, 0, 200));
                DrawRectangleLines(50, 50, 300, 400, Color.White);
                DrawText("INVENTAIRE", 60, 60, 20, Color.White);

                var items = heros.Inventory.GetAllItems();
                int y = 90;
                foreach (var item in items)
                {
                    DrawText($"- {item.Id} ({item.Quantity})", 60, y, 18, Color.LightGray);
                    y += 25;
                }

                DrawText("[Tab] Fermer", 60, 430, 16, Color.Gray);
            }
            playerInput.DrawDebug();

            EndDrawing();
        }
    }
}