using Raylib_cs;
using System.Numerics;
using DarkRequiem.scene;
using DarkRequiem.manager;

namespace DarkRequiem.scenes
{
    public class SettingsScene : IScene
    {
        public string Name => "Settings";

        private float musicVolume;
        private float sfxVolume;
        private IScene? previousScene;
        public Texture2D background;
        private Texture2D sliderTexture;


        private Rectangle backButton = new Rectangle(290, 415, 201, 50);
        private Rectangle musicSliderBar = new Rectangle(137, 175, 496, 30);
        private Rectangle sfxSliderBar = new Rectangle(137, 281, 496, 28);

        public SettingsScene(IScene? fromScene = null)
        {
            background = Raylib.LoadTexture("assets/images/background/Background_Settings.png");
            sliderTexture = Raylib.LoadTexture("assets/images/background/slider.png");
            previousScene = fromScene;
            musicVolume = AudioManager.MusicVolume;
            sfxVolume = AudioManager.SfxVolume;
        }

        public void Update()
        {
            Vector2 mouse = Raylib.GetMousePosition();

            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                if (Raylib.CheckCollisionPointRec(mouse, musicSliderBar))
                {
                    musicVolume = (mouse.X - musicSliderBar.X) / musicSliderBar.Width;
                    musicVolume = Math.Clamp(musicVolume, 0f, 1f);
                    AudioManager.SetMusicVolume(musicVolume);
                }
                else if (Raylib.CheckCollisionPointRec(mouse, sfxSliderBar))
                {
                    sfxVolume = (mouse.X - sfxSliderBar.X) / sfxSliderBar.Width;
                    sfxVolume = Math.Clamp(sfxVolume, 0f, 1f);
                    AudioManager.SetSfxVolume(sfxVolume);
                }
            }

            if (Raylib.IsMouseButtonPressed(MouseButton.Left) && Raylib.CheckCollisionPointRec(mouse, backButton))
            {
                if (previousScene != null)
                    SceneManager.SetScene(previousScene);
                else
                    SceneManager.SetScene(new MenuScene());
            }
        }
        private void DrawSlider(Rectangle bar, float value)
        {
            Raylib.DrawRectangleRec(bar, new Color(50, 50, 50, 0));

            float targetWidth = 48f;
            float targetHeight = 48f;

            float x = bar.X + value * bar.Width - targetWidth / 2;
            float y = bar.Y + bar.Height / 2 - targetHeight / 2;

            Rectangle source = new Rectangle(0, 0, sliderTexture.Width, sliderTexture.Height);
            Rectangle dest = new Rectangle(x, y, targetWidth, targetHeight);
            Vector2 origin = Vector2.Zero;

            Raylib.DrawTexturePro(sliderTexture, source, dest, origin, 0f, Color.White);
        }

        public void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkBlue);

            Rectangle source = new Rectangle(0, 0, background.Width, background.Height);
            Rectangle dest = new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
            Raylib.DrawTexturePro(background, source, dest, Vector2.Zero, 0f, Color.White);

            // Sliders avec texture scalée
            DrawSlider(musicSliderBar, musicVolume);
            DrawSlider(sfxSliderBar, sfxVolume);

            // Back button
            Raylib.DrawRectangleRec(backButton, new Color(50, 50, 50, 0));


            Raylib.EndDrawing();
        }

    }
}