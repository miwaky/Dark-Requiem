using DarkRequiem.map;
using DarkRequiem.manager;
using DarkRequiem.interact;
using DarkRequiem.player;

namespace DarkRequiem.controller
{
    public static class Collider
    {
        public static bool CheckAll(Player player, MapInfo map, int nouvelleCol, int nouvelleLig, ref RenduMap renduMap, Input input)
        {
            var npcSurNouvelleCase = GameManager.ActiveNpcs.FirstOrDefault(n => n.Colonne == nouvelleCol && n.Ligne == nouvelleLig);

            // =============== NPC COLLISION ===============
            if (npcSurNouvelleCase != null)
            {
                //Console.WriteLine($"Collision avec {npcSurNouvelleCase.Name} ({npcSurNouvelleCase.Type}) en ({npcSurNouvelleCase.Colonne}, {npcSurNouvelleCase.Ligne})");

                if (npcSurNouvelleCase.Type == "ennemy")
                {
                    //Console.WriteLine("Attack");
                    AudioManager.Play("attack");

                    // Détermine la direction vers l’ennemi
                    Player.Direction newDirection = player.CurrentDirection;

                    if (player.colonne < npcSurNouvelleCase.Colonne)
                        newDirection = Player.Direction.Right;
                    else if (player.colonne > npcSurNouvelleCase.Colonne)
                        newDirection = Player.Direction.Left;
                    else if (player.ligne < npcSurNouvelleCase.Ligne)
                        newDirection = Player.Direction.Down;
                    else if (player.ligne > npcSurNouvelleCase.Ligne)
                        newDirection = Player.Direction.Up;

                    // Met à jour l’orientation du joueur
                    player.StartAttack(newDirection);

                    // Et applique l’attaque
                    Battle.Attack(player, npcSurNouvelleCase);
                }
                else if (npcSurNouvelleCase.Type == "allie")
                {
                    InteractNpc.InitTalk(npcSurNouvelleCase);
                }

                input.collidedNpc = npcSurNouvelleCase;
                return false; // Bloque le tour
            }
            else
            {
                input.collidedNpc = null;
            }

            // =============== BREAKABLE TILE COLLISION ===============
            int breakableTile = map.InfoTuilles(nouvelleCol, nouvelleLig, "Breakable");

            if (breakableTile != 0)
            {
                AudioManager.Play("attack");

                // Déterminer la direction vers la tuile cassable
                Player.Direction direction = player.CurrentDirection;

                if (player.colonne < nouvelleCol)
                    direction = Player.Direction.Right;
                else if (player.colonne > nouvelleCol)
                    direction = Player.Direction.Left;
                else if (player.ligne < nouvelleLig)
                    direction = Player.Direction.Down;
                else if (player.ligne > nouvelleLig)
                    direction = Player.Direction.Up;

                player.StartAttack(direction);

                // Détruire et loot
                BreakableObject.HandleBreakableTile(map, nouvelleCol, nouvelleLig);
                return false;
            }

            // =============== PORTES =================================
            var doorCollision = GameManager.ActiveDoors.FirstOrDefault(d => d.OriginColonne == nouvelleCol && d.OriginLigne == nouvelleLig);
            if (doorCollision != null)
            {
                JsonManager.CollidedDoorCheck(player, ref renduMap, ref map, input, doorCollision);
                return false;
            }
            // =============== OBJET (Potion / monney) ===============
            var objetRamassable = GameManager.ActiveObjects.FirstOrDefault(o => o.colonne == nouvelleCol && o.ligne == nouvelleLig);

            if (objetRamassable != null)
            {
                //Console.WriteLine($"Objet ramassé : {objetRamassable.name} en ({nouvelleCol}, {nouvelleLig})");

                if (objetRamassable is Potion potion)
                {
                    InventoryHelper.AddPotion(ref player, 1);
                }
                else if (objetRamassable is Money money)
                {
                    InventoryHelper.AddMoney(ref player, money.moneyAdded);
                }
                GameManager.ActiveObjects.Remove(objetRamassable);
                return true; // Le joueur peut continuer son tour
            }

            // =============== Coffre ===============

            var chest = GameManager.ActiveChests.FirstOrDefault(c => c.Colonne == nouvelleCol && c.Ligne == nouvelleLig &&
             c.MapName.ToLower() == map.NomCarte.ToLower());

            if (chest != null)
            {
                chest.Open(player);
                return false;
            }

            // =============== Event (Switch) ===============
            // Vérifie s'il y a un switch actif sur cette case
            var triggeredSwitch = GameManager.ActiveSwitches.FirstOrDefault(sw =>
                sw.Colonne == nouvelleCol && sw.Ligne == nouvelleLig &&
                sw.MapName.ToLower() == map.NomCarte.ToLower());

            // Si le switch est trouvé ET non activé, on tente de l'activer
            if (triggeredSwitch != null && !triggeredSwitch.IsActivated)
            {
                triggeredSwitch.Activate();
                return false; // On bloque le tour le temps que le switch se déclenche
            }
            // =============== collision avec objet bloquant (comme un switch ou un obstacle de switch) ===============
            // Vérifie les tuiles physiques une fois le test logique fait
            int switchObstacle = map.InfoTuilles(nouvelleCol, nouvelleLig, "SwitchObstacle");
            int Switch = map.InfoTuilles(nouvelleCol, nouvelleLig, "Switch");
            int SwitchActivated = map.InfoTuilles(nouvelleCol, nouvelleLig, "SwitchActivate");

            // On bloque la case uniquement s'il n'y a pas de switch scripté sur cette tuile
            if ((switchObstacle > 0 || Switch > 0 || SwitchActivated > 0) && triggeredSwitch == null)
            {
                //Console.WriteLine($"[COLLISION] Le joueur est bloqué par une tuile d'obstacle switch à ({nouvelleCol}, {nouvelleLig})");
                return false;
            }
            // =============== Aucune collision ===============
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
