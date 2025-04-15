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

            heros = Player.GeneratePlayer(9, 7);
            playerInput = new Input(heros, map, renduMap);

            cameraManager.InitCameraPosition(heros.colonne, heros.ligne);

            NpcTextures.LoadAll();

            Ui.TextureLoadUi();
        }

        public void Update()
        {
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

            playerInput.Movement();

            bool isMoving = Vector2.Distance(heros.PositionPixel, heros.TargetPositionPixel) > 1f;
            heros.UpdateAnimation(deltaTime, isMoving, playerInput.CurrentDirection);

            playerInput.Action();
            cameraManager.UpdateZoneCamera(heros.colonne, heros.ligne, deltaTime);
            heros.UpdatePositionSmooth(deltaTime);
        }

        public void Draw()
        {
            BeginDrawing();
            ClearBackground(new Color(32, 33, 55, 255));

            BeginMode2D(cameraManager.GetCamera());

            renduMap.AfficherMap();
            renduMap.DrawObjects(GameManager.ActiveObjects);

            renduMap.DrawNpcs(GameManager.ActiveNpcs);
            heros.Draw();
            InteractNpc.ShowTalk();

            EndMode2D();
            Ui.UiPlayer(heros);
            //playerInput.DrawDebug();

            EndDrawing();
        }
    }
}