using DarkRequiem.map;
using DarkRequiem.manager;
using DarkRequiem.interact;
using DarkRequiem.player;
using System.Linq;

namespace DarkRequiem.controller
{
    public static class Collider // << Classe ajoutée ici
    {
        public static bool CheckAll(Player player, MapInfo map, int nouvelleCol, int nouvelleLig, ref RenduMap renduMap, Input input)
        {
            var npcSurNouvelleCase = GameManager.ActiveNpcs.FirstOrDefault(n => n.Colonne == nouvelleCol && n.Ligne == nouvelleLig);

            if (npcSurNouvelleCase != null)
            {
                Console.WriteLine($"Collision avec {npcSurNouvelleCase.Name} ({npcSurNouvelleCase.Type}) en ({npcSurNouvelleCase.Colonne}, {npcSurNouvelleCase.Ligne})");

                if (npcSurNouvelleCase.Type == "ennemy")
                {
                    // Attaque simple (1 attaque par collision)
                    Battle.ExecuteSingleAttack(player, npcSurNouvelleCase);
                }
                else if (npcSurNouvelleCase.Type == "allie")
                {
                    InteractNpc.InitTalk(npcSurNouvelleCase);
                }

                input.collidedNpc = npcSurNouvelleCase;
                return false; // Bloque temporairement la tuile NPC
            }
            else
            {
                input.collidedNpc = null;
            }

            var doorCollision = GameManager.ActiveDoors.FirstOrDefault(d => d.OriginColonne == nouvelleCol && d.OriginLigne == nouvelleLig);
            if (doorCollision != null)
            {
                SceneManager.CollidedDoorCheck(player, ref renduMap, ref map, input, doorCollision);
                return false;
            }

            return map.InfoTuilles(nouvelleCol, nouvelleLig, "Movable") == 2;
        }

        // reset le flag si plus sur la case du NPC
        public static void ResetNpcCombatIfNeeded(Input input)
        {
            foreach (var npc in GameManager.ActiveNpcs)
            {
                if (npc.Colonne != input.nouvelleColonne || npc.Ligne != input.nouvelleLigne)
                {
                    npc.HasFought = false; // On reset si joueur a quitté la case
                }
            }
        }
    }
}
