using Raylib_cs;
using DarkRequiem.manager;
using DarkRequiem.objects;

Raylib.InitWindow(768, 528, "Dark Requiem");
Raylib.SetTargetFPS(60);

SceneManager.InitWithMenu();

while (!Raylib.WindowShouldClose())
{
    SceneManager.Update();
    SceneManager.Draw();

    if (ExitManager.ShouldQuit)
    {
        Chest.Unload();
        Raylib.CloseWindow();
        break;
    }
}

