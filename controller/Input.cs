using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using DarkRequiem.npc;
using DarkRequiem.interact;
using DarkRequiem.map;
using DarkRequiem.manager;
namespace DarkRequiem.controller
{
    public class Input
    {
        private Player _player;
        private Map _map;
        public int tuileSouslesPieds;
        public int ObstacleCollision;
        public int Obstacle2Collision;
        public Npc? collidedNpc;
        public InteractDoor? collidedDoors;
        public int WallCollision;
        private float lastMoveTime = 0;
        private const float moveCooldown = 0.2f; // 200 ms (0.2 secondes)
        private bool down, up, left, right;

        public int nouvelleColonne;
        public int nouvelleLigne;

        public Input(Player player, Map map)
        {
            _player = player;
            _map = map;
            nouvelleColonne = player.colonne;
            nouvelleLigne = player.ligne;
        }

        public void Movement()
        {
            float currentTime = (float)GetTime(); // Récupère le temps actuel en secondes

            if (currentTime - lastMoveTime < moveCooldown)
            {
                return; // Bloque le mouvement tant que le cooldown n'est pas écoulé
            }

            bool moved = false;
            down = false;
            up = false;
            left = false;
            right = false;

            if (IsKeyDown(KeyboardKey.Left) || IsKeyDown(KeyboardKey.A))
            {
                nouvelleColonne--; // Déplacement vers la gauche
                left = true;
                moved = true;
                Console.WriteLine($"Déplacement gauche : {nouvelleColonne}, {nouvelleLigne}");

            }
            if (IsKeyDown(KeyboardKey.Right) || IsKeyDown(KeyboardKey.D))
            {
                nouvelleColonne++; // Déplacement vers la droite
                right = true;
                moved = true;
            }
            if (IsKeyDown(KeyboardKey.Up) || IsKeyDown(KeyboardKey.W))
            {
                nouvelleLigne--; // Déplacement vers le haut
                up = true;
                moved = true;
            }
            if (IsKeyDown(KeyboardKey.Down) || IsKeyDown(KeyboardKey.S))
            {
                nouvelleLigne++; // Déplacement vers le bas*
                down = true;
                moved = true;
            }
            if (moved)
            {
                // Vérification des limites de la carte
                nouvelleColonne = Math.Clamp(nouvelleColonne, 0, _map.Largeur - 1);
                nouvelleLigne = Math.Clamp(nouvelleLigne, 0, _map.Hauteur - 1);

                if (_player.colonne != nouvelleColonne || _player.ligne != nouvelleLigne)
                {
                    ObstacleCollision = _map.InfoTuilles(nouvelleColonne, nouvelleLigne, "Obstacle");
                    Obstacle2Collision = _map.InfoTuilles(nouvelleColonne, nouvelleLigne, "Obstacle2");
                    WallCollision = _map.InfoTuilles(nouvelleColonne, nouvelleLigne, "Wall");
                    // Vérifier s'il y a un NPC à la nouvelle position
                    collidedNpc = GameManager.ActiveNpcs.FirstOrDefault(n => n.Colonne == nouvelleColonne && n.Ligne == nouvelleLigne);
                    collidedDoors = GameManager.ActiveDoors.FirstOrDefault(n => n.OriginColonne == nouvelleColonne && n.OriginLigne == nouvelleLigne);

                    if (collidedNpc != null) //  Si un NPC est trouvé
                    {
                        Console.WriteLine($" Collision avec {collidedNpc.Name} ({collidedNpc.Type}) en ({collidedNpc.Colonne}, {collidedNpc.Ligne})");

                        if (collidedNpc.Type == "ennemy") //  Vérifier si c'est un ennemi
                        {
                            Console.WriteLine($" Combat contre {collidedNpc.Name} !");
                            Battle.StartCombat(_player, collidedNpc);
                            ResetMove();
                        }
                        else if (collidedNpc.Type == "allie")
                        {
                            InteractNpc.InitTalk(collidedNpc);
                            ResetMove();
                        }
                    }
                    else if (collidedDoors != null)
                    {
                        InteractDoor.InitDoor(_player, collidedNpc);
                        ResetMove();
                    }

                }

                if (Obstacle2Collision == 0 && ObstacleCollision == 0 && WallCollision == 0)
                {
                    int tuile = _map.InfoTuilles(nouvelleColonne, nouvelleLigne, "Sol");

                    if (tuile == 175 || tuile == 192 || tuile == 200)
                    {
                        _player.colonne = nouvelleColonne;
                        _player.ligne = nouvelleLigne;
                        Console.WriteLine("Move");

                        Console.WriteLine($"Position mise à jour : {_player.colonne}, {_player.ligne}");
                        lastMoveTime = currentTime;
                    }
                }
                else
                {
                    ResetMove();
                }
                down = false;
                up = false;
                left = false;
                right = false;
            }

        }

        public void DrawDebug()
        {
            int tuileSouslesPieds = _map.InfoTuilles(nouvelleColonne, nouvelleLigne, "Sol");
            DrawText("Tuile sous les pieds : " + tuileSouslesPieds, 10, 30, 20, Color.White);
            if (collidedNpc != null)
            {
                DrawText("NPC : " + collidedNpc.Name, 10, 70, 20, Color.White);
            }
            else
            {
                DrawText("NPC : Aucun", 10, 70, 20, Color.White);
            }
        }

        public void ResetMove()
        {
            if (down == true) nouvelleLigne--;
            if (up == true) nouvelleLigne++;
            if (left == true) nouvelleColonne++;
            if (right == true) nouvelleColonne--;
        }

        public void UpdateMapReference(Map newMap)
        {
            _map = newMap;
            Console.WriteLine($" Carte mise à jour dans Input.cs : {_map.NomCarte}");
        }

    }
}