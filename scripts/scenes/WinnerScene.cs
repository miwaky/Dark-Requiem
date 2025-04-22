using Raylib_cs;
using DarkRequiem.manager;
using DarkRequiem.scene;
using static Raylib_cs.Raylib;
using System.Numerics;

namespace DarkRequiem.scenes
{
    public class WinnerScene : IScene
    {
        public string Name => "Winner";
        private Texture2D background;

        public WinnerScene()
        {
            background = LoadTexture("assets/images/background/Background_end.png");
        }

        public void Update()
        {
            if (IsKeyPressed(KeyboardKey.Enter))
            {
                SceneManager.SetScene(new MenuScene());
            }
        }

        public void Draw()
        {
            BeginDrawing();
            ClearBackground(Color.Black);

            // Redimensionnement automatique
            Rectangle source = new Rectangle(0, 0, background.Width, background.Height);
            Rectangle dest = new Rectangle(0, 0, GetScreenWidth(), GetScreenHeight());
            DrawTexturePro(background, source, dest, Vector2.Zero, 0f, Color.White);



            EndDrawing();
        }
    }
}
