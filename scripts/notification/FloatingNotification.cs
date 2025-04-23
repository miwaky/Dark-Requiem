using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

public class FloatingNotification
{
    public string Message;
    public float Duration;
    public float Elapsed;
    public Color TextColor;
    public Vector2 StartPosition;
    public float Alpha = 1f;
    public float VerticalOffset = 0f;

    public FloatingNotification(string message, Vector2 startPosition, float duration = 2.5f)
    {
        Message = message;
        Duration = duration;
        Elapsed = 0f;
        TextColor = Color.White;
        StartPosition = startPosition;
    }

    public void Update(float deltaTime)
    {
        Elapsed += deltaTime;

        // Augmente le décalage vertical pour effet de montée
        VerticalOffset = MathF.Min(20f, Elapsed * 30f); // max 20px de montée

        // Commence à faire disparaître après 70% de la durée
        if (Elapsed > Duration * 0.7f)
        {
            float fadeOut = 1f - ((Elapsed - Duration * 0.7f) / (Duration * 0.3f));
            Alpha = MathF.Max(0f, fadeOut);
        }
    }

    public void Draw()
    {
        if (Alpha <= 0f) return;

        int screenWidth = GetScreenWidth();
        Vector2 size = MeasureTextEx(GetFontDefault(), Message, 18, 1);
        Vector2 pos = new Vector2(screenWidth - size.X - 20, StartPosition.Y - VerticalOffset);

        DrawText(Message, (int)pos.X, (int)pos.Y, 18, Fade(TextColor, Alpha));
    }

    public bool IsExpired => Elapsed > Duration;
}