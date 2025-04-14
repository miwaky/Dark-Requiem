using Raylib_cs;
using System.Numerics;
using DarkRequiem.scene;
using DarkRequiem.manager;

namespace DarkRequiem.scenes
{
    public class GameOverScene : IScene
    {
        public string Name => "GameOver";

        private Rectangle retryButton = new Rectangle(300, 220, 200, 50);
        private Rectangle menuButton = new Rectangle(300, 290, 200, 50);
        private Rectangle quitButton = new Rectangle(300, 360, 200, 50);

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
                else if (Raylib.CheckCollisionPointRec(mouse, quitButton))
                {
                    ExitManager.ShouldQuit = true;
                }
            }
        }

        public void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkBlue);

            Raylib.DrawText("GAME OVER", 280, 120, 50, Color.White);

            Raylib.DrawRectangleRec(retryButton, Color.Gray);
            Raylib.DrawText("Rejouer", (int)retryButton.X + 50, (int)retryButton.Y + 15, 20, Color.White);

            Raylib.DrawRectangleRec(menuButton, Color.Gray);
            Raylib.DrawText("Menu", (int)menuButton.X + 70, (int)menuButton.Y + 15, 20, Color.White);

            Raylib.DrawRectangleRec(quitButton, Color.Gray);
            Raylib.DrawText("Quitter", (int)quitButton.X + 60, (int)quitButton.Y + 15, 20, Color.White);

            Raylib.EndDrawing();
        }
    }
}
