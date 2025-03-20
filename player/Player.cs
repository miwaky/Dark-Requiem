using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;
using DarkRequiem.npc;

namespace DarkRequiem.player
{
    public class Player : CharacterManager
    {
        public int colonne;
        public int ligne;
        public Texture2D TextureIdle;
        public Texture2D TextureRun;
        public Vector2 PositionPixel;
        public Vector2 TargetPositionPixel;

        private float animationTimer;
        private int currentFrame;
        private float frameDuration = 0.5f;

        public enum PlayerState { Idle, Run }
        public enum Direction { Down, Left, Right, Up }

        public PlayerState State { get; private set; }
        public Direction CurrentDirection { get; private set; }

        public Player(string name, string type, int maxHp, int hp, int attack, int defense, int startCol, int startLigne)
            : base(name, type, maxHp, hp, attack, defense)
        {
            colonne = startCol;
            ligne = startLigne;
            TextureIdle = LoadTexture("assets/images/characters/Character_Idle.png");
            TextureRun = LoadTexture("assets/images/characters/Character_Run.png");
            PositionPixel = new Vector2(colonne * 16, ligne * 16);
            TargetPositionPixel = PositionPixel;

            State = PlayerState.Idle;
            CurrentDirection = Direction.Down;
            animationTimer = 0;
            currentFrame = 0;
        }

        public static Player GeneratePlayer(int colonne, int ligne)
        {
            Player hero = new Player("Hero", "Player", 100, 100, 15, 5, colonne, ligne);
            hero.colonne = colonne;
            hero.ligne = ligne;
            hero.PositionPixel = new Vector2(colonne * 16, ligne * 16);
            hero.TargetPositionPixel = hero.PositionPixel;
            return hero;
        }

        public void Close()
        {
            UnloadTexture(TextureIdle);
            UnloadTexture(TextureRun);
        }

        public void UpdatePositionSmooth(float deltaTime)
        {
            float speed = 10f;
            PositionPixel = Vector2.Lerp(PositionPixel, TargetPositionPixel, speed * deltaTime);

            // Alignement final clair
            if (Vector2.Distance(PositionPixel, TargetPositionPixel) < 1f)
                PositionPixel = TargetPositionPixel;
        }

        public void UpdateAnimation(float deltaTime, bool isMoving, Direction inputDirection)
        {
            if (isMoving)
            {
                State = PlayerState.Run;
                CurrentDirection = inputDirection;
            }
            else
            {
                State = PlayerState.Idle;
                currentFrame = 0; // réinitialise l'animation idle clairement
            }

            animationTimer += deltaTime;
            int maxFrames = (State == PlayerState.Run) ? 6 : 4;

            if (animationTimer >= frameDuration)
            {
                currentFrame++;
                if (currentFrame >= maxFrames)
                    currentFrame = 0;
                animationTimer = 0f;
            }
        }

        public void Draw()
        {
            Texture2D texture = (State == PlayerState.Run) ? TextureRun : TextureIdle;

            int frameWidth = 192 / 4;
            int frameHeight = 192 / 4;

            int row = (int)CurrentDirection; // Ligne verticale pour la direction
            int column = currentFrame;       // Colonne horizontale pour l'animation

            // Rectangle source corrigé (frames en horizontal, direction en vertical)
            Rectangle sourceRect = new Rectangle(
                column * frameWidth,     // X : progression des frames
                row * frameHeight,       // Y : direction
                frameWidth,
                frameHeight
            );

            float scale = 0.5f;


            Vector2 position = new Vector2(PositionPixel.X, PositionPixel.Y);

            DrawTexturePro(
                texture,
                sourceRect,
                new Rectangle(position.X, position.Y, frameWidth * scale, frameHeight * scale),
                Vector2.Zero,
                0f,
                Color.White
            );
        }


    }
}