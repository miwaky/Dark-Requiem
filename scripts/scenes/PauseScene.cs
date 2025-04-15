using Raylib_cs;
using System.Numerics;
using DarkRequiem.manager;
using DarkRequiem.scene;
using static Raylib_cs.Raylib;

namespace DarkRequiem.scenes
{
    public class PauseScene : IScene
    {
        public string Name => "Pause";
        private IScene previousScene;
        private Texture2D background;

        private Rectangle resumeButton = new Rectangle(32, 423, 222, 54);
        private Rectangle quitButton = new Rectangle(281, 423, 208, 54);
        private Rectangle settingsButton = new Rectangle(512, 423, 225, 54);

        public PauseScene(IScene fromScene)
        {
            previousScene = fromScene;
            background = LoadTexture("assets/images/background/Background_Paused.png");
        }

        public void Update()
        {
            if (IsMouseButtonPressed(MouseButton.Left))
            {
                Vector2 mouse = GetMousePosition();

                if (CheckCollisionPointRec(mouse, resumeButton))
                {
                    SceneManager.SetScene(previousScene);
                }
                else if (CheckCollisionPointRec(mouse, settingsButton))
                {
                    SceneManager.SetScene(new SettingsScene(this));
                }
                else if (CheckCollisionPointRec(mouse, quitButton))
                {
                    SceneManager.SetScene(new MenuScene());
                }
            }
        }

        public void Draw()
        {
            BeginDrawing();
            ClearBackground(Color.Black);

            Rectangle source = new Rectangle(0, 0, background.Width, background.Height);
            Rectangle dest = new Rectangle(0, 0, GetScreenWidth(), GetScreenHeight());
            DrawTexturePro(background, source, dest, Vector2.Zero, 0f, Color.White);

            // UI
            DrawRectangleRec(resumeButton, new Color(50, 50, 50, 128));

            DrawRectangleRec(settingsButton, new Color(50, 50, 50, 128));

            DrawRectangleRec(quitButton, new Color(50, 50, 50, 128));

            Vector2 mouse = GetMousePosition();
            string mouseText = $"Souris : {mouse.X:0}, {mouse.Y:0}";
            DrawText(mouseText, 10, 10, 20, Color.Yellow);

            EndDrawing();
        }
    }
}