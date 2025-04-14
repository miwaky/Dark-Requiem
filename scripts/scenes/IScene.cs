namespace DarkRequiem.scene
{
    public interface IScene
    {
        void Update();
        void Draw();
        string Name { get; } // Pour identifier la scène
    }
}