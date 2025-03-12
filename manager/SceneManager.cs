using DarkRequiem.map;
using System;
using System.Collections.Generic;
using DarkRequiem.controller;


namespace DarkRequiem.manager
{
    public class SceneManager
    {
        public static Dictionary<string, Map> LoadedMaps = new Dictionary<string, Map>(); //  Stocke toutes les cartes chargées
        public static Map CurrentMap { get; private set; } = new Map("assets/maps/map_village.json");

        public bool teleportation = true;

        public static void LoadMaps()
        {
            LoadedMaps["map_village"] = new Map("assets/maps/map_village.json");
            LoadedMaps["map_village_basement"] = new Map("assets/maps/map_village_basement.json"); // Nom uniformisé en minuscules

            Console.WriteLine(" Cartes chargées : " + string.Join(", ", LoadedMaps.Keys));
            Console.WriteLine(" Toutes les cartes ont été chargées !");

        }
        public static void SetCurrentMap(string mapName)
        {
            if (LoadedMaps.ContainsKey(mapName.ToLower()))
            {
                CurrentMap = LoadedMaps[mapName.ToLower()];
                Console.WriteLine($" Carte actuelle : {CurrentMap.NomCarte}");
            }
            else
            {
                Console.WriteLine($" Erreur : Carte '{mapName}' introuvable !");
            }
        }

        public static Map GetCurrentMap()
        {
            return CurrentMap;
        }

        public static void SwitchMapAndMovePlayer(Player player, ref RenduMap renduMap, ref Map map, ref Input input, string targetMap, int targetCol, int targetRow)
        {
            if (!LoadedMaps.ContainsKey(targetMap))
            {
                Console.WriteLine($"Erreur : Carte '{targetMap}' introuvable !");
                return;
            }
            SetCurrentMap(targetMap);
            GameManager.LoadForCurrentMap();
            player.colonne = targetCol;
            player.ligne = targetRow;
            input.nouvelleColonne = player.colonne;
            input.nouvelleLigne = player.ligne;
            // Met à jour la carte et l'affichage
            map = GetCurrentMap();
            renduMap = new RenduMap(map, "assets");
            input.UpdateMapReference(map); //  Met à jour `_map` dans `Input.cs`

            Console.WriteLine($" Changement vers {targetMap}, joueur repositionné en ({targetCol},{targetRow})");
        }


        public static void InitGame(ref RenduMap renduMap, ref Map map, string targetMap)
        {
            SceneManager.LoadMaps();

            if (!LoadedMaps.ContainsKey(targetMap))
            {
                Console.WriteLine($"Erreur : Carte '{targetMap}' introuvable !");
                return;
            }

            SetCurrentMap(targetMap);
            GameManager.LoadForCurrentMap();

            //  Met à jour la carte et l'affichage
            map = GetCurrentMap();
            if (map == null)
            {
                Console.WriteLine(" Erreur : `map` est null après `GetCurrentMap()` !");
                return;
            }

            renduMap = new RenduMap(map, "assets");
        }

    }
}
