using DarkRequiem.map;
using DarkRequiem.npc;
using DarkRequiem.interact;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarkRequiem.manager
{
    class GameManager
    {
        public static List<Npc> ActiveNpcs = new List<Npc>();
        public static List<InteractDoor> ActiveDoors = new List<InteractDoor>();

        public static void InitializeNewScene()
        {
            ActiveNpcs.Clear();
            ActiveDoors.Clear();
        }

        public static void LoadForCurrentMap()
        {
            Map currentMap = SceneManager.GetCurrentMap();
            if (currentMap == null) return;

            InitializeNewScene(); // Nettoie la liste avant de charger
            Console.WriteLine($"Chargement des NPCs et portes pour la carte : {currentMap.NomCarte}");

            // Chargement des NPCs
            foreach (var npcEntry in Npc.GetNpcDictionary())
            {
                Npc npc = npcEntry.Value;
                if (npc.MapName == currentMap.NomCarte)
                {
                    CreateNewMonster(npc.Id, npc.MapName, npc.Colonne, npc.Ligne);
                }
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

        public static void CreateNewMonster(int id, string mapName, int colonne, int ligne)
        {
            Dictionary<int, Npc> npcDictionary = Npc.GetNpcDictionary();

            if (npcDictionary.ContainsKey(id))
            {
                if (!SceneManager.LoadedMaps.ContainsKey(mapName.ToLower()))
                {
                    Console.WriteLine($"Erreur : Carte '{mapName}' introuvable !");
                    return;
                }

                Npc baseNpc = npcDictionary[id];
                Npc newMonster = new Npc(baseNpc.Id, baseNpc.SpriteID, baseNpc.Name, baseNpc.Type, baseNpc.HP, baseNpc.Attack, baseNpc.Defense)
                {
                    MapName = mapName,
                    Colonne = colonne,
                    Ligne = ligne
                };

                ActiveNpcs.Add(newMonster);
                Console.WriteLine($"Un {newMonster.Name} est apparu sur {newMonster.MapName} en ({colonne}, {ligne}) !");
                Console.WriteLine($"Monstres actifs : {ActiveNpcs.Count}");
            }
            else
            {
                Console.WriteLine($"Erreur : ID {id} inconnu, impossible de créer le monstre !");
            }
        }

        public static void CreateNewDoor(int idDoor, int originLigne, int originColonne, string originMap, int targetLigne, int targetColonne, string targetMap)
        {
            Dictionary<int, InteractDoor> doorDictionary = InteractDoor.GetDoorDictionary;

            if (doorDictionary.ContainsKey(idDoor))
            {
                if (!SceneManager.LoadedMaps.ContainsKey(originMap.ToLower()))
                {
                    Console.WriteLine($"Erreur : Carte '{originMap}' introuvable !");
                    return;
                }

                InteractDoor baseDoor = doorDictionary[idDoor];
                InteractDoor newDoor = new InteractDoor(
                    baseDoor.IdDoor,
                    baseDoor.OriginLigne,
                    baseDoor.OriginColonne,
                    baseDoor.OriginMap,
                    baseDoor.TargetLigne,
                    baseDoor.TargetColonne,
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
    }
}