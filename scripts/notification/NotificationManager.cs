using System.Numerics;
using static Raylib_cs.Raylib;

public static class NotificationManager
{
    private static List<FloatingNotification> notifications = new();

    public static void Add(string message)
    {
        float baseY = 30f;
        float spacing = 24f;
        float posX = GetScreenWidth() - 20f;

        // Décale vers le bas chaque nouvelle notif
        var notif = new FloatingNotification(
            message,
            new Vector2(posX, baseY + notifications.Count * spacing)
        );
        notifications.Add(notif);
    }

    public static void Update(float deltaTime)
    {
        foreach (var notif in notifications)
            notif.Update(deltaTime);

        notifications.RemoveAll(n => n.IsExpired);
    }

    public static void Draw()
    {
        foreach (var notif in notifications)
            notif.Draw();
    }
}
