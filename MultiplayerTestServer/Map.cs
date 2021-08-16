using System;

namespace MultiplayerTestServer
{
    class Map
    {
        public static float[,] Bounds =
        {
            { -8.59f, 8.6f }, // X
            { -4.695f, 4.73f }  // Y
        };

        public static float[] RandomCoordinates()
        {
            Random r = new Random();
            float x = (float)(r.NextDouble() * (Bounds[0, 1] - Bounds[0, 0]) + Bounds[0, 0]);
            float y = (float)(r.NextDouble() * (Bounds[1, 1] - Bounds[1, 0]) + Bounds[1, 0]);

            return new float[] { x, y };
        }
    }
}
