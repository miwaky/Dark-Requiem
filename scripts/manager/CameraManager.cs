using System.Numerics;
using Raylib_cs;

namespace DarkRequiem.manager
{
    public class CameraManager
    {
        private Camera2D camera;

        private float zoneWidthPx = 16 * 16;
        private float zoneHeightPx = 11 * 16;

        private Vector2 currentZoneCenter;
        private Vector2 targetZoneCenter;
        private bool isTransitioning = false;
        private float transitionTime = 0f;
        private float transitionDuration = 0.5f;

        private int screenWidth;
        private int screenHeight;

        public CameraManager(int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            camera = new Camera2D
            {
                Offset = new Vector2(screenWidth / 2, screenHeight / 2),
                Zoom = 3f,
                Rotation = 0f,
                Target = Vector2.Zero
            };

            currentZoneCenter = Vector2.Zero;
            targetZoneCenter = Vector2.Zero;
        }

        public void InitCameraPosition(int playerCol, int playerLig)
        {
            Vector2 initZoneCenter = CalculateZoneCenter(playerCol, playerLig);
            currentZoneCenter = initZoneCenter;
            targetZoneCenter = initZoneCenter;

            camera.Target = currentZoneCenter;
        }

        public void UpdateZoneCamera(int playerCol, int playerLig, float deltaTime)
        {
            Vector2 newZoneCenter = CalculateZoneCenter(playerCol, playerLig);

            if (newZoneCenter != targetZoneCenter && !isTransitioning)
            {
                isTransitioning = true;
                transitionTime = 0f;

                currentZoneCenter = camera.Target;
                targetZoneCenter = newZoneCenter;
            }

            if (isTransitioning)
            {
                transitionTime += deltaTime;
                float t = transitionTime / transitionDuration;

                if (t >= 1f)
                {
                    t = 1f;
                    isTransitioning = false;
                }

                camera.Target = Vector2.Lerp(currentZoneCenter, targetZoneCenter, t);
            }
            else
            {
                camera.Target = targetZoneCenter;
            }
        }

        private Vector2 CalculateZoneCenter(int playerCol, int playerLig)
        {
            int zoneX = playerCol / 16;
            int zoneY = playerLig / 11;

            float centerX = zoneX * zoneWidthPx + zoneWidthPx / 2f;
            float centerY = zoneY * zoneHeightPx + zoneHeightPx / 2f;

            return new Vector2(centerX, centerY);
        }

        public Camera2D GetCamera()
        {
            return camera;
        }

        public void ForceCenterOnPlayer(int playerCol, int playerLig)
        {
            Vector2 zoneCenter = CalculateZoneCenter(playerCol, playerLig);
            currentZoneCenter = zoneCenter;
            targetZoneCenter = zoneCenter;
            camera.Target = zoneCenter;
            isTransitioning = false;
            transitionTime = 0f;
        }
        public void ForceCameraPosition(int playerCol, int playerLig)
        {
            Vector2 instantCenter = CalculateZoneCenter(playerCol, playerLig);
            currentZoneCenter = instantCenter;
            targetZoneCenter = instantCenter;
            camera.Target = instantCenter;
            isTransitioning = false;
        }
    }
}
