using DarkRequiem.map;
using System;
using System.Collections.Generic;
using DarkRequiem.controller;
using DarkRequiem.interact;
using DarkRequiem.player;

namespace DarkRequiem.manager
{
    public class SceneManager
    {
        public static Dictionary<string, MapInfo> LoadedMaps = new Dictionary<string, MapInfo>(); //  Stocke toutes les cartes chargées
        public static MapInfo CurrentMap { get; private set; } = new MapInfo("assets/maps/forest.json");
        public static RenduMap? CurrentRenduMap { get; private set; }



        public static void LoadMaps()
        {
            LoadedMaps["forest"] = new MapInfo("assets/maps/forest.json");
            //DEBUG
            // Console.WriteLine(" Cartes chargées : " + string.Join(", ", LoadedMaps.Keys));
            // Console.WriteLine(" Toutes les cartes ont été chargées !");

        }
        public static void SetCurrentMap(string mapName)
        {
            CurrentMap = LoadedMaps[mapName.ToLower()];
        }

        public static MapInfo GetCurrentMap()
        {

            return CurrentMap;
        }

        public static void SwitchMapAndMovePlayer(Player player, ref RenduMap renduMap, ref MapInfo map, ref Input input, string targetMap, int targetCol, int targetRow)
        {
            //Debug
            // if (!LoadedMaps.ContainsKey(targetMap.ToLower()))
            // {
            //     Console.WriteLine($"Erreur : Carte '{targetMap}' introuvable !");
            //     return;
            // }

            SetCurrentMap(targetMap);
            GameManager.LoadForCurrentMap();

            player.colonne = targetCol;
            player.ligne = targetRow;
            input.nouvelleColonne = player.colonne;
            input.nouvelleLigne = player.ligne;

            map = GetCurrentMap();
            renduMap = new RenduMap(map, "assets");
            input.UpdateMapReference(map);

            CurrentMap = map;
            CurrentRenduMap = renduMap;
        }

        public static void CollidedDoorCheck(Player player, ref RenduMap renduMap, ref MapInfo map, Input input, InteractDoor door)
        {
            SwitchMapAndMovePlayer(player, ref renduMap, ref map, ref input, door.TargetMap.ToLower(), door.TargetColonne, door.TargetLigne);

        }

        public static void InitGame(ref RenduMap renduMap, ref MapInfo map, string targetMap)
        {
            SceneManager.LoadMaps();

            if (!LoadedMaps.ContainsKey(targetMap))
            {
                Console.WriteLine($"Erreur : Carte '{targetMap}' introuvable !");
                return;
            }

            SetCurrentMap(targetMap);
            GameManager.LoadForCurrentMap();

            map = GetCurrentMap();
            renduMap = new RenduMap(map, "assets");

            CurrentRenduMap = renduMap;

            if (map == null)
            {
                Console.WriteLine(" Erreur : `map` est null après `GetCurrentMap()` !");
                return;
            }
        }


    }
}
