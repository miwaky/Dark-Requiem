using Raylib_cs;
using System;
using System.Numerics;
using DarkRequiem.scene;
using DarkRequiem.manager;

namespace DarkRequiem.scenes
{
    public class GameOverScene : IScene
    {
        public string Name => "GameOver";

        private Rectangle retryButton = new Rectangle(112, 425, 249, 55);
        private Rectangle menuButton = new Rectangle(404, 425, 250, 55);
        public Texture2D background;

        public GameOverScene()
        {
            background = Raylib.LoadTexture("assets/images/background/Background_GameOver.png");
        }

        public void Update()
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                Vector2 mouse = Raylib.GetMousePosition();
                if (Raylib.CheckCollisionPointRec(mouse, retryButton))
                {
                    SceneManager.SetScene(new GameplayScene());
                }
                else if (Raylib.CheckCollisionPointRec(mouse, menuButton))
                {
                    SceneManager.SetScene(new MenuScene());
                }

            }
        }

        public void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkBlue);

            // Afficher le fond
            Rectangle source = new Rectangle(0, 0, background.Width, background.Height);
            Rectangle dest = new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
            Raylib.DrawTexturePro(background, source, dest, Vector2.Zero, 0f, Color.White);


            Raylib.DrawRectangleRec(retryButton, new Color(50, 50, 50, 50));

            Raylib.DrawRectangleRec(menuButton, new Color(50, 50, 50, 50));

            Raylib.EndDrawing();
        }
    }
}
