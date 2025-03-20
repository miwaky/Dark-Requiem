using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using DarkRequiem.npc;
using DarkRequiem.interact;
using DarkRequiem.map;
using DarkRequiem.manager;
using DarkRequiem.player;

namespace DarkRequiem.controller
{
    public class Input
    {
        private RenduMap _renduMap;
        private Player _player;
        public MapInfo _map;

        public Npc? collidedNpc;
        public InteractDoor? collidedDoors;

        private float lastMoveTime = 0;
        private const float moveCooldown = 0.15f;

        public int nouvelleColonne;
        public int nouvelleLigne;

        private bool inputLocked = false; // Verrouillage des inputs
        private bool keyPreviouslyPressed = false; // état précédent des touches

        public Player.Direction CurrentDirection { get; private set; } = Player.Direction.Down;

        public Input(Player player, MapInfo map, RenduMap renduMap)
        {
            _player = player;
            _map = map;
            _renduMap = renduMap;
            nouvelleColonne = player.colonne;
            nouvelleLigne = player.ligne;
        }

        public void Movement()
        {
            float currentTime = (float)GetTime();
            if (currentTime - lastMoveTime < moveCooldown) return;

            bool moved = false;
            nouvelleColonne = _player.colonne;
            nouvelleLigne = _player.ligne;

            Player.Direction intendedDirection = CurrentDirection; // direction temporaire

            if (IsKeyDown(KeyboardKey.Left) || IsKeyDown(KeyboardKey.A)) { nouvelleColonne--; moved = true; intendedDirection = Player.Direction.Left; }
            else if (IsKeyDown(KeyboardKey.Right) || IsKeyDown(KeyboardKey.D)) { nouvelleColonne++; moved = true; intendedDirection = Player.Direction.Right; }
            else if (IsKeyDown(KeyboardKey.Up) || IsKeyDown(KeyboardKey.W)) { nouvelleLigne--; moved = true; intendedDirection = Player.Direction.Up; }
            else if (IsKeyDown(KeyboardKey.Down) || IsKeyDown(KeyboardKey.S)) { nouvelleLigne++; moved = true; intendedDirection = Player.Direction.Down; }

            if (moved)
            {
                nouvelleColonne = Math.Clamp(nouvelleColonne, 0, _map.Largeur - 1);
                nouvelleLigne = Math.Clamp(nouvelleLigne, 0, _map.Hauteur - 1);

                bool canMove = Collider.CheckAll(_player, _map, nouvelleColonne, nouvelleLigne, ref _renduMap, this);

                // Met à jour clairement la direction, même si bloqué
                CurrentDirection = intendedDirection;

                if (canMove)
                {
                    _player.colonne = nouvelleColonne;
                    _player.ligne = nouvelleLigne;
                    _player.TargetPositionPixel = new Vector2(_player.colonne * 16, _player.ligne * 16);
                }

                GameManager.ExecuteNpcTurn(_player);
                Collider.ResetNpcCombatIfNeeded(this);
                collidedNpc = null;

                lastMoveTime = currentTime;
            }
        }


        public void Action()
        {
            if (IsKeyPressed(KeyboardKey.H))
            {
                Potion.HealPlayer(ref _player, 20);

                // Déclenche le tour ennemi après le soin :
                GameManager.ExecuteNpcTurn(_player);

                //après chaque tour :
                Collider.ResetNpcCombatIfNeeded(this);
                collidedNpc = null;
            }
        }
        public void DrawDebug()
        {
            int tuileSouslesPieds = _map.InfoTuilles(nouvelleColonne, nouvelleLigne, "Movable");
            DrawText("Tuile sous les pieds : " + tuileSouslesPieds, 10, 30, 20, Color.White);
            DrawText(collidedNpc != null ? "NPC : " + collidedNpc.Name : "NPC : Aucun", 10, 70, 20, Color.White);
        }

        public void UpdateMapReference(MapInfo newMap)
        {
            _map = newMap;
            Console.WriteLine($"Mise à jour de Input.cs avec la carte : {newMap.NomCarte}");
        }
    }
}
