using DarkRequiem.scene;

namespace DarkRequiem.manager
{
    public static class SceneManager
    {
        private static IScene? currentScene;

        public static void SetScene(IScene newScene)
        {
            currentScene = newScene;
        }

        public static void Update()
        {
            currentScene?.Update();
        }

        public static void Draw()
        {
            currentScene?.Draw();
        }

        public static void InitWithMenu()

        {
            AudioManager.LoadAll();
            AudioManager.PlayMusic("menu");

            currentScene = new MenuScene();
        }
    }
}
