using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using DarkRequiem.npc;
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

        private float lastMoveTime = 0;
        private const float moveCooldown = 0.15f;

        public int nouvelleColonne;
        public int nouvelleLigne;

        private bool isChargingDash = false;

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
            if (_player.IsAttacking) return;

            bool moved = false;
            nouvelleColonne = _player.colonne;
            nouvelleLigne = _player.ligne;

            Player.Direction currentDirection = _player.CurrentDirection;

            if (IsKeyDown(KeyboardKey.Space))
            {
                if (!_player.IsDashing && _player.CanDash())
                {
                    isChargingDash = true;
                }
            }

            if (isChargingDash && !IsKeyDown(KeyboardKey.Space))
            {
                isChargingDash = false; // dash annulé si on relâche
                return;
            }

            int dirX = 0, dirY = 0;

            if (IsKeyDown(KeyboardKey.Down) || IsKeyDown(KeyboardKey.S))
            {
                currentDirection = Player.Direction.Down;
                dirY = 1;
                moved = true;
            }
            else if (IsKeyDown(KeyboardKey.Up) || IsKeyDown(KeyboardKey.W))
            {
                currentDirection = Player.Direction.Up;
                dirY = -1;
                moved = true;
            }
            else if (IsKeyDown(KeyboardKey.Right) || IsKeyDown(KeyboardKey.D))
            {
                currentDirection = Player.Direction.Right;
                dirX = 1;
                moved = true;
            }
            else if (IsKeyDown(KeyboardKey.Left) || IsKeyDown(KeyboardKey.A))
            {
                currentDirection = Player.Direction.Left;
                dirX = -1;
                moved = true;
            }

            float deltaTime = GetFrameTime();
            _player.UpdateAnimation(deltaTime, moved, currentDirection);

            if (moved)
            {
                int step1Col = _player.colonne + dirX;
                int step1Lig = _player.ligne + dirY;

                if (!Collider.CheckAll(_player, _map, step1Col, step1Lig, ref _renduMap, this))
                {
                    isChargingDash = false;
                    return;
                }

                int finalCol = step1Col;
                int finalLig = step1Lig;

                if (isChargingDash)
                {
                    int step2Col = step1Col + dirX;
                    int step2Lig = step1Lig + dirY;

                    if (Collider.CheckAll(_player, _map, step2Col, step2Lig, ref _renduMap, this))
                    {
                        finalCol = step2Col;
                        finalLig = step2Lig;
                    }

                    _player.ConsumeDash();
                    isChargingDash = false;
                }

                CurrentDirection = currentDirection;

                _player.colonne = Math.Clamp(finalCol, 0, _map.Largeur - 1);
                _player.ligne = Math.Clamp(finalLig, 0, _map.Hauteur - 1);
                _player.TargetPositionPixel = new Vector2(_player.colonne * 16, _player.ligne * 16);

                _player.EndDash();
                _player.UpdateTurnRecovery();

                GameManager.ExecuteNpcTurn(_player, _map);
                Collider.ResetNpcCombatIfNeeded(this);
                collidedNpc = null;
                lastMoveTime = currentTime;
            }
        }
        public void Action()
        {
            if (IsKeyPressed(KeyboardKey.H))
            {
                if (!_player.Inventory.HasItem("potion", 1)) return;

                _player.Inventory.RemoveItem("potion", 1);
                Potion.HealPlayer(ref _player, 20);

                _player.UpdateTurnRecovery();
                GameManager.ExecuteNpcTurn(_player, _map);
                Collider.ResetNpcCombatIfNeeded(this);
                collidedNpc = null;
            }
        }

        public void DrawDebug()
        {
            // int tuileSouslesPieds = _map.InfoTuilles(nouvelleColonne, nouvelleLigne, "Movable");
            // DrawText("Tuile sous les pieds : " + tuileSouslesPieds, 10, 30, 20, Color.White);
            // DrawText(collidedNpc != null ? "NPC : " + collidedNpc.Name : "NPC : Aucun", 10, 70, 20, Color.White);
            DrawText($"Endurance : {_player.Endurance}", 10, 50, 20, Color.White);
        }

        public void UpdateMapReference(MapInfo newMap)
        {
            _map = newMap;
            // Console.WriteLine($"Mise à jour de Input.cs avec la carte : {newMap.NomCarte}");
        }
    }
}