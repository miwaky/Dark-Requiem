using Raylib_cs;
using System.Numerics;
using DarkRequiem.manager;
using DarkRequiem.scene;
using static Raylib_cs.Raylib;

namespace DarkRequiem.scenes
{
    public class ControlScene : IScene
    {
        public string Name => "Control";
        private Texture2D background;

        private Rectangle MenuButton = new Rectangle(273, 435, 222, 53);
        public ControlScene(IScene fromScene)
        {
            background = LoadTexture("assets/images/background/Background_Control.png");
        }

        public void Update()
        {
            if (IsMouseButtonPressed(MouseButton.Left))
            {
                Vector2 mouse = GetMousePosition();

                if (CheckCollisionPointRec(mouse, MenuButton))
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
            DrawRectangleRec(MenuButton, new Color(50, 50, 50, 0));

            // Vector2 mouse = GetMousePosition();
            // string mouseText = $"Souris : {mouse.X:0}, {mouse.Y:0}";
            // DrawText(mouseText, 10, 10, 20, Color.Yellow);

            EndDrawing();
        }
    }
}