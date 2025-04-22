using Raylib_cs;
using static Raylib_cs.Raylib;
using System;

namespace DarkRequiem.manager
{
    public static class TransitionManager
    {
        private static bool isTransitioning = false;
        private static float timer = 0f;
        private static float duration = 0.5f;
        private static Action? onComplete;
        private static Color overlayColor = Color.Black;
        private static bool inputBlocked = false;

        public static bool IsBlockingInput => isTransitioning || inputBlocked;

        public static void Start(float durationSeconds, Action onCompleteCallback, Color? colorOverride = null)
        {
            isTransitioning = true;
            timer = durationSeconds;
            duration = durationSeconds;
            onComplete = onCompleteCallback;
            overlayColor = colorOverride ?? Color.Black;
            inputBlocked = true;

            AudioManager.Play("transition");
        }

        public static void Update()
        {
            if (!isTransitioning) return;

            timer -= GetFrameTime();

            if (timer <= 0f)
            {
                isTransitioning = false;
                inputBlocked = false;
                onComplete?.Invoke();
            }
        }

        public static void DrawOverlay(int screenWidth, int screenHeight)
        {
            if (isTransitioning)
            {
                float alpha = (1f - timer / duration) * 255f;
                DrawRectangle(0, 0, screenWidth, screenHeight, Fade(overlayColor, alpha / 255f));
            }
        }
    }
}