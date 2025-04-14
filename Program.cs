using Raylib_cs;
using DarkRequiem.manager;

Raylib.InitWindow(768, 528, "Dark Requiem");
Raylib.SetTargetFPS(60);

SceneManager.InitWithMenu();

while (!Raylib.WindowShouldClose())
{
    SceneManager.Update();
    SceneManager.Draw();

    if (ExitManager.ShouldQuit)
    {
        Raylib.CloseWindow();
        break; // pour sortir du while proprement
    }
}

