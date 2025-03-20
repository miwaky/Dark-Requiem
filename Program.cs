using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using DarkRequiem.controller;
using DarkRequiem.manager;
using DarkRequiem.map;
using DarkRequiem.interact;
using DarkRequiem.player;
using DarkRequiem.npc;

namespace DarkRequiem
{
    public class DarkRequiem
    {
        public static int Main(string[] args)
        {
            const int screenWidth = 768;
            const int screenHeight = 528;

            InitWindow(screenWidth, screenHeight, "Dark Requiem");
            SetTargetFPS(60);

            MapInfo map = new MapInfo("assets/maps/forest.json");
            RenduMap renduMap = new RenduMap(map, "assets");

            SceneManager.InitGame(ref renduMap, ref map, "forest");

            CameraManager cameraManager = new CameraManager(screenWidth, screenHeight);

            Player heros = Player.GeneratePlayer(9, 7);
            Input playerInput = new Input(heros, map, renduMap);

            cameraManager.InitCameraPosition(heros.colonne, heros.ligne);

            NpcTextures.LoadAll();

            while (!WindowShouldClose())
            {
                float deltaTime = GetFrameTime(); // correction essentielle ici

                map = SceneManager.CurrentMap!;
                renduMap = SceneManager.CurrentRenduMap!;

                playerInput.Movement();

                bool isMoving = Vector2.Distance(heros.PositionPixel, heros.TargetPositionPixel) > 1f;
                heros.UpdateAnimation(deltaTime, isMoving, playerInput.CurrentDirection);

                playerInput.Action();

                cameraManager.UpdateZoneCamera(heros.colonne, heros.ligne, deltaTime);

                BeginDrawing();
                ClearBackground(new Color(32, 33, 55, 255));

                BeginMode2D(cameraManager.GetCamera());

                renduMap.AfficherMap();
                renduMap.DrawNpc();
                heros.UpdatePositionSmooth(deltaTime);
                heros.Draw();

                InteractNpc.ShowTalk();

                EndMode2D();

                playerInput.DrawDebug();

                EndDrawing();
            }

            renduMap.Close();
            heros.Close();
            CloseWindow();
            return 0;
        }
    }
}
