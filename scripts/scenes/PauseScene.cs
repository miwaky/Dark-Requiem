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
        private RenderTexture2D gameplaySnapshot;
        private IScene previousScene;
        private Rectangle resumeButton = new Rectangle(300, 200, 200, 60);
        private Rectangle quitButton = new Rectangle(300, 280, 200, 60);

        public PauseScene(IScene fromScene)
        {
            previousScene = fromScene;
            gameplaySnapshot = Raylib.LoadRenderTexture(960, 640);

            // Rendu de la scène précédente dans un screenshoot
            Raylib.BeginTextureMode(gameplaySnapshot);
            previousScene.Draw();
            Raylib.EndTextureMode();
        }
        public void Update()
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                Vector2 mouse = Raylib.GetMousePosition();
                if (Raylib.CheckCollisionPointRec(mouse, resumeButton))
                {
                    SceneManager.SetScene(previousScene);
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

            // Dessine l’image capturée du gameplay
            DrawTextureRec(
            gameplaySnapshot.Texture,
            new Rectangle(0, 0, gameplaySnapshot.Texture.Width, -gameplaySnapshot.Texture.Height),
            new Vector2(0, 0),
            Color.White
);

            Raylib.DrawRectangleRec(resumeButton, Color.DarkGreen);
            Raylib.DrawText("Reprendre", (int)resumeButton.X + 40, (int)resumeButton.Y + 15, 20, Color.White);

            Raylib.DrawRectangleRec(quitButton, Color.Red);
            Raylib.DrawText("Quitter", (int)quitButton.X + 50, (int)quitButton.Y + 15, 20, Color.White);

            Raylib.EndDrawing();
        }
    }
}