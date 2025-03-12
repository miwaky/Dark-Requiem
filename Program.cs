using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using System.Diagnostics;
using DarkRequiem.controller;
using DarkRequiem.manager;
using DarkRequiem.map;
using DarkRequiem.interact;

namespace DarkRequiem
{

    public class DarkRequiem
    {
        public static int Main(string[] args)
        {

            const int scale = 1; // Multiplicateur résolution gameboy * scale
            const int screenWidth = 1920 * scale;
            const int screenHeight = 1080 * scale;

            InitWindow(screenWidth, screenHeight, "Projet");
            SetTargetFPS(60);

            RenduMap renduMap = new RenduMap(new Map("/maps/map_village.json"), "assets");
            Map map = new Map("/maps/map_village.json");

            SceneManager.InitGame(ref renduMap, ref map, "map_village");


            //Player  
            // Crée une instance de Player et la stocke dans une variable
            Player heros = Player.GeneratePlayer(1, 1);
            Input playerInput = new Input(heros, map);

            int largeur = GetScreenWidth();
            int hauteur = GetScreenHeight();

            // Camera
            // Camera
            Camera2D camera = new Camera2D();
            camera.Target = new Vector2(heros.colonne * map.TailleTuile, heros.ligne * map.TailleTuile);
            camera.Offset = new Vector2(largeur / 2, hauteur / 2);
            camera.Zoom = 4f;



            while (!WindowShouldClose())
            {
                // Update ============================================

                playerInput.Movement();
                camera.Target = new Vector2((heros.colonne * map.TailleTuile) + map.TailleTuile / 2, (heros.ligne * map.TailleTuile) + map.TailleTuile / 2);

                if (IsKeyPressed(KeyboardKey.I))
                {
                    Console.WriteLine("pressed I");
                    GameManager.CreateNewMonster(2, "map_village", 3, 1);
                    GameManager.CreateNewMonster(2, "map_village_basement", 3, 1);
                }
                if (IsKeyPressed(KeyboardKey.T))
                {
                    Console.WriteLine("pressed T");
                    SceneManager.SwitchMapAndMovePlayer(heros, ref renduMap, ref map, ref playerInput, "map_village", 20, 14);
                }

                if (IsKeyPressed(KeyboardKey.E))
                {
                    Console.WriteLine("pressed E");
                    SceneManager.SwitchMapAndMovePlayer(heros, ref renduMap, ref map, ref playerInput, "map_village_basement", 11, 7);

                }


                // Draw ==============================================
                BeginDrawing();
                Raylib.ClearBackground(new Color(155, 188, 15, 255));
                Raylib.ClearBackground(new Color(32, 33, 55, 255));
                BeginMode2D(camera);
                renduMap.AfficherMap();
                renduMap.DrawMonsters();

                DrawTexture(heros.Texture, heros.colonne * map.TailleTuile, heros.ligne * map.TailleTuile, Color.White);
                // Console.WriteLine($"Joueur position : {heros.colonne}, {heros.ligne}");

                // Appliquer un filtre couleur (overlay)
                //Color gbFilter = new Color(139, 172, 15, 100); // Vert clair avec transparence
                // Raylib.DrawRectangle(0, 0, 1920, 1080, gbFilter);
                //Stop Draw

                if (playerInput.collidedNpc != null)
                {
                    InteractNpc.InitTalk(playerInput.collidedNpc);

                }
                EndMode2D();


                //Debug zone : 
                playerInput.DrawDebug();

                EndDrawing();


            }
            //Stop Game

            renduMap.Close();
            heros.Close();
            CloseWindow();
            return 0;
        }

    }
}