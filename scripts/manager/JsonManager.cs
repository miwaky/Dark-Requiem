using DarkRequiem.map;
using DarkRequiem.controller;
using DarkRequiem.interact;
using DarkRequiem.player;
using System.Numerics;

namespace DarkRequiem.manager
{
    public class JsonManager
    {
        public static Dictionary<string, MapInfo> LoadedMaps = new Dictionary<string, MapInfo>();
        public static MapInfo CurrentMap { get; private set; } = new MapInfo("assets/maps/forest.json");
        public static RenduMap? CurrentRenduMap { get; private set; }

        public static void LoadMaps()
        {
            LoadedMaps["forest"] = new MapInfo("assets/maps/forest.json");
            LoadedMaps["test"] = new MapInfo("assets/maps/test.json");
            LoadedMaps["dungeon"] = new MapInfo("assets/maps/dungeon.json");
        }

        public static void SetCurrentMap(string mapName)
        {
            CurrentMap = LoadedMaps[mapName.ToLower()];
        }

        public static MapInfo GetCurrentMap()
        {
            return CurrentMap;
        }

        public static void SwitchMapAndMovePlayer(Player player, ref RenduMap renduMap, ref MapInfo map, ref Input input, string targetMap, int targetCol, int targetRow, CameraManager cameraManager)
        {
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

            player.TargetPositionPixel = new Vector2(player.colonne * 16, player.ligne * 16);
            player.PositionPixel = player.TargetPositionPixel;
            cameraManager.ForceCameraPosition(player.colonne, player.ligne);
        }

        public static void CollidedDoorCheck(Player player, ref RenduMap renduMap, ref MapInfo map, Input input, InteractDoor door)
        {
            SwitchMapAndMovePlayer(player, ref renduMap, ref map, ref input, door.TargetMap.ToLower(), door.TargetColonne, door.TargetLigne, SceneManager.GetCameraManager());
        }

        public static void InitGame(ref RenduMap renduMap, ref MapInfo map, string targetMap)
        {
            JsonManager.LoadMaps();

            if (!LoadedMaps.ContainsKey(targetMap))
            {
                //Console.WriteLine($"Erreur : Carte '{targetMap}' introuvable !");
                return;
            }

            SetCurrentMap(targetMap);
            GameManager.LoadForCurrentMap();

            map = GetCurrentMap();
            renduMap = new RenduMap(map, "assets");

            CurrentRenduMap = renduMap;

            if (map == null)
            {
                //Console.WriteLine("Erreur : `map` est null après `GetCurrentMap()` !");
                return;
            }
        }
    }
}
