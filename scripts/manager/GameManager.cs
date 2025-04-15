using DarkRequiem.map;
using DarkRequiem.npc;
using DarkRequiem.interact;
using DarkRequiem.player;
using DarkRequiem.objects;

namespace DarkRequiem.manager
{
    class GameManager
    {
        public static List<Npc> ActiveNpcs = new List<Npc>();
        public static List<InteractDoor> ActiveDoors = new List<InteractDoor>();
        public static List<BreakableObject> ActiveBreakables = new();
        public static List<Objects> ActiveObjects = new();
        public static List<Npc> PendingKills = new();

        public static void InitializeNewScene()
        {
            ActiveNpcs.Clear();
            ActiveDoors.Clear();
            ActiveBreakables.Clear();
            ActiveObjects.Clear();

        }

        public static void LoadForCurrentMap()
        {
            MapInfo currentMap = JsonManager.GetCurrentMap();

            if (currentMap == null)
            {
                Console.WriteLine("currentMap == null");
                return;
            }

            InitializeNewScene(); // Nettoie la liste avant de charger

            // Chargement des NPCs
            foreach (var ennemy in currentMap.Ennemies)
            {
                var stats = NpcData.StatsParType[ennemy.Type];

                ActiveNpcs.Add(new Npc(
                    ennemy.Id,
                    ennemy.SpriteID,
                    ennemy.Type,
                    "ennemy",
                    stats.MaxHp,
                    stats.Hp,
                    stats.Attack,
                    stats.Defense,
                    stats.TextureType,
                    stats.AggroRange
                )
                {
                    MapName = currentMap.NomCarte,
                    Colonne = ennemy.Colonne,
                    Ligne = ennemy.Ligne
                });
            }
            // Chargement des portes interactives
            foreach (var doorEntry in InteractDoor.GetDoorDictionary)
            {
                InteractDoor interactDoor = doorEntry.Value;
                if (interactDoor.OriginMap == currentMap.NomCarte)
                {
                    CreateNewDoor(
                        interactDoor.IdDoor,
                        interactDoor.OriginLigne,
                        interactDoor.OriginColonne,
                        interactDoor.OriginMap,
                        interactDoor.TargetLigne,
                        interactDoor.TargetColonne,
                        interactDoor.TargetMap
                    );
                }
            }


        }

        public static void RemoveNpc(int npcId, string mapName, int colonne, int ligne)
        {
            var npcToRemove = ActiveNpcs.FirstOrDefault(n => n.Id == npcId && n.MapName == mapName && n.Colonne == colonne && n.Ligne == ligne);
            if (npcToRemove != null)
            {
                ActiveNpcs.Remove(npcToRemove);
                Console.WriteLine($"NPC {npcToRemove.Name} (ID: {npcToRemove.Id}) sur {npcToRemove.MapName} a été retiré du jeu.");
            }
        }

        public static void CreateNewDoor(int idDoor, int originLigne, int originColonne, string originMap, int targetColonne, int targetLigne, string targetMap)
        {
            Dictionary<int, InteractDoor> doorDictionary = InteractDoor.GetDoorDictionary;

            if (doorDictionary.ContainsKey(idDoor))
            {
                if (!JsonManager.LoadedMaps.ContainsKey(originMap.ToLower()))
                {
                    Console.WriteLine($"Erreur : Carte '{originMap}' introuvable !");
                    return;
                }

                InteractDoor baseDoor = doorDictionary[idDoor];
                InteractDoor newDoor = new InteractDoor(
                    baseDoor.IdDoor,
                    baseDoor.OriginColonne,
                    baseDoor.OriginLigne,
                    baseDoor.OriginMap,
                    baseDoor.TargetColonne,
                    baseDoor.TargetLigne,
                    baseDoor.TargetMap
                );

                ActiveDoors.Add(newDoor);
                Console.WriteLine($"Une porte ID {idDoor} a été ajoutée sur {originMap} en ({originColonne}, {originLigne}) !");
                Console.WriteLine($"Elle mène à {targetMap} en ({targetColonne}, {targetLigne}) !");
                Console.WriteLine($"Portes actives : {ActiveDoors.Count}");
            }
            else
            {
                Console.WriteLine($"Erreur : ID {idDoor} inconnu, impossible de créer la porte !");
            }
        }
        public static void ExecuteNpcTurn(Player player, MapInfo map)
        {
            foreach (var npc in ActiveNpcs)
            {
                if (npc.Type == "ennemy")
                {
                    AiManager.UpdateBehavior(npc, player, map, ActiveNpcs);
                }
            }
        }


    }
}