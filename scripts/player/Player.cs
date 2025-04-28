using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using DarkRequiem.npc;
using DarkRequiem.objects;
using DarkRequiem.manager;

namespace DarkRequiem.player
{
    public class Player : Character
    {
        public int colonne;
        public int ligne;

        public Texture2D TextureIdle;
        public Texture2D TextureRun;
        public Texture2D TextureSword;

        public Vector2 PositionPixel;
        public Vector2 TargetPositionPixel;

        private float animationTimer;
        private int currentFrame;
        private float frameDuration = 0.2f;

        public enum PlayerState { Idle, Run, Attack }
        public enum Direction { Down, Right, Up, Left }

        public PlayerState State { get; private set; }
        public Direction CurrentDirection { get; private set; }
        public int nbrframeRun { get; private set; } = 4;
        public int nbrframeIdle { get; private set; } = 4;
        public int nbrframeAttack { get; private set; } = 4;

        public Inventory Inventory { get; private set; }

        public int Endurance { get; set; } = 20;
        private int turnCounter = 0;
        public bool IsDashing { get; private set; } = false;

        public bool IsAttacking => State == PlayerState.Attack;

        public Player(string name, string type, int maxHp, int hp, int attack, int defense, int money, int nbrPotion, int startCol, int startLigne)
      : base(name, type, maxHp, hp, attack, defense)
        {
            Inventory = new Inventory();
            Inventory.AddItem("gold", "currency", money);
            Inventory.AddItem("potion", "consumable", nbrPotion);

            colonne = startCol;
            ligne = startLigne;
            TextureIdle = LoadTexture("assets/images/characters/Character_Idle.png");
            TextureRun = LoadTexture("assets/images/characters/Character_Run.png");
            TextureSword = LoadTexture("assets/images/characters/Character_sword.png");
            PositionPixel = new Vector2(colonne * 16, ligne * 16);
            TargetPositionPixel = PositionPixel;

            State = PlayerState.Idle;
            CurrentDirection = Direction.Down;
            animationTimer = 0;
            currentFrame = 0;
        }

        public static Player GeneratePlayer(int colonne, int ligne)
        {
            return new Player("Hero", "Player", 16, 16, 2, 0, 0, 0, colonne, ligne);
        }

        public void StartAttack(Direction direction)
        {
            CurrentDirection = direction;
            State = PlayerState.Attack;
            animationTimer = 0f;
            currentFrame = 0;
        }

        public void Close()
        {
            UnloadTexture(TextureIdle);
            UnloadTexture(TextureRun);
            UnloadTexture(TextureSword);
        }

        public void UpdateTurnRecovery()
        {
            turnCounter++;
            if (turnCounter >= 5)
            {
                turnCounter = 0;
                if (Endurance < 10) Endurance++;
            }
        }

        public bool CanDash()
        {
            return Endurance >= 3;
        }

        public void ConsumeDash()
        {
            Endurance -= 3;
            IsDashing = true;
        }

        public void EndDash()
        {
            IsDashing = false;
        }

        public void UpdatePositionSmooth(float deltaTime)
        {
            if (IsAttacking) return;

            float speed = 10f;
            PositionPixel = Vector2.Lerp(PositionPixel, TargetPositionPixel, speed * deltaTime);
        }

        public void UpdateAnimation(float deltaTime, bool isMoving, Direction inputDirection)
        {
            if (!IsAttacking)
            {
                State = isMoving ? PlayerState.Run : PlayerState.Idle;
                CurrentDirection = inputDirection;
            }

            animationTimer += deltaTime;

            int maxFrames = State switch
            {
                PlayerState.Attack => nbrframeAttack,
                PlayerState.Run => nbrframeRun,
                _ => nbrframeIdle
            };

            if (animationTimer >= frameDuration)
            {
                currentFrame = (currentFrame + 1) % maxFrames;
                animationTimer = 0f;

                if (State == PlayerState.Attack && currentFrame == 0)
                {
                    State = PlayerState.Idle;

                    foreach (var dead in GameManager.PendingKills)
                    {
                        GameManager.RemoveNpc(dead.Id, dead.MapName, dead.Colonne, dead.Ligne);
                    }

                    GameManager.PendingKills.Clear();
                }
            }
        }

        public void Draw()
        {
            Texture2D texture = State switch
            {
                PlayerState.Attack => TextureSword,
                PlayerState.Run => TextureRun,
                _ => TextureIdle
            };

            int frameWidth = State == PlayerState.Attack ? 32 : 16;
            int frameHeight = State == PlayerState.Attack ? 32 : 32;

            int row = State switch
            {
                PlayerState.Attack => CurrentDirection switch
                {
                    Direction.Down => 0,
                    Direction.Up => 1,
                    Direction.Right => 2,
                    Direction.Left => 3,
                    _ => 0
                },
                PlayerState.Run => CurrentDirection switch
                {
                    Direction.Down => 0,
                    Direction.Right => 1,
                    Direction.Up => 2,
                    Direction.Left => 3,
                    _ => 0
                },
                _ => CurrentDirection switch
                {
                    Direction.Down => 0,
                    Direction.Right => 1,
                    Direction.Up => 2,
                    Direction.Left => 3,
                    _ => 0
                }
            };

            currentFrame %= State switch
            {
                PlayerState.Attack => nbrframeAttack,
                PlayerState.Run => nbrframeRun,
                _ => nbrframeIdle
            };

            Rectangle sourceRect = new Rectangle(
                currentFrame * frameWidth,
                row * frameHeight,
                frameWidth,
                frameHeight
            );

            Rectangle destRect = new Rectangle(
                PositionPixel.X - (frameWidth - 16) / 2f,
                PositionPixel.Y - (frameHeight - 16),
                frameWidth,
                frameHeight
            );

            DrawTexturePro(
                texture,
                sourceRect,
                destRect,
                Vector2.Zero,
                0f,
                Color.White
            );
        }
    }
}
