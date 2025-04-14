using Raylib_cs;
using System.Numerics;
using DarkRequiem.manager;

namespace DarkRequiem.scene
{
    public class MenuScene : IScene
    {
        public string Name => "Menu";
        private Rectangle startButton = new Rectangle(300, 200, 200, 60);
        private Rectangle quitButton = new Rectangle(300, 280, 200, 60);

        public void Update()
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                Vector2 mouse = Raylib.GetMousePosition();
                if (Raylib.CheckCollisionPointRec(mouse, startButton))
                {
                    SceneManager.SetScene(new GameplayScene());
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
            Raylib.ClearBackground(Color.Black);

            Raylib.DrawText("DARK REQUIEM", 250, 100, 40, Color.RayWhite);
            Raylib.DrawRectangleRec(startButton, Color.DarkGray);
            Raylib.DrawText("Jouer", (int)startButton.X + 60, (int)startButton.Y + 15, 20, Color.White);

            Raylib.DrawRectangleRec(quitButton, Color.DarkGray);
            Raylib.DrawText("Quitter", (int)quitButton.X + 50, (int)quitButton.Y + 15, 20, Color.White);

            Raylib.EndDrawing();

        }
    }
}