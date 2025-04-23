using Raylib_cs;
using System.Numerics;
using DarkRequiem.manager;
using DarkRequiem.scenes;

namespace DarkRequiem.scene
{

    public class MenuScene : IScene
    {
        public string Name => "Menu";

        private Rectangle startButton = new Rectangle(30, 223, 257, 56);
        private Rectangle settingsButton = new Rectangle(30, 294, 257, 56);
        private Rectangle quitButton = new Rectangle(30, 369, 257, 56);
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
            }
        }

        public void Draw()
        {

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            Rectangle source = new Rectangle(0, 0, background.Width, background.Height);
            Rectangle dest = new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
            Raylib.DrawTexturePro(background, source, dest, Vector2.Zero, 0f, Color.White);

            Raylib.DrawRectangleRec(startButton, new Color(50, 50, 50, 128));

            Raylib.DrawRectangleRec(settingsButton, new Color(50, 50, 50, 128));

            Raylib.DrawRectangleRec(quitButton, new Color(50, 50, 50, 128));

            Raylib.EndDrawing();
        }
    }
}