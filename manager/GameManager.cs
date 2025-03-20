using DarkRequiem.map;
using DarkRequiem.npc;
using DarkRequiem.interact;
using System;
using System.Collections.Generic;
using System.Linq;
using DarkRequiem.player;

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
            MapInfo currentMap = SceneManager.GetCurrentMap();

            if (currentMap == null)
            {
                Console.WriteLine("currentMap == null");
                return;
            }

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
                Npc newMonster = new Npc(baseNpc.Id, baseNpc.SpriteID, baseNpc.Name, baseNpc.Type, baseNpc.MaxHp, baseNpc.Hp, baseNpc.Attack, baseNpc.Defense, baseNpc.TextureType)
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

        public static void CreateNewDoor(int idDoor, int originLigne, int originColonne, string originMap, int targetColonne, int targetLigne, string targetMap)
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
        public static void ExecuteNpcTurn(Player player)
        {
            foreach (var npc in ActiveNpcs)
            {
                // Vérifie si le joueur est adjacent au NPC
                if (Math.Abs(npc.Colonne - player.colonne) + Math.Abs(npc.Ligne - player.ligne) == 1)
                {
                    if (npc.Type == "ennemy")
                    {
                        Console.WriteLine($"{npc.Name} attaque {player.Name} durant son tour !");
                        player.TakeDamage(npc.DealDamage());
                        Console.WriteLine($"{player.Name} a maintenant {player.Hp}/{player.MaxHp} HP.");

                        if (!player.IsAlive())
                        {
                            Console.WriteLine("Game Over !");
                            // Ajoute une logique claire de Game Over ici
                        }
                    }
                }

                //logique de mouvement pour les NPCs
            }
        }

    }
}