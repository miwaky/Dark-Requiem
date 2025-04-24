using Raylib_cs;
using System.Numerics;
using DarkRequiem.manager;
using DarkRequiem.scenes;

namespace DarkRequiem.scene
{

    public class MenuScene : IScene
    {
        public string Name => "Menu";

        private Rectangle startButton = new Rectangle(70, 232, 218, 55);
        private Rectangle settingsButton = new Rectangle(70, 302, 218, 55);
        private Rectangle quitButton = new Rectangle(70, 376, 218, 55);
        private Rectangle controlButton = new Rectangle(549, 480, 130, 23);

        public Texture2D background;
        public MenuScene()
        {
            background = Raylib.LoadTexture("assets/images/background/Background_Menu.png");
            AudioManager.PlayMusic("menu"); // ou une musique de menu si différente
        }


        public void Update()
        {
            AudioManager.Update();

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
                else if (Raylib.CheckCollisionPointRec(mouse, settingsButton))
                {
                    SceneManager.SetScene(new SettingsScene(this));
                }
                else if (Raylib.CheckCollisionPointRec(mouse, controlButton))
                {
                    SceneManager.SetScene(new ControlScene(this));
                }
            }
        }

        public void Draw()
        {

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            Rectangle source = new Rectangle(0, 0, background.Width, background.Height);
            Rectangle dest = new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
            Raylib.DrawTexturePro(background, source, dest, Vector2.Zero, 0f, Color.White);

            Raylib.DrawRectangleRec(startButton, new Color(50, 50, 50, 0));

            Raylib.DrawRectangleRec(settingsButton, new Color(50, 50, 50, 0));

            Raylib.DrawRectangleRec(quitButton, new Color(50, 50, 50, 0));

            Raylib.DrawRectangleRec(controlButton, new Color(50, 50, 50, 0));
            // Vector2 mouse = Raylib.GetMousePosition();
            // string mouseText = $"Souris : {mouse.X:0}, {mouse.Y:0}";
            // Raylib.DrawText(mouseText, 10, 10, 20, Color.Yellow);

            Raylib.EndDrawing();
        }
    }
}